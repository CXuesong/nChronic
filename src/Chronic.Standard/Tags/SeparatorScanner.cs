using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Chronic
{
    public class SeparatorScanner : ITokenScanner
    {
        private static readonly Dictionary<Regex, Separator> Patterns = new Dictionary<Regex, Separator>
        {
            {@"^,$".Compile(), new SeparatorComma()},
            {@"^and$".Compile(), new SeparatorComma()},
            {@"^(at|@)$".Compile(), new SeparatorAt()},
            {@"^in$".Compile(), new SeparatorIn()},
            {@"^/$".Compile(), new SeparatorDate(Separator.Type.Slash)},
            {@"^-$".Compile(), new SeparatorDate(Separator.Type.Dash)},
            {@"^on$".Compile(), new SeparatorOn()},
        };

        public IList<Token> Scan(IList<Token> tokens, Options options)
        {
            tokens.ForEach(ApplyTags);
            return tokens;
        }

        static void ApplyTags(Token token)
        {
            foreach (var pattern in Patterns)
            {
                if (pattern.Key.IsMatch(token.Value))
                {
                    token.Tag(pattern.Value);
                }
            }
        }
    }
}