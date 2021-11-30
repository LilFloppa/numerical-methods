using MathUtilities;
using System.Collections.Generic;

namespace PotOfWater.Meshes
{
    public interface IMeshBuilder
    {
        public Mesh Build();
        public void AddPoints(Point[] points);
        public void AddElements(List<FiniteElement> elements);
        public void AddFirstBoundary(List<Edge> edges);
        public void AddSecondBoundary(List<Edge> edges);
        public void AddThirdBoundary(List<Edge> edges);
    }

    public class LinearMeshBuilder : IMeshBuilder
    {
        private Point[] points;
        private List<FiniteElement> elements;
        private List<Edge> firstBoundary;
        private List<Edge> secondBoundary;
        private List<Edge> thirdBoundary;
        public void AddPoints(Point[] points) => this.points = points;
        public void AddElements(List<FiniteElement> elements) => this.elements = elements;
        public void AddFirstBoundary(List<Edge> firstBoundary) => this.firstBoundary = firstBoundary;
        public void AddSecondBoundary(List<Edge> secondBoundary) => this.secondBoundary = secondBoundary;
        public void AddThirdBoundary(List<Edge> thirdBoundary) => this.thirdBoundary = thirdBoundary;
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
