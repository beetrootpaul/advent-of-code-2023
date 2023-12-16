extends Node2D

func _ready() -> void:
	var filePath = "res://day16/example1_in.txt"
	var file = FileAccess.open(filePath, FileAccess.READ)
	var input = file.get_as_text()
	print_debug(input)
	$RESULT_text.text = input
