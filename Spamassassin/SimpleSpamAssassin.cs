/* Copyright 2017 Fairfield Tek, L.L.C.
* Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
* to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute,
* sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
* OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
* OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Outlook365.SpamAssassin.Spamassassin
{
    /// <summary>
    ///     Simple class used to talk to the Spam Assassin Dameon
    /// </summary>
    public class SimpleSpamAssassin
    {
        /// <summary>
        ///     Regex used to read Spam Summary
        /// </summary>
        private static readonly Regex RegexSpamScore = new Regex("Spam: (?<IsSpam>.{4,5}.); (?<Score>\\d{1,4}.\\d) / (?<Threshold>\\d{1,4}.\\d)");

        /// <summary>
        ///     Function used to check Regex
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="groupName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private static string CheckRegexExpression(Regex regex, string groupName, string body)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(regex.ToString()))
                    return string.Empty;
                var match = regex.Match(body);
                return match.Groups[groupName].Captures.Count > 0 ? match.Groups[groupName].Captures[0].Value.Trim() : string.Empty;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        ///     Gets the Spam Report from SpamAssassin
        /// </summary>
        /// <param name="serverIp">IP Address of the SpamD Damean</param>
        /// <param name="message">MimeContent To check</param>
        /// <returns></returns>
        public static ParseSummary GetReport(string serverIp, string message)
        {
            //Prepare the message
            var messageBuffer = Encoding.ASCII.GetBytes($"REPORT SPAMC/1.2\r\n" + $"Content-Length: {message.Length}\r\n\r\n" + message);

            //Establish a socket connection to the SpamD service
            using (var spamAssassinSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                //Connect to service
                spamAssassinSocket.Connect(serverIp, 783);
                //Send Command
                spamAssassinSocket.Send(messageBuffer);
                //Shutdown socket send
                spamAssassinSocket.Shutdown(SocketShutdown.Send);

                //Read Response
                int received;
                string receivedMessage = string.Empty;
                do
                {
                    var receiveBuffer = new byte[1024];
                    received = spamAssassinSocket.Receive(receiveBuffer);
                    receivedMessage += Encoding.ASCII.GetString(receiveBuffer, 0, received);
                } while (received > 0);

                //Shut down socket recieve
                spamAssassinSocket.Shutdown(SocketShutdown.Both);

                //Parse the report
                var summary = new ParseSummary {Report = ParseResponse(receivedMessage)};

                //Read each line and try to parse it
                foreach (string line in receivedMessage.Split('\n'))
                    if (CheckRegexExpression(RegexSpamScore, "IsSpam", line) != "")
                    {
                        summary.IsSpam = CheckRegexExpression(RegexSpamScore, "IsSpam", line).Equals("true", StringComparison.InvariantCultureIgnoreCase);
                        if (double.TryParse(CheckRegexExpression(RegexSpamScore, "Score", line), out double chk))
                            summary.Score = chk;
                        if (double.TryParse(CheckRegexExpression(RegexSpamScore, "Threshold", line), out chk))
                            summary.SpamScore = chk;

                        break;
                    }

                return summary;
            }
        }

        private static List<RuleResult> ParseResponse(string receivedMessage)
        {
            //merge line endings
            receivedMessage = receivedMessage.Replace("\r\n", "\n");
            receivedMessage = receivedMessage.Replace("\r", "\n");
            var lines = receivedMessage.Split('\n');

            var results = new List<RuleResult>();
            bool inReport = false;
            foreach (string line in lines)
            {
                if (inReport)
                    try
                    {
                        results.Add(new RuleResult(line.Trim()));
                    }
                    catch
                    {
                        //past the end of the report
                    }

                if (line.StartsWith("---"))
                    inReport = true;
            }

            return results;
        }
    }
}