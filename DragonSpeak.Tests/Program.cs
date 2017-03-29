using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragonSpeak.NET;
using NUnit.Framework;

namespace DragonSpeak.Tests
{
    [TestFixture]
    internal class Program
    {
        private static DragonSpeakEngine Engine { get; set; }
            = new DragonSpeakEngine();

        public List<string> TestScripts = new List<string>() {
            // Cause and Effect
            @"
            (0:1) When an event occours,
                 (5:1) an action takes place.
            ",

            // Cause Condition and Effect
            @"
            (0:1) When an event occours,
             (1:1) and a condition returns true,
                 (5:1) an action takes place.
            ",

            // Cause Area and Effect
            @"
            (0:1) When an event occours,
             (3:1) at position (5,10) on the map,
                 (5:1) an action takes place.
            ",

            // Cause Area Filter and Effect
            @"
            (0:1) When an event occours,
             (3:1) at position (5,10) on the map,
                (4:1) only where the filter says,
                 (5:1) an action takes place.
            ",

            // Cause Condition Area Filter and Effect
            @"
            (0:1) When an event occours,
             (1:1) and a condition returns true,
               (3:1) at position (5,10) on the map,
                (4:1) only where the filter says,
                 (5:1) an action takes place.
            ",

           // Retrieve String In Trigger
            @"
            (0:1) When an event occours {Hello World!},
                 (5:1) an action takes place.
            ",

            // Retrieve Integer Number In Trigger
            @"
            (0:1) When an event occours at map 48,
                 (5:1) an action takes place.
            ",

            // Retrieve Double Number In Trigger
            @"
            (0:1) When an event occours at map 48.5,
                 (5:1) an action takes place.
            ",

            // Retrieve Array In Trigger
            @"
            (0:1) When an event contains [10, 25, 80, 92],
                 (5:1) an action takes place.
            ",
        };

        public class ContextExample : IContext
        {
            public int Value { get; set; }
        }

        [Test]
        public void CauseAndEffect()
        {
            var page = Engine.LoadFromString(TestScripts[0]);
            bool causeResult = false,
                 effectResult = false;

            page.SetTriggerHandler(new Trigger(TriggerCategory.Cause, 1), (ctx, trigger) => {
                causeResult = true;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Effect, 1), (ctx, trigger) => {
                effectResult = true;

                return true;
            });

            page.Execute(ids: 1);
            Assert.That(causeResult && effectResult, Is.True.After(5000, 5));
        }

        [Test]
        public void CauseConditionAndEffect()
        {
            var page = Engine.LoadFromString(TestScripts[1]);
            bool causeResult = false,
                 conditionResult = false,
                 effectResult = false;

            page.SetTriggerHandler(new Trigger(TriggerCategory.Cause, 1), (ctx, trigger) => {
                causeResult = true;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Condition, 1), (ctx, trigger) => {
                conditionResult = true;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Effect, 1), (ctx, trigger) => {
                effectResult = true;

                return true;
            });

            page.Execute(ids: 1);
            Assert.That(causeResult && conditionResult && effectResult, Is.True.After(5000, 5));
        }

        [Test]
        public void CauseAreaAndEffect()
        {
            var page = Engine.LoadFromString(TestScripts[2]);
            bool causeResult = false,
                 areaResult = false,
                 effectResult = false;

            page.SetTriggerHandler(new Trigger(TriggerCategory.Cause, 1), (ctx, trigger) => {
                causeResult = true;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Area, 1), (ctx, trigger) => {
                areaResult = true;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Effect, 1), (ctx, trigger) => {
                effectResult = true;

                return true;
            });

            page.Execute(ids: 1);
            Assert.That(causeResult && areaResult && effectResult, Is.True.After(5000, 5));
        }

        [Test]
        public void CauseAreaFilterAndEffect()
        {
            var page = Engine.LoadFromString(TestScripts[3]);
            bool causeResult = false,
                 areaResult = false,
                 filterResult = false,
                 effectResult = false;

            page.SetTriggerHandler(new Trigger(TriggerCategory.Cause, 1), (ctx, trigger) => {
                causeResult = true;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Area, 1), (ctx, trigger) => {
                areaResult = true;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Filter, 1), (ctx, trigger) => {
                filterResult = true;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Effect, 1), (ctx, trigger) => {
                effectResult = true;

                return true;
            });

            page.Execute(ids: 1);
            Assert.That(causeResult && areaResult && filterResult && effectResult, Is.True.After(5000, 5));
        }

        [Test]
        public void PassTriggerContents()
        {
            var page = Engine.LoadFromString(TestScripts[4]);
            var context = new ContextExample() { Value = 0 };
            bool causeResult = false,
                 conditionResult = false,
                 areaResult = false,
                 filterResult = false,
                 effectResult = false;

            page.SetTriggerHandler(new Trigger(TriggerCategory.Cause, 1), (ctx, trigger) => {
                causeResult = true;
                context.Value++;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Condition, 1), (ctx, trigger) => {
                conditionResult = true;
                context.Value++;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Area, 1), (ctx, trigger) => {
                areaResult = true;
                context.Value++;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Filter, 1), (ctx, trigger) => {
                filterResult = true;
                context.Value++;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Effect, 1), (ctx, trigger) => {
                effectResult = true;
                context.Value++;

                return true;
            });

            page.Execute(context: context, ids: 1);
            Assert.That(causeResult && conditionResult && areaResult && filterResult && effectResult
                        && context.Value == 5, Is.True.After(5000, 5));
        }

