package day_two

import (
	"bufio"
	"log"
	"os"
	"strconv"
	"strings"

	"github.com/brendenbissett/AdventOfCode/2025/golang/internal/lib"
)

const DEBUG = false

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

	var list []int

	for index, value := range values {
		if DEBUG {
			log.Printf("Index: %d, Value: %s\n", index, value)
		}

		idRange := strings.Split(value, "-")

		start, err := strconv.Atoi(idRange[0])
		if err != nil {
			log.Printf("Could not convert start value of %s\n", value)
			panic(err)
		}

		end, err := strconv.Atoi(idRange[1])
		if err != nil {
			log.Printf("Could not convert end value of %s\n", value)
			panic(err)
		}

		list = append(list, findInvalidIDs(start, end)...)
	}

	log.Printf("Found %d invalid IDs", len(list))
	log.Print(list)
	log.Printf("Sum of Invalid IDs: %d", summIDs(list))

}

func summIDs(arr []int) int {
	total := 0

	for i := 0; i < len(arr); i++ {
		total = total + arr[i]
	}

	return total
}

func findInvalidIDs(start int, end int) []int {

	var invalidIDs []int

	for i := start; i < end+1; i++ {
		if !isValid_part_2(i) {
			invalidIDs = append(invalidIDs, i)
		}
	}

	return invalidIDs
}

type stats struct {
	number    rune
	instances int
	indexes   []int
}

func isValid_part_1(value int) bool {

	var valueString = strconv.Itoa(value)

	// If not an even string, then it must be valid (Can't contain repeated value)
	strLen := len(valueString)
	if strLen%2 != 0 {
		return true
	}

	/*
		if DEBUG {
			if valueString == "1188511880" || valueString == "1188511885" {
				fmt.Println("found it")
			}
		}
	*/

	// Split string in half, and compare
	if strLen > 3 {
		front := valueString[:(strLen / 2)]
		back := valueString[(strLen / 2):]

		// front and back of string is not the same
		if front != back {
			return true
		}
	}

	// Build map
	m := make(map[rune]stats)

	for index, val := range valueString {

		value, ok := m[val]
		if ok {
			value.indexes = append(value.indexes, index)
			value.instances = value.instances + 1

			m[val] = value
		} else {
			m[val] = stats{
				number:    val,
				instances: 1,
				indexes:   []int{index},
			}
		}
	}

	// If only one item in map, then it's invalid
	if len(m) == 1 {
		return false
	}

	// Iterate over stats to find anomolies
	for _, v := range m {

		// If any key is not repeated, then it must be valid
		if v.instances == 1 {
			return true
		}

		// If any key is not repeated evenly, then it must be valid
		if v.instances%2 != 0 {
			return true
		}
	}

	//log.Println(m)

	return false
}

func isValid_part_2(value int) bool {

	var valueString = strconv.Itoa(value)

	// Duplicate string, then check for contains
	doubled := valueString + valueString
	duplicateFound := strings.Index(doubled[1:], valueString) < len(valueString)-1

	return !duplicateFound
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
