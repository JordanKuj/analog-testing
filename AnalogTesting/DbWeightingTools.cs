using System;

namespace AnalogTesting
{
    //https://en.wikipedia.org/wiki/A-weighting

    public static class DbWeightingTools
    {
        private static double DbToAmplitude(double frequency, FreqFunc weighting)
        {
            return 20 * Math.Log10(weighting(frequency)) - 20 * Math.Log10(weighting(1000));
        }

        private delegate double FreqFunc(double frequency);

        public static DbOffset AWeighting(double frequency)
        {
            FreqFunc f1 = (f) => Math.Pow(12194.0, 2) * Math.Pow(f, 4);
            FreqFunc f2 = (f) => Math.Pow(f, 2) + Math.Pow(20.6, 2);
            FreqFunc f3 = (f) => Math.Sqrt((Math.Pow(f, 2) + Math.Pow(107.7, 2)) * (Math.Pow(f, 2) + Math.Pow(737.9, 2)));
            FreqFunc f4 = (f) => Math.Pow(f, 2) + Math.Pow(12194.0, 2);

            var offset = DbToAmplitude(frequency, (f) => f1(f) / (f2(f) * f3(f) * f4(f)));

            return new DbOffset() { Frequency = frequency, Offset = offset };
        }

        public static DbOffset BWeighting(double frequency)
        {
            FreqFunc f1 = (f) => Math.Pow(12194.0, 2) * Math.Pow(f, 3);
            FreqFunc f2 = (f) => Math.Pow(f, 2) + Math.Pow(20.6, 2);
            FreqFunc f3 = (f) => Math.Sqrt(Math.Pow(f, 2) + Math.Pow(158.5, 2));
            FreqFunc f4 = (f) => Math.Pow(f, 2) + Math.Pow(12194.0, 2);

            var offset = DbToAmplitude(frequency, (f) => f1(f) / (f2(f) * f3(f) * f4(f)));

            return new DbOffset() { Frequency = frequency, Offset = offset };
        }

        public static DbOffset CWeighting(double frequency)
        {
            FreqFunc f1 = (f) => Math.Pow(12194.0, 2) * Math.Pow(f, 2);
            FreqFunc f2 = (f) => Math.Pow(f, 2) + Math.Pow(20.6, 2);
            //FreqFunc f3 = (f) => Math.Sqrt(Math.Pow(f, 2) + Math.Pow(158.5, 2));
            FreqFunc f4 = (f) => Math.Pow(f, 2) + Math.Pow(12194.0, 2);

            var offset = DbToAmplitude(frequency, (f) => f1(f) / (f2(f) * f4(f)));

            return new DbOffset() { Frequency = frequency, Offset = offset };
        }
        public static DbOffset ZWeighting(double frequency)
        {
            return new DbOffset() { Frequency = frequency, Offset = 0 };
        }


        public static DbOffset Weighting(WeightMode mode, double frequence)
        {
            switch (mode)
            {
                case (WeightMode.A): return AWeighting(frequence);
                case (WeightMode.B): return BWeighting(frequence);
                case (WeightMode.C): return CWeighting(frequence);
                default: return ZWeighting(frequence);
            }
        }

    }

    public enum WeightMode
    {
        A = 0, 
        B = 1, 
        C = 2, 
        Z = 3
    }
    public class DbOffset
    {
        public double Frequency;
        public double Offset;
    }
}
