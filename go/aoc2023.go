package main

import (
	"aoc2023/day14"
	"aoc2023/day15"
	"fmt"
	"os"
	"strconv"
)

func main() {
	day, _ := strconv.Atoi(os.Args[1])
	part, _ := strconv.Atoi(os.Args[2])

	if day == 14 && part == 1 {
		day14.Part1()
	} else if day == 14 && part == 2 {
		day14.Part2()
	} else if day == 15 && part == 1 {
		day15.Part1()
	} else {
		fmt.Printf("\nThere is no code for day %d part %d\n", day, part)
	}
}
