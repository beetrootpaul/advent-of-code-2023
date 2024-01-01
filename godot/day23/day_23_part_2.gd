extends Node2D

@export var tilemap: TileMap
@export var result_text: Label

@export var input_file: InputFile

enum InputFile { Example2, Puzzle2 }

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
	#
	"path_regular_marked": Vector2i(3, 3),
}

const tile_size = Vector2i(16, 16)

var forest_map: Array = []
var start: Vector2i = Vector2i(1, 1)
var end: Vector2i = Vector2i(1, 1)
var max_lengths: Array[Array] = []
var max_max_length: int = -1
var started: bool = false
var finished: bool = false
var curr_visited_path: Array = []

var computation: Thread


func _ready() -> void:
	var filePath: String
	match input_file:
		InputFile.Example2:
			filePath = "res://day23/example2_in.txt"
		InputFile.Puzzle2:
			filePath = "res://day23/puzzle2_in.txt"

	var input: String = FileAccess.open(filePath, FileAccess.READ).get_as_text()
	print_debug(input)
	# TODO: How to make it of type `Array[String]`?
	forest_map = (
		Array(input.split("\n"))
		. map(func(line: String): return line.strip_edges())
		. filter(func(line: String): return line.length() > 0)
	)
	add_padding()

	for col in forest_map[1].length():
		if forest_map[1][col] != "#":
			start = Vector2i(col, 1)
			break
	for col in forest_map[forest_map.size() - 2].length():
		if forest_map[forest_map.size() - 2][col] != "#":
			end = Vector2i(col, forest_map.size() - 2)
			break

	# Remove sample tiles used only for visualizatin purpose in the editor
	tilemap.clear()

	computation = Thread.new()
	# You can bind multiple arguments to a function Callable.
	computation.start(compute_longest_path.bind())


func _process(delta) -> void:
	draw_tiles()
	fit_tiles_in_camera()
	if started:
		result_text.text = str(max_max_length) + "\n" + str(max_lengths[end.y][end.x])
	if finished:
		result_text.text += "\nDONE"


func _exit_tree():
	computation.wait_to_finish()


func add_padding() -> void:
	var padded: Array = ["#".repeat(forest_map[0].length() + 2)]
	padded += forest_map.map(func(row: String): return "#" + row + "#")
	padded.append("#".repeat(forest_map[0].length() + 2))
	forest_map = padded


func draw_tiles() -> void:
	# Here we account for a 1 tile of padding around the map
	for row in range(1, forest_map.size() - 1):
		for col in range(1, forest_map[row].length() - 1):
			var bg_tile = (
				tiles.path_regular_marked
				if (Vector2i(col, row) in curr_visited_path)
				else tiles.path_regular
			)
			var fg_tile = tiles.forest_nil
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


func compute_longest_path() -> void:
	for forest_row in forest_map:
		var max_lengths_row: Array[int] = []
		for tile in forest_row:
			max_lengths_row.append(-1)
		max_lengths.append(max_lengths_row)

	#print_debug(forest_map.size() * forest_map[0].length())
	var map_size: int = forest_map.size() * forest_map[0].length()
	var step_duration: float = 6.25 / map_size
	var sync_computation_chain: int = max(map_size * map_size / 2_000_000, 1)

	started = true

	# element struct: xy, length, list of visited tile xy (path)
	var stack: Array = [[start, 0, [start]]]
	var chain = 1
	while !stack.is_empty():
		var tmp = stack.pop_back()
		var xy = tmp[0]
		var length = tmp[1]
		var visited_path = tmp[2]

		curr_visited_path = visited_path

		max_lengths[xy.y][xy.x] = length
		if xy == end:
			max_max_length = max(max_max_length, length)

		if chain % sync_computation_chain == 0:
			await get_tree().create_timer(0.01).timeout
		chain += 1

		for direction in [Vector2i.RIGHT, Vector2i.DOWN, Vector2i.LEFT, Vector2i.UP]:
			var next = xy + direction
			if next not in visited_path and forest_map[next.y][next.x] != "#":
				stack.push_back([next, length + 1, visited_path + [next]])

	finished = true


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
