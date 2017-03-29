using System;
using System.Timers;
using DragonSpeak.NET;

namespace DragonSpeak.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var source =
            @"(0:1) Whenever 10 seconds have passed,
               (1:1) and a condition returns true,
                  (5:1) an action takes place.";

            var engine = new DragonSpeakEngine();
            var page = engine.LoadFromString(source,
                         // Whenever a cause trigger is discovered.
                         // Which is useful for setting things like timers!
                         (sender, trigger) => {
                             var seconds = trigger.Get<int>(0);

                             EasyTimer.SetInterval(() => {
                                 sender.Execute(null, trigger.Id);
                             }, seconds * 1000);
                         });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Cause, 1), (ctx, trigger) => {
                Console.WriteLine($"A { trigger.Category } trigger was hit with the id { trigger.Id }.");

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Condition, 1), (ctx, trigger) => {
                Console.WriteLine($"A { trigger.Category } trigger was hit with the id { trigger.Id }.");

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Effect, 1), (ctx, trigger) => {
                Console.WriteLine($"A { trigger.Category } trigger was hit with the id { trigger.Id }.");

                return true;
            });

            Console.ReadLine();
        }
    }

    public static class EasyTimer
    {
        public static IDisposable SetInterval(Action method, int delayInMilliseconds)
        {
            var timer = new Timer(delayInMilliseconds);
            timer.Elapsed += (source, e) => method();

            timer.Enabled = true;
            timer.Start();

            // Returns a stop handle which can be used for stopping
            // the timer, if required
            return timer as IDisposable;
        }

        public static IDisposable SetTimeout(Action method, int delayInMilliseconds)
        {
            var timer = new Timer(delayInMilliseconds);
            timer.Elapsed += (source, e) => method();

            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Start();

            // Returns a stop handle which can be used for stopping
            // the timer, if required
            return timer as IDisposable;
        }
    }
}
