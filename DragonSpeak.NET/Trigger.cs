using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonSpeak.NET
{
    using Error;

    public class Trigger : IEquatable<Trigger>
    {
        /// <summary> The category in which the trigger resides. </summary>
        public TriggerCategory Category { get; set; } = TriggerCategory.Undefined;

        /// <summary> A list of all the user-input values present in the trigger text. </summary>
        public List<object> Contents { get; set; } = new List<object>();

        /// <summary> The index in the <see cref="Category"/> where this trigger resides. </summary>
        public int Id { get; set; } = -1;

        /// <summary> A friendly description of the trigger. </summary>
        public string Description { get; set; }

        /// <summary> The initial trigger executed. </summary>
        public Trigger Cause { get; set; }

        /// <summary> A list of all the previous conditions met. </summary>
        public List<Trigger> Conditions { get; set; } = new List<Trigger>();

        /// <summary> A list of all the previous areas met. </summary>
        public List<Trigger> Areas { get; set; } = new List<Trigger>();

        /// <summary> A list of all the previous filters met. </summary>
        public List<Trigger> Filters { get; set; } = new List<Trigger>();

        /// <summary> A list of all the previous effects met. </summary>
        public List<Trigger> Effects { get; set; } = new List<Trigger>();

        /// <param name="category"> The category in which this trigger resides. </param>
        /// <param name="id"> The index in the category in which this trigger resides. </param>
        public Trigger(TriggerCategory category, int id)
        {
            this.Category = category;
            this.Id = id;
        }

        /// <summary>
        /// Retrieves a converted (if IConvertable) or casted value from the <see cref="Contents"/>
        /// at the specified index.
        /// </summary>
        /// <typeparam name="T"> The type in which to convert or cast to. </typeparam>
        /// <param name="index"> The index in which the value resides in the <see cref="Contents"/>. </param>
        public T Get<T>(int index)
        {
            var content = this.Contents.ElementAtOrDefault(index) ??
                          throw new DragonSpeakException("Trigger does not contain any item at index {index}.");

            if (content is IConvertible) {
                return (T)Convert.ChangeType(content, typeof(T));
            }

            return (T)content;
        }

        /// <summary>
        /// Retrieves a converted (if IConvertable) or casted value from the <see cref="Contents"/>
        /// at the specified index.
        /// </summary>
        /// <param name="index"> The index in which the value resides in the <see cref="Contents"/>. </param>
        public object Get(int index)
        {
            var content = this.Contents.ElementAtOrDefault(index) ??
              throw new DragonSpeakException("Trigger does not contain any item at index {index}.");

            return content;
        }

        public override bool Equals(object trigger)
        {
            if (trigger is Trigger) {
                return Equals(trigger as Trigger);
            }

            return false;
        }

        public bool Equals(Trigger trigger)
        {
            return (trigger == null || GetType() != trigger.GetType()) ? false :
                    trigger.Category == Category && trigger.Id == Id;
        }

        public override int GetHashCode()
        {
            return (int)Category * 31 + Id;
        }
    }
}