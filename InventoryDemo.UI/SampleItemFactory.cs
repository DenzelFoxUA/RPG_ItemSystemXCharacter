using DemoGameLib;
using InventoryCoreLib;

namespace InventoryDemo.UI;

public sealed record PickupEntry(string Name, Func<IOwnedUsable> Create)
{
    public override string ToString() => Name;
}

public static class SampleItemFactory
{
    private const string Icons = "Icons/";
    private const string Miniatures = "Miniatures2D/";

    private static readonly ItemsCreator _itemsCreator = new();

    ////private static readonly PotionDefinition HealthPotionDefinition = new(
    ////    1, "Health Potion", "Restores 25 HP.", 25, 0.2, 20,
    ////    Icons + "HealthPotion.png", Miniatures + "HealthPotion.png");

    ////private static readonly PotionDefinition ManaPotionDefinition = new(
    ////    2, "Mana Potion", "Restores 20 mana.", 20, 0.2, 18,
    ////    Icons + "ManaPotion.png", Miniatures + "ManaPotion.png");

    ////private static readonly DaggerDefinition DaggerDefinition = new(
    ////    10, "Iron Dagger", "Simple one-handed dagger.", 1.1, 75,
    ////    100, 1, 18, false,
    ////    Icons + "Dagger.png", Miniatures + "Dagger.png");

    //private static readonly BodyArmor LeatherArmorDefinition = new(
    //    20, "Leather Armor", "Light armor. Adds defence and +3 Strength while equipped.",
    //    35, 140, 5.0, 120,
    //    Icons + "LeatherArmor.png", Miniatures + "LeatherArmor.png");

    //private static readonly AmuletDefinition AmuletDefinition = new(
    //    30, "Copper Amulet", "Demo amulet. Gives +1 Strength while equipped.",
    //    0, 80, 0.2, 0,
    //    Icons + "Amulet.png", Miniatures + "Amulet.png");


    //public static DaggerInstance? CreateDagger()
    //{
    //    GameCore core = new();

    //    var dagger = core.ItemsCreator.BuildItem(144);

    //    if (dagger is not null && dagger is DaggerInstance result)
    //        return result;
    //    else
    //        return null;
    //}

    //public static HealthPotion? CreateHealthPotion()
    //{
    //    GameCore core = new();

    //    var potion = core.ItemsCreator.BuildItem(501);

    //    return potion as HealthPotion;
    //}

    //public static ManaPotion? CreateManaPotion()
    //{
    //    GameCore core = new();

    //    var potion = core.ItemsCreator.BuildItem(502);

    //    return potion as ManaPotion;
    //}

    //public static StaminaPotion? CreateStaminaPotion()
    //{
    //    GameCore core = new();

    //    var potion = core.ItemsCreator.BuildItem(503);

    //    return potion as StaminaPotion;
    //}

    public static IReadOnlyList<PickupEntry> Entries { get; } =
    _itemsCreator
        .GetAvailableItems()
        .Select(item =>
            new PickupEntry(
                item.Name,
                () => _itemsCreator.BuildItem(item.Id)))
        .ToArray();
}
