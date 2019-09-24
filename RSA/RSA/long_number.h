#pragma once
#include <vector>
#include <iostream>
#include <string>

class LongNumber {
public:
	friend std::ostream& operator<<(std::ostream&, const LongNumber&);

	friend std::istream& operator>>(std::istream&, LongNumber&);
private:
	const int base = (int) 1e9;
	std::vector<int> buffer;
};