#include "long_number.h"

LongNumber::LongNumber(const LongNumber& other) {
	buffer = other.buffer;
}

LongNumber LongNumber::operator+(const LongNumber& other) const {
	LongNumber result(*this);
	int carry = 0;
	for (size_t i = 0; i < std::max(buffer.size(), other.buffer.size()) || carry; ++i) {
		if (i == buffer.size()) {
			result.buffer.push_back(0);
		}
		result.buffer[i] += carry + (i < other.buffer.size() ? other.buffer[i] : 0);
		carry = buffer[i] >= base;
		if (carry) {
			result.buffer[i] -= base;
		}
	}
	return result;
}

std::ostream& operator<<(std::ostream& os, const LongNumber& longNumber) {
	if (longNumber.buffer.size() == 0) {
		os << "0";
	}
	else {
		for (int i = (int) longNumber.buffer.size() - 1; i >= 0; --i) {
			os << longNumber.buffer[i];
		}
	}
	return os;
}

std::istream& operator>>(std::istream& is, LongNumber& longNumber) {
	std::string str; 
	is >> str;
	for (int i = (int) str.length(); i > 0; i -= 9) {
		if (i < 9) {
			longNumber.buffer.push_back(atoi(str.substr(0, i).c_str()));
		}
		else {
			longNumber.buffer.push_back(atoi(str.substr(i - 9, 9).c_str()));
		}
	}
	return is;
}