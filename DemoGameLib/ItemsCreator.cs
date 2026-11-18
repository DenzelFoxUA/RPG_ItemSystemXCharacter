
using InventoryCoreLib;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DemoGameLib
{
    public sealed class ItemsCreator
    {
        private readonly EffectsBuilder _effectsBuilder;
        private readonly JsonSerializerOptions _jsonOptions;


        #region ======== [ BUILD WEAPON ] =========================================

        private AttackDefinition BuildAttack(DTO.AttackData data)
        {
            return new AttackDefinition 
            {
                AttackRange = data.AttackRange,
                MaxDamage = data.MaxDamage
            };
        }

        private IAffect[] BuildItemEffects(DTO.ItemData data)
        {
            if (data.Equipable!.Effects is null)
                return [];

            return _effectsBuilder.Build(data.Equipable.Effects);
        }

        private BaseItemINS BuildWeapon(DTO.ItemData weapon)
        {
            switch (weapon.Type)
            {
                case "Melee": return BuildMelee(weapon);
                case "Ranged": return BuildRanged(weapon);
                //case "Bow": return BuildBow(weapon);

                default:  throw new InvalidDataException(
                    $"Unsupported item type: {weapon.Type}");
            };
        }

        private MeleeWeaponINS BuildMelee(DTO.ItemData data)
        {
            ValidateWeaponData(data);

            DTO.EquipableData equipablePart = data.Equipable!;
            DTO.DurableData durablePart = data.Durable!;
            DTO.WeaponData weapon = data.Weapon!;

            AttackDefinition primaryAttack = 
                BuildAttack(weapon.PrimaryAttack);

            AttackDefinition? secondaryAttack =
                weapon.SecondaryAttack is not null ?
                BuildAttack(weapon.SecondaryAttack)
                    : null;

            MeleeDefinition definition = new(
                data.Id,
                data.Name, 
                data.Description, 
                data.Weight,
                data.Price,
                durablePart.MaxDurability,
                equipablePart.LevelRequired,
                weapon.IsTwoHanded,
                primaryAttack,
                secondaryAttack,
                durablePart.IsIndestructible,
                data.Icon,
                data.Miniature2D);

            IAffect [] effects =
                BuildItemEffects(data);

            return new MeleeWeaponINS(
                definition,
                effects);
        }

        private RangedINS BuildRanged(DTO.ItemData data)
        {
            ValidateWeaponData(data);

            if(data.Ammo is null)
            {
                throw new InvalidDataException(
                    $"{data.Name}: Ammo data is missing.");
            }

            DTO.EquipableData equipablePart = data.Equipable!;
            DTO.DurableData durablePart = data.Durable!;
            DTO.WeaponData weapon = data.Weapon!;
            DTO.AmmoData ammo = data.Ammo!;

            AttackDefinition primaryAttack =
                BuildAttack(weapon.PrimaryAttack);

            AttackDefinition? secondaryAttack =
                weapon.SecondaryAttack is not null ?
                BuildAttack(weapon.SecondaryAttack)
                    : null;

            RangedWeapon definition = new RangedWeapon(
                data.Id,
                data.Name,
                ammo.AmmoType,
                data.Description,
                data.Weight,
                data.Price,
                durablePart.MaxDurability,
                equipablePart.LevelRequired,
                primaryAttack,
                secondaryAttack,
                durablePart.IsIndestructible,
                false,
                data.Icon,
                data.Miniature2D);

            IAffect[] effects =
                BuildItemEffects(data);

            return new RangedINS(
                definition,
                effects);
        }

        #endregion

        #region ====== [ BUILD AMMO ] ===============================================

        private BaseItemINS BuildAmmo(DTO.ItemData ammo)
        {
            DTO.AmmoData ammoData = ammo.Ammo!;

            AmmoDefinition definition = new AmmoDefinition(
                ammo.Id,
                ammoData.AmmoType,
                ammo.Name,
                ammo.Description,
                ammo.Weight,
                ammo.Price,
                ammo.MaxStackSize,
                ammo.Icon,
                ammo.Miniature2D
                );

            IAffect[] effects = BuildItemEffects(ammo);

            return new AmmoINS(definition, ammo.MaxStackSize, effects);
        }

        #endregion

        #region =========== [ BUID TOOLS ] ====================================

        private BaseItemINS BuildTool(DTO.ItemData itemData)
        {

            if (itemData.Tool is null)
                throw new ObjectNotInitializedException(nameof(itemData.Tool));

            DTO.ToolData tool = itemData.Tool;

            ToolDefinition definition = new ToolDefinition(
                itemData.Id,
                itemData.Name,
                tool.UseValue,
                itemData.Description,
                itemData.Weight,
                itemData.Price,
                itemData.MaxStackSize,
                itemData.Icon,
                itemData.Miniature2D);

            return tool.ToolType switch
            {
                "Repair" => new RepairTool(definition,definition.MaxStackSize),
                //"Lockpick" => new HelmetINS(definition, effects),
                _ => throw new InvalidDataException(
                    $"Unsupported item type: {itemData.Type}")
            };
        }

        #endregion

        #region ======== [ BUILD ARMOR ] ==============================================

        private BaseItemINS BuildArmor(DTO.ItemData armor)
        {
            ValidateArmorData(armor);

            DTO.EquipableData equipablePart = armor.Equipable!;
            DTO.DurableData durablePart = armor.Durable!;
            DTO.ArmorData armorPart = armor.Armor!;

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
                "Body" => new BodyArmorINS(definition, effects),
                "Head" => new HelmetINS(definition,effects),
                "Gloves" => new GlovesINS(definition, effects),
                "Boots" => new BootsINS(definition, effects),
                "Shield" => new ShieldINS(definition, effects),
                _ => throw new InvalidDataException(
                    $"Unsupported item type: {armor.Type}")
            };
        }

        #endregion

        #region ======== [ BUILD JEWELRY ] ==========================================

        private BaseItemINS BuildJewelry(DTO.ItemData trinkett)
        {
            if (trinkett.Equipable is null)
            {
                throw new InvalidDataException(
                    $"{trinkett.Name}: Equipable data is missing.");
            }

            DTO.EquipableData equipablePart = trinkett.Equipable!;

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

        #region ======== [ BUILD CONSUMABLE ] =======================================

        private BaseItemINS BuildConsumable(DTO.ItemData data)
        {
            if (data.Consumable is null)
            {
                throw new InvalidDataException(
                    $"{data.Name}: Consumable data is missing.");
            }

            DTO.ConsumableData consumable = data.Consumable;

            return consumable.ConsumableType switch
            {
                "Potion" => BuildPotion(data, consumable),
                "Food" => BuildFood(data, consumable),

                _ => throw new InvalidDataException(
                    $"Unsupported consumable type: '{data.Type}'.")
            };
        }

        private BaseItemINS BuildPotion(DTO.ItemData data, 
            DTO.ConsumableData consumable)
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

            return new PotionINS(definition, 1, effects);
        }

        private BaseItemINS BuildFood(DTO.ItemData data, 
            DTO.ConsumableData consumable)
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

            return new FoodINS(definition, 1, effects);
        }

        #endregion

        #region ========= [ DTO VALIDATORS ] ============================================

        private void ValidateWeaponData(DTO.ItemData data)
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

        private void ValidateArmorData(DTO.ItemData armor)
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

        #endregion

        private FrozenDictionary<uint, DTO.ItemData>? ITEMS_LIB { get; init; }

        #region ==== [CREATOR CTOR] =============================================

        public ItemsCreator()
        {
            _effectsBuilder = new EffectsBuilder();

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            options.Converters.Add(new EffectDataConverter());

            _jsonOptions = options;

            ITEMS_LIB = FilesLoader.LoadItemsFromFiles(_jsonOptions)
                .ToFrozenDictionary();
        }

        #endregion

        public IOwnedUsable BuildItem(uint id)
        {
            if (ITEMS_LIB is null)
                throw new ObjectNotInitializedException(nameof(ITEMS_LIB));

            if (!ITEMS_LIB.TryGetValue(id, out DTO.ItemData? item))
                throw new KeyNotFoundException($"Item with ID {id} not found.");

            BaseItemINS result = item.Category switch
            {
                "Weapon" => BuildWeapon(item),
                "Armor" =>  BuildArmor(item),
                "Consumable" => BuildConsumable(item),
                "Jewelry" => BuildJewelry(item),
                "Ammo" => BuildAmmo(item),
                "Tool" => BuildTool(item),
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
            if (ITEMS_LIB is null)
                throw new ObjectNotInitializedException(nameof(ITEMS_LIB));

            foreach (var item in ITEMS_LIB.Values)
            {
                yield return (item.Id,item.Name);
            }
        }

    }
}
