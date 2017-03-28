namespace DragonSpeak.NET.Enums
{
    internal enum TokenType : byte
    {
        Trigger,
        String,
        Number,
        Array,
        Comment,
        Whitespace,
        Word,
        Symbol,
        EOF
    }
}