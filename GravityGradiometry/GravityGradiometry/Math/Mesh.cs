using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GravityGradiometry
{
    internal static class Mesh
    {

        private static (int s, int q)[] adjacentCells = {
                    (-1, -1),
                    (-1, 0),
                    (-1, 1),
                    (0, -1),
                    (0, 1),
                    (1, -1),
                    (1, 0),
                    (1, 1)
                };

        public static IEnumerable<int> GetAdjacentCells(int cell, int xCellCount, int zCellCount)
        {
            int celli = cell / xCellCount;
            int cellj = cell % xCellCount;

            foreach (var adjCell in adjacentCells)
            {
                int s = celli + adjCell.s;
                int q = cellj + adjCell.q;

                if (s >= 0 && s < xCellCount && q >= 0 && q < zCellCount)
                {
                    Console.WriteLine($"yield { s * xCellCount + q}");
                    yield return s * xCellCount + q;
                }
            }
        }
    }
}
