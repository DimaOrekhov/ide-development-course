using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Expressions;

namespace Expressions
{
    public class LexedString : IEnumerable<Token>
    {
        private delegate Token ElementaryParser();

        private static readonly List<string> KnownOperators = new(){"+", "-", "*", "/"};
        private readonly string _text;
        private int _currentPosition;
        private readonly List<ElementaryParser> _parsers;
        
        public LexedString(string text)
        {
            _text = text;
            _currentPosition = 0;
            _parsers = new List<ElementaryParser> {ParseOperator, ParseVariable, ParseLiteral, ParseParen};
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

        private Token ParseNextToken() => _parsers.Select(parser => parser()).FirstOrDefault(token => token != null);

        private OperatorToken ParseOperator()
        {
            var initialPosition = _currentPosition;
            return KnownOperators.Contains(_text[_currentPosition].ToString()) 
                ? new OperatorToken(_text[_currentPosition++].ToString(), initialPosition) 
                : null;
        }

        private VariableToken ParseVariable()
        {
            var initialPosition = _currentPosition;
            return char.IsLetter(_text[_currentPosition]) 
                ? new VariableToken(_text[_currentPosition++].ToString(), initialPosition) 
                : null;
        }

        private LiteralToken ParseLiteral()
        {
            var initialPosition = _currentPosition;
            return char.IsDigit(_text[_currentPosition])
                ? new LiteralToken(_text[_currentPosition++].ToString(), initialPosition)
                : null;
        }
        
        private ParenToken ParseParen()
        {
            ParenToken result = _text[_currentPosition] switch
            {
                '(' => new OpeningParenToken(_currentPosition),
                ')' => new ClosingParenToken(_currentPosition),
                _ => null
            };

            if (result != null)
            {
                _currentPosition++;
            }

            return result;
        }
        
        public IEnumerator<Token> GetEnumerator()
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