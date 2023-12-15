package day14

import (
	"bufio"
	"fmt"
	"os"
	"strings"
)

func Part1() {
	fmt.Println("[ Day 14 Part 1 ]")

	fmt.Println("\nParsing stdin...")
	platform, failure := parseFile()
	if failure {
		return
	}
	fmt.Println("\nParsed:")
	for _, rowData := range platform {
		fmt.Println(string(rowData))
	}

	tiltPlatformNorth(platform)
	fmt.Println("\nTilted north:")
	for _, rowData := range platform {
		fmt.Println(string(rowData))
	}

	totalLoad := measureTotalLoad(platform)
	fmt.Println("\nTotal load:")
	fmt.Println(totalLoad)
}

func Part2() {
	fmt.Println("[ Day 14 Part 2 ]")

	fmt.Println("\nParsing stdin...")
	platform, failure := parseFile()
	if failure {
		return
	}
	fmt.Println("\nParsed:")
	for _, rowData := range platform {
		fmt.Println(string(rowData))
	}

	cycleByPlatforms := make(map[string]int)
	totalLoadByCycle := make(map[int]int)

	fmt.Println()

	cycleOfLoopStart := 0
	cycleOfLoopEnd := 0

	cycleN := 1_000_000_000
	for cycle := 1; cycle <= cycleN; cycle++ {
		performTiltingCycle(platform)
		fmt.Printf("Finished cycle %d (%d%%)\n", cycle, 100*cycle/cycleN)
		//for _, rowData := range platform {
		//	fmt.Println(string(rowData))
		//}

		key := asKey(platform)

		if cycleByPlatforms[key] == 0 {
			cycleByPlatforms[key] = cycle
			totalLoadByCycle[cycle] = measureTotalLoad(platform)
		} else {
			cycleOfLoopStart = cycleByPlatforms[key]
			cycleOfLoopEnd = cycle - 1
			break
		}
	}

	fmt.Println()

	fmt.Println("Loop start :", cycleOfLoopStart)
	fmt.Println("Loop end   :", cycleOfLoopEnd)

	earliestCycleSameAsN := cycleN
	if cycleOfLoopStart > 0 && cycleOfLoopEnd > 0 {
		loopingCycleN := cycleN - cycleOfLoopStart
		loopLength := cycleOfLoopEnd - cycleOfLoopStart + 1
		earliestCycleSameAsN = cycleOfLoopStart + (loopingCycleN % loopLength)
	}
	fmt.Printf("Earliest cycle same as cycle N(%d) = %d\n", cycleN, earliestCycleSameAsN)

	fmt.Println("\nTotal load:")
	fmt.Println(totalLoadByCycle[earliestCycleSameAsN])
}

func parseFile() ([][]rune, bool) {
	var platform [][]rune

	scanner := bufio.NewScanner(os.Stdin)
	for scanner.Scan() {
		platform = append(platform, []rune(scanner.Text()))
	}
	if err := scanner.Err(); err != nil {
		fmt.Println("Failed to scan line by line:", err)
		return nil, true
	}

	return platform, false
}

func performTiltingCycle(platform [][]rune) {
	tiltPlatformNorth(platform)
	tiltPlatformWest(platform)
	tiltPlatformSouth(platform)
	tiltPlatformEast(platform)
}

func tiltPlatformNorth(platform [][]rune) {
	for col := range platform[0] {
		var firstObstacleRow = -1
		for row, rowData := range platform {
			if rowData[col] == '#' {
				firstObstacleRow = row
			} else if rowData[col] == 'O' {
				platform[row][col] = '.'
				platform[firstObstacleRow+1][col] = 'O'
				firstObstacleRow = firstObstacleRow + 1
			}
		}
	}
}

func tiltPlatformWest(platform [][]rune) {
	for _, rowData := range platform {
		var firstObstacleCol = -1
		for col := range rowData {
			if rowData[col] == '#' {
				firstObstacleCol = col
			} else if rowData[col] == 'O' {
				rowData[col] = '.'
				rowData[firstObstacleCol+1] = 'O'
				firstObstacleCol = firstObstacleCol + 1
			}
		}
	}
}

func tiltPlatformSouth(platform [][]rune) {
	for col := range platform[0] {
		var firstObstacleRow = len(platform)
		for row := len(platform) - 1; row >= 0; row-- {
			rowData := platform[row]
			if rowData[col] == '#' {
				firstObstacleRow = row
			} else if rowData[col] == 'O' {
				platform[row][col] = '.'
				platform[firstObstacleRow-1][col] = 'O'
				firstObstacleRow = firstObstacleRow - 1
			}
		}
	}
}

func tiltPlatformEast(platform [][]rune) {
	for _, rowData := range platform {
		var firstObstacleCol = len(rowData)
		for col := len(rowData) - 1; col >= 0; col-- {
			if rowData[col] == '#' {
				firstObstacleCol = col
			} else if rowData[col] == 'O' {
				rowData[col] = '.'
				rowData[firstObstacleCol-1] = 'O'
				firstObstacleCol = firstObstacleCol - 1
			}
		}
	}
}

func asKey(platform [][]rune) string {
	var asStrings []string
	for _, rowData := range platform {
		asStrings = append(asStrings, string(rowData))
	}
	return strings.Join(asStrings[:], "")
}

func measureTotalLoad(platform [][]rune) int {
	var totalLoad = 0

	var maxLoad = len(platform)
	for row, rowData := range platform {
		var loadForThisRow = maxLoad - row
		for col := range rowData {
			if rowData[col] == 'O' {
				totalLoad += loadForThisRow
			}
		}
	}

	return totalLoad
}
