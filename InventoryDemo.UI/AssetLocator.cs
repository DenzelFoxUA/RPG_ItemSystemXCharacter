using Avalonia.Media.Imaging;

namespace InventoryDemo.UI;

public static class AssetLocator
{
    private static readonly Dictionary<string, Bitmap> _cache = new(StringComparer.OrdinalIgnoreCase);

    public static string? Resolve(string? resourcePath)
    {
        if (string.IsNullOrWhiteSpace(resourcePath))
            return null;

        if (Path.IsPathRooted(resourcePath) && File.Exists(resourcePath))
            return resourcePath;

        string relative = resourcePath
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar)
            .TrimStart(Path.DirectorySeparatorChar);

        if (relative.StartsWith($"Resources{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            relative = relative[("Resources".Length + 1)..];

        foreach (string root in EnumerateResourceRoots())
        {
            string candidate = Path.Combine(root, relative);
            if (File.Exists(candidate))
                return candidate;
        }

        return null;
    }

    public static Bitmap? LoadBitmap(string? resourcePath)
    {
        string? path = Resolve(resourcePath);
        if (path is null)
            return null;

        if (_cache.TryGetValue(path, out Bitmap? cached))
            return cached;

        try
        {
            var bitmap = new Bitmap(path);
            _cache[path] = bitmap;
            return bitmap;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cannot load image '{resourcePath}': {ex.Message}");
            return null;
        }
    }

    private static IEnumerable<string> EnumerateResourceRoots()
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (string start in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
        {
            DirectoryInfo? current = new DirectoryInfo(start);
            for (int i = 0; i < 8 && current is not null; i++, current = current.Parent)
            {
                foreach (string candidate in new[]
                {
                    Path.Combine(current.FullName, "Resources"),
                    Path.Combine(current.FullName, "RPGInventoryProject", "Resources"),
                    Path.Combine(current.FullName, "InventoryMain Lib", "Resources"),
                    Path.Combine(current.FullName, "InventoryMainLib", "Resources")
                })
                {
                    if (Directory.Exists(candidate) && seen.Add(candidate))
                        yield return candidate;
                }
            }
        }
    }
}
