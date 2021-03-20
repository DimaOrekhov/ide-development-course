using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Expressions;

namespace Expressions
{
    public class LexedString : IEnumerable<Token>
    {
        public LexedString(string text)
        {
            _text = text;
            _currentPosition = 0;
            _parsers = new List<Func<Token>> {ParseOperator, ParseVariable, ParseLiteral, ParseParen};
            SkipWhiteSpace();
        }

        private static readonly List<string> KnownOperators = new(){"+", "-", "*", "/"};
        private readonly string _text;
        private int _currentPosition;
        private readonly List<Func<Token>> _parsers;

        private void SkipWhiteSpace()
        {
            while (_currentPosition < _text.Length && char.IsWhiteSpace(_text[_currentPosition]))
            {
                _currentPosition++;
            }
        }
        
        private bool HasTextLeft() => _currentPosition < _text.Length;

        private Token ParseNextToken() => _parsers.Select(parser => parser()).FirstOrDefault(token => token != null);

        private OperatorToken ParseOperator() =>
            KnownOperators.Contains(_text[_currentPosition].ToString()) 
                ? new OperatorToken(_text[_currentPosition++].ToString()) 
                : null;

        private VariableToken ParseVariable() =>
            char.IsLetter(_text[_currentPosition]) 
                ? new VariableToken(_text[_currentPosition++].ToString()) 
                : null;

        private LiteralToken ParseLiteral() =>
            char.IsDigit(_text[_currentPosition])
                ? new LiteralToken(_text[_currentPosition++].ToString())
                : null;

        private ParenToken ParseParen()
        {
            ParenToken result = _text[_currentPosition] switch
            {
                '(' => new OpeningParenToken(),
                ')' => new ClosingParenToken(),
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