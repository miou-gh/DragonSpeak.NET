﻿namespace DragonSpeak.NET.Lexical
{
    internal class TokenPosition
    {
        public int Index { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }

        public TokenPosition(int index, int line, int column)
        {
            this.Index = index;
            this.Line = line;
            this.Column = column;
        }
    }
}
