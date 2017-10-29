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
using System.Diagnostics;
using System.IO;
using System.Threading;
using Outlook365.SpamAssassin.Data;
using Outlook365.SpamAssassin.Email;
using Outlook365.SpamAssassin.Singleton;

namespace Outlook365.SpamAssassin
{
    public class WorkInstance : BaseSingletonLoop
    {
        /// <summary>
        ///     Last Time The sa-Update command was run.
        /// </summary>
        public DateTime? LastRun { get; private set; }

        /// <summary>
        ///     Is the service started?
        /// </summary>
        private volatile bool _started;

        private bool StartSpamd()
        {
            //If already Started Exit out.
            if (_started)
                return true;

            //Check if spamd.exe is running
            var pname = Process.GetProcessesByName("spamd");
            if (pname.Length != 0)
                return true;

            //Start spamd dameon
            var program = new Process
            {
                StartInfo =
                {
                    FileName = Path.Combine(Config.ReadConfig().SpamAssassinWorkingFolder, "spamd.exe"),
                    WorkingDirectory = Config.ReadConfig().SpamAssassinWorkingFolder,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
            try
            {
                program.Start();

                _started = true;
                return true;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                _started = false;
            }
            return false;
        }

        public new void Start()
        {
            Config.CreateConfig();
            if (StartSpamd())
                base.Start();
        }

#if DEBUG
#else
        /// <summary>
        ///     Calls the sa-Update utility to update spam database
        /// </summary>
        private void UpdateSpamD()
        {
            var program = new Process
            {
                StartInfo =
                {
                    FileName = Path.Combine(Config.ReadConfig().SpamAssassinWorkingFolder, "sa-update.exe"),
                    WorkingDirectory = Config.ReadConfig().SpamAssassinWorkingFolder,
                    Arguments = "--no-gpg",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
            program.Start();
            LastRun = DateTime.Now;
        }
#endif
        public override void OnTick()
        {
            try
            {
                var mrs = Config.ReadConfig();

#if DEBUG

                EmailReader.ProcessEmail(mrs);
#else

                //Set max threads = 2 * processors
                ThreadPool.SetMaxThreads(Environment.ProcessorCount * 2, Environment.ProcessorCount * 2);

                //Get Available Threads
                ThreadPool.GetAvailableThreads(out int availThread, out int compThread);

                //Check if any are free?
                if (availThread / 2.0 <= 1.0 || compThread / 2.0 <= 1.0)
                    return;

                //Queue work thread
                ThreadPool.QueueUserWorkItem(EmailReader.ProcessEmail, mrs);

                //Sleep for 2 seconds
                Thread.Sleep(2000);

                //Have we updated the spam rules in the last 30 minutes?
                if (LastRun != null && (DateTime.Now - LastRun.Value).Minutes <= 30)
                    return;

                //Update Spam Rules.
                UpdateSpamD();
#endif
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
    }

    public class WorkService : BaseLazySingleton<WorkInstance>
    {
    }
}