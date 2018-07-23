#include "stdafx.h"
#include "datastorage.h"

DataStorage::DataStorage(unsigned short maxSize) : storage(maxSize)
{

}

DataStorage::~DataStorage()
{
	
}

float& DataStorage::operator[](const int indx)
{
	if (indx >= 0) {
		return storage[indx];
	}
	else {
		return storage[storage.size() + indx];
	}
}

unsigned short DataStorage::size() const
{
	return storage.size();
}

DataStorage & DataStorage::operator+=(const float value)
{
	storage << value;
	return *this;
}

Vector<float> DataStorage::getAll() const
{
	return storage;
}

Vector<float> DataStorage::getLast(const unsigned short size) const
{
	Vector<float> result = size;
	unsigned short indx = size > storage.size() ? 0 : storage.size() - size;
	unsigned short limit = storage.size() - 1;
	while (indx <= limit) {
		result << storage[indx];
		indx++;
	}
	return result;
}

float& DataStorage::last()
{
	return storage.last();
}

void DataStorage::clear()
{
	storage.clear();
}

DataStorage & DataStorage::operator<<(const float v) 
{
	storage << v;
	return *this;
}

DataStorage & DataStorage::operator>>(float & out) 
{
	out = storage.pop();
	return *this;
}
