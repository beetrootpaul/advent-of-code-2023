extends Node2D

@export var tilemap: TileMap
@export var input_file: InputFile

enum InputFile { Example1, Puzzle1 }

const tiles = {
	"forestNil": Vector2i(0, 0),
	"forestFull": Vector2i(1, 0),
	#
	"forestEdgesLeftRight": Vector2i(2, 0),
	"forestEdgesTopBottom": Vector2i(3, 0),
	#
	"forestEdgesTopLeft": Vector2i(0, 1),
	"forestEdgesTopRight": Vector2i(1, 1),
	"forestEdgesBottomLeft": Vector2i(2, 1),
	"forestEdgesBottomRight": Vector2i(3, 1),
	#
	"forestEdgesLeft": Vector2i(0, 2),
	"forestEdgesTop": Vector2i(1, 2),
	"forestEdgesRight": Vector2i(2, 2),
	"forestEdgesBottom": Vector2i(3, 2),
	#
	"pathRegular": Vector2i(0, 3),
	"pathSlopeEast": Vector2i(1, 3),
	"pathSlopeSouth": Vector2i(2, 3),
	#
	"pathRegularMarked": Vector2i(3, 3),
	"pathSlopeSouthMarked": Vector2i(0, 4),
	"pathSlopeEastMarked": Vector2i(1, 4),
}

const tile_size = Vector2i(16, 16)


func _ready() -> void:
	var filePath: String
	match input_file:
		InputFile.Example1:
			filePath = "res://day23/example1_in.txt"
		InputFile.Puzzle1:
			filePath = "res://day23/puzzle1_in.txt"

	var input: String = FileAccess.open(filePath, FileAccess.READ).get_as_text()
	print_debug(input)
	# TODO: How to make it of type `Array[String]`?
	var forestMap: Array = (
		Array(input.split("\n"))
		. map(func(line: String): return line.strip_edges())
		. filter(func(line: String): return line.length() > 0)
	)

	# Remove sample tiles used only for visualizatin purpose in the editor
	tilemap.clear()

	draw_tiles(forestMap)


func _process(delta) -> void:
	fit_tiles_in_camera()


func draw_tiles(forestMap: Array) -> void:
	for row in forestMap.size():
		for col in forestMap[row].length():
			var bgTile = tiles.pathRegular
			var fgTile = tiles.forestNil
			if forestMap[row][col] == ">":
				bgTile = tiles.pathSlopeEast
			if forestMap[row][col] == "v":
				bgTile = tiles.pathSlopeSouth
			if forestMap[row][col] == "#":
				fgTile = tiles.forestFull
			if (
				forestMap[row][col] != "#"
				and row > 0
				and col > 0
				and row < forestMap.size() - 1
				and col < forestMap[row].length() - 1
			):
				if forestMap[row][col - 1] == "#" and forestMap[row][col + 1] == "#":
					fgTile = tiles.forestEdgesLeftRight
				elif forestMap[row - 1][col] == "#" and forestMap[row + 1][col] == "#":
					fgTile = tiles.forestEdgesTopBottom
				elif forestMap[row - 1][col] == "#" and forestMap[row][col - 1] == "#":
					fgTile = tiles.forestEdgesTopLeft
				elif forestMap[row - 1][col] == "#" and forestMap[row][col + 1] == "#":
					fgTile = tiles.forestEdgesTopRight
				elif forestMap[row + 1][col] == "#" and forestMap[row][col - 1] == "#":
					fgTile = tiles.forestEdgesBottomLeft
				elif forestMap[row + 1][col] == "#" and forestMap[row][col + 1] == "#":
					fgTile = tiles.forestEdgesBottomRight
				elif forestMap[row][col - 1] == "#":
					fgTile = tiles.forestEdgesLeft
				elif forestMap[row - 1][col] == "#":
					fgTile = tiles.forestEdgesTop
				elif forestMap[row][col + 1] == "#":
					fgTile = tiles.forestEdgesRight
				elif forestMap[row + 1][col] == "#":
					fgTile = tiles.forestEdgesBottom
			elif forestMap[row][col] != "#":
				if forestMap[row][col - 1] == "#" and forestMap[row][col + 1] == "#":
					fgTile = tiles.forestEdgesLeftRight
				elif forestMap[row - 1][col] == "#" and forestMap[row + 1][col] == "#":
					fgTile = tiles.forestEdgesTopBottom
			tilemap.set_cell(0, Vector2i(col, row), 0, bgTile)
			tilemap.set_cell(1, Vector2i(col, row), 0, fgTile)


func fit_tiles_in_camera():
	var viewport_rect: Vector2 = get_viewport_rect().size
	var tilemap_rect: Vector2 = Vector2(tilemap.get_used_rect().size * tile_size)
	var perfect_fit_scale = viewport_rect / tilemap_rect
	#print_debug("perfect_fit_scale: ", perfect_fit_scale)
	var ratio_preserving_scale = Vector2(
		min(perfect_fit_scale.x, perfect_fit_scale.y), min(perfect_fit_scale.x, perfect_fit_scale.y)
	)
	#print_debug("ratio_preserving_scale: ", ratio_preserving_scale)
	tilemap.scale = ratio_preserving_scale
	tilemap_rect = tilemap_rect * tilemap.scale

	tilemap.position = Vector2(
		(viewport_rect.x - tilemap_rect.x) / 2, (viewport_rect.y - tilemap_rect.y) / 2
	)
