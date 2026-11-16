using CharacterMainLib;
using InventoryCoreLib;
namespace InventoryDemo.UI;

public sealed class FoePresentation
{
    public string Name { get; }
    public IDemoActor Actor { get; }
    public string? MiniaturePath { get; }
    public Action<float>? UpdateAction { get; }
    public Func<string>? StatusText { get; }

    public FoePresentation(
        string name,
        IDemoActor actor,
        string? miniaturePath = null,
        Action<float>? updateAction = null,
        Func<string>? statusText = null)
    {
        Name = name;
        Actor = actor;
        MiniaturePath = miniaturePath;
        UpdateAction = updateAction;
        StatusText = statusText;
    }
}
