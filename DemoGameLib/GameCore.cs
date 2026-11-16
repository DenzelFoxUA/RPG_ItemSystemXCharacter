using InventoryCoreLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoGameLib
{
    public sealed record PickupEntry(string Name, Func<IOwnedUsable> Create)
    {
        public override string ToString() => Name;
    }

    public static class GameCore
    {
        private static readonly ItemsCreator _itemsCreator = new();

        public static IReadOnlyList<PickupEntry> Entries { get; } =
        _itemsCreator
            .GetAvailableItems()
            .Select(item => new PickupEntry(
                    item.Name, () => _itemsCreator.BuildItem(item.Id)))
            .ToArray();
    }
}
