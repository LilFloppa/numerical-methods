#pragma once
#include "stdafx.h"
#include "FEMInfo.h"

class MeshBuilder
{
public:
	MeshBuilder(
		IIterator beginX, IIterator endX, int xCount, 
		IIterator beginY, IIterator endY, int yCount, 
		IIterator beginZ, IIterator endZ, int zCount)
	{
		connections.resize(xCount * yCount * zCount, -1);

		for (IIterator iterX = beginX; iterX < endX; iterX++)
			for (IIterator iterY = beginY; iterY < endY; iterY++)
				for (IIterator iterZ = beginZ; iterZ < endZ; iterZ++)
				{
					FE e;
					e.beginX = iterX->begin;
					e.beginY = iterY->begin;
					e.beginZ = iterZ->begin;
					e.endX = iterX->end;
					e.endY = iterY->end;
					e.endZ = iterZ->end;

					for (int i = 0; i < 2; i++)
						for (int j = 0; j < 2; j++)
							for (int k = 0; k < 2; k++)
								if (connections[xCount * yCount * iterZ->n[i] + xCount * iterY->n[j] + iterX->n[k]] == -1)
								{
									connections[xCount * yCount * iterZ->n[i] + xCount * iterY->n[j] + iterX->n[k]] = nodeCount;
									e.verts.push_back(nodeCount++);
								}
								else
								{
									e.verts.push_back(connections[xCount * yCount * iterZ->n[i] + xCount * iterY->n[j] + iterX->n[k]]);
								}

					elements.push_back(e);
				}
			
	}

	FEIterator Begin() { return elements.begin(); }
	FEIterator End() { return elements.end(); }

private:
	std::vector<FE> elements;
	std::vector<int> connections;
	int nodeCount = 0;
};