#include "stdafx.h"
#include "analyzer.h"

unsigned short Analyzer::elemAmount = 3;
float Analyzer::allowError = 0.5;

#define ABS(x) x > 0 ? x : -x

FlyingState Analyzer::analyze(const DataStorage & hInfo)
{
	Vector<float> heights = hInfo.getLast(Analyzer::elemAmount);
	FlyingState result = FlyingState::UNAVAILABLE;
	float average = 0;
	heights.for_each([&average](float h) {
		average += h;
	});
	average /= heights.size();
	float disperse = 0;
	heights.for_each([&disperse,average](float h) {
		disperse += ABS(h - average);
 	});
	if (disperse <= Analyzer::allowError) {
		result = FlyingState::HOVER;
	}
	else {
		result = heights.last() > 0 ? FlyingState::RISING : FlyingState::FALLING;
	}
	return result;
}

Analyzer::Analyzer() : prev(FlyingState::UNAVAILABLE),current(FlyingState::UNAVAILABLE)
{
}

Analyzer::~Analyzer()
{
}

FlyingState Analyzer::Analyze(const DataStorage & heights)
{
	prev = current;
	current = Analyzer::analyze(heights);
	return current;
}

FlyingState Analyzer::getCurrentState() const
{
	return current;
}

FlyingState Analyzer::getPreviousState() const
{
	return prev;
}
