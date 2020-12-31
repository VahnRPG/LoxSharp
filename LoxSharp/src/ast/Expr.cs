using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp {
	abstract public class Expr {
		public interface Visitor<T> {
			T visitAssignExpr(Assign expr);
			T visitBinaryExpr(Binary expr);
			T visitCallExpr(Call expr);
			T visitGroupingExpr(Grouping expr);
			T visitLiteralExpr(Literal expr);
			T visitLogicalExpr(Logical expr);
			T visitUnaryExpr(Unary expr);
			T visitVariableExpr(Variable expr);
		}

		public class Assign : Expr {
			public readonly Token name;
			public readonly Expr value;
	
			public Assign(Token name, Expr value) {
				this.name = name;
				this.value = value;
			}
			
			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitAssignExpr(this);
			}
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

		public class Call : Expr {
			public readonly Expr callee;
			public readonly Token paren;
			public readonly List<Expr> arguments;
	
			public Call(Expr callee, Token paren, List<Expr> arguments) {
				this.callee = callee;
				this.paren = paren;
				this.arguments = arguments;
			}
			
			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitCallExpr(this);
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

		public class Logical : Expr {
			public readonly Expr left;
			public readonly Token opr;
			public readonly Expr right;
	
			public Logical(Expr left, Token opr, Expr right) {
				this.left = left;
				this.opr = opr;
				this.right = right;
			}
			
			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitLogicalExpr(this);
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

		public class Variable : Expr {
			public readonly Token name;
	
			public Variable(Token name) {
				this.name = name;
			}
			
			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitVariableExpr(this);
			}
		}

		abstract public T accept<T>(Visitor<T> visitor);
	}
}