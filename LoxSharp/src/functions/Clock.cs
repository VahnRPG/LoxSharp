using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp {
	public class Clock : LoxCallable {
		public int arity() {
			return 0;
		}

		public object call(Interpreter interpreter, List<object> arguments) {
			return Utils.CurrentTimeMillis();
		}

		public override string ToString() {
			return "<native fn>";
		}
	}
}