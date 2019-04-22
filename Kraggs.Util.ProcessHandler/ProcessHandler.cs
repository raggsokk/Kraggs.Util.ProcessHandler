using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kraggs.Util
{
    public partial class ProcessHandler : IProcessHandler
    {
        #region IProcessHandler

        public ProcessResult Execute(ProcessSetup setup, int timeout = 30000)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessResult> ExecuteAsync(ProcessSetup setup, CancellationToken cancel = default)
        {
            throw new NotImplementedException();
        }

        #endregion        
    }
}
