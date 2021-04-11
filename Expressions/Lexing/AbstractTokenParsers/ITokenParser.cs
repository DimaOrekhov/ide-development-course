using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Expressions.Lexing.Tokens;

namespace Expressions.Lexing
{
    public interface ITokenParser
    {
        ParsingResult Parse(string text, Position initialPosition);
    }

    public class ParsingResult
    {
    }

    public class SuccessfulParsingResult : ParsingResult
    {
        public readonly Token Token;

        public SuccessfulParsingResult(Token token)
        {
            Token = token;
        }
    }

    public class FailedParsingResult : ParsingResult
    {
    }

    public abstract class RegexParser : ITokenParser
    {
        protected abstract Regex Regex
        {
            get;
        }

        protected abstract ElementaryToken MatchToToken(Match match);

        public ParsingResult Parse(string text, Position initialPosition)
        {
            var match = Regex.Match(text, initialPosition.AbsoluteOffset);
            
            if (!match.Success)
            {
                return new FailedParsingResult();
            }
            
            return new SuccessfulParsingResult(MatchToToken(match));
        }
    }

    public abstract class SequenceParser : ITokenParser
    {
        private bool _skipWhiteSpace;
        
        protected SequenceParser(bool skipWhitespace)
        {
            _skipWhiteSpace = skipWhitespace;
        }
        
        protected abstract IEnumerable<ITokenParser> Parsers
        {
            get;
        }
        
        public ParsingResult Parse(string text, Position initialPosition)
        {
            var currentPosition = initialPosition;
            var tokens = new List<Token>();
            
            foreach (var parser in Parsers)
            {
                var result = parser.Parse(text, currentPosition);

                if (result is FailedParsingResult)
                {
                    return new FailedParsingResult();
                }

                var token = ((SuccessfulParsingResult) result).Token;
                
                tokens.Add(token);

                currentPosition = new Position(token.End.Line, token.End.Offset, token.End.AbsoluteOffset + 1);
                if (_skipWhiteSpace)
                {
                    // TODO: parse and drop whitespace
                }
            }
            
            throw new NotImplementedException();
        }
    }
}