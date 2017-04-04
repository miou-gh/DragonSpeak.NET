using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace DragonSpeak.NET
{
    using Lexical;
    using Delegates;
    using Enums;

    public class DragonSpeakEngine
    {
        private Lexer Lexer { get; set; }
        private Parser Parser { get; set; }
        
        public DragonSpeakOptions Options { get; private set; }
        public List<Page> Pages { get; private set; }

        /// <summary>
        /// <param name="options"/> The default configuration is used if not specified. </param>
        /// </summary>
        public DragonSpeakEngine(DragonSpeakOptions options = null)
        {
            this.Options = options ?? new DragonSpeakOptions();

            this.Lexer = new Lexer(
                new TokenDefinition(TokenType.Trigger,    new Regex(@"\(([0-9]{1})\:([0-9]{1," + int.MaxValue + @"})\)"                                                      , RegexOptions.Compiled)),
                new TokenDefinition(TokenType.String,     new Regex(@"\" + this.Options.StringBeginSymbol + @"(.*?)\" + this.Options.StringEndSymbol                         , RegexOptions.Compiled)),
                new TokenDefinition(TokenType.Array,      new Regex(@"(?:\" + this.Options.ArrayBeginSymbol + @"((.*?)(?:,(.+?))*)\" + this.Options.ArrayEndSymbol + @")+"   , RegexOptions.Compiled)),
                new TokenDefinition(TokenType.Number,     new Regex(@"[-+]?([0-9]*\.[0-9]+|[0-9]+)"                                                                          , RegexOptions.Compiled)),
                new TokenDefinition(TokenType.Comment,    new Regex(@"\" + this.Options.CommentSymbol + @".*[\r|\n]"                                                         , RegexOptions.Compiled), ignored: true),
                new TokenDefinition(TokenType.Word,       new Regex(@"\w+"                                                                                                   , RegexOptions.Compiled), ignored: true),
                new TokenDefinition(TokenType.Symbol,     new Regex(@"\W"                                                                                                    , RegexOptions.Compiled), ignored: true),
                new TokenDefinition(TokenType.Whitespace, new Regex(@"\s+"                                                                                                   , RegexOptions.Compiled), ignored: true));

            this.Parser = new Parser(Lexer);
            this.Pages = new List<Page>();
        }

        /// <summary> Load a DragonSpeak script from the specified source text. </summary>
        /// <param name="pageSource"> The script text to load. </param>
        /// <param name="causeDicoveryHandler">
        /// A delegate which is invoked whenever a <see cref="TriggerCategory.Cause"/> trigger is
        /// found during parsing.
        /// </param>
        /// <returns> A page containing triggers to execute. </returns>
        public Page LoadFromString(string pageSource,
            CauseTriggerDiscoveryHandler causeDicoveryHandler = null)
        {
            var blocks = this.Parser.Parse(pageSource);
            var page = new Page(this, causeDicoveryHandler).Insert(blocks);

            this.Pages.Add(page);
            return this.Pages.Last();
        }
    }

    public class DragonSpeakOptions
    {
        public DragonSpeakOptions()
        {

        }

        /// <summary>
        /// Allow unhandled <see cref="TriggerCategory.Cause"/> triggers to return true.
        /// </summary>
        public bool IgnoreUnhandledCauseTriggers { get; set; } = true;

        /// <summary>
        /// Allow an existing <see cref="Page.TriggerHandler"/> to be overridden.
        /// <para>Default: false</para>
        /// </summary>
        public bool CanOverrideTriggerHandlers { get; set; } = false;

        /// <summary>
        /// Beginning array literal symbol
        /// <para>Default: [</para>
        /// </summary>
        public char ArrayBeginSymbol { get; set; } = '[';

        /// <summary>
        /// Ending array literal symbol
        /// <para>Default: ]</para>
        /// </summary>
        public char ArrayEndSymbol { get; set; } = ']';

        /// <summary>
        /// Beginning string literal symbol
        /// <para>Default: {</para>
        /// </summary>
        public char StringBeginSymbol { get; set; } = '{';

        /// <summary>
        /// Ending string literal symbol
        /// <para>Default: }</para>
        /// </summary>
        public char StringEndSymbol { get; set; } = '}';

        /// <summary>
        /// Comment literal symbol
        /// <para>Default: *</para>
        /// </summary>
        public string CommentSymbol { get; set; } = "*";
    }

}
