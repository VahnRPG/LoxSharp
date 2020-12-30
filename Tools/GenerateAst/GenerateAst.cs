using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp.GenerateAst {
	public class GenerateAst {
		public static void Main(string[] args) {
			if (args.Length != 1) {
				Console.Error.WriteLine("Usage: generate_ast <output directory>");
				Environment.Exit(64);
			}

			string output_dir = args[0];
			defineAst(output_dir, "Expr", new List<string> {
				"Binary	 : Expr left, Token opr, Expr right",
				"Grouping : Expr expression",
				"Literal	: object value",
				"Unary		: Token opr, Expr right"
			});
		}

		private static void defineAst(string output_dir, string base_name, List<string> types) {
			string path = output_dir + "/" + base_name + ".cs";

			using (StreamWriter writer = new StreamWriter(path)) {
				writer.NewLine = "\n";

				writer.WriteLine("using System;");
				writer.WriteLine("using System.Collections.Generic;");
				writer.WriteLine("using System.Linq;");
				writer.WriteLine("using System.Text;");
				writer.WriteLine("using System.Threading.Tasks;");
				writer.WriteLine("");
				writer.WriteLine("namespace LoxSharp {");
				writer.WriteLine("	abstract public class " + base_name + " {");

				defineVisitor(writer, base_name, types);

				bool found = false;
				foreach (var type in types) {
					string class_name = type.Split(':')[0].Trim();
					string fields = type.Split(':')[1].Trim();

					if (found) {
						writer.WriteLine("");
					}

					defineType(writer, base_name, class_name, fields);
					found = true;
				}

				writer.WriteLine("");
				writer.WriteLine("		abstract public T accept<T>(Visitor<T> visitor);");

				writer.WriteLine("	}");

				writer.Write("}");
				writer.Close();
			}
		}

		private static void defineVisitor(StreamWriter writer, string base_name, List<string> types) {
			writer.WriteLine("		public interface Visitor<T> {");

			foreach (var type in types) {
				string type_name = type.Split(':')[0].Trim();
				writer.WriteLine("			T visit" + type_name + base_name + "(" + type_name + " " + base_name.ToLower() + ");");
			}

			writer.WriteLine("		}");
			writer.WriteLine("");
		}

		private static void defineType(StreamWriter writer, string base_name, string class_name, string field_list) {
			writer.WriteLine("		public class " + class_name + " : " + base_name + " {");

			string[] fields = field_list.Split(',');
			foreach (var field in fields) {
				writer.WriteLine("			public readonly " + field.Trim() + ";");
			}
			writer.WriteLine("");
			writer.WriteLine("			public " + class_name + "(" + field_list + ") {");

			foreach (var field in fields) {
				string name = field.Trim().Split(' ')[1];
				writer.WriteLine("				this." + name + " = " + name + ";");
			}

			writer.WriteLine("			}");
			writer.WriteLine("");
			writer.WriteLine("			public override T accept<T>(Visitor<T> visitor) {");
			writer.WriteLine("				return visitor.visit" + class_name + base_name + "(this);");
			writer.WriteLine("			}");
			writer.WriteLine("		}");
		}
	}
}