#pragma once

#include <SLAE/Matrix.h>

#include "FEMInfo.h"

class SLAEBuilder
{
public:
	SLAEBuilder(int nodeCount, int JASize, int* IA, int* JA) : nodeCount(nodeCount), JASize(JASize), IA(IA), JA(JA)
	{
		InitMatrix();
	}

private:
	SLAE::Matrix A;
	int nodeCount, JASize;
	int* IA;
	int* JA;

	void InitMatrix()
	{
		A.N = nodeCount;
		A.DI = new double[nodeCount];
		A.AL = new double[JASize];
		A.AU = new double[JASize];
		A.IA = IA;
		A.JA = JA;

		for (int i = 0; i < nodeCount; i++)
			A.DI[i] = 0.0;

		for (int i = 0; i < JASize; i++)
			A.AL[i] = A.AU[i] = 0.0;
	}

	void Lambda()
	{

	}
};