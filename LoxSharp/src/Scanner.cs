using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LoxSharp.src.TokenType;

namespace LoxSharp.src {
	public class Scanner {
		private readonly string source;
		private readonly char[] source_chars;
		private readonly List<Token> tokens = new List<Token>();
		private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>() {
			{ "and", AND },
			{ "class", CLASS },
			{ "else", ELSE },
			{ "false", FALSE },
			{ "for", FOR },
			{ "function", FUNCTION },
			{ "if", IF },
			{ "nil", NIL },
			{ "or", OR },
			{ "print", PRINT },
			{ "break", BREAK },
			{ "continue", CONTINUE },
			{ "return", RETURN },
			{ "super", SUPER },
			{ "this", THIS },
			{ "true", TRUE },
			{ "var", VAR },
			{ "while", WHILE },
		};

		private int start = 0;
		private int current = 0;
		private int line = 1;

		public Scanner(string source) {
			this.source = source;
			this.source_chars = source.ToCharArray();
		}

		public List<Token> scanTokens() {
			while (!isAtEnd()) {
				start = current;
				scanToken();
			}

			tokens.Add(new Token(EOF, "", null, line));

			return tokens;
		}

		private void scanToken() {
			char c = advance();
			switch (c) {
				case '(':
					addToken(LEFT_PAREN);
					break;
				case ')':
					addToken(RIGHT_PAREN);
					break;
				case '{':
					addToken(LEFT_BRACE);
					break;
				case '}':
					addToken(RIGHT_BRACE);
					break;
				case ',':
					addToken(COMMA);
					break;
				case '.':
					addToken(DOT);
					break;
				case '-':
					addToken(match('-') ? MINUS_MINUS : MINUS);
					break;
				case '+':
					addToken(match('+') ? PLUS_PLUS : PLUS);
					break;
				case ';':
					addToken(SEMICOLON);
					break;
				case '*':
					addToken(STAR);
					break;
				case '!':
					addToken(match('=') ? BANG_EQUAL : BANG);
					break;
				case '=':
					addToken(match('=') ? EQUAL_EQUAL : EQUAL);
					break;
				case '<':
					addToken(match('=') ? LESS_EQUAL : LESS);
					break;
				case '>':
					addToken(match('=') ? GREATER_EQUAL : GREATER);
					break;
				case '/':
					if (match('/')) {
						while (peek() != '\n' && !isAtEnd()) {
							advance();
						}
					}
					else if (match('*')) {
						Console.WriteLine("Multiline check passed!");
						while (peek() != '*' && peekNext() != '/' && !isAtEnd() && !isAtNextEnd()) {
							if (peek() == '\n') {
								line++;
							}
							advance();
						}
						advance(2);
						Console.WriteLine("Multiline check finished!");
					}
					else {
						addToken(SLASH);
					}
					break;
				case ' ':
				case '\r':
				case '\t':
					break;
				case '\n':
					line++;
					break;
				case '"':
					processString();
					break;
				case ':':
					addToken(COLON);
					break;
				default:
					if (isDigit(c)) {
						processNumber();
					}
					else if (isAlpha(c)) {
						processIdentifier();
					}
					else {
						LoxSharp.error(line, "Unexpected character");
					}
					break;
			}
		}

		private void processString() {
			while (peek() != '"' && !isAtEnd()) {
				if (peek() == '\n') {
					line++;
				}
				advance();
			}

			if (isAtEnd()) {
				LoxSharp.error(line, "Unterminated string");
				return;
			}

			advance();

			string value = source.Substring(start + 1, current - start - 2);
			addToken(STRING, value);
		}

		private void processNumber() {
			while (isDigit(peek())) {
				advance();
			}

			if (peek() == '.' && isDigit(peekNext())) {
				advance();
				while (isDigit(peek())) {
					advance();
				}
			}

			addToken(NUMBER, double.Parse(source.Substring(start, current - start)));
		}

		private void processIdentifier() {
			while (isAlphaNumeric(peek())) {
				advance();
			}

			string text = source.Substring(start, current - start);
			TokenType type = (keywords.ContainsKey(text) ? keywords[text] : NONE);
			if (type == NONE) {
				type = IDENTIFIER;
			}

			addToken(type);
		}

		private bool match(char expected) {
			if (isAtEnd()) {
				return false;
			}
			else if (source_chars[current] != expected) {
				return false;
			}

			current++;

			return true;
		}

		private char peek() {
			if (isAtEnd()) {
				return '\0';
			}

			return source_chars[current];
		}

		private char peekNext() {
			if (current + 1 >= source.Length) {
				return '\0';
			}

			return source_chars[current + 1];
		}

		private bool isDigit(char c) {
			return c >= '0' && c <= '9';
		}

		private bool isAlpha(char c) {
			return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
		}

		private bool isAlphaNumeric(char c) {
			return isAlpha(c) || isDigit(c);
		}

		private char advance(int length = 1) {
			current += length;

			return source_chars[current - 1];
		}

		private void addToken(TokenType type, object literal = null) {
			string text = source.Substring(start, current - start);
			tokens.Add(new Token(type, text, literal, line));
		}

		private bool isAtEnd() {
			return current >= source.Length;
		}

		private bool isAtNextEnd() {
			return current + 1 >= source.Length;
		}
	}
}