using System.Collections.Generic;
using System.Linq;

namespace DragonSpeak.NET
{
    using Lexical;
    using Delegates;
    using Error;

    public class Page
    {
        private List<TriggerBlock> TriggerBlocks { get; set; }

        public DragonSpeakEngine Engine { get; private set; }

        private Dictionary<Trigger, TriggerHandler> Handlers { get; set; }

        private CauseTriggerDiscoveryHandler OnCauseTriggerDiscovered { get; set; }

        public Page(DragonSpeakEngine engine, CauseTriggerDiscoveryHandler causeDiscoveryHandler)
        {
            this.Engine = engine;

            this.TriggerBlocks = new List<TriggerBlock>();
            this.Handlers = new Dictionary<Trigger, TriggerHandler>();

            this.OnCauseTriggerDiscovered = causeDiscoveryHandler;
        }

        /// <summary> Assigns the specified <see cref="TriggerHandler"/> to the <paramref name="trigger"/>. </summary>
        /// <remarks>
        /// By default, a non-set <see cref="TriggerCategory.Cause"/><see cref="Trigger"/> returns true.
        /// </remarks>
        /// <param name="trigger"> <see cref="Trigger"/> </param>
        /// <param name="handler"> <see cref="TriggerHandler"/> </param>
        public void SetTriggerHandler(Trigger trigger, TriggerHandler handler, string description = null)
        {
            trigger.Description = description;

            if (this.Handlers.ContainsKey(trigger)) {
                if (this.Engine.Options.CanOverrideTriggerHandlers)
                    this.Handlers[trigger] = handler;
                else
                    throw new DragonSpeakException($"A trigger handler for ({(int)trigger.Category}:{trigger.Id} already exists.");
            }

            this.Handlers.Add(trigger, handler);
        }

        /// <summary>
        /// Unassigns the specified <see cref="TriggerHandler"/> from all triggers matching the
        /// <paramref name="category"/> and <paramref name="id"/>.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="id"></param>
        public void RemoveTriggerHandler(TriggerCategory category, int id)
        {
            var triggers = from trigger in this.Handlers
                           where trigger.Key.Category == category && trigger.Key.Id == id
                           select trigger.Key;

            foreach (var trigger in triggers)
                this.Handlers.Remove(trigger);
        }

        internal Page Insert(IEnumerable<TriggerBlock> triggerBlocks)
        {
            foreach (var triggerBlock in triggerBlocks) {
                var initialTrigger = triggerBlock[0];

                switch (initialTrigger.Category) {
                    case TriggerCategory.Cause:
                        this.TriggerBlocks.Add(triggerBlock);
                        this.OnCauseTriggerDiscovered?.Invoke(this, initialTrigger);
                        break;
                    default:
                        throw new DragonSpeakException(
                         string.Concat("A paragraph may only start with a 'cause' trigger.\n",
                         $"'({ (int)initialTrigger.Category }:{ initialTrigger.Id })' at index { initialTrigger.Position.Index }",
                         $" (line { initialTrigger.Position.Line}, column { initialTrigger.Position.Column})."));
                }
            }

            return this;
        }

        private void ExecuteBlock<T>(IContext context, T triggerBlock) where T : TriggerBlock
        {
            var causeTrigger = triggerBlock[0];

            // invoke the beginning cause trigger 
            // and check if the options allow ignoring unhandled cause triggers.
            if (this.Handlers.ContainsKey(causeTrigger)) {
                if (!this.Handlers[causeTrigger](context, causeTrigger))
                    return;
            } else {
                if (!this.Engine.Options.IgnoreUnhandledCauseTriggers) {
                    throw new DragonSpeakException($"There are no handlers set for trigger '({(int)triggerBlock[0].Category}:{triggerBlock[0].Id})'.");
                }
            }

            // evaluate any additional conditions in the block.
            for (var i = 1; i < triggerBlock.Count; i++) {
                var previousTrigger = triggerBlock[i - 1];
                var currentTrigger = triggerBlock[i];

                currentTrigger.Cause = triggerBlock[0];
                currentTrigger.Conditions = previousTrigger.Conditions;
                currentTrigger.Areas = previousTrigger.Areas;
                currentTrigger.Filters = previousTrigger.Filters;
                currentTrigger.Effects = previousTrigger.Effects;

                if (!this.Handlers.ContainsKey(currentTrigger)) {
                    throw new DragonSpeakException($"There are no handlers set for trigger '({(int)currentTrigger.Category}:{currentTrigger.Id})'.");
                }

                switch (currentTrigger.Category) {
                    case TriggerCategory.Cause:
                        throw new DragonSpeakException("Triggers cannot contain sibling causes.");

                    case TriggerCategory.Condition:
                        if (!this.Handlers[currentTrigger](context, currentTrigger))
                            return;

                        currentTrigger.Conditions.Add(currentTrigger);
                        break;

                    case TriggerCategory.Area:
                        if (!this.Handlers[currentTrigger](context, currentTrigger))
                            return;

                        currentTrigger.Areas.Add(currentTrigger);
                        break;

                    case TriggerCategory.Filter:
                        if (!this.Handlers[currentTrigger](context, currentTrigger))
                            return;

                        currentTrigger.Filters.Add(currentTrigger);
                        break;

                    case TriggerCategory.Effect:
                        if (!this.Handlers[currentTrigger](context, currentTrigger))
                            return;

                        currentTrigger.Effects.Add(currentTrigger);
                        break;
                    default:
                        throw new DragonSpeakException($"There is no such trigger category '{currentTrigger.Category}'.");
                }
            }
        }

        /// <summary>
        /// Execute every <see cref="TriggerCategory.Cause"/> trigger with the specified id(s).
        /// </summary>
        /// <param name="context"> A context to carry through trigger handlers. </param>
        public void Execute(IContext context = null, params int[] ids)
        {
            var blocks = from id in ids from block in TriggerBlocks
                         where block[0].Category == TriggerCategory.Cause
                         where block[0].Id == id select block;

            foreach (var block in blocks)
                ExecuteBlock(context, block);
        }
    }
}