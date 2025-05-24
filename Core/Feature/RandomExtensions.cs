namespace StardewRaft.Core.Feature
{
    public static class RandomExtensions
    {
        public static T RandomWithWeight<T>(this Random rnd, Dictionary<T, double> weights)
        {
            double totalWeight = weights.Values.Sum();
            double randomValue = rnd.NextDouble() * totalWeight;
            double cumulative = 0f;

            foreach (var (item, weight) in weights)
            {
                cumulative += weight;
                if (randomValue <= cumulative)
                    return item;
            }

            return weights.First().Key;
        }
    }
}
