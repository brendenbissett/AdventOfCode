package lib

import (
	"fmt"
	"os"
)

func OpenFile(filePathName string) (*os.File, error) {
	// Open the file.
	fmt.Printf("Reading file: %s\n", filePathName)
	file, err := os.Open(filePathName)
	if err != nil {
		return nil, err
	}

	return file, err
}
