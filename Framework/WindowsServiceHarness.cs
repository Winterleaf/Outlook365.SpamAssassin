
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
        /// Initializes a new instance of the <see cref="T:Outlook365.SpamAssassin.Framework.WindowsServiceHarness" /> class. 
        /// Constructor a generic windows service from the given class
        /// </summary>
        /// <param name="serviceImplementation">
        /// Service implementation.
        /// </param>
        public WindowsServiceHarness(IWindowsService serviceImplementation)
        {
            // make sure service passed in is valid

            // set instance and backward instance
            ServiceImplementation = serviceImplementation ?? throw new ArgumentNullException(nameof(serviceImplementation), "IWindowsService cannot be null in call to GenericWindowsService");

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
        /// Called when service is requested to start
        /// </summary>
        /// <param name="args">
        /// The startup arguments array.
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
        /// Set configuration data
        /// </summary>
        /// <param name="serviceImplementation">
        /// The service with configuration settings.
        /// </param>
        private void ConfigureServiceFromAttributes(IWindowsService serviceImplementation)
        {
            WindowsServiceAttribute attribute = serviceImplementation.GetType().GetAttribute<WindowsServiceAttribute>();

            if (attribute != null)
            {
                // wire up the event log source, if provided
                if (!string.IsNullOrWhiteSpace(attribute.EventLogSource))
                {
                    // assign to the base service's EventLog property for auto-log events.
                    EventLog.Source = attribute.EventLogSource;
                }

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