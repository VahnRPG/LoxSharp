using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp.src {
	public enum TokenType {
		NONE,

		//Single-character tokens
		LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
		COMMA, DOT, SEMICOLON, SLASH, STAR,

		// One or two character tokens
		BANG, BANG_EQUAL,
		EQUAL, EQUAL_EQUAL,
		GREATER, GREATER_EQUAL,
		LESS, LESS_EQUAL,
		MINUS, MINUS_MINUS,
		PLUS, PLUS_PLUS,

		//Literals
		IDENTIFIER, STRING, NUMBER,

		//Keywords
		AND, CLASS, ELSE, FALSE, FUNCTION, FOR, IF, NIL, OR,
		PRINT, BREAK, CONTINUE, RETURN, THIS, TRUE, VAR, WHILE,

		//Inheritence
		COLON, SUPER,

		EOF
	}
}