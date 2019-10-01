#include <fstream>

#include "long_number.h"

int main(int argc, char* argv) {
	std::ifstream inputFile("input.txt");

	LongNumber longNumber;
	inputFile >> longNumber;
	LongNumber other = longNumber;

	std::ofstream outputFile("output.txt");
	outputFile << 2 * other;

	return 0;
}