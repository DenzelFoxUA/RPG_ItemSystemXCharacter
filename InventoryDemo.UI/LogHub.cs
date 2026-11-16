using System.Collections.ObjectModel;
using System.Text;

namespace InventoryDemo.UI;

public static class LogHub
{
    private static readonly List<string> _history = new();
    private static readonly object _sync = new();

    public static event Action<string>? LineAdded;

    public static IReadOnlyList<string> History
    {
        get
        {
            lock (_sync)
                return _history.ToArray();
        }
    }

    public static void Publish(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        string line = $"[{DateTime.Now:HH:mm:ss}] {message}";
        lock (_sync)
            _history.Add(line);

        LineAdded?.Invoke(line);
    }
}

public sealed class MirroringTextWriter : TextWriter
{
    private readonly TextWriter _original;

    public MirroringTextWriter(TextWriter original) => _original = original;

    public override Encoding Encoding => _original.Encoding;

    public override void WriteLine(string? value)
    {
        _original.WriteLine(value);
        LogHub.Publish(value);
    }

    public override void WriteLine()
    {
        _original.WriteLine();
    }

    public override void Write(char value) => _original.Write(value);
    public override void Write(string? value) => _original.Write(value);
}
