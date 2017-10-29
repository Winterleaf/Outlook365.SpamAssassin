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
    /// The windows service attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WindowsServiceAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Outlook365.SpamAssassin.Framework.WindowsServiceAttribute" /> class. 
        /// Marks an IWindowsService with configuration and installation attributes.
        /// </summary>
        /// <param name="name">
        /// The name of the windows service.
        /// </param>
        public WindowsServiceAttribute(string name)
        {
            // set name and default description and display name to name.
            Name = name;
            Description = name;
            DisplayName = name;

            // default all other attributes.
            CanStop = true;
            CanShutdown = true;
            CanPauseAndContinue = true;
            StartMode = ServiceStartMode.Manual;
            EventLogSource = null;
            Password = null;
            UserName = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     True if service supports pause and continue (defaults to true).
        /// </summary>
        public bool CanPauseAndContinue { get; set; }

        /// <summary>
        ///     True if service supports shutdown event (defaults to true).
        /// </summary>
        public bool CanShutdown { get; set; }

        /// <summary>
        ///     True if service supports stop event (defaults to true).
        /// </summary>
        public bool CanStop { get; set; }

        /// <summary>
        ///     A textural description of the service name (defaults to Name).
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The displayable name that shows in service manager (defaults to Name).
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     Specifies the event log source to set the service's EventLog to.  If this is
        ///     empty or null (the default) no event log source is set.  If set, will auto-log
        ///     start and stop events.
        /// </summary>
        public string EventLogSource { get; set; }

        /// <summary>
        ///     The name of the service.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The password to run the service under (defaults to null).  Ignored
        ///     if the UserName is empty or null, this property is ignored.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     The method to start the service when the machine reboots (defaults to Manual).
        /// </summary>
        public ServiceStartMode StartMode { get; set; }

        /// <summary>
        ///     The user to run the service under (defaults to null).  A null or empty
        ///     UserName field causes the service to run as ServiceAccount.LocalService.
        /// </summary>
        public string UserName { get; set; }

        #endregion
    }
}