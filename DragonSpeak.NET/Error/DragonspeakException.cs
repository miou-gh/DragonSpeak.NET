using System;
using System.Runtime.Serialization;

namespace DragonSpeak.NET.Error
{
    [Serializable]
    public class DragonSpeakException : Exception
    {
        public DragonSpeakException()
        {
        }

        public DragonSpeakException(string message) : base(message)
        {
        }

        public DragonSpeakException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DragonSpeakException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
