using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LoxSharp.TokenType;

namespace LoxSharp {
	public class Parser {
		private class ParseError : SystemException {
		}

		private readonly List<Token> tokens;

		private int current = 0;

		public Parser(List<Token> tokens) {
			this.tokens = tokens;
		}

		public Expr parse() {
			try {
				return expression();
			}
			catch (ParseError error) {
				return null;
			}
		}

		private Expr expression() {
			return equality();
		}

		private Expr equality() {
			Expr expr = comparison();

			while (match(BANG_EQUAL, EQUAL_EQUAL)) {
				Token opr = previous();
				Expr right = comparison();
				expr = new Expr.Binary(expr, opr, right);
			}

			return expr;
		}

		private Expr comparison() {
			Expr expr = term();

			while (match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL)) {
				Token opr = previous();
				Expr right = term();
				expr = new Expr.Binary(expr, opr, right);
			}

			return expr;
		}

		private Expr term() {
			Expr expr = factor();

			while (match(MINUS, PLUS)) {
				Token opr = previous();
				Expr right = factor();
				expr = new Expr.Binary(expr, opr, right);
			}

			return expr;
		}

		private Expr factor() {
			Expr expr = unary();

			while (match(SLASH, STAR)) {
				Token opr = previous();
				Expr right = unary();
				expr = new Expr.Binary(expr, opr, right);
			}

			return expr;
		}

		private Expr unary() {
			if (match(BANG, MINUS)) {
				Token opr = previous();
				Expr right = unary();

				return new Expr.Unary(opr, right);
			}

			return primary();
		}

		private Expr primary() {
			if (match(FALSE)) {
				return new Expr.Literal(false);
			}
			if (match(TRUE)) {
				return new Expr.Literal(true);
			}
			if (match(NIL)) {
				return new Expr.Literal(null);
			}

			if (match(NUMBER, STRING)) {
				return new Expr.Literal(previous().literal);
			}

			if (match(LEFT_PAREN)) {
				Expr expr = expression();
				consume(RIGHT_PAREN, "Expect ')' after expression");

				return new Expr.Grouping(expr);
			}

			throw error(peek(), "Expect expression");
		}

		private Token consume(TokenType type, string message) {
			if (check(type)) {
				return advance();
			}

			throw error(peek(), message);
		}

		private bool match(params TokenType[] types) {
			foreach (var type in types) {
				if (check(type)) {
					advance();

					return true;
				}
			}

			return false;
		}

		private bool check(TokenType type) {
			if (isAtEnd()) {
				return false;
			}

			return (peek().type == type);
		}

		private Token advance() {
			if (!isAtEnd()) {
				current++;
			}

			return previous();
		}

		private bool isAtEnd() {
			return peek().type == EOF;
		}

		private Token peek() {
			return tokens[current];
		}

		private Token previous() {
			return tokens[current - 1];
		}

		private ParseError error(Token token, string message) {
			LoxSharp.error(token, message);

			return new ParseError();
		}

		private void synchronize() {
			advance();

			while (!isAtEnd()) {
				if (previous().type == SEMICOLON) {
					return;
				}

				switch (peek().type) {
					case CLASS:
					case FUNCTION:
					case VAR:
					case FOR:
					case IF:
					case WHILE:
					case PRINT:
					case RETURN:
						return;
				}

				advance();
			}
		}
	}
}