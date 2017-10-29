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