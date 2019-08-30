using static EventuallyNet.EventuallyStatic;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Shouldly;

namespace EventuallyNet.UnitTests
{
    [TestFixture]
    public class SimpleTests
    {
        [Test(Description = "The eventually construct should just return if the by-name returns normally")]
        public void ShouldJustReturnIfTheByNameReturnsNormally()
        {            
            Eventually(() => (1 + 1).ShouldBe(2));
        }
        
        [Test(Description = "The eventually construct should just return if the by-name returns normally")]
        public async Task ShouldJustReturnIfTheByNameReturnsNormallyAsync()
        {
            await EventuallyAsync(async () => await Task.Run(() => (1 + 1).ShouldBe(2)));
        }
        
        [Test(Description = "The eventually construct should invoke the function just once if the by-name returns normally the first time")]
        public void ShouldInvokeTheFunctionJustOnceIfTheByNameReturnsNormallyTheFirstTime()
        {      
            var count = 0;
            Eventually(() => count++);
            count.ShouldBe(1);
        }
        
        [Test(Description = "The eventually construct should invoke the function just once if the by-name returns normally the first time")]
        public async Task ShouldInvokeTheFunctionJustOnceIfTheByNameReturnsNormallyTheFirstTimeAsync()
        {
            var count = 0;
            await EventuallyAsync(async () => await Task.Run(() => count++));
            count.ShouldBe(1);
        }
        
        [Test(Description = "The eventually construct should invoke the function just once and return the result if the by-name returns normally the first time")]
        public void ShouldInvokeTheFunctionJustOnceAndReturnTheResultIfTheByNameReturnsNormallyTheFirstTime()
        {            
            var count = 0;
            var result = Eventually(() =>
            {
                count++;
                return 99;
            });
            count.ShouldBe(1);
            result.ShouldBe(99);
        }
        
        [Test(Description = "The eventually construct should invoke the function just once and return the result if the by-name returns normally the first time")]
        public async Task ShouldInvokeTheFunctionJustOnceAndReturnTheResultIfTheByNameReturnsNormallyTheFirstTimeAsync()
        {            
            var count = 0;
            var result = await EventuallyAsync(async () => await Task.Run(() =>
            {
                count++;
                return 99;
            }));
            count.ShouldBe(1);
            result.ShouldBe(99);
        }
        
        [Test(Description = "The eventually construct should invoke the function five times if the by-name throws an exception four times before finally returning normally the fifth time")]
        public void ShouldInvokeTheFunctionFiveTimesIfTheByNameThrowsAnExceptionFourTimesBeforeFinallyReturningNormallyTheFifthTime()
        {            
            var count = 0;
            Eventually(() =>
            {
                count++;
                if (count < 5)
                {
                    throw new Exception();
                }
                (1 + 1).ShouldBe(2);
            });
        }
        
        [Test(Description = "The eventually construct should invoke the function five times if the by-name throws an exception four times before finally returning normally the fifth time")]
        public async Task ShouldInvokeTheFunctionFiveTimesIfTheByNameThrowsAnExceptionFourTimesBeforeFinallyReturningNormallyTheFifthTimeAsync()
        {            
            var count = 0;
            await EventuallyAsync(async () => await Task.Run(() =>
            {
                count++;
                if (count < 5)
                {
                    throw new Exception();
                }
                (1 + 1).ShouldBe(2);
            }));
        }
        
        [Test(Description = "The eventually construct should eventually blow up with a TestFailedDueToTimeoutException if the by-name continuously throws an exception")]
        public void ShouldEventuallyBlowUpWithATestFailedDueToTimeoutExceptionIfTheByNameContinuouslyThrowsAnException()
        {            
            var count = 0;
            var caught = Should.Throw<EventuallyTimeoutException>(() => Eventually(() =>
            {
                count++;
                throw new Exception();
            }));
            caught.Message.ShouldBe($"The code passed to eventually never returned normally. Attempted {count} times over 00:00:00.1500000. Last failure message: Exception of type 'System.Exception' was thrown.");
            caught.Interval.ShouldBe(TimeSpan.FromMilliseconds(15));
            caught.Timeout.ShouldBe(TimeSpan.FromMilliseconds(150));
        }
        
        [Test(Description = "The eventually construct should eventually blow up with a TestFailedDueToTimeoutException if the by-name continuously throws an exception")]
        public async Task
            ShouldEventuallyBlowUpWithATestFailedDueToTimeoutExceptionIfTheByNameContinuouslyThrowsAnExceptionAsync()
        {
            var count = 0;
            var caught =
                await Should.ThrowAsync<EventuallyTimeoutException>(async () => await EventuallyAsync(async () =>
                    await Task.Run(
                        () =>
                        {
                            {
                                count++;
                                throw new Exception();
                            }
                        })));

            caught.Message.ShouldBe(
                $"The code passed to eventually never returned normally. Attempted {count} times over 00:00:00.1500000. Last failure message: Exception of type 'System.Exception' was thrown.");
            caught.Interval.ShouldBe(TimeSpan.FromMilliseconds(15));
            caught.Timeout.ShouldBe(TimeSpan.FromMilliseconds(150));
        }
        
