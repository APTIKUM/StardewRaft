using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using StardewRaft.Core.Feature;

namespace StardewRaft.Core.Factories
{
    public static class SeaTrashFactory
    {
        private static Random _rnd = new Random();
        private class SeaTrashTypeConfig
        {
            [JsonPropertyName("resourceType")]
            public string ResourceType { get; set; }

            [JsonPropertyName("spawnWeight")]
            public double SpawnWeight { get; set; }

            [JsonPropertyName("categories")]
            public List<SeaTrashCategoriesConfig> Categories { get; set; }
        }
        private class SeaTrashCategoriesConfig
        {
            [JsonPropertyName("skin")]
            public string Skin { get; set; }

            [JsonPropertyName("colliderSize")]
            public int[] ColliderSize { get; set; }
        }

        private static readonly Dictionary<SeaTrashType, SeaTrashTypeConfig> _seaTrashTypeConfig;
        private static readonly Dictionary<SeaTrashType, double> _seaTrashWeights;

        static SeaTrashFactory()
        {
            var configPath = Path.Combine(
                            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                            "Core", "Configs", "sea_trash.json");

            var jsonConfig = JsonLoader.Load<Dictionary<string, SeaTrashTypeConfig>>(configPath);

            _seaTrashTypeConfig = jsonConfig.ToDictionary(
                             item => item.Key switch
                             {
                                 "plasticSeaTrash" => SeaTrashType.Plastic,
                                 "woodSeaTrash" => SeaTrashType.Wood,
                                 "leafSeaTrash" => SeaTrashType.Leaf,
                                 "barrelSeaTrash" => SeaTrashType.Barrel,
                                 _ => throw new JsonException($"Unknown trash type: {item.Key}")
                             },
                             item => item.Value);

            _seaTrashWeights = _seaTrashTypeConfig.ToDictionary(item => item.Key, item => item.Value.SpawnWeight);
        }

        public static SeaTrash Create(SeaTrashType type, Vector2 position)
        {
            if (!_seaTrashTypeConfig.TryGetValue(type, out var typeConfig))
            {
                throw new ArgumentException($"Unknown seaTrash type: {type}");
            }

            var categoriesConfig = typeConfig.Categories[_rnd.Next(typeConfig.Categories.Count)];
            var colliderSize = new Size(categoriesConfig.ColliderSize[0], categoriesConfig.ColliderSize[1]);

            return new SeaTrash(type, categoriesConfig.Skin, position, colliderSize);
        }

        public static SeaTrash CreateRandom(Vector2 position)
        {
            return Create(_rnd.RandomWithWeight(_seaTrashWeights), position);
        }
    }
}
