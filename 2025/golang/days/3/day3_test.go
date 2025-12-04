package day_three

import (
	"testing"
)

// TestHelloName calls greetings.Hello with a name, checking
// for a valid return value.
func TestDay3(t *testing.T) {

	testCases := []struct {
		name     string
		input    string
		expected int
	}{
		{
			name:     "987654321111111",
			input:    "987654321111111",
			expected: 98,
		},
		{
			name:     "811111111111119",
			input:    "811111111111119",
			expected: 89,
		},
		{
			name:     "234234234234278",
			input:    "234234234234278",
			expected: 78,
		},
		{
			name:     "818181911112111",
			input:    "818181911112111",
			expected: 92,
		},
	}

	for _, tc := range testCases {
		out := GetJoltage(tc.input)
		if out != tc.expected {
			t.Errorf("%s test failed. expected %d - received %d", tc.name, tc.expected, out)
		}
	}
}
