#include "long_number.h"

std::ostream& operator<<(std::ostream& os, const LongNumber& longNumber) {
	if (longNumber.buffer.size() == 0) {
		os << "0";
	}
	else {
		for (int i = (int)longNumber.buffer.size() - 1; i >= 0; --i) {
			os << longNumber.buffer[i];
		}
	}
	return os;
}

std::istream& operator>>(std::istream& is, LongNumber& longNumber) {
	std::string str; 
	is >> str;
	for (int i = (int) str.length(); i > 0; i -= 9) {
		if (i < 9)
			longNumber.buffer.push_back(atoi(str.substr(0, i).c_str()));
		else
			longNumber.buffer.push_back(atoi(str.substr(i - 9, 9).c_str()));
	}
	return is;
}