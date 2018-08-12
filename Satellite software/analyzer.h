#pragma once
#include "datastorage.h"
enum FlyingState {
	RISING,
	HOVER,
	FALLING,
	UNAVAILABLE
};
class Analyzer
{
private:
	FlyingState prev,current;
	static float allowError;
	static unsigned short elemAmount;
	static FlyingState analyze(const DataStorage&);
public:
	Analyzer();
	~Analyzer();
	FlyingState Analyze(const DataStorage&);
	FlyingState getCurrentState() const;
	FlyingState getPreviousState() const;
};

