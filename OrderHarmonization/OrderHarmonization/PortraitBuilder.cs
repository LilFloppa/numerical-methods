using MathUtilities;
using OrderHarmonization.Meshes;
using System.Collections.Generic;

namespace OrderHarmonization
{
    public class PortraitBuilder
	{
		private ProblemInfo info;
		private int nodeCount;
		private SortedSet<int>[] connections;

		public PortraitBuilder(ProblemInfo info)
		{
			this.info = info;
			nodeCount = info.Mesh.NodeCount;
			connections = new SortedSet<int>[nodeCount];
			for (int i = 0; i < nodeCount; i++)
				connections[i] = new SortedSet<int>();

			BuildConnections(info.Mesh);
		}

		public Portrait Build()
		{
			Portrait p = new Portrait();
			p.IA = new int[nodeCount + 1];
			p.IA[0] = p.IA[1] = 0;

			for (int i = 2; i <= nodeCount; i++)
			{
				int col = p.IA[i - 1];
				p.IA[i] = col + connections[i - 1].Count;
			}

			p.JA = new int[p.IA[nodeCount]];

			for (int i = 1, k = 0; i < nodeCount; i++)
				foreach (int j in connections[i])
				{
					p.JA[k] = j;
					k++;
				}

			return p;
		}

		private void BuildConnections(Mesh mesh)
		{
			foreach (FiniteElement e in mesh.Elements)
			{
				for (int i = 1; i < info.Basis.Size; i++)
				{
					int a = e.Vertices[i];
					if (a != -1)
					{
						for (int j = 0; j < i; j++)
						{
							int b = e.Vertices[j];
							if (a < b) (a, b) = (b, a);

							if (b != -1)
								connections[a].Add(b);
						}
					}
				}
			}
		}
	};
}
