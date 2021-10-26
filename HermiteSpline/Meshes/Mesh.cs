using MathUtilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HermiteSpline.Meshes
{
    public class Mesh : IEnumerable<Element>
    {
        public int NodeCount { get; private set; }
        public List<Element> Elements { get; set; } = new List<Element>();

        public Mesh(int nodeCount) => NodeCount = nodeCount;

        public Element FindContainingElement(Point p)
        {
            foreach (var e in Elements)
                if (e.Contains(p))
                    return e;

            throw new Exception($"Failed to find element for point ({ p.X }, { p.Y })");
        }
        public void ClearData() => Elements.ForEach(e => e.DataIndices.Clear());
        public IEnumerator<Element> GetEnumerator() => Elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Elements.GetEnumerator();
    }
}
