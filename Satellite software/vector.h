#pragma once
#include "action.h"
template<class T>
class Vector {
private:
	unsigned short firstIndx,lastIndx;
	unsigned short maxSize;
	T* storage;
public:
	void shiftLeft(unsigned short pos);
public:
	Vector();
	Vector(unsigned short max);
	void push(T value,bool eraseIfFull = true);
	T pop();
	T shift();
	T& first();
	T& last();
	void clear();
	unsigned short size() const;
	template<typename fn>
	void for_each(fn);
	T& operator[](unsigned short);
	T operator[](unsigned short) const;
	Vector& operator<<(T value);
	bool operator!();
};

template<class T>
inline void Vector<T>::shiftLeft(unsigned short pos)
{
	unsigned short iterator = firstIndx;
	lastIndx = pos < (lastIndx-firstIndx) ? lastIndx - pos : firstIndx;
	while (iterator < lastIndx) {
		storage[iterator] = storage[iterator + pos];
		iterator++;
	}
}

template<class T>
inline Vector<T>::Vector() : firstIndx(0),lastIndx(0),maxSize(64), storage(new T[maxSize])
{
}

template<class T>
inline Vector<T>::Vector(unsigned short max) : firstIndx(0),lastIndx(0),maxSize(max),storage(new T[maxSize])
{
}

template<class T>
inline void Vector<T>::push(T value, bool eraseIfFull)
{
	if (maxSize == lastIndx)
		if (eraseIfFull)
		{
			shiftLeft(1);
		}
		else {
			return;
		}
	storage[lastIndx++] = value;
}

template<class T>
inline T Vector<T>::pop()
{
	return lastIndx == firstIndx ? T() : storage[--lastIndx];	
}

template<class T>
inline T Vector<T>::shift()
{
	return firstIndx == lastIndx ? T() : storage[firstIndx++];
}

template<class T>
inline T& Vector<T>::first()
{
	return storage[firstIndx];
}

template<class T>
inline T& Vector<T>::last()
{
	return storage[lastIndx-1];
}

template<class T>
inline void Vector<T>::clear()
{
	delete storage;
	firstIndx = 0;
	lastIndx = maxSize;
}

template<class T>
inline unsigned short Vector<T>::size() const
{
	return lastIndx - firstIndx;
}

template<class T>
inline T & Vector<T>::operator[](unsigned short indx)
{
	return *(storage + firstIndx + indx);
}

template<class T>
inline T Vector<T>::operator[](unsigned short indx) const
{
	return *(storage + firstIndx + indx);
}

template<class T>
inline Vector<T> & Vector<T>::operator<<(T value)
{
	this->push(value);
	return *this;
}

template<class T>
inline bool Vector<T>::operator!()
{
	return firstIndx != lastIndx;
}

template<class T>
template<typename fn>
inline void Vector<T>::for_each(fn func)
{
	unsigned short it = firstIndx;
	while (it != lastIndx) {
		func(storage[it]);
		it++;
	}
}
