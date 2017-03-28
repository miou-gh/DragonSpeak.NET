using System.Text.RegularExpressions;

namespace DragonSpeak.NET.Lexical
{
    using Enums;

    internal class TokenDefinition
    {
        public Regex Pattern { get; private set; }
        public TokenType Type { get; private set; }

        public bool Ignored { get; private set; }

        public TokenDefinition(TokenType type, Regex pattern, bool ignored = false)
        {
            this.Type = type;
            this.Pattern = pattern;
            this.Ignored = ignored;
        }
    }
}
