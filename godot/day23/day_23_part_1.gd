extends Node2D

@export var tilemap: TileMap


func _ready():
	var filePath = "res://day23/example1_in.txt"
	#var filePath = "res://day23/puzzle1_in.txt"

	var input = FileAccess.open(filePath, FileAccess.READ).get_as_text()
	print_debug(input)
	var lines = input.split("\n")

	# TODO: tmp
	remove_child(tilemap)


func _process(delta):
	pass
