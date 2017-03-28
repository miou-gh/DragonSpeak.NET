namespace DragonSpeak.NET.Lexical
{
    using Enums;

    internal class Token
    {
        public TokenPosition Position { get; set; }
        public TokenType Type { get; set; }

        public string Value { get; set; }

        public Token(TokenType type, string value, TokenPosition position)
        {
            this.Type = type;
            this.Value = value;
            this.Position = position;
        }
    }
}
