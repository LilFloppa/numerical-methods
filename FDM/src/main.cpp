#include <SLAE/SLAESolver.h>
#include <fstream>

int main()
{
	SLAESolver::Matrix m;
	SLAESolver::ReadMatrix("input/size.txt", "input/matrix.txt", m);

	return 0;
}