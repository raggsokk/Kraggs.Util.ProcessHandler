using System;
using System.Collections.Generic;
using System.Text;

namespace Kraggs.Util
{
    public class ProcessSetup
    {
        // required options.
        public string Executable { get; set; }
        public string Arguments { get; set; }

        // optional options.
        public string WorkingDir { get; set; }

        public IEnumerable<(string key, string value)> EnvironmentVariables { get; set; }

    }
}
