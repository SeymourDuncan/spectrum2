using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectrum2.Model
{
    public static class Logger
    {
        public static void AddMessage(string msg)
        {
            RaiseEvent(msg);
        }
        public static void AddError(string msg)
        {
            RaiseEvent(msg);
        }

        public static event EventHandler<string> MessageAdded;

        private static void RaiseEvent(string msg)
        {
            var handler = MessageAdded;
            if (handler != null)
                handler(typeof(Logger), msg);
        }
    }
}
