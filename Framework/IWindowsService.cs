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
    /// <inheritdoc />
    /// <summary>
    ///     The interface that any windows service should implement to be used
    ///     with the GenericWindowsService executable.
    /// </summary>
    public interface IWindowsService : IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        ///     This method is called when a service gets a request to resume
        ///     after a pause is issued.
        /// </summary>
        void OnContinue();

        /// <summary>
        ///     This method is called when a service gets a request to pause,
        ///     but not stop completely.
        /// </summary>
        void OnPause();

        /// <summary>
        ///     This method is called when the machine the service is running on
        ///     is being shutdown.
        /// </summary>
        void OnShutdown();

        /// <summary>
        /// This method is called when the service gets a request to start.
        /// </summary>
        /// <param name="args">
        /// Any command line arguments
        /// </param>
        void OnStart(string[] args);

        /// <summary>
        ///     This method is called when the service gets a request to stop.
        /// </summary>
        void OnStop();


        void WriteLine(string msg);

        #endregion
    }
}