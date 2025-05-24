using System.Text.Json;

namespace StardewRaft.Core.Feature
{
    public static class JsonLoader
    {
        public static T Load<T>(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
