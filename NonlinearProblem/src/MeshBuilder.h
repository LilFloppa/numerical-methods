#pragma once

#include <vector>
#include "FEMInfo.h"

class MeshBuilder
{
public:
	MeshBuilder(int intervalCount, double begin, double end) : intervalCount(intervalCount), begin(begin), end(end)
	{
		double h = (end - begin) / intervalCount;

		for (int i = 0; i < intervalCount; i++)
		{
			FiniteElement element;
			element.begin = begin + h * i;
			element.end = begin + h * (i + 1);
			element.v1 = nodeCount++;
			element.v2 = nodeCount++;
			element.v3 = nodeCount;
			elements.push_back(element);
		}

		nodeCount++;
	}

	FiniteElementIterator Begin() { return elements.begin(); }
	FiniteElementIterator End() { return elements.end(); }

	int GetNodeCount() { return nodeCount; }

private:
	int intervalCount = 0, nodeCount = 0;
	double begin = 0.0, end = 0.0;
	std::vector<FiniteElement> elements;
};