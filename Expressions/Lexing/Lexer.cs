using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Expressions.Lexing.Tokens;

namespace Expressions.Lexing
{
    public class LexedString : IEnumerable<ElementaryToken>
    {
        private delegate ElementaryToken ElementaryParser();

        private static readonly List<string> KnownOperators = new(){"+", "-", "*", "/"};
        private readonly string _text;
        private int _currentPosition;
        private readonly List<ElementaryParser> _parsers;
        
        public LexedString(string text)
        {
            _text = text;
            _currentPosition = 0;
            _parsers = new List<ElementaryParser> {ParseOperator, ParseIdentifier, ParseLiteral, ParseParen};
            SkipWhiteSpace();
        }

        private void SkipWhiteSpace()
        {
            while (_currentPosition < _text.Length && char.IsWhiteSpace(_text[_currentPosition]))
            {
                _currentPosition++;
            }
        }
        
        private bool HasTextLeft() => _currentPosition < _text.Length;

        private ElementaryToken ParseNextToken() => _parsers.Select(parser => parser()).FirstOrDefault(token => token != null);

        public static Position CreateDummyPosition(int absoluteOffset) => new Position(0, absoluteOffset, absoluteOffset);
        private OperatorToken ParseOperator()
        {
            var initialPosition = _currentPosition;
            return KnownOperators.Contains(_text[_currentPosition].ToString()) 
                ? new OperatorToken(_text[_currentPosition++].ToString(), 
                    CreateDummyPosition(initialPosition), 
                    CreateDummyPosition(initialPosition)) 
                : null;
        }

        private IdentifierToken ParseIdentifier()
        {
            var initialPosition = _currentPosition;
            
            if (char.IsLetter(_text[_currentPosition]) || _text[_currentPosition] == '_')
            {
                _currentPosition++;
            }
            else
            {
                return null;
            }

            while (HasTextLeft() && 
                   (char.IsLetterOrDigit(_text[_currentPosition]) 
                    || _text[_currentPosition] == '_'))
            {
                _currentPosition++;
            }

            var value = _text.Substring(initialPosition, _currentPosition - initialPosition);
            return new IdentifierToken(value, CreateDummyPosition(initialPosition), CreateDummyPosition(_currentPosition - 1));
        }

        private LiteralToken ParseLiteral()
        {
            var initialPosition = _currentPosition;
            return char.IsDigit(_text[_currentPosition])
                ? new LiteralToken(_text[_currentPosition++].ToString(), 
                    CreateDummyPosition(initialPosition), 
                    CreateDummyPosition(initialPosition))
                : null;
        }
        
        private ParenToken ParseParen()
        {
            ParenToken result = _text[_currentPosition] switch
            {
                '(' => new OpeningParenToken(CreateDummyPosition(_currentPosition), CreateDummyPosition(_currentPosition)),
                ')' => new ClosingParenToken(CreateDummyPosition(_currentPosition), CreateDummyPosition(_currentPosition)),
                _ => null
            };

            if (result != null)
            {
                _currentPosition++;
            }

            return result;
        }
        
        public IEnumerator<ElementaryToken> GetEnumerator()
        {
            while (HasTextLeft())
            {
                var nextToken = ParseNextToken();
                if (nextToken == null)
                {
                    yield break;
                }
                SkipWhiteSpace();
                yield return nextToken;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}