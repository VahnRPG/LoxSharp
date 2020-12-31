using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp.src {
	public class Token {
		public readonly TokenType type;
		public readonly string lexeme;
		public readonly object literal;
		public readonly int line;

		public Token(TokenType type, string lexeme, object literal, int line) {
			this.type = type;
			this.lexeme = lexeme;
			this.literal = literal;
			this.line = line;
		}

		public override string ToString() {
			if (type == TokenType.EOF) {
				return type.ToString();
			}

			return type + " ('" + lexeme + "') = '" + literal + "'";
		}
	}
}