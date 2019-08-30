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
            
            return CheckResult(result);
        }

        public async Task<TResult> EventuallyAsync<TResult>(Func<Task<TResult>> function)
        {
            var result = await _policyWrapAsync(_patienceConfig)
                .ExecuteAndCaptureAsync(function);

            return CheckResult(result);
        }
        
        public void Eventually(Action function)
        {
            var result = _policyWrap(_patienceConfig)
                .ExecuteAndCapture(function);

            CheckResult(result);
        }
        
        public async Task EventuallyAsync(Func<Task> function)
        {
            var result = await _policyWrapAsync(_patienceConfig).ExecuteAndCaptureAsync(function);

            CheckResult(result);
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
        
        private AsyncPolicy _asyncTimeoutPolicy(TimeSpan timeout)
        {
            Task OnTimeout(Context context, TimeSpan timeSpan, Task task)
            {
                context.Add("executionTime", timeSpan);
                return task;
            }

            return Policy.TimeoutAsync(timeout, OnTimeout);
        }

        private AsyncPolicy _asyncRetryPolicy(TimeSpan interval)
        {
            void OnRetry(Exception exception, TimeSpan timeSpan, Context context)
            {
                if (context.ContainsKey("retryCount"))
                {
                    var count = (int)context["retryCount"];
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
                .WaitAndRetryForeverAsync(Provider, onRetry: OnRetry);
        }

        private void CheckResult(PolicyResult result)
        {
            if (result.Outcome != OutcomeType.Failure)
            {
                return;
            }

            if (!result.Context.ContainsKey("retryCount"))
            {
                throw new EventuallyTimeoutException(_patienceConfig);
            }

            var retryCount = (int)result.Context["retryCount"];
            var lastException = result.Context["exception"] as Exception;
            throw new EventuallyTimeoutException(_patienceConfig, retryCount, lastException);
        }
        
        
        private TResult CheckResult<TResult>(PolicyResult<TResult> result)
        {
            if (result.Outcome != OutcomeType.Failure)
            {
                return result.Result;
            }

            if (!result.Context.ContainsKey("retryCount"))
            {
                throw new EventuallyTimeoutException(_patienceConfig);
            }

            var retryCount = (int)result.Context["retryCount"];
            var lastException = result.Context["exception"] as Exception;
            throw new EventuallyTimeoutException(_patienceConfig, retryCount, lastException);
        }

        private PolicyWrap _policyWrap(PatienceConfig patienceConfig) =>
            Policy.Wrap(_timeoutPolicy(patienceConfig.Timeout), _retryPolicy(patienceConfig.Interval));

        private AsyncPolicyWrap _policyWrapAsync(PatienceConfig patienceConfig) =>
            Policy.WrapAsync(_asyncTimeoutPolicy(patienceConfig.Timeout), _asyncRetryPolicy(patienceConfig.Interval));
    }
}