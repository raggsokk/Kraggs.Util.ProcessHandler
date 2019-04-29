using System;
using System.Collections.Generic;
using System.Text;

namespace Kraggs.Util
{
    /// <summary>
    /// Class handling setting up running an external process.
    /// Note that cancellation and timeouts are handled as function parameters instead.
    /// </summary>
    public class ProcessSetup
    {
        // required options.
        /// <summary>
        /// Reference to the executable file to run.
        /// </summary>
        public string Executable { get; set; }
        /// <summary>
        /// The string string argument.
        /// </summary>
        public string Arguments { get; set; }

        // optional options.
        /// <summary>
        /// Specifiy which path should be current on execution.
        /// </summary>
        public string WorkingDir { get; set; }

        /// <summary>
        /// Optionally a list of environment variables to set before exeuting.
        /// </summary>
        public IEnumerable<(string key, string value)> EnvironmentVariables { get; set; }

    }
}
