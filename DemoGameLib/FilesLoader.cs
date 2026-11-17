
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DemoGameLib
{
    internal static class FilesLoader
    {
        internal static Dictionary<uint, DTO.ItemData> 
            LoadItemsFromFiles(JsonSerializerOptions options)
        {
            Dictionary<uint, DTO.ItemData> tempList = new();

            string path = Path.Combine(
                        AppContext.BaseDirectory,
                        "JSON_Data",
                        "Items"
                        );

            IEnumerable<string> files =
                Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                string resultJson = File.ReadAllText(file);

                DTO.ItemData currentItem =
                    JsonSerializer.Deserialize<DTO.ItemData>(resultJson, options) ??
                    throw new InvalidDataException
                    ($"::ERR_DESERIALIZATION::\n File: {file}.");

                tempList.Add(currentItem.Id, currentItem);
            }

            return tempList;
        }
    }
}
