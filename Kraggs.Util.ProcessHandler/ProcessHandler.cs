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
            // naive implementation just call static version.
            // BUT, future version will have virtual methods to override behaviour.
            //throw new NotImplementedException();
            return ProcessHandler.RunProcess(setup, timeout);
        }

        public async Task<ProcessResult> ExecuteAsync(ProcessSetup setup, CancellationToken cancel = default)
        {
            // naive implementation just call static version.
            // BUT, future version will have virtual methods to override behaviour.
            //throw new NotImplementedException();

            //var src = new CancellationTokenSource(30000);            

            return await ProcessHandler.RunProcessAsync(setup, cancel).ConfigureAwait(false); ;
        }

        #endregion        
    }
}
