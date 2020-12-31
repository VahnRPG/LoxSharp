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
			T visitIfStmt(If stmt);
			T visitPrintStmt(Print stmt);
			T visitVarStmt(Var stmt);
			T visitWhileStmt(While stmt);
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

		public class If : Stmt {
			public readonly Expr condition;
			public readonly Stmt thenBranch;
			public readonly Stmt elseBranch;
	
			public If(Expr condition, Stmt thenBranch, Stmt elseBranch) {
				this.condition = condition;
				this.thenBranch = thenBranch;
				this.elseBranch = elseBranch;
			}
			
			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitIfStmt(this);
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

		public class While : Stmt {
			public readonly Expr condition;
			public readonly Stmt body;
	
			public While(Expr condition, Stmt body) {
				this.condition = condition;
				this.body = body;
			}
			
			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitWhileStmt(this);
			}
		}

		abstract public T accept<T>(Visitor<T> visitor);
	}
}