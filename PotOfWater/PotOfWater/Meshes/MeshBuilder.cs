using MathUtilities;
using System.Collections.Generic;

namespace PotOfWater.Meshes
{
    public interface IMeshBuilder
    {
        public Mesh Build();
        public void AddPoints(Point[] points);
        public void AddElements(List<FiniteElement> elements);
        public void AddFirstBoundary(List<FirstBoundaryEdge> edges);
        public void AddSecondBoundary(List<SecondBoundaryEdge> edges);
        public void AddThirdBoundary(List<ThirdBoundaryEdge> edges);
    }

    public class LinearMeshBuilder : IMeshBuilder
    {
        private Point[] points;
        private List<FiniteElement> elements;
        private List<FirstBoundaryEdge> firstBoundary;
        private List<SecondBoundaryEdge> secondBoundary;
        private List<ThirdBoundaryEdge> thirdBoundary;
        public void AddPoints(Point[] points) => this.points = points;
        public void AddElements(List<FiniteElement> elements) => this.elements = elements;
        public void AddFirstBoundary(List<FirstBoundaryEdge> firstBoundary) => this.firstBoundary = firstBoundary;
        public void AddSecondBoundary(List<SecondBoundaryEdge> secondBoundary) => this.secondBoundary = secondBoundary;
        public void AddThirdBoundary(List<ThirdBoundaryEdge> thirdBoundary) => this.thirdBoundary = thirdBoundary;
        public Mesh Build()
        {
            return new Mesh
            {
                NodeCount = points.Length,
                Elements = elements,
                Points = points,
                FirstBoundary = firstBoundary,
                SecondBoundary = secondBoundary,
                ThirdBoundary = thirdBoundary
            };
        }
    }
}
