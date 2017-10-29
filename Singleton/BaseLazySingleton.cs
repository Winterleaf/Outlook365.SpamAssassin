using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Outlook365.SpamAssassin.Singleton
{
    /// <summary>
    /// Class used to manage singletons
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseLazySingleton<T> where T : class, new()
    {
        private static readonly Lazy<T> LazyInstance = new Lazy<T>(CreateInstanceOfT, LazyThreadSafetyMode.ExecutionAndPublication);
        
        /// <summary>
        /// Instance of the singleton
        /// </summary>
        public static T Instance => LazyInstance.Value;
        
        private static T CreateInstanceOfT()
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }
        
    }
}
