using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


using System.Diagnostics;

namespace Kraggs.Util
{
    /// <summary>
    /// Implements executing an external process with overridable functions.
    /// </summary>
    public partial class ProcessHandler : IProcessHandler
    {
        #region IProcessHandler

        /// <summary>
        /// Implements executing an external process with overridable functions.
        /// </summary>
        /// <param name="setup"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public ProcessResult Execute(ProcessSetup setup, int timeout = 30000)
        {
            var result = new ProcessResult()
            {
                Output = new List<string>(),
                Errors = new List<string>()
            };

            using (var process = new Process())
            {
                if(!this.OnSetupProcess(process, setup))
                {
                    // handle erros.
                    result.Errors.Add("OnSetupProcess returned false");
                    return result;
                }
                // we assume user has handled setup correctly.

                process.OutputDataReceived += (sender, args) =>
                {
                    //TODO: should we just give user all output or filter?
                    if (args.Data != null)
                        OnOutputReceived(result, args.Data);
                };
                process.StartInfo.RedirectStandardOutput = true;

                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                        OnErrorReceived(result, args.Data);
                };
                process.StartInfo.RedirectStandardError = true;

                // try to start process.
                try
                {
                    result.WasStarted = process.Start();
                }
                catch(Exception e)
                {
                    // Usually it occurs when an executable file is not found or is not executable
                    // or requires admin privileges.
                    result.Errors.Add(e.Message);
                    return result;
                }

                // start listening on readlines.
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                var flagExitedBeforeTimeout = process.WaitForExit(timeout);

                if (flagExitedBeforeTimeout)
                {
                    process.WaitForExit(); // wait more to ensure events are processed.

                    result.HasCompleted = true;
                    result.ExitCode = process.ExitCode;
                }
                else
                {
                    // kill process.
                    try
                    {
                        process.Kill();
                    }
                    catch { } // ignore error.
                }
                //TODO: should we indicate to user that process was killed?

                return result;
            }

        }

        /// <summary>
        /// Implements executing an external process async with overridable functions.
        /// </summary>
        /// <param name="setup"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public async Task<ProcessResult> ExecuteAsync(ProcessSetup setup, CancellationToken cancel = default)
        {
            var result = new ProcessResult()
            {
                Output = new List<string>(),
                Errors = new List<string>()
            };

            using (var process = new Process())
            {
                if(!OnSetupProcess(process, setup))
                {
                    // handle erros.
                    result.Errors.Add("OnSetupProcess returned false");
                    return result;
                }
                // we assume user has handled setup correctly.

                // setup output event handling.
                var outputCloseEvent = new TaskCompletionSource<bool>();
                process.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data == null)
                        outputCloseEvent.SetResult(true);
                    else
                        OnOutputReceived(result, args.Data);
                };
                process.StartInfo.RedirectStandardOutput = true;

                // setup error event listening.
                var errorCloseEvent = new TaskCompletionSource<bool>();
                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data == null)
                        errorCloseEvent.SetResult(true);
                    else
                        OnErrorReceived(result, args.Data);
                };
                process.StartInfo.RedirectStandardError = true;

                // setup exit event listening.
                var exitCloseEvent = new TaskCompletionSource<bool>();
                process.Exited += (sender, args) =>
                {
                    exitCloseEvent.SetResult(true);
                };
                process.EnableRaisingEvents = true;

                // try to start process.
                try
                {
                    result.WasStarted = process.Start();
                }
                catch (Exception e)
                {
                    // Usually it occurs when an executable file is not found or is not executable
                    // or requires admin privileges.
                    result.Errors.Add(e.Message);
                    return result;
                }

                // start listening on readlines.
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                //TODO: find out how to await a cancellation token.

                /*
                 * There are now 2 types of tasks.
                 * * Those that should all be finished.
                 * * Those that can be either.
                 * 
                 * So wrap the event tasks into one taskProcess which awaits all.
                 * Then create a task which handles waiting and cancellation.
                 * 
                 * At last, wait for which finished first.
                 */

                // wrap all the event task into one task.
                var taskProcess = Task.WhenAll(exitCloseEvent.Task, outputCloseEvent.Task, errorCloseEvent.Task);

                // create a task for handling waiting for exit or cancellation whichever happens first.              
                var taskWaitForExit = OnWaitForExitAsync(process, cancel);

                // this awaits any of event tasks and exit task.
                // when any is finished it waits for result from exit task anyway.
                if(await Task.WhenAny(taskProcess, taskWaitForExit) == taskProcess && taskWaitForExit.Result)
                {
                    result.ExitCode = process.ExitCode;
                    result.HasCompleted = true;                    
                }
                else
                {
                    // process canceled. kill it...
                    try
                    {
                        process.Kill();
                    }
                    catch { } // ignore error.
                }

                return result;
            }

        }

        #endregion

        #region user overridable functions

        /// <summary>
        /// Should return a task that can be waited for that either cancelation is sendt or process exited normally.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        protected virtual Task<bool> OnWaitForExitAsync(Process process, CancellationToken cancel)
        {
            //TODO: Should this really be virtual? Could be static and reused by static alternative version...


            // try to detect if cancellatin token is valid or just default.
            //if (cancel == CancellationToken.None)
            //    return Task.Run(() => process.WaitForExit(Timeout.Infinite));
            //else
            {
                //TODO: Find a better way to wait for process finish OR cancellation.
                //Unfortunately, there seems to be no good way to wait for cancellation.
                return Task.Run(() =>
                {
                    // loop wait for exit and check for cancellation.
                    while (!process.WaitForExit(100)) // cancel has worst case delay of 100ms.
                    {
                        if (cancel.IsCancellationRequested)
                            return false;
                    }

                    // if we got here the process has exited normally.
                    return true;
                });
            }
        }

        /// <summary>
        /// Handles setting the process startup info from setup.
        /// We dont use a processtartinfo object since that would limit what a user could do.
        /// Unfortunately, that also means the user could easily break/expand its usage.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="setup"></param>
        /// <returns></returns>        
        protected virtual bool OnSetupProcess(Process process, ProcessSetup setup)
        {
            process.StartInfo.FileName = setup.Executable;
            if (!string.IsNullOrWhiteSpace(setup.WorkingDir))
                process.StartInfo.WorkingDirectory = setup.WorkingDir;
            process.StartInfo.Arguments = setup.Arguments;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            if (setup.EnvironmentVariables != null)
            {
                //TODO: Should we check existing environmentvariables before just writing ours?
                foreach (var kv in setup.EnvironmentVariables)
                    process.StartInfo.Environment.Add(kv.key, kv.value);
            }

            return true;
        }

        /// <summary>
        /// Gets called when the external process writes to the output pipe.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="data"></param>
        protected virtual void OnOutputReceived(ProcessResult result, string data)
        {
            //TODO: this function might need more context parameters than result and string data...
            result.Output.Add(data);
        }

        /// <summary>
        /// Gets called when the external process writes to the error pipe.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="data"></param>
        protected virtual void OnErrorReceived(ProcessResult result, string data)
        {
            result.Errors.Add(data);
        }

        #endregion
    }
}
