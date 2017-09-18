using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Chronic
{
    public class PointerScanner : ITokenScanner
    {
        private static readonly Dictionary<Regex, Pointer> Patterns = new Dictionary<Regex, Pointer>
        {
            {new Regex(@"\bin\b"), new Pointer(Pointer.Type.Future)},
            {new Regex(@"\bfuture\b"), new Pointer(Pointer.Type.Future)},
            {new Regex(@"\bpast\b"), new Pointer(Pointer.Type.Past)},
        };

        public IList<Token> Scan(IList<Token> tokens, Options options)
        {
            tokens.ForEach(ApplyTags);
            return tokens;
        }

        private void ApplyTags(Token token)
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