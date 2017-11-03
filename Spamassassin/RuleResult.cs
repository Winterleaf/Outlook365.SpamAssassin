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

namespace Outlook365.SpamAssassin.Spamassassin
{
    /// <summary>
    ///     Ruleset Result Definitions
    /// </summary>
    public class RuleResult
    {
        /// <summary>
        ///     Rule Description
        /// </summary>
        public string Description = "";

        /// <summary>
        ///     Rule
        /// </summary>
        public string Rule = "";

        /// <summary>
        ///     Score
        /// </summary>
        public double Score;

        /// <summary>
        ///     Parses a line of the result into a rule result
        /// </summary>
        /// <param name="line"></param>
        public RuleResult(string line)
        {
            Score = double.Parse(line.Substring(0, line.IndexOf(" ", StringComparison.Ordinal)).Trim());
            line = line.Substring(line.IndexOf(" ", StringComparison.Ordinal) + 1);
            Rule = line.Substring(0, 23).Trim();
            Description = line.Substring(23).Trim();
        }
    }
}