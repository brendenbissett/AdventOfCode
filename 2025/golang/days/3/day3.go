package day_three

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
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

type battery struct {
	value   int
	indexes []int
}

func GetJoltage(bank string) int {

	batteries := strings.Split(bank, "")
	var list []battery

	fmt.Println(batteries)

	for i := 9; i >= 0; i-- {
		var stringVal = strconv.Itoa(i)
		if strings.Contains(bank, stringVal) {

			list = append(list, battery{
				value:   i,
				indexes: getAllIndexes(bank, stringVal),
			})
		}
	}

	fmt.Println(list)

	// Logic
	count := len(list)
	if count >= 2 {
		item1 := list[0]
		item2 := list[1]

		var result = ""
		if item1.indexes[0] < item2.indexes[0] {
			result = fmt.Sprintf("%d%d", item1.value, item2.value)
		} else {
			result = fmt.Sprintf("%d%d", item2.value, item1.value)

			if item1.indexes[0] < count-1 {
				// as long as largest number is not the last
			}
		}

		intVal, err := strconv.Atoi(result)
		if err != nil {
			fmt.Printf("Could not convert %s to interger. %e\n", result, err)
		}
		return intVal
	}

	return 0
}

func getAllIndexes(text string, searchTerm string) []int {

	var indexes []int

	offset := 0
	index := 0
	for {
		index = strings.Index(text[offset:], searchTerm)
		if index == -1 {
			break
		}

		absoluteIndex := index + offset
		indexes = append(indexes, absoluteIndex)

		offset = absoluteIndex + len(searchTerm)
	}

	return indexes
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
