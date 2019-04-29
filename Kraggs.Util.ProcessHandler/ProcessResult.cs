using System;
using System.Collections.Generic;
//using System

namespace Kraggs.Util
{
    /// <summary>
    /// Represents the result returned from an execution of an external process.
    /// </summary>
    public class ProcessResult
    {
        /// <summary>
        /// Returns true if the process was started.
        /// Can be false if setup fails, executable file is not found or is not executable or no access.
        /// </summary>
        public bool WasStarted { get; set; }

        /// <summary>
        /// Returns true if the process was started and exited of its own free will.
        /// This also indicates that the exit code has valid data.
        /// </summary>
        public bool HasCompleted { get; set; }

        /// <summary>
        /// The process exit code if we got that far. Use HasCompleted to check for proper process exit happened.
        /// </summary>
        public int? ExitCode { get; set; }

        /// <summary>
        /// A list of output lines.
        /// </summary>
        public List<string> Output { get; set; }

        /// <summary>
        /// A list of error lines.
        /// </summary>
        public List<string> Errors { get; set; }


    }
}
