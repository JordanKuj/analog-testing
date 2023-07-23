using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FftSharp;
using FftSharp.Windows;

namespace AnalogTesting.signal
{
    internal class FFTSProcessSignal : IProcessSignal
    {

        private double[][] RawAnalog;// SplitIntoWindows(double[] rawAnalog, )


        public IEnumerable<FftResult> ProcessDataSetsAsync(int samplingRate, SignalAnalsys options)
        {
            //var signal = rawAnalog;

            for (var analogIndex = 0; analogIndex < RawAnalog.Length; analogIndex++)
            {
                var slice = RawAnalog[analogIndex];//new ArraySegment<double>(rawAnalog, analogIndex * options.SampleSize, options.SampleSize);//  rawAnalog.AsSpan(analogIndex * options.SampleSize, options.SampleSize).ToArray();
                Debug.Assert(slice.Length == options.SampleSize);

                IWindow window = null;

                switch (options.Function)
                {
                    case SignalAnalsys.WindowFunction.BlackMan:
                        window = new Blackman();
                        break;
                    case SignalAnalsys.WindowFunction.Cosine:
                        window = new Cosine();
                        break;
                    case SignalAnalsys.WindowFunction.FlatTop:
                        window = new FlatTop();
                        break;
                    case SignalAnalsys.WindowFunction.Hamming:
                        window = new Hamming();
                        break;
                    case SignalAnalsys.WindowFunction.Kaiser:
                        window = new Kaiser();
                        break;
                    case SignalAnalsys.WindowFunction.Rectangular:
                        window = new Rectangular();
                        break;
                    case SignalAnalsys.WindowFunction.Tukey:
                        if (options.Alpha.HasValue)
                            window = new Tukey(options.Alpha.Value);
                        else
                            window = new Tukey();
                        break;
                    case SignalAnalsys.WindowFunction.Welch:
                        window = new Welch();
                        break;
                    default: throw new NotImplementedException();
                }
                //window.crea
                var windowData = window.Apply(slice, true);
                var spectrum = FftSharp.FFT.Forward(windowData);
                yield return new FftResult()
                {
                    Values = spectrum
                    .Select((x, i) =>
                    {
                        var freq = IndexToFrequency(i, samplingRate, options.SampleSize);
                        var db = MathTools.VoltsToDb(x.Magnitude, .0068, 94);
                        var offset = DbWeightingTools.Weighting(options.DbWeightMode, freq);
                        var fixedDb = Math.Max(0, db - offset.Offset);
                        return new FftResult.FrequencyValue(freq, fixedDb);
                        //return new FftResult.FrequencyValue(freq, x.Magnitude);
                    })
                    .ToArray()
                };


            }



            //var window = new Hanning();//.Create(options.SampleSize);
            //window.no
            //window.ApplyInPlace(signal);


            //foreach (var signal in spectrum)
            //{

            //    var cplx = signal.ToArray();
            //    yield return new FftResult()
            //    {
            //        Values = cplx
            //        .Flatten()
            //        .Select((x, i) =>
            //        {
            //            var freq = IndexToFrequency(i, samplingRate, options.SampleSize);
            //            var db = MathTools.VoltsToDb(x.Magnitude, .0068, 94);
            //            var offset = DbWeightingTools.Weighting(options.DbWeightMode, freq);
            //            var fixedDb = Math.Max(0, db - offset.Offset);
            //            return new FftResult.FrequencyValue(freq, fixedDb);
            //        })
            //        .ToArray()
            //    };
            //}

        }
        public static double IndexToFrequency(int index, double samplingRate, int samples)
        {
            if (index == 0)
            {
                return 0;
            }
            return index * samplingRate / (samples * 1.0);
        }

        public void SetData(double[] rawAnalog, int sampleSize)
        {
            var sampleSegments = rawAnalog.Length / sampleSize;
            RawAnalog = new double[sampleSegments][];

            for (var analogIndex = 0; analogIndex < sampleSegments; analogIndex++)
            {
                RawAnalog[analogIndex] = new ArraySegment<double>(rawAnalog, analogIndex * sampleSize, sampleSize).ToArray();//  rawAnalog.AsSpan(analogIndex * options.SampleSize, options.SampleSize).ToArray();
            }
        }
    }
}
