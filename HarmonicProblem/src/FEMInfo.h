#pragma once
#include "stdafx.h"

#pragma region Raw interval from file
struct RawInterval
{
	double begin, end;
	double q;
	int n;
};

inline std::istream& operator>>(std::istream& s, RawInterval& i)
{
	s >> i.begin >> i.end >> i.n >> i.q;
	return s;
}
#pragma endregion

#pragma region Interval prepeared for mesh building
struct Interval
{
	double begin, end;
	int n[2];
};

using IIterator = std::vector<Interval>::iterator;
#pragma endregion

#pragma region FiniteElement
struct FE
{
	double beginX, endX;
	double beginY, endY;
	double beginZ, endZ;
	std::vector<int> verts;
};

using FEIterator = std::vector<FE>::iterator;
#pragma endregion

