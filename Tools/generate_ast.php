#!/usr/bin/env php
<?php
if (count($argv) != 2) {
	echo "Usage: generate_ast.php <output directory>";
	exit(64);
}

$output_dir = $argv[1];
define_ast($output_dir, "Expr", array(
	"Assign   : Token name, Expr value",
	"Binary   : Expr left, Token opr, Expr right",
	"Grouping : Expr expression",
	"Literal  : object value",
	"Unary    : Token opr, Expr right",
	"Variable : Token name",
));

define_ast($output_dir, "Stmt", array(
	"Block      : List<Stmt> statements",
	"Expression : Expr expression",
	"Print      : Expr expression",
	"Var        : Token name, Expr initializer",
));

function define_ast(string $output_dir, string $base_name, array $types) {
	$output = "using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp {
	abstract public class ".$base_name." {";

	$output = define_visitor($output, $base_name, $types);

	$found = false;
	foreach($types as $type) {
		$class_name = trim(explode(':', $type)[0]);
		$fields = trim(explode(':', $type)[1]);

		if ($found) {
			$output .= "\n";
		}

		$output = define_type($output, $base_name, $class_name, $fields);
		$found = true;
	}
	
	$output .= "

		abstract public T accept<T>(Visitor<T> visitor);
	}
}";
	
	file_put_contents($output_dir."/".$base_name.".cs", $output);
}

function define_visitor(string $output, string $base_name, array $types) {
	$output .= "
		public interface Visitor<T> {";

	foreach($types as $type) {
		$type_name = trim(explode(':', $type)[0]);
		$output .= "
			T visit".$type_name.$base_name."(".$type_name." ".strtolower($base_name).");";
	}

	$output .= "
		}
";
	
	return $output;
}

function define_type(string $output, string $base_name, string $class_name, string $field_list) {
	$output .= "
		public class ".$class_name." : ".$base_name." {";
	
	$fields = explode(',', $field_list);
	foreach($fields as $field) {
		$output .= "
			public readonly ".trim($field).";";
	}
	
	$output .= "
	
			public ".$class_name."(".$field_list.") {";

	foreach($fields as $field) {
		$name = explode(' ', trim($field))[1];
		$output .= "
				this.".$name." = ".$name.";";
	}
	
	$output .= "
			}
			
			public override T accept<T>(Visitor<T> visitor) {
				return visitor.visit".$class_name.$base_name."(this);
			}
		}";
	
	return $output;
}
?>