#pragma once

#include <vector>

#include "FEMInfo.h"

class PortraitBuilder
{
public:
	PortraitBuilder(int nodeCount, FiniteElementIterator begin, FiniteElementIterator end): nodeCount(nodeCount), begin(begin), end(end)
	{
		connections.resize(nodeCount);
		BuildConnections();
		FillIA();
		FillJA();
	}


	int* GetIA() { return IA; }
	int* GetJA() { return JA; }
	int GetJASize() { return JASize; }

private:
	int nodeCount, JASize;
	FiniteElementIterator begin, end;
	std::vector <std::vector<int>> connections;

	int* IA;
	int* JA;

	void BuildConnections()
	{
		for (FiniteElementIterator iter = begin; iter != end; iter++)
		{
			FiniteElement e = *iter;
			connections[e.v3].push_back(e.v1);
			connections[e.v3].push_back(e.v2);
			connections[e.v2].push_back(e.v1);
		}
	}

	void FillIA()
	{
		IA = new int[nodeCount + 1];
		IA[0] = IA[1] = 0;

		for (int i = 2; i < nodeCount + 1; i++)
			IA[i] = IA[i - 1] + connections[i - 1].size();
	}

	void FillJA()
	{
		JASize = IA[nodeCount];
		JA = new int[JASize];

		for (int i = 0; i < JASize; i++)
			JA[i] = 0;			

		for (int i = 0, pos = 0; i < nodeCount; i++)
			for (int j : connections[i])
			{
				JA[pos] = j;
				pos++;
			}
	}
};