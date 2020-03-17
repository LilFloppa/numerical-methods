#pragma once

#include <fstream>
#include <iostream>
#include <string>
#include <vector>

#include "../FEM/Core.h"

bool ReadMeshData(std::string filename, int& n, double& begin, double& end)
{
	std::ifstream in(filename);

	if (in.is_open())
	{
		in >> n >> begin >> end;
		in.close();
		return true;
	}
	else
	{
		return false;
	}
}

void BuildMesh(int n, double begin, double end, std::vector<double>& mesh, std::vector<FiniteElement>& elements)
{
	double h = (end - begin) / n;

	int nodeNo = 0;
	for (int i = 0; i < n; i++)
	{
		FiniteElement element;
		element.begin = begin + h * i;
		element.end = begin + h * (i + 1);
		element.v1 = nodeNo++;
		element.v2 = nodeNo++;
		element.v3 = nodeNo;
		elements.push_back(element);

		mesh.push_back(begin + h * i);
		mesh.push_back(begin + h * i + h / 2);
	}

	mesh.push_back(end);
}