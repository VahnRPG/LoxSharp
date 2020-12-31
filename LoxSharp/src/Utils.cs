using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp {
	public static class Utils {
		private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static double CurrentTimeMillis() {
			return (DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
		}

		public static void Put<T, U>(this Dictionary<T, U> dictionary, T key, U value) {
			if (dictionary.ContainsKey(key)) {
				dictionary[key] = value;
			}
			else {
				dictionary.Add(key, value);
			}
		}
	}
}