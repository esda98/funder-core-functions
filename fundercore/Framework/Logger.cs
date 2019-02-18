using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Text;

namespace fundercore.Framework
{
    class Logger
    {
        private static TraceWriter writer;
        public static void write(string text)
        {  
            //write out text given through tracewrite if available, otherwise send to stdout
            if (writer == null) {
                Console.WriteLine(text);
            } else {
                writer.Info(text);
            }
        }
        public static void initialize(TraceWriter givenWriter)
        {
            writer = givenWriter;
        }
    }
}
