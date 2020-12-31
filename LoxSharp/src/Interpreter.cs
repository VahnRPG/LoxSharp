using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LoxSharp.src.TokenType;

namespace LoxSharp.src {
	public class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object> {
		public readonly LoxEnvironment globals = new LoxEnvironment();
		private readonly Dictionary<Expr, int?> locals = new Dictionary<Expr, int?>();

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

			int? distance = locals.Get(expr);
			if (distance != null) {
				environment.assignAt((int) distance, expr.name, value);
			}
			else {
				globals.assign(expr.name, value);
			}

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

		public object visitGetExpr(Expr.Get expr) {
			object obj = evaluate(expr.obj);
			if (obj is LoxInstance) {
				return ((LoxInstance) obj).get(expr.name);
			}

			throw new RuntimeError(expr.name, "Only instances have properties");
		}

		public object visitSetExpr(Expr.Set expr) {
			object obj = evaluate(expr.obj);
			if (!(obj is LoxInstance)) {
				throw new RuntimeError(expr.name, "Only instances have fields");
			}

			object value = evaluate(expr.value);
			((LoxInstance) obj).set(expr.name, value);

			return value;
		}

		public object visitThisExpr(Expr.This expr) {
			return lookUpVariable(expr.keyword, expr);
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
			return lookUpVariable(expr.name, expr);
		}

		private object lookUpVariable(Token name, Expr expr) {
			int? distance = locals.Get(expr);
			if (distance != null) {
				return environment.getAt((int) distance, name.lexeme);
			}

			return globals.get(name);
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

		public void resolve(Expr expr, int depth) {
			locals.Add(expr, depth);
		}

		public object visitBlockStmt(Stmt.Block stmt) {
			executeBlock(stmt.statements, new LoxEnvironment(environment));

			return null;
		}

		public object visitClassStmt(Stmt.Class stmt) {
			object superclass = null;
			if (stmt.superclass != null) {
				superclass = evaluate(stmt.superclass);
				if (!(superclass is LoxClass)) {
					throw new RuntimeError(stmt.superclass.name, "Superclass must be a class");
				}
			}

			environment.define(stmt.name.lexeme, null);

			if (stmt.superclass != null) {
				environment = new LoxEnvironment(environment);
				environment.define("super", superclass);
			}

			Dictionary<string, LoxFunction> methods = new Dictionary<string, LoxFunction>();
			foreach (var method in stmt.methods) {
				LoxFunction function = new LoxFunction(method, environment, method.name.lexeme.Equals("init"));
				methods.Put(method.name.lexeme, function);
			}

			LoxClass klass = new LoxClass(stmt.name.lexeme, (LoxClass) superclass, methods);

			if (superclass != null) {
				environment = environment.enclosing;
			}

			environment.assign(stmt.name, klass);

			return null;
		}

		public object visitSuperExpr(Expr.Super expr) {
			int? distance = locals.Get(expr);
			LoxClass superclass = (LoxClass) environment.getAt((int) distance, "super");
			LoxInstance obj = (LoxInstance) environment.getAt((int) distance - 1, "this");

			LoxFunction method = superclass.findMethod(expr.method.lexeme);
			if (method == null) {
				throw new RuntimeError(expr.method, "Undefined property '" + expr.method.lexeme + "'");
			}

			return method.bind(obj);
		}

		public object visitExpressionStmt(Stmt.Expression stmt) {
			evaluate(stmt.expression);

			return null;
		}

		public object visitFunctionStmt(Stmt.Function stmt) {
			LoxFunction function = new LoxFunction(stmt, environment, false);
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
				try {
					execute(stmt.body);
				}
				catch (Break breakValue) {
					break;
				}
			}

			return null;
		}

		public object visitBreakStmt(Stmt.Break stmt) {
			throw new Break();
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