using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp {
	public class LoxEnvironment {
		public readonly LoxEnvironment enclosing;

		private readonly Dictionary<string, object> values = new Dictionary<string, object>();

		public LoxEnvironment(LoxEnvironment enclosing = null) {
			this.enclosing = enclosing;
		}

		public void define(string name, object value) {
			if (values.ContainsKey(name)) {
				values[name] = value;
			}
			else {
				values.Add(name, value);
			}
		}

		public void assign(Token name, object value) {
			if (values.ContainsKey(name.lexeme)) {
				values[name.lexeme] = value;

				return;
			}

			if (enclosing != null) {
				enclosing.assign(name, value);

				return;
			}

			throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'");
		}

		public object get(Token name) {
			if (values.ContainsKey(name.lexeme)) {
				return values[name.lexeme];
			}

			if (enclosing != null) {
				return enclosing.get(name);
			}

			throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'");
		}
	}
}