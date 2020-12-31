using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp.src {
	public class LoxInstance {
		private readonly Dictionary<string, object> fields = new Dictionary<string, object>();

		private LoxClass klass;

		public LoxInstance(LoxClass klass) {
			this.klass = klass;
		}

		public object get(Token name) {
			if (fields.ContainsKey(name.lexeme)) {
				return fields[name.lexeme];
			}

			LoxFunction method = klass.findMethod(name.lexeme);
			if (method != null) {
				return method.bind(this);
			}

			throw new RuntimeError(name, "Undefined property '" + name.lexeme + "'");
		}

		public void set(Token name, object value) {
			fields.Put(name.lexeme, value);
		}

		public override string ToString() {
			return klass.name + " instance";
		}
	}
}