#include <fstream>

#include "long_number.h"

int main(int argc, char* argv) {
	std::ifstream inputFile("input.txt");

	LongNumber longNumber;
	inputFile >> longNumber;
	LongNumber other = longNumber * 2;
	other = other - 1;

	std::ofstream outputFile("output.txt");
	outputFile << ((other % longNumber + 1) == longNumber);

	return 0;
}