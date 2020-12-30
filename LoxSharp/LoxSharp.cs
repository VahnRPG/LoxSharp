﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp {
	public class LoxSharp {
		private static bool hadError = false;

		public static void Main(string[] args) {
			if (args.Length > 1) {
				Console.WriteLine("Usage: [script]");
				Environment.Exit(64);
			}
			else if (args.Length == 1) {
				runFile(args[0]);
			}
			else {
				runPrompt();
			}
		}

		private static void runFile(string path) {
			if (!File.Exists(path)) {
				Console.WriteLine("File could not be found: " + path);
				Environment.Exit(0);
			}

			string file_text = File.ReadAllText(path);
			run(file_text);
			if (hadError) {
				Environment.Exit(65);
			}
		}

		private static void runPrompt() {
			for (;;) {
				Console.Write("> ");
				string line = Console.ReadLine();
				if (line == null) {
					Console.WriteLine("Null passed!");
					break;
				}
				else if (line.Trim().ToLower() == "exit") {
					Console.WriteLine("Exit passed!");
					break;
				}

				run(line);
				hadError = false;
			}
		}

		private static void run(string script) {
			Scanner scanner = new Scanner(script);
			List<Token> tokens = scanner.scanTokens();
			Parser parser = new Parser(tokens);
			Expr expression = parser.parse();

			if (hadError) {
				return;
			}

			Console.WriteLine(new AstPrinter().print(expression));
		}

		public static void error(int line, string message) {
			report(line, "", message);
		}

		public static void error(Token token, string message) {
			if (token.type == TokenType.EOF) {
				report(token.line, " at end", message);
			}
			else {
				report(token.line, " at '" + token.lexeme + "'", message);
			}
		}

		private static void report(int line, string where, string message) {
			Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
			hadError = true;
		}
	}
}