using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp {
	public class LoxFunction : LoxCallable {
		private readonly Stmt.Function declaration;
		private readonly LoxEnvironment closure;

		public LoxFunction(Stmt.Function declaration, LoxEnvironment closure) {
			this.declaration = declaration;
			this.closure = closure;
		}

		public int arity() {
			return declaration.parameters.Count;
		}

		public object call(Interpreter interpreter, List<object> arguments) {
			LoxEnvironment environment = new LoxEnvironment(closure);
			for (int i = 0; i < declaration.parameters.Count; i++) {
				environment.define(declaration.parameters[i].lexeme, arguments[i]);
			}

			try {
				interpreter.executeBlock(declaration.body, environment);
			}
			catch (Return returnValue) {
				return returnValue.value;
			}

			return null;
		}

		public override string ToString() {
			return "<fn " + declaration.name.lexeme + ">";
		}
	}
}