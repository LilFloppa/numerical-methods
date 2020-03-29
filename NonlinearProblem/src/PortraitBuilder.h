#pragma once

#include <vector>

#include "FEMInfo.h"

class PortraitBuilder
{
public:
	PortraitBuilder(int nodeCount, FEIterator begin, FEIterator end): nodeCount(nodeCount), begin(begin), end(end)
	{
		connections.resize(nodeCount);
		BuildConnections();
		FillIA();
	}


	std::vector<int> GetIA() { return IA; }
	int GetJASize() { return IA.back(); }

private:
	int nodeCount, JASize;
	FEIterator begin, end;
	std::vector <std::vector<int>> connections;

	std::vector<int> IA;

	void BuildConnections()
	{
		for (FEIterator iter = begin; iter != end; iter++)
		{
			FiniteElement e = *iter;
			connections[e.v[2]].push_back(e.v[0]);
			connections[e.v[2]].push_back(e.v[1]);
			connections[e.v[1]].push_back(e.v[0]);
		}
	}

	void FillIA()
	{
		IA.resize(nodeCount + 1);
		IA[0] = IA[1] = 0;

		for (int i = 2; i < nodeCount + 1; i++)
			IA[i] = IA[i - 1] + connections[i - 1].size();
	}
};