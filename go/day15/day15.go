package day15

import (
	"bufio"
	"fmt"
	"os"
	"slices"
	"strconv"
	"strings"
)

func Part1() {
	fmt.Println("[ Day 15 Part 1 ]")

	scanner := bufio.NewScanner(os.Stdin)
	if !scanner.Scan() {
		fmt.Println(scanner.Err())
		return
	}
	initializationSequence := scanner.Text()
	steps := strings.Split(initializationSequence, ",")

	var sumOfHashes int = 0
	for _, step := range steps {
		h := hash(step)
		fmt.Printf("step: %s --> hash: %d\n", step, h)
		sumOfHashes += int(h)
	}

	fmt.Println("RESULT:", sumOfHashes)
}

type labeledLens struct {
	label       string
	focalLength int
}

func Part2() {
	fmt.Println("[ Day 15 Part 2 ]")

	scanner := bufio.NewScanner(os.Stdin)
	if !scanner.Scan() {
		fmt.Println(scanner.Err())
		return
	}
	initializationSequence := scanner.Text()
	steps := strings.Split(initializationSequence, ",")

	boxes := make(map[uint8][]labeledLens)
	for i := 0; i < 256; i++ {
		boxes[uint8(i)] = []labeledLens{}
	}
	for _, step := range steps {
		labelAndMaybeFocalLength := strings.Split(strings.Split(step, "-")[0], "=")
		label := labelAndMaybeFocalLength[0]
		focalLength := 0
		if len(labelAndMaybeFocalLength) > 1 {
			focalLength, _ = strconv.Atoi(labelAndMaybeFocalLength[1])
		}
		box := hash(label)
		fmt.Printf("\nstep: %-9s --> label: %-7s --> box: %3d , fl: %2d \n", step, label, box, focalLength)

		if focalLength > 0 {
			foundIndex := slices.IndexFunc(boxes[box], func(ll labeledLens) bool { return ll.label == label })
			if foundIndex >= 0 {
				boxes[box][foundIndex].focalLength = focalLength
			} else {
				boxes[box] = append(boxes[box], labeledLens{label, focalLength})
			}
		} else {
			foundIndex := slices.IndexFunc(boxes[box], func(ll labeledLens) bool { return ll.label == label })
			if foundIndex >= 0 {
				boxes[box] = append(boxes[box][:foundIndex], boxes[box][foundIndex+1:]...)
			}
		}

		printBoxes(boxes)
	}

	totalFocusingPower := 0
	for boxNumber, box := range boxes {
		for llPosition, ll := range box {
			focusingPower := (int(boxNumber) + 1) * (llPosition + 1) * ll.focalLength
			fmt.Printf("> %-7s ~> focusing power = %4d\n", ll.label, focusingPower)
			totalFocusingPower += focusingPower
		}
	}
	fmt.Printf("\nTOTAL FOCUSING POWER: %d\n", totalFocusingPower)
}

func printBoxes(boxes map[uint8][]labeledLens) {
	fmt.Printf("== BOXES ==\n")
	for i, box := range boxes {
		if len(box) == 0 {
			continue
		}
		fmt.Printf("Box %3d: ", i)
		for _, ll := range box {
			fmt.Printf("[%s %d] ", ll.label, ll.focalLength)
		}
		fmt.Println()
	}
}

func hash(stepOrLabel string) uint8 {
	var h uint8 = 0
	for i := 0; i < len(stepOrLabel); i++ {
		h += stepOrLabel[i]
		h *= 17
	}
	return h
}
