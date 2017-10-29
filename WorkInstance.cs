using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Outlook365.SpamAssassin.Data;
using Outlook365.SpamAssassin.Email;
using Outlook365.SpamAssassin.Singleton;

namespace Outlook365.SpamAssassin
{
    public class WorkInstance : BaseSingletonLoop
    {
        /// <summary>
        /// Last Time The sa-Update command was run.
        /// </summary>
        public DateTime? LastRun { get; private set; }

        /// <summary>
        /// Is the service started?
        /// </summary>
        private volatile bool _started;

        private bool StartSpamd()
        {
            //If already Started Exit out.
            if (_started)
                return true;

            //Check if spamd.exe is running
            Process[] pname = Process.GetProcessesByName("spamd");
            if (pname.Length != 0)
                return true;

            //Start spamd dameon
            Process program = new Process
            {
                StartInfo =
                {
                    FileName =Path.Combine( Data.Config.ReadConfig().SpamAssassinWorkingFolder,"spamd.exe"),
                    WorkingDirectory =Data.Config.ReadConfig().SpamAssassinWorkingFolder,
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
        /// Calls the sa-Update utility to update spam database
        /// </summary>
        private void UpdateSpamD()
        {

            Process program = new Process
            {
                StartInfo =
                {
                    FileName =Path.Combine( Data.Config.ReadConfig().SpamAssassinWorkingFolder,"sa-update.exe"),
                    WorkingDirectory =Data.Config.ReadConfig().SpamAssassinWorkingFolder,
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
                var mrs = Data.Config.ReadConfig();

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