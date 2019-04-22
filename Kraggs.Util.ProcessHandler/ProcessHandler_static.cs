using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

namespace Kraggs.Util
{

    partial class ProcessHandler
    {
        /*
         * Also, add static helper functions for running external processes without needing to create a ProcessHandler.         
         * 
         */
        #region Static Helper functions for running external processes.

        public static ProcessResult RunProcess(ProcessSetup setup, int timeout = 30000)
        {
            // this should probably be the other way around thou.
            //return new ProcessHandler().Execute(setup, timeout);
            throw new NotImplementedException();
        }

        public static async Task<ProcessResult> RunProcess(ProcessSetup setup, CancellationToken cancel = default)
        {
            //return await new ProcessHandler().ExecuteAsync(setup, cancel);
            throw new NotImplementedException();
        }

        #endregion
    }
}
