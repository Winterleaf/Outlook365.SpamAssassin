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

namespace Outlook365.SpamAssassin.Framework
{
    /// <summary>
    /// The console harness.
    /// </summary>
    public static class ConsoleHarness
    {
        // Run a service from the console given a service implementation
        #region Public Methods and Operators

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <param name="service">
        /// The service.
        /// </param>
        public static void Run(string[] args, IWindowsService service)
        {
            bool isRunning = true;

            // simulate starting the windows service
            service.OnStart(args);

            // let it run as long as Q is not pressed
            while (isRunning)
            {
                WriteToConsole(ConsoleColor.Yellow, "Enter either [Q]uit, [P]ause, [R]esume : ");
                isRunning = HandleConsoleInput(service, Console.ReadLine());
            }

            // stop and shutdown
            service.OnStop();
            service.OnShutdown();
        }

        #endregion

        // Private input handler for console commands.

        // Helper method to write a message to the console at the given foreground color.
        #region Methods

        /// <summary>
        /// The write to console.
        /// </summary>
        /// <param name="foregroundColor">
        /// The foreground color.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="formatArguments">
        /// The format arguments.
        /// </param>
        public static void WriteToConsole(ConsoleColor foregroundColor, string format, params object[] formatArguments)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;

            Console.WriteLine(format, formatArguments);
            Console.Out.Flush();

            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// The handle console input.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool HandleConsoleInput(IWindowsService service, string line)
        {
            bool canContinue = true;

            // check input
            if (line != null)
            {
                switch (line.ToUpper())
                {
                    case "Q":
                        canContinue = false;
                        break;

                    case "P":
                        service.OnPause();
                        break;

                    case "R":
                        service.OnContinue();
                        break;

                    default:
                        WriteToConsole(ConsoleColor.Red, "Did not understand that input, try again.");
                        break;
                }
            }

            return canContinue;
        }

        #endregion
    }
}