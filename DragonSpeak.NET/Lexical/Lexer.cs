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
                var foundPattern = false;

                foreach (var definition in this.TokenDefinitions) {
                    var match = definition.Pattern.Match(pageSource, currentIndex);

                    if (match.Success && (match.Index - currentIndex) == 0) {
                        foundPattern = true;
                        
                        var value = match.Value;

                        if (!definition.Ignored) {
                            yield return new Token(definition.Type, value, new TokenPosition(currentIndex, currentLine, currentColumn));
                        }

                        var terminator = TerminationPattern.Match(value);

                        if (terminator.Success) {
                            currentLine += 1;
                            currentColumn = value.Length - (terminator.Index + terminator.Length);
                        } else {
                            currentColumn += match.Length;
                        }

                        currentIndex += match.Length;
                        break;
                    }
                }

                if (!foundPattern) {
                    throw new DragonSpeakException($"Unrecognized symbol '{pageSource[currentIndex]}' at index {currentIndex} (line {currentLine}, column {currentColumn}).");
                }
            }

            yield return new Token(TokenType.EOF, null, new TokenPosition(currentIndex, currentLine, currentColumn));
        }
    }
}
