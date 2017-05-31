using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cimpress.Nancy.Components
{
    public class NoopLogger : INancyLogger
    {

        public void Debug(object data)
        {
        }

        public void Info(object data)
        {
        }

        public void Error(object data)
        {
        }
    }
}
