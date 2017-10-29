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
using System.IO;
using System.Linq;
using System.ServiceProcess;
using Outlook365.SpamAssassin.Framework;

namespace Outlook365.SpamAssassin
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                // if install was a command line flag, then run the installer at runtime.
                if (args.Contains("-install", StringComparer.InvariantCultureIgnoreCase))
                {
                    WindowsServiceInstaller.RuntimeInstall<ServiceImplementation>();
                }

                // if uninstall was a command line flag, run uninstaller at runtime.
                else if (args.Contains("-uninstall", StringComparer.InvariantCultureIgnoreCase))
                {
                    WindowsServiceInstaller.RuntimeUnInstall<ServiceImplementation>();
                }

                // otherwise, fire up the service as either console or windows service based on UserInteractive property.
                else
                {
                    ServiceImplementation implementation = new ServiceImplementation();

                    // if started from console, file explorer, etc, run as console app.
                    if (Environment.UserInteractive)
                    {
                        ConsoleHarness.Run(args, implementation);
                    }

                    // otherwise run as a windows service
                    else
                    {
                        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                        ServiceBase.Run(new WindowsServiceHarness(implementation));
                    }
                }
            }

            catch (Exception ex)
            {
                ConsoleHarness.WriteToConsole(ConsoleColor.Red, "An exception occurred in Main(): {0}", ex);
            }
        }
    }
}