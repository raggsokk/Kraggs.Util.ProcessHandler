using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

namespace Kraggs.Util
{
    public interface IProcessHandler
    {
        ProcessResult Execute(ProcessSetup setup, int timeout = 30000);

        Task<ProcessResult> ExecuteAsync(ProcessSetup setup, CancellationToken cancel = default);
    }
}
