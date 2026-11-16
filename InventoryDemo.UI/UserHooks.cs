using Avalonia.Controls;
using CharacterMainLib;
using InventoryCoreLib;

namespace InventoryDemo.UI;

// This file is intentionally separated from MainWindow.cs.
// Put your demo actions, scenarios and context-menu extensions here.
public sealed partial class MainWindow
{
    private Action<float>? _activeScenario;

    /// <summary>
    /// Called once after the UI and demo character are created.
    /// This is the easiest place to choose a scenario, player avatar or foe.
    /// </summary>
    private void ConfigureDemo()
    {
        // Example:
        // _activeScenario = Scenario01;
        // SetPlayerAvatar("PlayerCharacterAvatar/DefaultPlayer.png");
        //
        // var foe = new TrainingDummy("Orc", 400);
        // SetCurrentFoe(new FoePresentation(
        //     foe.Name,
        //     foe,
        //     "Foes Miniatures/MyOrc.png",
        //     foe.Update,
        //     () => $"HP: {foe.Health}/{foe.MaxHealth}"));

        _activeScenario = Scenario01;
    }

    // Buttons 1-3 are the default demo actions.
    private void Action01PrimaryAttack() => PrimaryAttack();
    private void Action02SecondaryAttack() => SecondaryAttack();
    private void Action03RestoreFoe() => RestoreCurrentFoe();

    // Button 4 starts/stops the current scenario loop.
    private void Action04Scenario() => ToggleScenario();

    // Buttons 5-12 are intentionally empty programmable hooks.
    private void Action05() { }
    private void Action06() { }
    private void Action07() { }
    private void Action08() { }
    private void Action09() { }
    private void Action10() { }
    private void Action11() { }
    private void Action12() { }

    /// <summary>
    /// The scenario loop calls this once per iteration after Character.Update(deltaTime).
    /// Button 4 starts/stops the loop. The loop itself is implemented as while(true).
    /// </summary>
    private void CurrentScenarioUpdate(float deltaTime)
        => _activeScenario?.Invoke(deltaTime);

    /// <summary>
    /// Default empty scenario. Add Scenario02, Scenario03, etc. and select one in ConfigureDemo().
    /// </summary>
    private void Scenario01(float deltaTime)
    {
        // Your simulation code goes here.
        // Example state-machine style logic is recommended instead of Thread.Sleep().
    }

    /// <summary>
    /// Add custom right-click commands here without editing MainWindow.cs.
    /// </summary>
    private void ExtendInventoryContextMenu(
        List<MenuItem> menuItems,
        Position position,
        IOwnedUsable item)
    {
        // Example:
        // var inspect = new MenuItem { Header = "Inspect" };
        // inspect.Click += (_, _) => Console.WriteLine(item.ToString());
        // menuItems.Add(inspect);
    }
}
