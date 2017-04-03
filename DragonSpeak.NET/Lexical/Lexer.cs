using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace DragonSpeak.NET.Lexical
{
    using Interfaces;
    using Enums;
    using Error;

    internal class Lexer : ILexer
    {
        public TokenDefinition[] TokenDefinitions { get; set; }
        public Regex TerminationPattern = new Regex(@"\r\n|\r|\n", RegexOptions.Compiled);

        public Lexer(params TokenDefinition[] definitions)
        {
            this.TokenDefinitions = definitions;
        }

        public IEnumerable<Token> Tokenize(string pageSource)
        {
            var currentIndex = 0;
            var currentLine = 1;
            var currentColumn = 0;

            while (currentIndex < pageSource.Length) {
                var token = (from definition in this.TokenDefinitions
                             let match = definition.Pattern.Match(pageSource, currentIndex)
                             where match.Success && (match.Index - currentIndex) == 0
                             select new { definition, match }).FirstOrDefault();

                var terminator = TerminationPattern.Match(token.match.Value);

                currentIndex += token.match.Length;
                currentLine += terminator.Success ? 1 : 0;
                currentColumn = terminator.Success ? token.match.Value.Length - (terminator.Index + terminator.Length) : currentColumn + token.match.Length;

                if (token == null || token.definition == null) {
                    throw new DragonSpeakException($"Unrecognized symbol '{pageSource[currentIndex]}' at index {currentIndex} (line {currentLine}, column {currentColumn}).");
                }

                if (!token.definition.Ignored) {
                    yield return new Token(token.definition.Type, token.match.Value, new TokenPosition(currentIndex, currentLine, currentColumn));
                }
            }

            yield return new Token(TokenType.EOF, null, new TokenPosition(currentIndex, currentLine, currentColumn));
        }
    }
}
