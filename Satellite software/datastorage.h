#pragma once

#include "vector.h"

class DataStorage {
private:
	Vector<float> storage;
public:
	DataStorage(unsigned short maxSize = 30);
	~DataStorage();
	float& operator[](const int);
	unsigned short size() const;
	DataStorage& operator +=(const float);
	Vector<float> getAll()const;
	Vector<float> getLast(const unsigned short) const;
	float& last();
	void clear();
	DataStorage& operator<<(const float);
	DataStorage& operator>>(float&);
};