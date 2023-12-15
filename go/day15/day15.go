package day15

import (
	"bufio"
	"fmt"
	"os"
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

func hash(step string) uint8 {
	var h uint8 = 0
	for i := 0; i < len(step); i++ {
		h += step[i]
		h *= 17
	}
	return h
}
