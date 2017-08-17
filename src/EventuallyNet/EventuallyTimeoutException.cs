using System;

namespace EventuallyNet
{
    public class EventuallyTimeoutException : Exception
    {
        private static string DidNotEventuallySucceedWithError(TimeSpan timeout, int attempts, string innerExcaptionMessage) =>
            $"The code passed to eventually never returned normally. Attempted {attempts} times over {timeout}. Last failure message: {innerExcaptionMessage}";

        private static  string DidNotEventuallySucceedWithTimeout(TimeSpan timeout) =>
            $"The code passed to eventually dit not complete and was ended after {timeout} timeout.";
          
        public TimeSpan Timeout { get; }
        public TimeSpan Interval { get; }
      
        public EventuallyTimeoutException(PatienceConfig patienceConfig, int attempts, Exception innerException)
            : base(DidNotEventuallySucceedWithError(patienceConfig.Timeout, attempts, innerException.Message), innerException)
        {
            Timeout = patienceConfig.Timeout;
            Interval = patienceConfig.Interval;
        }

        public EventuallyTimeoutException(PatienceConfig patienceConfig)
            : base(DidNotEventuallySucceedWithTimeout(patienceConfig.Timeout))
        {
            Timeout = patienceConfig.Timeout;
            Interval = patienceConfig.Interval;
        }
    }
}