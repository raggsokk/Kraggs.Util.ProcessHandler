using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

namespace Kraggs.Util
{
    public static class ProcessHandlerExtensions
    {
        public static ProcessResult Execute(this IProcessHandler handler, string executable, string arguments, int timeout = 30000)
        {
            var setup = new ProcessSetup()
            {
                Executable = executable, Arguments = arguments
            };

            return handler.Execute(setup, timeout);
        }

        public static async Task<ProcessResult> ExecuteAsync(this IProcessHandler handler, string executable, string arguments, CancellationToken cancel = default)
        {
            var setup = new ProcessSetup()
            {
                Executable = executable,
                Arguments = arguments
            };

            return await handler.ExecuteAsync(setup, cancel);
        }
    }
}
