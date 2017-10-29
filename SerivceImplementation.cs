using System;
using System.ServiceProcess;
using Outlook365.SpamAssassin.Framework;

namespace Outlook365.SpamAssassin
{
    /// <inheritdoc />
    /// <summary>
    ///     The actual implementation of the windows service goes here...
    /// </summary>
    [WindowsService("Outlook365.SpamAssassin", DisplayName = "Outlook365.SpamAssassin", Description = "Outlook365.SpamAssassin",
        EventLogSource = "Outlook365.SpamAssassin", StartMode = ServiceStartMode.Automatic)]
    public class ServiceImplementation : IWindowsService
    {
     /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }


        /// <inheritdoc />
        /// <summary>
        ///     This method is called when the service gets a request to start.
        /// </summary>
        /// <param name="args">Any command line arguments</param>
        public void OnStart(string[] args)
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.DarkYellow, "Service Starting");

            WorkService.Instance.Start();
        }

        /// <inheritdoc />
        /// <summary>
        ///     This method is called when the service gets a request to stop.
        /// </summary>
        public void OnStop()
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.DarkYellow, "Service Stopping");
            WorkService.Instance.Stop();
        }

        public void WriteLine(string msg)
        {
            
            ConsoleHarness.WriteToConsole(ConsoleColor.DarkYellow, msg);
        }

        /// <inheritdoc />
        /// <summary>
        ///     This method is called when a service gets a request to pause,
        ///     but not stop completely.
        /// </summary>
        public void OnPause()
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.DarkYellow, "Service Stopping");
            WorkService.Instance.Stop();
        }

        /// <inheritdoc />
        /// <summary>
        ///     This method is called when a service gets a request to resume
        ///     after a pause is issued.
        /// </summary>
        public void OnContinue()
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.DarkYellow, "Service Starting");
            WorkService.Instance.Start();
        }

        /// <inheritdoc />
        /// <summary>
        ///     This method is called when the machine the service is running on
        ///     is being shutdown.
        /// </summary>
        public void OnShutdown()
        {
            WorkService.Instance.Stop();
        }
    }
}