        [Test(Description = "The eventually construct should eventually blow up with a TFE if the by-name continuously throws an exception, and include the last failure message in the TFE messagen")]
        public void ShouldEventuallyBlowUpWithATFEIfTheByNameContinuouslyThrowsAnExceptionAndIncludeTheLastFailureMessageInTheTFEMessage()
        {            
            var count = 0;
            var caught = Should.Throw<EventuallyTimeoutException>(() => Eventually(() =>
            {
                count++;
                (1 + 1).ShouldBe(3);
            }));
            caught.Message.ShouldContain($"The code passed to eventually never returned normally. Attempted {count} times over 00:00:00.1500000. Last failure message:");
            caught.Message.ShouldContain("should be");
            caught.InnerException.Source.ShouldBe("Shouldly");
            caught.Interval.ShouldBe(TimeSpan.FromMilliseconds(15));
            caught.Timeout.ShouldBe(TimeSpan.FromMilliseconds(150));
        }
        
        [Test(Description = "The eventually construct should eventually blow up with a TFE if the by-name continuously throws an exception, and include the last failure message in the TFE messagen")]
        public async Task ShouldEventuallyBlowUpWithATFEIfTheByNameContinuouslyThrowsAnExceptionAndIncludeTheLastFailureMessageInTheTFEMessageAsync()
        {            
            var count = 0;
            var caught = await Should.ThrowAsync<EventuallyTimeoutException>(async () =>
                await EventuallyAsync(async () =>
                    await Task.Run(() =>
                    {
                        count++;
                        (1 + 1).ShouldBe(3);
                    })));
            caught.Message.ShouldContain($"The code passed to eventually never returned normally. Attempted {count} times over 00:00:00.1500000. Last failure message:");
            caught.Message.ShouldContain("should be");
            caught.InnerException.Source.ShouldBe("Shouldly");
            caught.Interval.ShouldBe(TimeSpan.FromMilliseconds(15));
            caught.Timeout.ShouldBe(TimeSpan.FromMilliseconds(150));
        }
        
        [Test(Description = "The eventually construct should provides correct stack depth when eventually is called from the overload method")]
        [Ignore("Need an implimitation")]
        public void ShouldProvidesCorrectStackDepthWhenEventuallyIsCalledFromTheOverloadMethod()
        {            

        }
        
        [Test(Description = "The eventually construct should by default invoke an always-failing by-name for at least 150 millis")]
        public void ShouldByDefaultInvokeAnAlwaysFailingByNameForAtLeast150Millis()
        {
            DateTime? startTime = null;
            Should.Throw<EventuallyTimeoutException>(() => Eventually(() =>
            {
                if (startTime == null)
                    startTime = DateTime.Now;
                (1 + 1).ShouldBe(3);
            }));
            (DateTime.Now - startTime)?.TotalMilliseconds.ShouldBeGreaterThanOrEqualTo(150);
        }
        
        [Test(Description = "The eventually construct should by default invoke an always-failing by-name for at least 150 millis")]
        public async Task ShouldByDefaultInvokeAnAlwaysFailingByNameForAtLeast150MillisAsync()
        {
            DateTime? startTime = null;
            await Should.ThrowAsync<EventuallyTimeoutException>(async () => await EventuallyAsync(() => Task.Run(() =>
            {
                if (startTime == null)
                    startTime = DateTime.Now;
                (1 + 1).ShouldBe(3);
            })));
            (DateTime.Now - startTime)?.TotalMilliseconds.ShouldBeGreaterThanOrEqualTo(150);
        }
        
        [Test(Description = "The eventually construct should, if an alternate implicit Timeout is provided, invoke an always-failing by-name by at least the specified timeout")]
        public void ShouldIfAnAlternateImplicitTimeoutIsProvidedInvokeAnAlwaysFailingByNameByAtLeastTheSpecifiedTimeout()
        {
            DateTime? startTime = null;
            var config = new PatienceConfig(TimeSpan.FromMilliseconds(1500), TimeSpan.FromMilliseconds(15));
            Should.Throw<EventuallyTimeoutException>(() => Eventually(() =>
            {
                if (startTime == null)
                    startTime = DateTime.Now;
                (1 + 1).ShouldBe(3);
            }, config));
            (DateTime.Now - startTime)?.TotalMilliseconds.ShouldBeGreaterThanOrEqualTo(1500);
        }
        
        [Test(Description = "The eventually construct should, if an alternate implicit Timeout is provided, invoke an always-failing by-name by at least the specified timeout")]
        public async Task ShouldIfAnAlternateImplicitTimeoutIsProvidedInvokeAnAlwaysFailingByNameByAtLeastTheSpecifiedTimeoutAsync()
        {
            DateTime? startTime = null;
            var config = new PatienceConfig(TimeSpan.FromMilliseconds(1500), TimeSpan.FromMilliseconds(15));
            await Should.ThrowAsync<EventuallyTimeoutException>(async () => await EventuallyAsync(async () =>
                    await Task.Run(() =>
                    {
                        if (startTime == null)
                            startTime = DateTime.Now;
                        (1 + 1).ShouldBe(3);
                    }),
                config));
            (DateTime.Now - startTime)?.TotalMilliseconds.ShouldBeGreaterThanOrEqualTo(1500);
        }
    }
}