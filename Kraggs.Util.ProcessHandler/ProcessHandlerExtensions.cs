using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Kraggs.Util
{
    /// <summary>
    /// Public extensions for the ProcessHandler Interface.
    /// </summary>
    public static class ProcessHandlerExtensions
    {
        /// <summary>
        /// Executes an external process with specified arguments and a timeout.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="executable"></param>
        /// <param name="arguments"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        [DebuggerNonUserCode()]
        public static ProcessResult Execute(this IProcessHandler handler, string executable, string arguments, int timeout = 30000)
        {
            var setup = new ProcessSetup()
            {
                Executable = executable, Arguments = arguments
            };

            return handler.Execute(setup, timeout);
        }

        /// <summary>
        /// Executes an external process async with specified arguments and a timeout.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="executable"></param>
        /// <param name="arguments"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        [DebuggerNonUserCode()]
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