        [Test]
        public void RetrieveStringInTrigger()
        {
            var page = Engine.LoadFromString(TestScripts[5]);
            var input = "Hello World!";
            bool causeResult = false,
                 effectResult = false,
                 retrieveResult = false;

            page.SetTriggerHandler(new Trigger(TriggerCategory.Cause, 1), (ctx, trigger) => {
                causeResult = true;

                if (trigger.Get<string>(0) == input) {
                    retrieveResult = true;
                }

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Effect, 1), (ctx, trigger) => {
                effectResult = true;

                return true;
            });

            page.Execute(ids: 1);
            Assert.That(causeResult && effectResult && retrieveResult, Is.True.After(5000, 5));
        }

        [Test]
        public void RetrieveIntegerNumberInTrigger()
        {
            var page = Engine.LoadFromString(TestScripts[6]);
            var input = 48;
            bool causeResult = false,
                 effectResult = false,
                 retrieveResult = false;

            page.SetTriggerHandler(new Trigger(TriggerCategory.Cause, 1), (ctx, trigger) => {
                causeResult = true;

                if (trigger.Get<int>(0) == input) {
                    retrieveResult = true;
                }

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Effect, 1), (ctx, trigger) => {
                effectResult = true;

                return true;
            });

            page.Execute(ids: 1);
            Assert.That(causeResult && effectResult && retrieveResult, Is.True.After(5000, 5));
        }

        [Test]
        public void RetrieveDoubleNumberInTrigger()
        {
            var page = Engine.LoadFromString(TestScripts[7]);
            var input = 48.5;
            bool causeResult = false,
                 effectResult = false,
                 retrieveResult = false;

            page.SetTriggerHandler(new Trigger(TriggerCategory.Cause, 1), (ctx, trigger) => {
                causeResult = true;

                if (trigger.Get<double>(0) == input) {
                    retrieveResult = true;
                }

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Effect, 1), (ctx, trigger) => {
                effectResult = true;

                return true;
            });

            page.Execute(ids: 1);
            Assert.That(causeResult && effectResult && retrieveResult, Is.True.After(5000, 5));
        }

        [Test]
        public void RetrieveArrayInTrigger()
        {
            var page = Engine.LoadFromString(TestScripts[8]);
            var input = new double[] { 10, 25, 80, 92 };
            bool causeResult = false,
                 effectResult = false,
                 retrieveResult = false;

            page.SetTriggerHandler(new Trigger(TriggerCategory.Cause, 1), (ctx, trigger) => {
                causeResult = true;

                if (trigger.Get<double[]>(0).All(x => input.Contains(x))) {
                    retrieveResult = true;
                }

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Effect, 1), (ctx, trigger) => {
                effectResult = true;

                return true;
            });

            page.Execute(ids: 1);
            Assert.That(causeResult && effectResult && retrieveResult, Is.True.After(5000, 5));
        }

        [Test]
        public void OneThousandCauseEffectExecutions()
        {
            var page = Engine.LoadFromString(TestScripts[0]);
            int causeResults = 0,
                effectResults = 0;

            page.SetTriggerHandler(new Trigger(TriggerCategory.Cause, 1), (ctx, trigger) => {
                causeResults++;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Effect, 1), (ctx, trigger) => {
                effectResults++;

                return true;
            });

            for (var i = 0; i < 1000; i++) {
                page.Execute(ids: 1);
            }

            Assert.That(causeResults == 1000 && effectResults == 1000, Is.True.After(5000, 5));
        }

        [Test]
        public void OneHundredThousandCauseEffectExecutions()
        {
            var page = Engine.LoadFromString(TestScripts[0]);
            int causeResults = 0,
                effectResults = 0;

            page.SetTriggerHandler(new Trigger(TriggerCategory.Cause, 1), (ctx, trigger) => {
                causeResults++;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Effect, 1), (ctx, trigger) => {
                effectResults++;

                return true;
            });

            for (var i = 0; i < 100 * 1000; i++) {
                page.Execute(ids: 1);
            }

            Assert.That(causeResults == 100 * 1000 && effectResults == 100 * 1000, Is.True);
        }

        [Test]
        public void OneMillionCauseEffectExecutions()
        {
            var page = Engine.LoadFromString(TestScripts[0]);
            int causeResults = 0,
                effectResults = 0;

            page.SetTriggerHandler(new Trigger(TriggerCategory.Cause, 1), (ctx, trigger) => {
                causeResults++;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Effect, 1), (ctx, trigger) => {
                effectResults++;

                return true;
            });

            for (var i = 0; i < 1000000; i++) {
                page.Execute(ids: 1);
            }

            Assert.That(causeResults == 1000000 && effectResults == 1000000, Is.True);
        }

        [Test]
        public void OneThousandCausesAllExecute()
        {
            var causeEffectScript = TestScripts[0];
            var repeatedScript = new StringBuilder();

            for (var i = 0; i < 1000; i++) {
                repeatedScript.AppendLine(causeEffectScript);
            }

            var page = Engine.LoadFromString(repeatedScript.ToString());
            int causeResults = 0,
                effectResults = 0;

            page.SetTriggerHandler(new Trigger(TriggerCategory.Cause, 1), (ctx, trigger) => {
                causeResults++;

                return true;
            });

            page.SetTriggerHandler(new Trigger(TriggerCategory.Effect, 1), (ctx, trigger) => {
                effectResults++;

                return true;
            });

            page.Execute(ids: 1);
            Assert.That(causeResults == 1000 && effectResults == 1000);
        }

        [Test]
        public void CauseTriggerDiscoveryHandlerExecution()
        {
            var triggerDiscovered = false;

            var page = Engine.LoadFromString(TestScripts[0],
                         (sender, trigger) => {
                             triggerDiscovered = true;
                         });

            Assert.That(triggerDiscovered, Is.True.After(5000, 5));
        }
    }
}