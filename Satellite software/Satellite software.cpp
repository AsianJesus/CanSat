// Satellite software.cpp: определяет точку входа для консольного приложения.
//

#include "stdafx.h"
#include <iostream>
#include "analyzer.h"
#include "device.h"

int main()	
{
	Analyzer a;
	DataStorage heights;
	heights << 10;
	a.Analyze(heights);
	heights << 20;
	a.Analyze(heights);
	heights << 20 << 10 << 11;
	a.Analyze(heights);
	std::cout << a.getPreviousState() << '\t' << a.getCurrentState() << std::endl;
	system("pause");
    return 0;
}

