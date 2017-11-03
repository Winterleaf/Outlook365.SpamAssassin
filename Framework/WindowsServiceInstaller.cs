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
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace Outlook365.SpamAssassin.Framework
{
    /// <inheritdoc />
    /// <summary>
    ///     A generic windows service installer
    /// </summary>
    [RunInstaller(true)]
    public partial class WindowsServiceInstaller : Installer
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the type of the windows service to install.
        /// </summary>
        public WindowsServiceAttribute Configuration { get; set; }

        #endregion

        #region Constructors and Destructors

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Outlook365.SpamAssassin.Framework.WindowsServiceInstaller" /> class.
        ///     Creates a blank windows service installer with configuration in ServiceImplementation
        /// </summary>
        public WindowsServiceInstaller() : this(typeof(IWindowsService))
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Outlook365.SpamAssassin.Framework.WindowsServiceInstaller" /> class.
        ///     Creates a windows service installer using the type specified.
        /// </summary>
        /// <param name="windowsServiceType">
        ///     The type of the windows service to install.
        /// </param>
        public WindowsServiceInstaller(Type windowsServiceType)
        {
            if (!windowsServiceType.GetInterfaces().Contains(typeof(IWindowsService)))
                throw new ArgumentException("Type to install must implement IWindowsService.", nameof(windowsServiceType));

            var attribute = windowsServiceType.GetAttribute<WindowsServiceAttribute>();

            Configuration = attribute ?? throw new ArgumentException("Type to install must be marked with a WindowsServiceAttribute.",
                                nameof(windowsServiceType));
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Performs a transacted installation at run-time of the AutoCounterInstaller and any other listed installers.
        /// </summary>
        /// <typeparam name="T">The IWindowsService implementer to install.</typeparam>
        public static void RuntimeInstall<T>() where T : IWindowsService
        {
            string path = "/assemblypath=" + Assembly.GetEntryAssembly().Location;

            using (var ti = new TransactedInstaller())
            {
                ti.Installers.Add(new WindowsServiceInstaller(typeof(T)));
                ti.Context = new InstallContext(null, new[] {path});
                ti.Install(new Hashtable());
            }
        }

        /// <summary>
        ///     Performs a transacted un-installation at run-time of the AutoCounterInstaller and any other listed installers.
        /// </summary>
        /// <param name="otherInstallers">
        ///     The other installers to include in the transaction
        /// </param>
        /// <typeparam name="T">
        ///     The IWindowsService implementer to install.
        /// </typeparam>
        public static void RuntimeUnInstall<T>(params Installer[] otherInstallers) where T : IWindowsService
        {
            string path = "/assemblypath=" + Assembly.GetEntryAssembly().Location;

            using (var ti = new TransactedInstaller())
            {
                ti.Installers.Add(new WindowsServiceInstaller(typeof(T)));
                ti.Context = new InstallContext(null, new[] {path});
                ti.Uninstall(null);
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Installer class, to use run InstallUtil against this .exe
        /// </summary>
        /// <param name="savedState">
        ///     The saved state for the installation.
        /// </param>
        public override void Install(IDictionary savedState)
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.White, "Installing service {0}.", Configuration.Name);

            // install the service 
            ConfigureInstallers();
            base.Install(savedState);

            // wire up the event log source, if provided
            if (string.IsNullOrWhiteSpace(Configuration.EventLogSource))
                return;
            // create the source if it doesn't exist
            if (!EventLog.SourceExists(Configuration.EventLogSource))
                EventLog.CreateEventSource(Configuration.EventLogSource, "Application");
        }

        /// <inheritdoc />
        /// <summary>
        ///     Rolls back to the state of the counter, and performs the normal rollback.
        /// </summary>
        /// <param name="savedState">
        ///     The saved state for the installation.
        /// </param>
        public override void Rollback(IDictionary savedState)
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.White, "Rolling back service {0}.", Configuration.Name);

            // load the assembly file name and the config
            ConfigureInstallers();
            base.Rollback(savedState);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Removes the counters, then calls the base uninstall.
        /// </summary>
        /// <param name="savedState">
        ///     The saved state for the installation.
        /// </param>
        public override void Uninstall(IDictionary savedState)
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.White, "Un-Installing service {0}.", Configuration.Name);

            // load the assembly file name and the config
            ConfigureInstallers();
            base.Uninstall(savedState);

            // wire up the event log source, if provided
            if (string.IsNullOrWhiteSpace(Configuration.EventLogSource))
                return;
            // create the source if it doesn't exist
            if (EventLog.SourceExists(Configuration.EventLogSource))
                EventLog.DeleteEventSource(Configuration.EventLogSource);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Method to configure the installers
        /// </summary>
        private void ConfigureInstallers()
        {
            // load the assembly file name and the config
            Installers.Add(ConfigureProcessInstaller());
            Installers.Add(ConfigureServiceInstaller());
        }

        /// <summary>
        ///     Helper method to configure a process installer for this windows service
        /// </summary>
        /// <returns>Process installer for this service</returns>
        private ServiceProcessInstaller ConfigureProcessInstaller()
        {
            var result = new ServiceProcessInstaller();

            // if a user name is not provided, will run under local service acct
            if (string.IsNullOrEmpty(Configuration.UserName))
            {
                result.Account = ServiceAccount.LocalService;
                result.Username = null;
                result.Password = null;
            }
            else
            {
                // otherwise, runs under the specified user authority
                result.Account = ServiceAccount.User;
                result.Username = Configuration.UserName;
                result.Password = Configuration.Password;
            }

            return result;
        }

        /// <summary>
        ///     Helper method to configure a service installer for this windows service
        /// </summary>
        /// <returns>Process installer for this service</returns>
        private ServiceInstaller ConfigureServiceInstaller()
        {
            // create and config a service installer
            var result = new ServiceInstaller
            {
                ServiceName = Configuration.Name,
                DisplayName = Configuration.DisplayName,
                Description = Configuration.Description,
                StartType = Configuration.StartMode
            };

            return result;
        }

        #endregion
    }
}