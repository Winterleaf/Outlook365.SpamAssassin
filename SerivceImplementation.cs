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
        ///     This method is called when the machine the service is running on
        ///     is being shutdown.
        /// </summary>
        public void OnShutdown()
        {
            WorkService.Instance.Stop();
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
    }
}