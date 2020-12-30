using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LoxSharp.TokenType;

namespace LoxSharp {
	public class Interpreter : Expr.Visitor<object> {
		public void interpret(Expr expression) {
			try {
				object value = evaluate(expression);
				Console.WriteLine(stringify(value));
			}
			catch (RuntimeError error) {
				LoxSharp.runtimeError(error);
			}
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

		public object visitGroupingExpr(Expr.Grouping expr) {
			return evaluate(expr.expression);
		}

		public object visitLiteralExpr(Expr.Literal expr) {
			return expr.value;
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
	}
}