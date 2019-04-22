using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

using System.Diagnostics;

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
            var result = new ProcessResult();

            using (var process = new Process())
            {
                SetupProcess(process, setup);

                // setup output handling.
                result.Output = new List<string>();
                process.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                        result.Output.Add(args.Data);
                };
                process.StartInfo.RedirectStandardOutput = true;

                // setup error handling
                result.Errors = new List<string>();
                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                        result.Errors.Add(args.Data);
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

                // start listening for readlines.
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                var flagTimedOut = process.WaitForExit(timeout);

                if (!flagTimedOut)
                    process.WaitForExit(); // wait more for redirect handlers to cmplete.
                else
                {
                    try
                    {
                        // kill hung process.
                        process.Kill();
                    }
                    catch
                    { } // ignore error.
                }

                result.HasCompleted = true;
                result.ExitCode = process.ExitCode;
                return result;                
            }
        }

        public static async Task<ProcessResult> RunProcessAsync(ProcessSetup setup, CancellationToken cancel = default)
        {
            var result = new ProcessResult();

            using (var process = new Process())
            {
                SetupProcess(process, setup);

                // setup output handling.
                result.Output = new List<string>();
                var outputCloseEvent = new TaskCompletionSource<bool>();
                process.OutputDataReceived += (sender, args) =>
                {
                    var d = args.Data;
                    //if (string.IsNullOrWhiteSpace(d))
                    if(d == null)
                        outputCloseEvent.SetResult(true);
                    else                    
                        result.Output.Add(d);
                };
                process.StartInfo.RedirectStandardOutput = true;

                // setup error handling
                result.Errors = new List<string>();
                var errorCloseEvent = new TaskCompletionSource<bool>();
                process.ErrorDataReceived += (sender, args) =>
                {
                    var e = args.Data;
                    if (e == null)
                        errorCloseEvent.SetResult(true);
                    else
                        result.Errors.Add(args.Data);
                };
                process.StartInfo.RedirectStandardError = true;

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

                // start listening for readlines.
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                //TODO: Change timeout code to use cancellation instead.
                int timeout = 30000;

                var waitForExit = WaitForExitAsync(process, timeout);

                var processTask = Task.WhenAll(waitForExit, outputCloseEvent.Task, errorCloseEvent.Task);

                if(await Task.WhenAny(Task.Delay(timeout), processTask) == processTask && waitForExit.Result)
                {
                    result.HasCompleted = true;
                    result.ExitCode = process.ExitCode;                    
                }
                else
                {
                    try
                    {
                        process.Kill();
                    }
                    catch { } // ignore error.                    
                }

                return result;
            }
        }

        private static Task<bool> WaitForExitAsync(Process process, int timeout)
        {
            return Task.Run(() => process.WaitForExit(timeout));
        }

        private static Task<bool> WaitForExitAsync(Process process, CancellationToken cancel)
        {
            return Task.Run(() => 
            {
                // wait for 100ms, check cancellation requested, if not wait again until exited.
                while(!process.WaitForExit(100))
                {
                    if (cancel.IsCancellationRequested)
                        return false; // true of false here?
                }

                return true;
            });
        }

        private static void SetupProcess(Process process, ProcessSetup setup)
        {
            process.StartInfo.FileName = setup.Executable;
            if (!string.IsNullOrWhiteSpace(setup.WorkingDir))
                process.StartInfo.WorkingDirectory = setup.WorkingDir;
            process.StartInfo.Arguments = setup.Arguments;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            if(setup.EnvironmentVariables != null)
            {
                foreach (var kv in setup.EnvironmentVariables)
                    process.StartInfo.Environment.Add(kv.key, kv.value);
            }

            // setup events and delegates.

        }

        #endregion
    }
}
