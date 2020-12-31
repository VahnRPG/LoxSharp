using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LoxSharp.TokenType;

namespace LoxSharp {
	public class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object> {
		public readonly LoxEnvironment globals = new LoxEnvironment();

		private LoxEnvironment environment;

		public Interpreter() {
			this.environment = globals;

			globals.define("clock", new Clock());
		}

		public void interpret(List<Stmt> statements) {
			try {
				foreach (var statement in statements) {
					execute(statement);
				}
			}
			catch (RuntimeError error) {
				LoxSharp.runtimeError(error);
			}
		}

		public object visitAssignExpr(Expr.Assign expr) {
			object value = evaluate(expr.value);
			environment.assign(expr.name, value);

			return value;
		}

		public object visitBinaryExpr(Expr.Binary expr) {
			object left = evaluate(expr.left);
			object right = evaluate(expr.right);

			switch (expr.opr.type) {
				case GREATER:
					checkNumberOperands(expr.opr, left, right);

					return (double) left > (double) right;
				case GREATER_EQUAL:
					checkNumberOperands(expr.opr, left, right);

					return (double) left >= (double) right;
				case LESS:
					checkNumberOperands(expr.opr, left, right);

					return (double) left < (double) right;
				case LESS_EQUAL:
					checkNumberOperands(expr.opr, left, right);

					return (double) left <= (double) right;
				case MINUS:
					checkNumberOperands(expr.opr, left, right);

					return (double) left - (double) right;
				case PLUS:
					if (left is double && right is double) {
						return (double) left + (double) right;
					}
					else if (left is string && right is string) {
						return (string) left + (string) right;
					}
					else if (left is string || right is string) {
						return left.ToString() + right.ToString();
					}

					throw new RuntimeError(expr.opr, "Operands must be two numbers or two strings");
				case SLASH:
					checkNumberOperands(expr.opr, left, right);
					if ((double) right == 0) {
						throw new RuntimeError(expr.opr, "Cannot divide by zero");
					}

					return (double) left / (double) right;
				case STAR:
					checkNumberOperands(expr.opr, left, right);

					return (double) left * (double) right;
				case BANG_EQUAL:
					return !isEqual(left, right);
				case EQUAL_EQUAL:
					return isEqual(left, right);
			}

			return null;
		}

		public object visitCallExpr(Expr.Call expr) {
			object callee = evaluate(expr.callee);

			List<object> arguments = new List<object>();
			foreach (var argument in expr.arguments) {
				arguments.Add(evaluate(argument));
			}

			if (!(callee is LoxCallable)) {
				throw new RuntimeError(expr.paren, "Can only call functions and classes");
			}

			LoxCallable function = (LoxCallable) callee;
			if (arguments.Count != function.arity()) {
				throw new RuntimeError(expr.paren, "Expected " + function.arity() + " arguments but got " + arguments.Count);
			}

			return function.call(this, arguments);
		}

		public object visitGroupingExpr(Expr.Grouping expr) {
			return evaluate(expr.expression);
		}

		public object visitLiteralExpr(Expr.Literal expr) {
			return expr.value;
		}

		public object visitLogicalExpr(Expr.Logical expr) {
			object left = evaluate(expr.left);

			if (expr.opr.type == OR) {
				if (isTruthy(left)) {
					return left;
				}
			}
			else {
				if (!isTruthy(left)) {
					return left;
				}
			}

			return evaluate(expr.right);
		}

		public object visitUnaryExpr(Expr.Unary expr) {
			object right = evaluate(expr.right);
			switch (expr.opr.type) {
				case BANG:
					return !isTruthy(right);
				case MINUS:
					checkNumberOperand(expr.opr, right);
					return -(double) right;
			}

			return null;
		}

		public object visitVariableExpr(Expr.Variable expr) {
			return environment.get(expr.name);
		}

		private void checkNumberOperand(Token opr, object operand) {
			if (operand is double) {
				return;
			}

			throw new RuntimeError(opr, "Operand must be a number");
		}

		private void checkNumberOperands(Token opr, object left, object right) {
			if (left is double && right is double) {
				return;
			}

			throw new RuntimeError(opr, "Operands must be numbers");
		}

		private bool isTruthy(object value) {
			if (value == null) {
				return false;
			}
			if (value is bool) {
				return (bool) value;
			}

			return true;
		}

		private bool isEqual(object a, object b) {
			if (a == null) {
				return (b == null);
			}

			return a.Equals(b);
		}

		private string stringify(object value) {
			if (value == null) {
				return "nil";
			}

			if (value is double) {
				string text = value.ToString();
				if (text.EndsWith(".0")) {
					text = text.Substring(0, text.Length - 2);
				}

				return text;
			}

			return value.ToString();
		}

		private object evaluate(Expr expr) {
			return expr.accept(this);
		}

		private void execute(Stmt stmt) {
			stmt.accept(this);
		}

		public object visitExpressionStmt(Stmt.Expression stmt) {
			evaluate(stmt.expression);

			return null;
		}

		public object visitFunctionStmt(Stmt.Function stmt) {
			LoxFunction function = new LoxFunction(stmt, environment);
			environment.define(stmt.name.lexeme, function);

			return null;
		}

		public object visitIfStmt(Stmt.If stmt) {
			if (isTruthy(evaluate(stmt.condition))) {
				execute(stmt.thenBranch);
			}
			else if (stmt.elseBranch != null) {
				execute(stmt.elseBranch);
			}

			return null;
		}

		public object visitVarStmt(Stmt.Var stmt) {
			object value = null;
			if (stmt.initializer != null) {
				value = evaluate(stmt.initializer);
			}

			environment.define(stmt.name.lexeme, value);

			return null;
		}

		public object visitWhileStmt(Stmt.While stmt) {
			while (isTruthy(evaluate(stmt.condition))) {
				execute(stmt.body);
			}

			return null;
		}

		public object visitReturnStmt(Stmt.Return stmt) {
			object value = null;
			if (stmt.value != null) {
				value = evaluate(stmt.value);
			}

			throw new Return(value);
		}

		public object visitPrintStmt(Stmt.Print stmt) {
			object value = evaluate(stmt.expression);
			Console.WriteLine(stringify(value));

			return null;
		}

		public object visitBlockStmt(Stmt.Block stmt) {
			executeBlock(stmt.statements, new LoxEnvironment(environment));

			return null;
		}

		public void executeBlock(List<Stmt> statements, LoxEnvironment environment) {
			LoxEnvironment previous = this.environment;

			try {
				this.environment = environment;

				foreach (var statement in statements) {
					execute(statement);
				}
			}
			finally {
				this.environment = previous;
			}
		}
	}
}