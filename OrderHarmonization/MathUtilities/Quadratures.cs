﻿using System;
using System.Collections.Generic;

namespace MathUtilities
{
    public static class Quadratures
    {
        private class QuadratureNode
        {
            public QuadratureNode(Point node, double weight)
            {
                Node = node;
                Weight = weight;
            }
            public Point Node { get; }
            public double Weight { get; }
        }

        private static IEnumerable<QuadratureNode> TriangleOrder4()
        {
            const double a = 0.816847572980459;
            const double b = 0.108103018168070;
            const double c = 0.091576213509771;
            const double d = 0.445948490915965;
            const double w1 = 0.109951743655322;
            const double w2 = 0.223381589678011;
            double[] p1 = { a, c, c, b, d, d };
            double[] p2 = { c, a, c, d, b, d };
            double[] w = { w1, w1, w1, w2, w2, w2 };
            for (int i = 0; i < w.Length; i++)
                yield return new QuadratureNode(new Point { X = p1[i], Y = p2[i] }, w[i] / 2.0);
        }

        private static IEnumerable<QuadratureNode> TriangleOrder18()
        {
            const int n = 66;
            double[] p1 = {0.0116731059668, 0.9810030858388, 0.0106966317092, 0.9382476983551, 0.0126627518417,
                0.0598109409984, 0.0137363297927, 0.9229527959405, 0.0633107354993, 0.0117265100335,
                0.1554720587323, 0.8343293888982, 0.8501638031957, 0.0128816350522, 0.1510801608959,
                0.0101917879217, 0.2813372399303, 0.7124374628501, 0.2763025250863, 0.0109658368561,
                0.4289110517884, 0.4215420555115, 0.5711258590444, 0.5826868270511, 0.0130567806713,
                0.0130760400964, 0.7263437062407, 0.0687230068637, 0.8652302101529, 0.0648599071037,
                0.1483494943362, 0.0624359898396, 0.7871369011735, 0.0519104921610, 0.1543129927444,
                0.2617842745603, 0.7667257872813, 0.2582103676627, 0.0679065925147, 0.5293578274804,
                0.0666036150484, 0.0585675461899, 0.0644535360411, 0.6748138429151, 0.3914602310369,
                0.6487701492307, 0.3946498220408, 0.5390137151933, 0.1627895082785, 0.6812436322641,
                0.1542832878020, 0.2522727750445, 0.2547981532407, 0.1485580549194, 0.2930239606436,
                0.2808991272310, 0.4820989592971, 0.5641878245444, 0.1307699644344, 0.1479692221948,
                0.5638684222946, 0.4361157428790, 0.3603263935285, 0.4224188334674, 0.3719001833052,
                0.2413645006928};
            double[] p2 = {0.9812565951289, 0.0071462504863, 0.0115153933376, 0.0495570591341, 0.9370123620615,
                0.0121364578922, 0.0612783625597, 0.0141128270602, 0.9220197291727, 0.1500520475229,
                0.8325147121589, 0.0125228158759, 0.1371997508736, 0.8477627063479, 0.0136526924039,
                0.5770438618345, 0.7066853759623, 0.0124569780990, 0.0121741311386, 0.4194306712466,
                0.5599616067469, 0.0116475994785, 0.0118218313989, 0.4057889581177, 0.2725023750868,
                0.7224712523233, 0.2602984019251, 0.0631417277210, 0.0720611837338, 0.8590433543910,
                0.7888788352240, 0.1493935499354, 0.0656382042757, 0.5255635695605, 0.0716383926917,
                0.0621479485288, 0.1658211554831, 0.6800119766139, 0.7571515437782, 0.4121503841107,
                0.2612513087886, 0.3902236114535, 0.6373626559761, 0.0637583342061, 0.5503238090563,
                0.2836728360263, 0.0605175522554, 0.0611990176936, 0.6861322141035, 0.1567968345899,
                0.1667512624020, 0.2504803933395, 0.4994090649043, 0.5756023096087, 0.5656897354162,
                0.1437921574248, 0.2518557535865, 0.1462966743153, 0.4489577586117, 0.3001174386829,
                0.2813772089298, 0.4252053446420, 0.2599190004889, 0.1453238443303, 0.3780122703567,
                0.3847563284940};
            double[] w = {0.0025165756986, 0.0025273452007, 0.0033269295333, 0.0081503492125, 0.0086135525742,
                0.0087786746179, 0.0097099585562, 0.0102466211915, 0.0108397688341, 0.0129385390176,
                0.0136339823583, 0.0138477328147, 0.0139421540105, 0.0144121399968, 0.0153703455534,
                0.0162489802253, 0.0169718304280, 0.0170088532421, 0.0170953520675, 0.0173888854559,
                0.0174543962439, 0.0178406757287, 0.0178446863879, 0.0179046337552, 0.0181259756201,
                0.0184784838882, 0.0185793564371, 0.0203217151777, 0.0213771661809, 0.0231916854098,
                0.0274426710859, 0.0290301922340, 0.0294522738505, 0.0299436251629, 0.0307026948119,
                0.0325263365863, 0.0327884208506, 0.0331234675192, 0.0346167526875, 0.0347081373976,
                0.0347372049404, 0.0348528762454, 0.0348601561186, 0.0355471569975, 0.0360182996383,
                0.0362926285843, 0.0381897702083, 0.0392252800118, 0.0482710125888, 0.0489912121566,
                0.0497220833872, 0.0507065736986, 0.0509771994043, 0.0521360063667, 0.0523460874925,
                0.0524440683552, 0.0527459644823, 0.0529449063728, 0.0542395594501, 0.0543470203419,
                0.0547100548639, 0.0557288345913, 0.0577734264233, 0.0585393781623, 0.0609039250680,
                0.0637273964449};
            for (int i = 0; i < n; i++)
                yield return new QuadratureNode(new Point { X = p1[i], Y = p2[i] }, 0.25 * w[i]);
        }

        public static double TriangleGauss4(Func<double, double, double> f)
        {
            double result = 0;

            foreach (var node in TriangleOrder4())
            {
                double ksi = node.Node.X;
                double etta = node.Node.Y;

                result += node.Weight * f(ksi, etta);
            }

            return result;
        }

        public static double TriangleGauss18(Func<double, double, double> f)
        {
            double result = 0;

            foreach (var node in TriangleOrder18())
            {
                double ksi = node.Node.X;
                double etta = node.Node.Y;

                result += node.Weight * f(ksi, etta);
            }

            return result;
        }

        public static double NewtonCotes(double a, double b, Func<double, double> f)
        {
            double h = (b - a) / 1000;
            double result = f(a);

            for (double x = h; x < b; x += h)
                result += 2 * f(x);

            result += f(b);
            result *= 7;

            for (double x = a; x < b; x += h)
                result += 32 * f(x + h / 4) + 12 * f(x + h / 2) + 32 * f(x + 3 * h / 4);

            result = result * 0.5 * h / 45;

            return result;
        }

    }
}
