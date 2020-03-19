#include <vector>

#include "MeshBuilder.h"
#include "PortraitBuilder.h"
#include "SLAEBuilder.h"

int main()
{
	MeshBuilder mesh(2, 0.0, 1.0);
	PortraitBuilder portrait(mesh.GetNodeCount(), mesh.Begin(), mesh.End());
	SLAEBuilder slae(mesh.GetNodeCount(), portrait.GetJASize(), portrait.GetIA(), portrait.GetJA());

	//TODO: На каждой итерации нужно пересчитывать матрицу и вектор правой части
	//TODO: Передавая в SLAEBuilder начальное приближение
	//TODO: При сборке матрицы нужно для каждой локальной матрицы пересчитывать коэффициенты лямбда
	//TODO: локальная матрица собирается так: L = G1 * l1 + G2 * l2 + G3 * l3 + sigma * M / dt
	return 0;
}