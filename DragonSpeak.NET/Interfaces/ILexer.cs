using System.Collections.Generic;

namespace DragonSpeak.NET.Interfaces
{
    using Lexical;

    internal interface ILexer
    {
        IEnumerable<Token> Tokenize(string source);
    }
}
