using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CharacterMainLib
{
    public static class CharDefaultModel
    {
        public static FrozenDictionary<string, VitalsBaseValues> Vitals { get; } =
            new Dictionary<string, VitalsBaseValues>
            {
                { "Health",new VitalsBaseValues(100,500) },
                { "Mana", new VitalsBaseValues(100,400) },
                { "Stamina", new VitalsBaseValues(100,400) }

            }.ToFrozenDictionary();

        public static ImmutableArray<string> Attributes { get; } =
            [
                "Dexterity",
                "Strength",
                "Willpower",
                "Endurance",
                "Luck",
                "Intelligence"
            ];

        public static ImmutableArray<string> EquipmentSection { get; } =
            [
                "Head",
                "Body",
                "Gloves",
                "Boots",
                "Ring",
                "Amulet",
                "Weapon",
                "Shield",
                "Ammo"
            ];

    }
}
