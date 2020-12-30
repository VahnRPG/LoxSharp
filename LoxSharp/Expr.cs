using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp {
	abstract public class Expr {
		public interface Visitor<T> {
			T visitBinaryExpr(Binary expr);
			T visitGroupingExpr(Grouping expr);
			T visitLiteralExpr(Literal expr);
			T visitUnaryExpr(Unary expr);
		}

		public class Binary : Expr {
			public readonly Expr left;
			public readonly Token opr;
			public readonly Expr right;

			public Binary(Expr left, Token opr, Expr right) {
				this.left = left;
				this.opr = opr;
				this.right = right;
			}

			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitBinaryExpr(this);
			}
		}

		public class Grouping : Expr {
			public readonly Expr expression;

			public Grouping(Expr expression) {
				this.expression = expression;
			}

			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitGroupingExpr(this);
			}
		}

		public class Literal : Expr {
			public readonly object value;

			public Literal(object value) {
				this.value = value;
			}

			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitLiteralExpr(this);
			}
		}

		public class Unary : Expr {
			public readonly Token opr;
			public readonly Expr right;

			public Unary(Token opr, Expr right) {
				this.opr = opr;
				this.right = right;
			}

			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitUnaryExpr(this);
			}
		}

		abstract public T accept<T>(Visitor<T> visitor);
	}
}