using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp {
	abstract public class Stmt {
		public interface Visitor<T> {
			T visitBlockStmt(Block stmt);
			T visitExpressionStmt(Expression stmt);
			T visitPrintStmt(Print stmt);
			T visitVarStmt(Var stmt);
		}

		public class Block : Stmt {
			public readonly List<Stmt> statements;
	
			public Block(List<Stmt> statements) {
				this.statements = statements;
			}
			
			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitBlockStmt(this);
			}
		}

		public class Expression : Stmt {
			public readonly Expr expression;
	
			public Expression(Expr expression) {
				this.expression = expression;
			}
			
			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitExpressionStmt(this);
			}
		}

		public class Print : Stmt {
			public readonly Expr expression;
	
			public Print(Expr expression) {
				this.expression = expression;
			}
			
			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitPrintStmt(this);
			}
		}

		public class Var : Stmt {
			public readonly Token name;
			public readonly Expr initializer;
	
			public Var(Token name, Expr initializer) {
				this.name = name;
				this.initializer = initializer;
			}
			
			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitVarStmt(this);
			}
		}

		abstract public T accept<T>(Visitor<T> visitor);
	}
}