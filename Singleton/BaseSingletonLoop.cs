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
using System.Threading;

namespace Outlook365.SpamAssassin.Singleton
{
    /// <summary>
    ///     Looping class base class
    /// </summary>
    public class BaseSingletonLoop
    {
        private volatile bool _isStopped = true;
        private volatile bool _stopJob = true;

        private Thread _worker;

        public bool Running => !_stopJob;

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

        public bool IsStopped()
        {
            return _stopJob && _isStopped;
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

        public void Stop()
        {
            _stopJob = true;
            while (!_isStopped)
                Thread.Sleep(1);
            _isStopped = true;
            OnStop();
        }
    }
}