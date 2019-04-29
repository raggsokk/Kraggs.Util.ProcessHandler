using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

namespace Kraggs.Util
{
    /// <summary>
    /// Public interface for handling running external processes.
    /// </summary>
    public interface IProcessHandler
    {
        ProcessResult Execute(ProcessSetup setup, int timeout = 30000);

        Task<ProcessResult> ExecuteAsync(ProcessSetup setup, CancellationToken cancel = default);
    }
}
