using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp.src {
	abstract public class Stmt {
		public interface Visitor<T> {
			T visitBlockStmt(Block stmt);
			T visitClassStmt(Class stmt);
			T visitExpressionStmt(Expression stmt);
			T visitIfStmt(If stmt);
			T visitFunctionStmt(Function stmt);
			T visitPrintStmt(Print stmt);
			T visitBreakStmt(Break stmt);
			T visitReturnStmt(Return stmt);
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

		public class Class : Stmt {
			public readonly Token name;
			public readonly Expr.Variable superclass;
			public readonly List<Stmt.Function> methods;
	
			public Class(Token name, Expr.Variable superclass, List<Stmt.Function> methods) {
				this.name = name;
				this.superclass = superclass;
				this.methods = methods;
			}
			
			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitClassStmt(this);
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

		public class Function : Stmt {
			public readonly Token name;
			public readonly List<Token> parameters;
			public readonly List<Stmt> body;
	
			public Function(Token name, List<Token> parameters, List<Stmt> body) {
				this.name = name;
				this.parameters = parameters;
				this.body = body;
			}
			
			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitFunctionStmt(this);
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

		public class Break : Stmt {
			public readonly Token keyword;
	
			public Break(Token keyword) {
				this.keyword = keyword;
			}
			
			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitBreakStmt(this);
			}
		}

		public class Return : Stmt {
			public readonly Token keyword;
			public readonly Expr value;
	
			public Return(Token keyword, Expr value) {
				this.keyword = keyword;
				this.value = value;
			}
			
			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visitReturnStmt(this);
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