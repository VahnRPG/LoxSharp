using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp.src {
	public static class Utils {
		private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static double CurrentTimeMillis() {
			return (DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
		}

		public static V Get<K, V>(this Dictionary<K, V> dictionary, K key) {
			V value;
			if (dictionary.TryGetValue(key, out value)) {
				return value;
			}

			return default(V);
		}

		public static void Put<K, V>(this Dictionary<K, V> dictionary, K key, V value) {
			if (dictionary.ContainsKey(key)) {
				dictionary[key] = value;
			}
			else {
				dictionary.Add(key, value);
			}
		}
	}
}