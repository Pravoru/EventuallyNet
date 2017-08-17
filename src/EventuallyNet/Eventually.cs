using System;
using System.Threading.Tasks;
using Polly;
using Polly.Wrap;

namespace EventuallyNet
{
    public class EventuallyClass
    {
        private readonly PatienceConfig _patienceConfig;

        public EventuallyClass(PatienceConfig patienceConfig)
        {
            _patienceConfig = patienceConfig;
        }

        public TResult Eventually<TResult>(Func<TResult> function)
        {
            var result = _policyWrap(_patienceConfig)
                .ExecuteAndCapture(function);

            if (result.Outcome == OutcomeType.Failure)
            {
                if (result.Context.ContainsKey("retryCount"))
                {
                    var retryCount = (int)result.Context["retryCount"];
                    var lastException = result.Context["exception"] as Exception;
                    throw new EventuallyTimeoutException(_patienceConfig, retryCount, lastException);
                }
                else
                {
                    throw new EventuallyTimeoutException(_patienceConfig);
                }
            }
            else
            {
                return result.Result;
            }
        }
        
        public void Eventually(Action function)
        {
            var result = _policyWrap(_patienceConfig)
                .ExecuteAndCapture(function);

            if (result.Outcome == OutcomeType.Failure)
            {
                if (result.Context.ContainsKey("retryCount"))
                {
                    var retryCount = (int)result.Context["retryCount"];
                    var lastException = result.Context["exception"] as Exception;
                    throw new EventuallyTimeoutException(_patienceConfig, retryCount, lastException);
                }
                else
                {
                    throw new EventuallyTimeoutException(_patienceConfig);
                }
            }
        } 
        
        private Policy _timeoutPolicy(TimeSpan timeout)
        {
            void OnTimeout(Context context, TimeSpan timeSpan, Task task)
            {
                context.Add("executionTime", timeSpan);
            }
            
            return Policy
                .Timeout(timeout, OnTimeout);
        }

        private Policy _retryPolicy(TimeSpan interval)
        {
            void OnRetry(Exception exception, TimeSpan timeSpan, Context context)
            {
                if (context.ContainsKey("retryCount"))
                {
                    var count = (int) context["retryCount"];
                    context.Remove("retryCount");
                    context.Remove("exception");
                    context.Add("exception", exception);
                    context.Add("retryCount", count + 1);
                }
                else
                {
                    context.Add("retryCount", 1);
                    context.Add("exception", exception);
                }
            }

            TimeSpan Provider(int i, Context ctx) => interval;

            return Policy
                .Handle<Exception>()
                .WaitAndRetryForever(Provider, OnRetry);
            
        }

        private PolicyWrap _policyWrap(PatienceConfig patienceConfig) =>
            Policy.Wrap(_timeoutPolicy(patienceConfig.Timeout), _retryPolicy(patienceConfig.Interval));


    }
}