package day_two

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strings"

	"github.com/brendenbissett/AdventOfCode/2025/golang/internal/lib"
)

const DEBUG = true

type NumberRange struct {
	Lower int
	Upper int
}

func SolveDay2(filePath string) {

	// Get file (TODO, this "helper" function feels like a waste)
	file, err := lib.OpenFile(filePath)
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	values := parseFileContents(file)

	for index, value := range values {
		fmt.Printf("Index: %d, Value: %s\n", index, value)
	}

}

func parseFileContents(file *os.File) []string {

	var values []string

	// Create a new scanner for the file. The default split function is bufio.ScanLines.
	scanner := bufio.NewScanner(file)

	// Iterate over the lines of the file. The scanner.Scan() method
	// returns true as long as there is a line to read.

	for scanner.Scan() {
		values = strings.Split(scanner.Text(), ",") // Get the current line of text
	}

	// Check for errors during scanning.
	if err := scanner.Err(); err != nil {
		log.Fatal(err)
	}

	return values
}
