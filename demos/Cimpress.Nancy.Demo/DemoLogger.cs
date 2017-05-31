using System;
using System.Collections.Generic;
using Cimpress.Nancy.Components;

namespace Cimpress.Nancy.Demo
{
    public class DemoLogger : INancyLogger
    {

        public void Debug(object data)
        {
            Console.WriteLine($"Debug: {data}");
        }

        public void Info(object data)
        {
            Console.WriteLine($"Info: {data}");
        }

        public void Error(object data)
        {
            Console.WriteLine($"Error: {data}");
        }
    }
}
