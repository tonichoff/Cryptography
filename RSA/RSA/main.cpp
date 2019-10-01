#include <fstream>

#include "long_number.h"

int main(int argc, char* argv) {
	std::ifstream inputFile("input.txt");

	LongNumber longNumber;
	inputFile >> longNumber;
	LongNumber other = longNumber;

	std::ofstream outputFile("output.txt");
	outputFile << (other == longNumber) << std::endl;
	outputFile << (other == longNumber * 2) << std::endl;
	outputFile << (other < longNumber * 2) << std::endl;
	outputFile << (2 * other < longNumber) << std::endl;
	outputFile << (other > longNumber * 2) << std::endl;
	outputFile << (2 * other > longNumber) << std::endl;
	outputFile << (other <= longNumber * 2) << std::endl;
	outputFile << (2 * other <= longNumber) << std::endl;
	outputFile << (other >= longNumber * 2) << std::endl;
	outputFile << (2 * other >= longNumber) << std::endl;
	outputFile << (other <= longNumber) << std::endl;
	outputFile << (other >= longNumber) << std::endl;

	return 0;
}