using Accord.Audio;
using Accord.Audio.Windows;
using Accord.Math;
using OpenLayers.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AnalogTesting
{

    public interface IProcessSignal
    {
        void SetData(double[] rawAnalog, int sampleSize);

        IEnumerable<FftResult> ProcessDataSetsAsync(int samplingRate,
        SignalAnalsys options);
    }

    public class ProcessSignal// : IProcessSignal
    {


        //IEnumerable<FftResult> ProcessDataSetsAsync(double[] rawAnalog,
        //    int samplingRate,
        //    SignalAnalsys options)
        //{

        //    //for (var idx = 0; idx < (rawAnalog.Count() / sampleSize) + 1; idx++)
        //    //{
        //    //var startIdx = idx * sampleSize;
        //    //var remainingValues = rawAnalog.Count() - startIdx;
        //    //double[] data = null;
        //    //if (remainingValues == 0)
        //    //{
        //    //    break;
        //    //}
        //    //if (remainingValues < sampleSize)
        //    //{
        //    //    data = rawAnalog
        //    //       .Skip(startIdx)
        //    //       .Take(remainingValues)
        //    //       .Concat(Enumerable.Repeat(0d, sampleSize - remainingValues))
        //    //       .ToArray();
        //    //       //.Select(x => new Complex(x, 0))
        //    //}
        //    //else
        //    //{
        //    //    data = rawAnalog
        //    //        .Skip(startIdx)
        //    //        .Take(sampleSize)
        //    //        .ToArray();
        //    //        //.Select(x => new Complex(x, 0))
        //    //}
        //    var sourceSignal = Signal.FromArray(rawAnalog.ToArray(), samplingRate, SampleFormat.Format64BitIeeeFloat);

        //    IWindow window = null;
        //    if (options.Function == SignalAnalsys.WindowFunction.BlackMan)
        //    {
        //        if(options.Alpha.HasValue)
        //            window = new BlackmanWindow(options.Alpha.Value, options.SampleSize);
        //        else
        //            window = new BlackmanWindow(options.SampleSize);
        //    }
        //    //if

        //    //else
        //    //{
        //    //    if (options.Alpha.HasValue)
        //    //        window = new RaisedCosineWindow(options.Alpha.Value, options.SampleSize);
        //    //    else
        //    //        window = RaisedCosineWindow.Hann( new RaisedCosineWindow(options.SampleSize);
        //    //}
        //    //window = RaisedCosineWindow.Hann(sampleSize);

        //    var windows = sourceSignal.Split(window, options.SampleSize / 2);
        //    var complex = windows.Apply(ComplexSignal.FromSignal);
        //    complex.ForwardFourierTransform();

        //    foreach (var signal in complex)
        //    {

        //        var cplx = signal.ToArray();
        //        yield return new FftResult()
        //        {
        //            Values = cplx
        //            .Flatten()
        //            .Select((x, i) =>
        //            {
        //                var freq = IndexToFrequency(i, samplingRate, options.SampleSize);
        //                var db = MathTools.VoltsToDb(x.Magnitude, .0068, 94);
        //                var offset = DbWeightingTools.Weighting(options.DbWeightMode, freq);
        //                var fixedDb = Math.Max(0, db - offset.Offset);
        //                return new FftResult.FrequencyValue(freq, fixedDb);
        //            })
        //            .ToArray()
        //        };
        //    }


        //    //complex.Split(sampleSize/2). //.TakeWhile((val,idx)=> )

        //    //var windows = window.Apply();


        //    //Accord.Math.FourierTransform.FFT(data, Accord.Math.FourierTransform.Direction.Forward);

        //    //var result = data.Take(data.Length / 2)
        //    //    .Select((v, i) => new FftResult.FrequencyValue(IndexToFrequency(i, samplingRate, sampleSize), MathTools.VoltsToDb(v.Magnitude, .00002, 94))).ToArray();

        //    //}
        //}

        public static double IndexToFrequency(int index, double samplingRate, int samples)
        {
            if (index == 0)
            {
                return 0;
            }
            return index * samplingRate / (samples * 1.0);
        }

    }

    public class SignalAnalsys
    {
        public enum WindowFunction
        {
            BlackMan,
            Cosine,
            FlatTop,
            Hamming,
            Kaiser,
            Rectangular,
            Tukey,
            Welch
        }

        public WindowFunction Function { get; set; }
        public double? Alpha { get; set; }
        public WeightMode DbWeightMode { get; set; }
        public int SampleSize { get; set; }
    }


    public class FftResult
    {
        public FrequencyValue[] Values { get; set; }

        public class FrequencyValue
        {
            public double Frequency { get; set; }
            public double Magnitude { get; set; }
            public FrequencyValue(double frequency, double magnitude)
            {
                Frequency = frequency;
                Magnitude = magnitude;
            }
        }

    }
}