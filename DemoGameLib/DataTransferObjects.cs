
using System.Collections.Generic;

namespace DemoGameLib.DTO
{
    #region [ DATA TRANSFER OBJECTS ]

    internal sealed class ItemData
    {
        public string Category { get; init; } = "";
        public string Type { get; init; } = "";

        public uint Id { get; init; }
        public string Name { get; init; } = "";
        public string Description { get; init; } = "";

        public double Weight { get; init; }
        public uint Price { get; init; }

        public string Icon { get; init; } = "";
        public string Miniature2D { get; init; } = "";

        public bool IsStackable { get; init; }
        public uint MaxStackSize { get; init; }


        //optional
        public EquipableData? Equipable { get; init; }
        public DurableData? Durable { get; init; }
        public WeaponData? Weapon { get; init; }
        public ArmorData? Armor { get; init; }
        public AmmoData? Ammo { get; init; }
        public ConsumableData? Consumable { get; init; }
        public ToolData? Tool { get; init; }

    }

    internal sealed class EquipableData
    {
        public string EquipmentCategory { get; init; } = "";
        public uint LevelRequired { get; init; }

        public IEnumerable<EffectData> Effects { get; init; } = [];
    }

    internal sealed class DurableData
    {
        public uint MaxDurability { get; init; }
        public bool IsIndestructible { get; init; }
    }

    internal sealed class AttackData
    {
        public uint MaxDamage { get; init; }

        public double AttackRange { get; init; }
    }

    internal sealed class WeaponData
    {
        public AttackData PrimaryAttack { get; init; } = default!;

        public AttackData? SecondaryAttack { get; init; }

        public bool IsTwoHanded { get; init; }
    }

    internal sealed class ArmorData
    {
        public uint MaxDefence { get; init; }
    }

    internal sealed class ConsumableData
    {
        public bool IsSelfApplycable { get; init; }
        public string ConsumableType { get; init; } = "";
        public uint Volume { get; init; }

        //для кастомних ефектів на майбутнє
        public IEnumerable<EffectData> CustomEffects { get; init; } = [];
    }

    internal sealed class ToolData
    {
        public string ToolType { get; init; } = "";
        public uint UseValue { get; init; }
    }

    internal sealed class AmmoData
    {
        public string AmmoType { get; init; } = "";
    
    }


    #endregion
}
