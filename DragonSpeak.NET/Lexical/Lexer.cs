using System.Collections.Generic;
using System.Text.RegularExpressions;

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
                Token mToken = null;
                TokenDefinition mDefinition = null;

                foreach (var definition in this.TokenDefinitions) {
                    var match = definition.Pattern.Match(pageSource, currentIndex);

                    if (match.Success && (match.Index - currentIndex) == 0) {
                        var terminator = TerminationPattern.Match(match.Value);

                        mToken = new Token(definition.Type, match.Value, new TokenPosition(currentIndex, currentLine, currentColumn));
                        mDefinition = definition;

                        currentIndex += match.Length;
                        currentLine += terminator.Success ? 1 : 0;
                        currentColumn = terminator.Success ? match.Value.Length - (terminator.Index + terminator.Length) : currentColumn + match.Length;

                        break;
                    }
                }

                if (mToken == null && mDefinition == null) {
                    throw new DragonSpeakException($"Unrecognized symbol '{pageSource[currentIndex]}' at index {currentIndex} (line {currentLine}, column {currentColumn}).");
                }

                if (!mDefinition.Ignored) {
                    yield return mToken;
                }
            }

            yield return new Token(TokenType.EOF, null, new TokenPosition(currentIndex, currentLine, currentColumn));
        }
    }
}
