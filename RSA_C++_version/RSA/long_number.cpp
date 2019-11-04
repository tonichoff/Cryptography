#include "long_number.h"

LongNumber::LongNumber(const LongNumber& other) {
	buffer = other.buffer;
}

LongNumber::LongNumber(const int& integer) {
	buffer.push_back(integer);
}

LongNumber LongNumber::operator=(const LongNumber& other) {
	buffer = other.buffer;
	return *this;
}

LongNumber LongNumber::operator+(const LongNumber& other) const {
	LongNumber result(*this);
	int carry = 0;
	for (size_t i = 0; i < std::max(buffer.size(), other.buffer.size()) || carry; ++i) {
		if (i == result.buffer.size()) {
			result.buffer.push_back(0);
		}
		result.buffer[i] += carry + (i < other.buffer.size() ? other.buffer[i] : 0);
		carry = result.buffer[i] >= base;
		if (carry) {
			result.buffer[i] -= base;
		}
	}
	return result;
}

LongNumber LongNumber::operator-(const LongNumber& other) const {
	LongNumber result(*this);
	int carry = 0;
	for (size_t i = 0; i < other.buffer.size() || carry; ++i) {
		result.buffer[i] -= carry + (i < other.buffer.size() ? other.buffer[i] : 0);
		carry = result.buffer[i] < 0;
		if (carry) {
			result.buffer[i] += base;
		}
	}
	while (result.buffer.size() > 1 && result.buffer.back() == 0) {
		result.buffer.pop_back();
	}
	return result;
}

LongNumber LongNumber::operator*(const int& other) const {
	LongNumber result(*this);
	int carry = 0;
	for (size_t i = 0; i < result.buffer.size() || carry; ++i) {
		if (i == result.buffer.size()) {
			result.buffer.push_back(0);
		}
		long long cur = carry + result.buffer[i] * 1ll * other;
		result.buffer[i] = int(cur % base);
		carry = int(cur / base);
	}
	while (result.buffer.size() > 1 && result.buffer.back() == 0) {
		result.buffer.pop_back();
	}
	return result;
}

LongNumber LongNumber::operator*(const LongNumber& other) const {
	LongNumber result;
	result.buffer.resize(buffer.size() + other.buffer.size());
	for (size_t i = 0; i < buffer.size(); ++i) {
		for (int j = 0, carry = 0; j < (int) other.buffer.size() || carry; ++j) {
			long long cur = result.buffer[i + j] + buffer[i] * 1ll * (j < (int) other.buffer.size() ? other.buffer[j] : 0) + carry;
			result.buffer[i + j] = int(cur % base);
			carry = int(cur / base);
		}
	}
	while (result.buffer.size() > 1 && result.buffer.back() == 0) {
		result.buffer.pop_back();
	}
	return result;
}

LongNumber LongNumber::operator/(const int& other) const {
	LongNumber result(*this);
	int carry = 0;
	for (int i = (int) buffer.size() - 1; i >= 0; --i) {
		long long cur = buffer[i] + carry * 1ll * base;
		result.buffer[i] = int (cur / other);
		carry = int (cur % other);
	}
	while (result.buffer.size() > 1 && result.buffer.back() == 0) {
		result.buffer.pop_back();
	}
	return result;
}

int LongNumber::operator%(const int& other) const {
	LongNumber result(*this);
	int carry = 0;
	for (int i = (int)buffer.size() - 1; i >= 0; --i) {
		long long cur = buffer[i] + carry * 1ll * base;
		result.buffer[i] = int(cur / other);
		carry = int(cur % other);
	}
	return carry;
}
	
LongNumber LongNumber::operator/(const LongNumber& other) const {
	LongNumber left(1);
	LongNumber right(*this);
	while (true) {
		LongNumber mid = (left + right) / 2;
		if (mid * other > *this) {
			right = mid;
		}
		else {
			LongNumber reminder = *this - mid * other;
			if (reminder >= other) {
				left = mid;
			}
			else {
				return mid;
			}
		}
	}
}

LongNumber LongNumber::operator%(const LongNumber& other) const {
	LongNumber left(1);
	LongNumber right(*this);
	while (true) {
		LongNumber mid = (left + right) / 2;
		if (mid * other > *this) {
			right = mid;
		}
		else {
			LongNumber reminder = *this - (mid * other);
			if (reminder >= other) {
				left = mid;
			}
			else {
				return reminder;
			}
		}
	}
}

bool LongNumber::operator<(const LongNumber& other) const {
	if (buffer.size() < other.buffer.size()) {
		return true;
	}
	else if (buffer.size() > other.buffer.size()) {
		return false;
	}
	for (int i = buffer.size() - 1; i >= 0; --i) {
		if (buffer[i] < other.buffer[i]) {
			return true;
		}
		else if (buffer[i] > other.buffer[i]) {
			return false;
		}
	}
	return false;
}

bool LongNumber::operator==(const LongNumber& other) const {
	if (buffer.size() != other.buffer.size()) {
		return false;
	}
	for (int i = buffer.size() - 1; i >= 0; --i) {
		if (buffer[i] != other.buffer[i]) {
			return false;
		}
	}
	return true;
}

bool LongNumber::operator>(const LongNumber& other) const {
	return !(*this < other || *this == other);
}

bool LongNumber::operator<=(const LongNumber& other) const {
	return (*this < other || *this == other);
}

bool LongNumber::operator>=(const LongNumber& other) const {
	return (*this > other || *this == other);
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

LongNumber operator*(const int& number, const LongNumber& longNumber) {
	return longNumber * number;
}