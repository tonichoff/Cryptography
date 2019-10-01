#pragma once
#include <vector>
#include <iostream>
#include <string>
#include <algorithm>

class LongNumber {
public:
	LongNumber() {}
	LongNumber(const LongNumber&);

	LongNumber operator+(const LongNumber&) const;
	LongNumber operator-(const LongNumber&) const;

	friend std::ostream& operator<<(std::ostream&, const LongNumber&);
	friend std::istream& operator>>(std::istream&, LongNumber&);
private:
	const int base = (int) 1e9;
	std::vector<int> buffer;
};