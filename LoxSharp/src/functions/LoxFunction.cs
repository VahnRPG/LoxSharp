using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp.src {
	public class LoxFunction : LoxCallable {
		private readonly Stmt.Function declaration;
		private readonly LoxEnvironment closure;
		private readonly bool isInitializer;

		public LoxFunction(Stmt.Function declaration, LoxEnvironment closure, bool isInitializer) {
			this.declaration = declaration;
			this.closure = closure;
			this.isInitializer = isInitializer;
		}

		public LoxFunction bind(LoxInstance instance) {
			LoxEnvironment environment = new LoxEnvironment(closure);
			environment.define("this", instance);

			return new LoxFunction(declaration, environment, isInitializer);
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
				if (isInitializer) {
					return closure.getAt(0, "this");
				}

				return returnValue.value;
			}

			if (isInitializer) {
				return closure.getAt(0, "this");
			}

			return null;
		}

		public override string ToString() {
			return "<fn " + declaration.name.lexeme + ">";
		}
	}
}