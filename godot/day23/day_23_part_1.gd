extends Node2D

@export var tilemap: TileMap
@export var input_file: InputFile

enum InputFile { Example1, Puzzle1 }

const tiles = {
	"forest_nil": Vector2i(0, 0),
	"forest_full": Vector2i(1, 0),
	#
	"forest_edges_lr": Vector2i(2, 0),
	"forest_edges_tb": Vector2i(3, 0),
	#
	"forest_edges_tl": Vector2i(0, 1),
	"forest_edges_tr": Vector2i(1, 1),
	"forest_edges_bl": Vector2i(2, 1),
	"forest_edges_br": Vector2i(3, 1),
	#
	"forest_edges_l": Vector2i(0, 2),
	"forest_edges_t": Vector2i(1, 2),
	"forest_edges_r": Vector2i(2, 2),
	"forest_edges_b": Vector2i(3, 2),
	#
	"path_regular": Vector2i(0, 3),
	"path_slope_east": Vector2i(1, 3),
	"path_slope_south": Vector2i(2, 3),
	#
	"path_regular_marked": Vector2i(3, 3),
	"path_slope_east_marked": Vector2i(0, 4),
	"path_slope_south_marked": Vector2i(1, 4),
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
	var forest_map: Array = (
		Array(input.split("\n"))
		. map(func(line: String): return line.strip_edges())
		. filter(func(line: String): return line.length() > 0)
	)
	print_debug(forest_map)
	forest_map = add_padding(forest_map)
	print_debug(forest_map)

	# Remove sample tiles used only for visualizatin purpose in the editor
	tilemap.clear()

	draw_tiles(forest_map)
	#
	#await compute_lonest_path()


func _process(delta) -> void:
	fit_tiles_in_camera()


func add_padding(forest_map: Array) -> Array:
	var padded: Array = ["#".repeat(forest_map[0].length() + 2)]
	padded += forest_map.map(func(row: String): return "#" + row + "#")
	padded.append("#".repeat(forest_map[0].length() + 2))
	return padded


func draw_tiles(forest_map: Array) -> void:
	# Here we account for a 1 tile of padding around the map
	for row in range(1, forest_map.size() - 1):
		for col in range(1, forest_map[row].length() - 1):
			var bg_tile = tiles.path_regular
			var fg_tile = tiles.forest_nil
			if forest_map[row][col] == ">":
				bg_tile = tiles.path_slope_east
			if forest_map[row][col] == "v":
				bg_tile = tiles.path_slope_south
			if forest_map[row][col] == "#":
				fg_tile = tiles.forest_full
			if forest_map[row][col] != "#":
				if forest_map[row][col - 1] == "#" and forest_map[row][col + 1] == "#":
					fg_tile = tiles.forest_edges_lr
				elif forest_map[row - 1][col] == "#" and forest_map[row + 1][col] == "#":
					fg_tile = tiles.forest_edges_tb
				elif forest_map[row - 1][col] == "#" and forest_map[row][col - 1] == "#":
					fg_tile = tiles.forest_edges_tl
				elif forest_map[row - 1][col] == "#" and forest_map[row][col + 1] == "#":
					fg_tile = tiles.forest_edges_tr
				elif forest_map[row + 1][col] == "#" and forest_map[row][col - 1] == "#":
					fg_tile = tiles.forest_edges_bl
				elif forest_map[row + 1][col] == "#" and forest_map[row][col + 1] == "#":
					fg_tile = tiles.forest_edges_br
				elif forest_map[row][col - 1] == "#":
					fg_tile = tiles.forest_edges_l
				elif forest_map[row - 1][col] == "#":
					fg_tile = tiles.forest_edges_t
				elif forest_map[row][col + 1] == "#":
					fg_tile = tiles.forest_edges_r
				elif forest_map[row + 1][col] == "#":
					fg_tile = tiles.forest_edges_b
			elif forest_map[row][col] != "#":
				if forest_map[row][col - 1] == "#" and forest_map[row][col + 1] == "#":
					fg_tile = tiles.forest_edges_lr
				elif forest_map[row - 1][col] == "#" and forest_map[row + 1][col] == "#":
					fg_tile = tiles.forest_edges_tb
			# Here we account for a 1 tile of padding around the map
			tilemap.set_cell(0, Vector2i(col - 1, row - 1), 0, bg_tile)
			tilemap.set_cell(1, Vector2i(col - 1, row - 1), 0, fg_tile)


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
