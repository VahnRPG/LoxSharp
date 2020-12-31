using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp {
	public class AstPrinter : Expr.Visitor<string> {
		public string print(Expr expr) {
			return expr.accept(this);
		}

		public string visitAssignExpr(Expr.Assign expr) {
			throw new NotImplementedException();
		}

		public string visitBinaryExpr(Expr.Binary expr) {
			return parenthesize(expr.opr.lexeme, expr.left, expr.right);
		}

		public string visitGroupingExpr(Expr.Grouping expr) {
			return parenthesize("group", expr.expression);
		}

		public string visitLiteralExpr(Expr.Literal expr) {
			if (expr.value == null) {
				return "nil";
			}

			return expr.value.ToString();
		}

		public string visitUnaryExpr(Expr.Unary expr) {
			return parenthesize(expr.opr.lexeme, expr.right);
		}

		public string visitVariableExpr(Expr.Variable expr) {
			throw new NotImplementedException();
		}

		public string visitBlockStmt(Stmt.Block stmt) {
			throw new NotImplementedException();
		}

		private string parenthesize(string name, params Expr[] exprs) {
			StringBuilder builder = new StringBuilder();

			builder.Append("(").Append(name);
			foreach (var expr in exprs) {
				builder.Append(" ");
				builder.Append(expr.accept(this));
			}
			builder.Append(")");

			return builder.ToString();
		}
	}
}