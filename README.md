## Kraggs.Util.ProcessHandler
A small little library for handling running external processes.


#### Goals:
* Handle process lifetime and exit.
* Handle running processes as a task async.
* Put an interface between running a process and handling the result.


#### Usage:

*Very simple static usage:*

```csharp
using Kraggs.Util;

var setup = new ProcessSetup()
{
    Executable = "ping.exe",
    Arguments = "127.0.0.1"
};

var taskPing = ProcessHandler.RunProcessAsync(setup);

```

*Extension usage:*

```csharp
using Kraggs.Util;

var handler = new ProcessHandler();

var taskPing = handler.ExecuteAsync("ping.exe", "127.0.0.1");

```

*Intended usage:*

By seperating the execution and error handling with an interface, it should be possible to unit test the process handling code.

```csharp
using Kraggs.Util;

// Code to handle the Ping executable.
public class PingHandler
{
    private IProcessHandler pHandler;

    public PingClass(IProcessHandler handler = null)
    {
        // optionally using mocking handler for unit testing.
        if(handler != null)
            pHandler = handler;	
        else
            pHandler = new ProcessHandler();
    }

    public async Task<ProcessResult> Ping(string hostname, int count)
    {
        var setup = new ProcessSetup();
        if(Windows) {
            setup.Executable = "C:\Windows\System32\ping.exe";
            setup.Arguments = $" -n {count} {hostname}";
        } else {
            setup.Executable = "/sbin/ping";
            setup.Arguments = $" -c {count} {hostname}";
        }
		
        //TODO: handle the output of ping accordingly.
        return await pHandler.ExecuteAsync(setup);
    }
}
```

#### Disclaimer
I am not a professional programmer so use this code on you own risk.


The static code for running a process async is based on the gists [AlexMAS](https://gist.github.com/AlexMAS/276eed492bc989e13dcce7c78b9e179d) and [georg-jung](https://gist.github.com/georg-jung/3a8703946075d56423e418ea76212745).

I modified it to use a cancellation token rather than a timeout, and probably added some bugs on the way.
A version of the code using a timout is still present in the code.

#### License
MIT