using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp.src {
	public class Break : SystemException {
		public Break() : base(null, null) {
		}
	}
}