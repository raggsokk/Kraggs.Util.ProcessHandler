using System;
using System.Collections.Generic;
//using System

namespace Kraggs.Util
{
    public class ProcessResult
    {
        public bool WasStarted { get; set; }

        public bool HasCompleted { get; set; }

        public int? ExitCode { get; set; }

        public IEnumerable<string> Output { get; set; }

        public IEnumerable<string> Errors { get; set; }


    }
}
