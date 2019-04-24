using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace fundercore.Framework {
    class Logger {
        private static ILogger writer;
        public static void write(string text) {
            //write out text given through tracewrite if available, otherwise send to stdout
            if (writer == null) {
                Console.WriteLine(text);
            } else {
                writer.LogInformation(text);
            }
        }
        public static void initialize(ILogger givenWriter) {
            writer = givenWriter;
        }
    }
}
