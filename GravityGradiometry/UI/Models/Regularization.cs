namespace UI.Models
{
    public class Regularization
    {
        public double Alpha { get; set; }
        public double[] Gamma { get; set; }

        public bool Validate() => Alpha > 0.0;
    }
}
