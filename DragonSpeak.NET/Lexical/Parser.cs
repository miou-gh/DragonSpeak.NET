using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DragonSpeak.NET.Lexical
{
    using Enums;

    internal class Parser
    {
        public Lexer Lexer { get; set; }

        public Parser(Lexer lexer)
        {
            this.Lexer = lexer;
        }

        public IEnumerable<TriggerBlock> Parse(string pageSource)
        {
            Trigger currentTrigger = null,
                    previousTrigger = null;

            var currentTriggerBlock = new TriggerBlock();

            using (var iterator = this.Lexer.Tokenize(pageSource).GetEnumerator()) {
                while (iterator.MoveNext()) {
                    var token = iterator.Current;

                    switch (token.Type) {
                        case TokenType.Trigger:
                            if (currentTrigger != null) {
                                if (previousTrigger != null) {
                                    if (previousTrigger.Category == TriggerCategory.Effect && currentTrigger.Category == TriggerCategory.Cause) {
                                        yield return currentTriggerBlock;

                                        currentTriggerBlock = new TriggerBlock();
                                    }
                                }

                                currentTriggerBlock.Add(currentTrigger);
                                previousTrigger = currentTrigger;
                            }

                            var category = (TriggerCategory)int.Parse(token.Value.Substring(1, token.Value.IndexOf(':') - 1));
                            var id = token.Value.Substring(token.Value.IndexOf(':') + 1);
                                id = id.Substring(0, id.Length - 1);

                            currentTrigger = new Trigger(category, int.Parse(id));
                            break;

                        case TokenType.Array:
                            currentTrigger.Contents.Add(token.Value.Substring(1, token.Value.Length - 2).Split(',').Select(x => double.Parse(x.Trim())).ToArray());
                            break;

                        case TokenType.String:
                            currentTrigger.Contents.Add(token.Value.Substring(1, token.Value.Length - 2));
                            break;
                        case TokenType.Number:
                            currentTrigger.Contents.Add(double.Parse(token.Value, NumberStyles.AllowDecimalPoint));
                            break;
                        case TokenType.EOF:
                            if (currentTrigger != null) {
                                if (currentTrigger.Category != TriggerCategory.Undefined) {
                                    currentTriggerBlock.Add(currentTrigger);
                                    yield return currentTriggerBlock;
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
