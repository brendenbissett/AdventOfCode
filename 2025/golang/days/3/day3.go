package day_three

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strings"

	"github.com/brendenbissett/AdventOfCode/2025/golang/internal/lib"
)

const DEBUG = true

func SolveDay3_Part1(filePath string) {
	log.Println("-- SolveDay3_Part1 -- ")

	// Get file (TODO, this "helper" function feels like a waste)
	file, err := lib.OpenFile(filePath)
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	banks := parseFileContents(file)

	for i, v := range banks {
		fmt.Printf("%d - %s\n", i, v)

		GetJoltage(v)
	}
}

func GetJoltage(bank string) int {

	batteries := strings.Split(bank, "")

	fmt.Println(batteries)

	for i := 9; i >= 0; i-- {
		var stringVal = string(i)
		if strings.Contains(bank, stringVal) {
			index := strings.Index(bank, stringVal)
			fmt.Printf("Found %d at index %d\n", i, index)
		}
	}

	return 0
}

func parseFileContents(file *os.File) []string {

	var values []string

	// Create a new scanner for the file. The default split function is bufio.ScanLines.
	scanner := bufio.NewScanner(file)

	// Iterate over the lines of the file. The scanner.Scan() method
	// returns true as long as there is a line to read.

	for scanner.Scan() {
		values = append(values, scanner.Text())
	}

	// Check for errors during scanning.
	if err := scanner.Err(); err != nil {
		log.Fatal(err)
	}

	return values
}
