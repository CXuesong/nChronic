using System.Collections.Generic;

namespace Chronic
{
    public class GrabberScanner : ITokenScanner
    {
        private static readonly Dictionary<string, Grabber> _matches = new Dictionary<string, Grabber>
        {
            {"last", new Grabber(Grabber.Type.Last)},
            {"next", new Grabber(Grabber.Type.Next)},
            {"this", new Grabber(Grabber.Type.This)}
        };

        public IList<Token> Scan(IList<Token> tokens, Options options)
        {
            tokens.ForEach(ApplyGrabberTags);
            return tokens;
        }

        static void ApplyGrabberTags(Token token)
        {
            foreach (var match in _matches)
            {
                if (match.Key == token.Value)
                {
                    token.Tag(match.Value);
                }
            }
        }
    }
}