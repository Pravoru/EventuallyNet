using System;

namespace EventuallyNet
{
    public class PatienceConfig
    {
        public readonly TimeSpan Timeout;
        public readonly TimeSpan Interval;

        public PatienceConfig(TimeSpan timeout, TimeSpan interval)
        {
            Timeout = timeout;
            Interval = interval;
        }
    }
}