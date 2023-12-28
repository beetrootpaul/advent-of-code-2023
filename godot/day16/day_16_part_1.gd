extends Node2D

const RIGHT = 1
const DOWN = 2
const LEFT = 4
const UP = 8

func _ready() -> void:
	#var filePath = "res://day16/example1_in.txt"
	var filePath = "res://day16/puzzle1_in.txt"
	
	var input = FileAccess.open(filePath, FileAccess.READ).get_as_text()
	print_debug(input)
	
	var contraption = []
	var visited = []
	var lines = input.split("\n")
	for line in lines:
		contraption.append(line)
		var visitedRow = []
		for i in line.length():
			visitedRow.append(0)
		visited.append(visitedRow)
	var n = contraption[0].length()
	#print_debug(n)
	#print_debug(contraption)
	
	var queueToVisit = [_traversal(0,0,RIGHT)]
	while (queueToVisit.size() > 0):
		var t = queueToVisit.pop_front()
	#print_debug(t)
		if (t.y < 0 || t.x < 0 || t.y >= n || t.x >= n):
			pass
		elif (visited[t.y][t.x] & t.direction == t.direction):
			pass
		elif (contraption[t.y][t.x] == "."):
			visited[t.y][t.x] = visited[t.y][t.x] | t.direction
			if (t.direction == LEFT):
				queueToVisit.push_back(_traversal(t.x - 1, t.y, LEFT))
			if (t.direction == RIGHT):
				queueToVisit.push_back(_traversal(t.x + 1, t.y, RIGHT))
			if (t.direction == UP):
				queueToVisit.push_back(_traversal(t.x, t.y - 1, UP))
			if (t.direction == DOWN):
				queueToVisit.push_back(_traversal(t.x, t.y + 1, DOWN))
		elif (contraption[t.y][t.x] == "|"):
			visited[t.y][t.x] = visited[t.y][t.x] | t.direction
			if (t.direction == LEFT):
				queueToVisit.push_back(_traversal(t.x, t.y - 1, UP))
				queueToVisit.push_back(_traversal(t.x, t.y + 1, DOWN))
			if (t.direction == RIGHT):
				queueToVisit.push_back(_traversal(t.x, t.y - 1, UP))
				queueToVisit.push_back(_traversal(t.x, t.y + 1, DOWN))
			if (t.direction == UP):
				queueToVisit.push_back(_traversal(t.x, t.y - 1, UP))
			if (t.direction == DOWN):
				queueToVisit.push_back(_traversal(t.x, t.y + 1, DOWN))
		elif (contraption[t.y][t.x] == "-"):
			visited[t.y][t.x] = visited[t.y][t.x] | t.direction
			if (t.direction == LEFT):
				queueToVisit.push_back(_traversal(t.x - 1, t.y, LEFT))
			if (t.direction == RIGHT):
				queueToVisit.push_back(_traversal(t.x + 1, t.y, RIGHT))
			if (t.direction == UP):
				queueToVisit.push_back(_traversal(t.x - 1, t.y, LEFT))
				queueToVisit.push_back(_traversal(t.x + 1, t.y, RIGHT))
			if (t.direction == DOWN):
				queueToVisit.push_back(_traversal(t.x - 1, t.y, LEFT))
				queueToVisit.push_back(_traversal(t.x + 1, t.y, RIGHT))
		elif (contraption[t.y][t.x] == "/"):
			visited[t.y][t.x] = visited[t.y][t.x] | t.direction
			if (t.direction == LEFT):
				queueToVisit.push_back(_traversal(t.x, t.y + 1, DOWN))
			if (t.direction == RIGHT):
				queueToVisit.push_back(_traversal(t.x, t.y - 1, UP))
			if (t.direction == UP):
				queueToVisit.push_back(_traversal(t.x + 1, t.y, RIGHT))
			if (t.direction == DOWN):
				queueToVisit.push_back(_traversal(t.x - 1, t.y, LEFT))
		elif (contraption[t.y][t.x] == "\\"):
			visited[t.y][t.x] = visited[t.y][t.x] | t.direction
			if (t.direction == LEFT):
				queueToVisit.push_back(_traversal(t.x, t.y - 1, UP))
			if (t.direction == RIGHT):
				queueToVisit.push_back(_traversal(t.x, t.y + 1, DOWN))
			if (t.direction == UP):
				queueToVisit.push_back(_traversal(t.x - 1, t.y, LEFT))
			if (t.direction == DOWN):
				queueToVisit.push_back(_traversal(t.x + 1, t.y, RIGHT))
				#print_debug(visited)
	
	var energizedTiles = 0
	for r in n:
		for c in n:
			if visited[r][c] > 0:
				energizedTiles += 1

	print_debug(energizedTiles)
	$RESULT_text.text = str(energizedTiles)

func _traversal(x,y,direction):
	var t = {}
	t.x = x
	t.y = y
	t.direction = direction
	return t
