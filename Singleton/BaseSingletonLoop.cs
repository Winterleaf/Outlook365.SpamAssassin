using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Outlook365.SpamAssassin.Singleton
{
    /// <summary>
    /// Looping class base class
    /// </summary>
    public class BaseSingletonLoop
    {
        private volatile bool _isStopped = true;
        private volatile bool _stopJob = true;

        private Thread _worker;

        public bool Running => !_stopJob;

        public bool IsStopped()
        {
            return _stopJob && _isStopped;
        }

        public void Stop()
        {
            _stopJob = true;
            while (!_isStopped)
                Thread.Sleep(1);
            _isStopped = true;
            OnStop();
        }

        public virtual bool OnStart()
        {
            return true;
        }

        public virtual void OnStop()
        {
        }

        public virtual void OnTick()
        {
        }

        public bool Start()
        {
            if (!_isStopped)
                return true;

            if (_worker != null && _worker.IsAlive)
                return true;

            _worker = null;
            _worker = new Thread(DoWork);
            _worker.Start();

            _stopJob = false;

            return true;
        }

        private void DoWork()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Normal;
            _isStopped = false;
            while (!_stopJob)
                try
                {
                    OnTick();
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                }


            _isStopped = true;
        }
    }
}
