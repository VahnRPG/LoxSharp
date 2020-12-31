using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp.src {
	public class StackList<T> : List<T> {
		public void Push(T item) {
			this.Add(item);
		}
		
		public T Pop() {
			int last = this.Count - 1;
			T item = this[last];
			this.RemoveAt(last);

			return item;
		}

		public T Peek() {
			return this[this.Count - 1];
		}

		public bool IsEmpty() {
			return !this.Any();
		}
	}
}