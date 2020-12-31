using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp.src {
	public class LoxSharp {
		private static readonly Interpreter interpreter = new Interpreter();

		private static bool hadError = false;
		private static bool hadRuntimeError = false;

		public static void Main(string[] args) {
			if (args.Length > 1) {
				Console.WriteLine("Usage: [script]");
				Environment.Exit(64);
			}
			else if (args.Length == 1) {
				runFile(args[0]);
			}
			else {
				runFile("test.lox");
				//runPrompt();
			}
		}

		private static void runFile(string path) {
			if (!File.Exists(path)) {
				Console.WriteLine("File could not be found: " + path);
				Environment.Exit(0);
			}

			string file_text = File.ReadAllText(path);
			run(file_text);

			//Indicate an error in the exit code
			if (hadError) {
				Environment.Exit(65);
			}
			if (hadRuntimeError) {
				Environment.Exit(70);
			}
		}

		private static void runPrompt() {
			for (;;) {
				Console.Write("> ");
				string line = Console.ReadLine();
				if (line == null || line.Trim().ToLower() == "exit") {
					Console.WriteLine("Goodbye!");
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
			List<Stmt> statements = parser.parse();

			if (hadError) {
				return;
			}

			Resolver resolver = new Resolver(interpreter);
			resolver.resolve(statements);

			if (hadError) {
				return;
			}

			interpreter.interpret(statements);
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

		public static void runtimeError(RuntimeError error) {
			Console.Error.WriteLine(error.Message + "\n[" + error.token.line + "]");
			hadRuntimeError = true;
		}

		private static void report(int line, string where, string message) {
			Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
			hadError = true;
		}
	}
}