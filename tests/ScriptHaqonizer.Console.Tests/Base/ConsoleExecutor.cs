using System.Diagnostics;
using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace ScriptHaqonizer.Console.Tests.Base;

public record ConsoleRunResult(int ExitCode, string ErrorOutput, string StandardOutput)
{
    public void Log(ITestOutputHelper outputHelper)
    {
        outputHelper.WriteLine(ErrorOutput);
        outputHelper.WriteLine(StandardOutput);
    }
}

internal static class ConsoleExecutor
{
    public static ConsoleRunResult Execute(string args)
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var projectName = typeof(Options).Assembly.GetName().Name!;

        string dest;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            dest = Path.Combine(baseDir, projectName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            dest = Path.Combine(baseDir, projectName + ".exe");
        }
        else
        {
            throw new InvalidOperationException("Launching the console for the current operating system is not implemented.");
        }

        using var cmd = new Process();
        cmd.StartInfo.FileName = dest;
        cmd.StartInfo.Arguments = args;
        cmd.StartInfo.RedirectStandardError = true;
        cmd.StartInfo.RedirectStandardOutput = true;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.StartInfo.UseShellExecute = false;
        cmd.Start();
        cmd.WaitForExit();
        var code = cmd.ExitCode;
        var errorOutput = cmd.StandardError.ReadToEnd();
        var standardOutput = cmd.StandardOutput.ReadToEnd();
        return new ConsoleRunResult(code, errorOutput, standardOutput);
    }
}