using System;
using System.Threading;
using System.Collections.Concurrent;

namespace TwitchSlaysTheSpire.Common
{
    public class Bridge
    {
        public ConcurrentQueue<Command> Queue { get; } = new ConcurrentQueue<Command>();

        public AutoResetEvent QueueFlag { get; } = new AutoResetEvent(false);

        public string BossName { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}