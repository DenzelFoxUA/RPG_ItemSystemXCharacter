
using InventoryCoreLib;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DemoGameLib
{
    public sealed class ItemsCreator
    {
        private readonly EffectsBuilder _effectsBuilder;
        private readonly JsonSerializerOptions _jsonOptions;

        #region [ DATA TRANSFER PACKETS ]

        private sealed class ItemData
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
            public ConsumableData? Consumable { get; init; }
            public ToolData? Tool { get; init; }

        }

        private sealed class EquipableData
        {
            public string EquipmentCategory { get; init; } = "";
            public uint LevelRequired { get; init; }

            public IEnumerable<EffectData> Effects { get; init; } = [];
        }

        private sealed class DurableData
        {
            public uint MaxDurability { get; init; }
            public bool IsIndestructible { get; init; }
        }

        private sealed class AttackData
        {
            public uint MaxDamage { get; init; }

            public double AttackRange { get; init; }
        }

        private sealed class WeaponData
        {
            public AttackData PrimaryAttack { get; init; } = default!;

            public AttackData? SecondaryAttack { get; init; }

            public bool IsTwoHanded { get; init; }
        }

        private sealed class ArmorData
        {
            public uint MaxDefence { get; init; }
        }

        private sealed class ConsumableData
        {
            public bool IsSelfApplycable { get; init; }
            public string ConsumableType { get; init; } = "";
            public uint Volume { get; init; }

            //для кастомних ефектів на майбутнє
            public IEnumerable<EffectData> CustomEffects { get; init; } = [];
        }

        private sealed class ToolData
        {
            public uint MaxNumberOfUses { get; init; }
        }

        #endregion

        // ====================== LOAD FILES ==================================

        private Dictionary<uint, ItemData> LoadItemsFromFiles()
        {
            Dictionary<uint, ItemData> tempList = new();

            string path = Path.Combine(
                        AppContext.BaseDirectory,
                        "JSON_Data",
                        "Items"
                        );

            IEnumerable<string> files = 
                Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);

            //if (files.Count() == 0)
            //    throw new FileLoadException($"NO JSON files found in: {path}!");

            foreach (var file in files)
            {
                string resultJson = File.ReadAllText(file);

                ItemData currentItem = JsonSerializer.Deserialize<ItemData>(resultJson, _jsonOptions)
                ?? throw new InvalidDataException($"::ERR_DESERIALIZATION::\n File: {file}.");

                tempList.Add(currentItem.Id, currentItem);
            }

            return tempList;
        }

        // ==========================================================================

        #region [ BUILD WEAPON ] ===================================================

        private AttackDefinition BuildAttack(AttackData data)
        {
            return new AttackDefinition 
            {
                AttackRange = data.AttackRange,
                MaxDamage = data.MaxDamage
            };
        }

        private IAffect[] BuildItemEffects(ItemData data)
        {
            if (data.Equipable!.Effects is null)
                return [];

            return _effectsBuilder.Build(data.Equipable.Effects);
        }

        private BaseItemInstance BuildWeapon(ItemData weapon)
        {
            switch (weapon.Type)
            {
                case "Dagger": return BuildDagger(weapon);
                //case "Sword": return BuildSword(weapon);
                //case "Bow": return BuildBow(weapon);

                default:  throw new InvalidDataException(
                    $"Unsupported item type: {weapon.Type}");
            };
        }

        private DaggerInstance BuildDagger(ItemData data)
        {
            ValidateWeaponData(data);

            EquipableData equipablePart = data.Equipable!;
            DurableData durablePart = data.Durable!;
            WeaponData weapon = data.Weapon!;

            AttackDefinition primaryAttack = 
                BuildAttack(weapon.PrimaryAttack);

            AttackDefinition? secondaryAttack =
                weapon.SecondaryAttack is not null ?
                BuildAttack(weapon.SecondaryAttack)
                    : null;

            DaggerDefinition definition = new(
                data.Id,
                data.Name, 
                data.Description, 
                data.Weight,
                data.Price,
                durablePart.MaxDurability,
                equipablePart.LevelRequired,
                primaryAttack,
                secondaryAttack,
                durablePart.IsIndestructible,
                data.Icon,
                data.Miniature2D);

            IAffect [] effects =
                BuildItemEffects(data);

            return new DaggerInstance(
                definition,
                effects);
        }

        #endregion

        #region [ BUILD ARMOR ] ====================================================

        private BaseItemInstance BuildArmor(ItemData armor)
        {
            ValidateArmorData(armor);

            EquipableData equipablePart = armor.Equipable!;
            DurableData durablePart = armor.Durable!;
            ArmorData armorPart = armor.Armor!;

            ArmorDefinition definition = new ArmorDefinition(
                armor.Id,
                armor.Name,
                armor.Description,
                armorPart.MaxDefence,
                armor.Price,
                armor.Weight,
                durablePart.MaxDurability,
                equipablePart.EquipmentCategory,
                armor.Icon,
                armor.Miniature2D,
                durablePart.IsIndestructible
                );

            IAffect[] effects = BuildItemEffects(armor);

            return equipablePart.EquipmentCategory switch
            {
                "Body" => new BodyArmorInstance(definition, effects),
                "Head" => new HelmetInstance(definition,effects),
                "Gloves" => new GlovesInstance(definition, effects),
                "Boots" => new BootsInstance(definition, effects),
                "Shield" => new ShieldInstance(definition, effects),
                _ => throw new InvalidDataException(
                    $"Unsupported item type: {armor.Type}")
            };
        }

        #endregion

        #region [ BUILD JEWELRY ] ==========================================

        private BaseItemInstance BuildJewelry(ItemData trinkett)
        {
            if (trinkett.Equipable is null)
            {
                throw new InvalidDataException(
                    $"{trinkett.Name}: Equipable data is missing.");
            }

            EquipableData equipablePart = trinkett.Equipable!;

            JewelryDefinition definition = new JewelryDefinition(
                trinkett.Id,
                trinkett.Name,
                trinkett.Description,
                trinkett.Price,
                trinkett.Weight,
                equipablePart.EquipmentCategory,
                trinkett.Icon,
                trinkett.Miniature2D
                );

            IAffect[] effects = BuildItemEffects(trinkett);

            return new JewelryInstance(definition, effects);
        }

        #endregion

        #region [ BUILD CONSUMABLE ] =======================================

        private BaseItemInstance BuildConsumable(ItemData data)
        {
            if (data.Consumable is null)
            {
                throw new InvalidDataException(
                    $"{data.Name}: Consumable data is missing.");
            }

            ConsumableData consumable = data.Consumable;

            return consumable.ConsumableType switch
            {
                "Potion" => BuildPotion(data, consumable),
                "Food" => BuildFood(data, consumable),

                _ => throw new InvalidDataException(
                    $"Unsupported consumable type: '{data.Type}'.")
            };
        }

        private BaseItemInstance BuildPotion(ItemData data, ConsumableData consumable)
        {
            PotionDefinition definition =
                new PotionDefinition(
                    data.Id,
                    data.Name,
                    data.Description,
                    consumable.Volume,
                    data.Weight,
                    data.Price,
                    data.MaxStackSize,
                    data.Icon,
                    data.Miniature2D);

            IAffect[] effects = _effectsBuilder.Build(consumable.CustomEffects);

            return new PotionInstance(definition, 1, effects);
        }

        private BaseItemInstance BuildFood(ItemData data,ConsumableData consumable)
        {
            IAffect[] effects = _effectsBuilder.Build(consumable.CustomEffects);

            FoodDefinition definition = new FoodDefinition(
                data.Id,
                data.Name,
                data.Description,
                data.Weight,
                data.Price,
                data.MaxStackSize,
                data.Icon,
                data.Miniature2D);

            return new FoodInstance(definition, 1, effects);
        }

        #endregion


        // ======================== VALIDATORS ======================================

        private void ValidateWeaponData(ItemData data)
        {
            if (data.Equipable is null)
                throw new InvalidDataException(
                    $"{data.Name}: Equipable data is missing.");

            if (data.Durable is null)
                throw new InvalidDataException(
                    $"{data.Name}: Durable data is missing.");

            if (data.Weapon is null)
                throw new InvalidDataException(
                    $"{data.Name}: Weapon data is missing.");

            if (data.Weapon.PrimaryAttack is null)
                throw new InvalidDataException(
                    $"{data.Name}: Primary attack is missing.");
        }

        private void ValidateArmorData(ItemData armor)
        {
            if (armor.Equipable is null)
            {
                throw new InvalidDataException(
                    $"{armor.Name}: Equipable data is missing.");
            }

            if (armor.Durable is null)
            {
                throw new InvalidDataException(
                    $"{armor.Name}: Durable data is missing.");
            }

            if (armor.Armor is null)
            {
                throw new InvalidDataException(
                    $"{armor.Name}: Armor data is missing.");
            }
        }


        // ===========================================================================

        private FrozenDictionary<uint, ItemData>? ITEMS_LIB { get; init; }

        public ItemsCreator()
        {
            _effectsBuilder = new EffectsBuilder();

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            options.Converters.Add(new EffectDataConverter());

            _jsonOptions = options;

            ITEMS_LIB = LoadItemsFromFiles().ToFrozenDictionary();
        }

        public IOwnedUsable BuildItem(uint id)
        {
            if (ITEMS_LIB is null)
                throw new ObjectNotInitializedException(nameof(ITEMS_LIB));

            if (!ITEMS_LIB.TryGetValue(id, out ItemData? item))
                throw new KeyNotFoundException($"Item with ID {id} not found.");

            BaseItemInstance result = item.Category switch
            {
                "Weapon" => BuildWeapon(item),
                "Armor" =>  BuildArmor(item),
                "Consumable" => BuildConsumable(item),
                "Jewelry" => BuildJewelry(item),
                //"Tool" => break;
                //"Scroll" => break;
                //"Recharger" => break;
                _ => throw new InvalidDataException(
                    $"Unsupported item category: '{item.Category}'.")
            };

            return result as IOwnedUsable ?? throw new InvalidCastException(
                    $"{result.GetType().Name} doesn't implement IOwnedUsable.");
        }

        public IEnumerable<(uint Id, string Name)> GetAvailableItems()
        {
            foreach (var item in ITEMS_LIB.Values)
            {
                yield return (item.Id,item.Name);
            }
        }

    }
}
