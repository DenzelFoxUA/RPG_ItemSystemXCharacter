using CharacterMainLib;
using InventoryCoreLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DemoGameLib
{
    public static class CharacterCreator
    {
        #region [ CHARACTER CONFIG PACKET]

        public sealed class AttributeConfig
        {
            public uint CurrAttributeValue { get; init; }
        }

        public sealed class CharacterConfig
        {
            public string Name { get; init; } = "";

            public CharacterBase BaseFeatures { get; init; } = new();

            public Dictionary<string, AttributeConfig> Attributes
            { get; init; } = new();
        }

        #endregion


        public static DemoCharacter CreateDefault()
        {
            var features = new CharacterBase
            {
                MaxLevel = 99,
                BaseSpeed = 5.0f,
                MaxSpeed = 12.0f,
                BaseCarryWeight = 30.0,
                MinCarryWeight = 0.0,
                MaxCarryWeight = 120.0,
                MinAttributeValue = 5,
                MaxAttributeValue = 100
            };

            var player = new DemoCharacter(
                "Corvo Attano",
                features,
                new InventorySize(4, 6),
                CharDefaultModel.Vitals,
                CharDefaultModel.Attributes,
                CharDefaultModel.EquipmentSection);

            player.DamageVital("Health", 40);

            return player;
        }

        public static DemoCharacter CreateFromJson(string charClass,InventorySize inventorySize)
        {

            string path = Path.Combine(
                        AppContext.BaseDirectory,
                        "JSON_Data",
                        "Characters",
                        $"{ charClass }.json");

            string json = File.ReadAllText(path);

            CharacterConfig config = JsonSerializer.Deserialize<CharacterConfig>(json)
                ?? throw new InvalidDataException("ERR:: Invalid character configuration.");

            var character = new DemoCharacter(
                config.Name,
                config.BaseFeatures,
                inventorySize,
                CharDefaultModel.Vitals,
                CharDefaultModel.Attributes,
                CharDefaultModel.EquipmentSection);


            foreach (var attribute in config.Attributes)
            {
                string name = attribute.Key;
                uint desired = attribute.Value.CurrAttributeValue;

                if (desired < config.BaseFeatures.MinAttributeValue ||
                    desired > config.BaseFeatures.MaxAttributeValue)
                {
                    throw new InvalidDataException($"Invalid initial attribute value: {name}");
                }

                uint current = character.AttributeStats[name].CurrentValue;

                if (desired > current)
                    character.IncreaseAttribute(name, desired - current);
                else if (desired < current)
                    character.DecreaseAttribute(name, current - desired);
            }

            return character;
        }

    }
}
