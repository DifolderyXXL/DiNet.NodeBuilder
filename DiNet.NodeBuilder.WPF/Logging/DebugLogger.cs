using System.Diagnostics;

namespace DiNet.NodeBuilder.WPF.Logging;
public class DebugLogger : ILogger
{
    public static DebugLogger Shared { get; } = new ();

    public void Log(object? value)
    {
#if DEBUG
        Debug.WriteLine(value);
#endif
    }
}

