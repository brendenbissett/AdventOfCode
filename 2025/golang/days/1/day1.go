package day_one

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
)

const MIN = 0
const MAX = 99
const debug = true

func SolveDay1(filePath string) {

	var starting int = 50
	var endRotationOnZeroCount int = 0
	var passedZeroCount int = 0
	var passedZeroCountTotal int = 0
	var lineNumber int = 0

	// Open the file.
	fmt.Printf("Reading file: %s\n", filePath)
	file, err := os.Open(filePath)
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close() // Ensure the file is closed when the function returns

	// Create a new scanner for the file. The default split function is bufio.ScanLines.
	scanner := bufio.NewScanner(file)

	// Iterate over the lines of the file. The scanner.Scan() method
	// returns true as long as there is a line to read.
	if debug {
		fmt.Printf("The dial starts by pointing at %d.\n", starting)
	}
	for scanner.Scan() {

		passedZeroCount = 0
		line := scanner.Text() // Get the current line of text

		lineNumber = lineNumber + 1

		moves, err := strconv.Atoi(line[1:])
		if err != nil {
			fmt.Printf("\n\nUnable to process line %d - %s\n\n", lineNumber, line)
			break
		}

		direction := string((line[0]))

		if debug {
			//fmt.Printf("%d (%s) - %s - %d\n", lineNumber, line, direction, moves)
		}

		// Part Two
		// Too Low: 6067
		// Too High: 6121

		// Logic
		if moves > MAX {
			mod := moves % (MAX + 1)
			passedZeroCount = (moves / (MAX + 1))
			if mod > 0 {
				// Remove "times we'll loop for no reason. IE, rotate more than MAX."
				moves = mod
			}
		}

		if direction == "L" { //L

			if starting-moves < MIN {
				if starting > 0 {
					passedZeroCount = passedZeroCount + 1
				}
				starting = MAX - (moves - starting) + 1
			} else {
				starting = starting - moves
			}

		} else { // R
			if starting+moves > MAX {

				moves = moves - (MAX - starting)
				starting = MIN + moves - 1

				if starting > 0 {
					passedZeroCount = passedZeroCount + 1
				}

			} else {
				starting = starting + moves
			}
		}

		// Count the number of times we ended at 0
		if starting == 0 {
			endRotationOnZeroCount = endRotationOnZeroCount + 1
		}

		passedZeroCountTotal = passedZeroCountTotal + passedZeroCount

		if debug {
			if passedZeroCount > 0 {
				fmt.Printf("The dial is rotated %s to point at %d; during this rotation, it points at 0 %d times.\n", line, starting, passedZeroCount)
			} else {
				fmt.Printf("The dial is rotated %s to point at %d.\n", line, starting)
			}
		}

	}

	// Check for errors during scanning.
	if err := scanner.Err(); err != nil {
		log.Fatal(err)
	}

	fmt.Printf("Number of times dial ended on zero: %d\n", endRotationOnZeroCount)
	fmt.Printf("Number of times dial passed zero: %d\n", passedZeroCountTotal)
	fmt.Printf("Part 2 Combined answer: %d\n", endRotationOnZeroCount+passedZeroCountTotal)
}
