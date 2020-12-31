using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp {
	public static class Utils {
		private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static double CurrentTimeMillis() {
			return (double) (DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
		}
	}
}