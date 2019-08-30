using System;
using System.Threading.Tasks;

namespace EventuallyNet
{
    public static class EventuallyStatic
    {
        public static TResult Eventually<TResult>(Func<TResult> function, PatienceConfig patienceConfig = null)
        {
            var config = patienceConfig ?? DefaultPatienceConfig;
            return new EventuallyClass(config).Eventually(function);
        }

        public static void Eventually(Action function, PatienceConfig patienceConfig = null)
        {
            var config = patienceConfig ?? DefaultPatienceConfig;
            new EventuallyClass(config).Eventually(function);
        }
        
        public static async Task<TResult> EventuallyAsync<TResult>(Func<Task<TResult>> func, PatienceConfig patienceConfig = null)
        {
            var config = patienceConfig ?? DefaultPatienceConfig;
            return await new EventuallyClass(config).EventuallyAsync(func);
        }

        public static async Task EventuallyAsync(Func<Task> func, PatienceConfig patienceConfig = null)
        {
            var config = patienceConfig ?? DefaultPatienceConfig;
            await new EventuallyClass(config).EventuallyAsync(func);
        }

        public static readonly PatienceConfig DefaultPatienceConfig = new PatienceConfig(TimeSpan.FromMilliseconds(150), TimeSpan.FromMilliseconds(15));
    }
    
}