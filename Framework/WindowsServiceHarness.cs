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

namespace Outlook365.SpamAssassin.Framework
{
    /// <inheritdoc />
    /// <summary>
    ///     A generic Windows Service that can handle any assembly that
    ///     implements IWindowsService (including AbstractWindowsService)
    /// </summary>
    public partial class WindowsServiceHarness : ServiceBase
    {
        #region Constructors and Destructors

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Outlook365.SpamAssassin.Framework.WindowsServiceHarness" /> class.
        ///     Constructor a generic windows service from the given class
        /// </summary>
        /// <param name="serviceImplementation">
        ///     Service implementation.
        /// </param>
        public WindowsServiceHarness(IWindowsService serviceImplementation)
        {
            // make sure service passed in is valid

            // set instance and backward instance
            ServiceImplementation = serviceImplementation ??
                                    throw new ArgumentNullException(nameof(serviceImplementation),
                                        "IWindowsService cannot be null in call to GenericWindowsService");

            // configure our service
            ConfigureServiceFromAttributes(serviceImplementation);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Get the class implementing the windows service
        /// </summary>
        public IWindowsService ServiceImplementation { get; }

        #endregion

        #region Methods

        /// <inheritdoc />
        /// <summary>
        ///     Override service control on continue
        /// </summary>
        protected override void OnContinue()
        {
            // perform class specific behavior 
            ServiceImplementation.OnContinue();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when service is paused
        /// </summary>
        protected override void OnPause()
        {
            // perform class specific behavior 
            ServiceImplementation.OnPause();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when the Operating System is shutting down
        /// </summary>
        protected override void OnShutdown()
        {
            // perform class specific behavior
            ServiceImplementation.OnShutdown();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when service is requested to start
        /// </summary>
        /// <param name="args">
        ///     The startup arguments array.
        /// </param>
        protected override void OnStart(string[] args)
        {
            ServiceImplementation.OnStart(args);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when service is requested to stop
        /// </summary>
        protected override void OnStop()
        {
            ServiceImplementation.OnStop();
        }

        /// <summary>
        ///     Set configuration data
        /// </summary>
        /// <param name="serviceImplementation">
        ///     The service with configuration settings.
        /// </param>
        private void ConfigureServiceFromAttributes(IWindowsService serviceImplementation)
        {
            var attribute = serviceImplementation.GetType().GetAttribute<WindowsServiceAttribute>();

            if (attribute != null)
            {
                // wire up the event log source, if provided
                if (!string.IsNullOrWhiteSpace(attribute.EventLogSource))
                    EventLog.Source = attribute.EventLogSource;

                CanStop = attribute.CanStop;
                CanPauseAndContinue = attribute.CanPauseAndContinue;
                CanShutdown = attribute.CanShutdown;

                // we don't handle: laptop power change event
                CanHandlePowerEvent = false;

                // we don't handle: Term Services session event
                CanHandleSessionChangeEvent = false;

                // always auto-event-log 
                AutoLog = true;
            }
            else
            {
                throw new InvalidOperationException(
                    $"IWindowsService implementer {serviceImplementation.GetType().FullName} must have a WindowsServiceAttribute.");
            }
        }

        #endregion
    }
}