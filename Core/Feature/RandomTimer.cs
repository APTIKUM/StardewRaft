using Timer = System.Windows.Forms.Timer;

namespace StardewRaft.Core.Feature
{
    public class RandomTimer : Timer
    {
        private Random _rnd = new();
        public int MinInterval { get; private set; }
        public int MaxInterval { get; private set; }

        public RandomTimer(int minInterval, int maxInterval)
        {
            MinInterval = minInterval;
            MaxInterval = maxInterval;
            SetRandomTick();
            Tick += RandomTimer_Tick;
        }

        private void RandomTimer_Tick(object? sender, EventArgs e)
        {
            SetRandomTick();
        }

        public void SetRandomTick()
        {
            Interval = _rnd.Next(MinInterval, MaxInterval);
        }
    }
}
