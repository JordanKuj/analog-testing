using System;
using System.Collections.Generic;
using System.Linq;

namespace AnalogTesting
{
    public static class MathTools
    {
        public static double Scale(double value,
            double minRaw,
            double maxRaw,
            double minScaled,
            double maxScaled)
        {
            try
            {
                return (((maxScaled - minScaled) * (value - minRaw)) / (maxRaw - minRaw)) + minScaled;
            }
            catch (DivideByZeroException)
            {
                return double.NaN;
            }
        }

        public static double DbToSones(double db)
        {
            //Conversion table borrowed from https://www.industrialfansdirect.com/pages/dba-sones-decibel-levels
            //Did a logarithmic regression on https://keisan.casio.com/exec/system/14059930226691
            return 27.992337 + (14.4187156 * Math.Log(db));
        }

        public static double VoltsToDb(double value, double voltsAtCalibratedDb, double calibratedDb)
        {
            return (20 * Math.Log10(Math.Abs(value) / voltsAtCalibratedDb)) + calibratedDb;
        }

        //public static double VoltsToDb(double value, Models.Dto.Configuration.DbCalibration dbCalibration)
        //{
        //    return VoltsToDb(value, dbCalibration.ReferenceValue, dbCalibration.Decibels);
        //}

        //private static RmsReading GetRms(this IEnumerable<double> samples, ElementRange range = null)
        //{
        //    double rms = 0f;
        //    double peak = 0f;
        //    var data = samples.Skip(range?.Index ?? 0).Take(range?.Count ?? samples.Count());

        //    foreach (var sample in data)
        //    {
        //        var abs = Math.Abs(sample);
        //        if (abs > peak)
        //        {
        //            peak = abs;
        //        }

        //        rms += Math.Pow(sample, 2);
        //    }

        //    //var rms = data.Aggregate((total, value) => total + Math.Pow(value, 2));
        //    //var peak = data.Select(x=>Math.Abs(x)).Max();

        //    rms = (float)Math.Sqrt(rms / data.Count());
        //    return new RmsReading(rms, peak);
        //}

        //public static RmsReading GetAmplitude(this IEnumerable<AnalogSample> samples)
        //{
        //    return samples.Select(x => x.Microphone).GetRms();
        //}

        //public static RmsReading GetAmplitude(this IEnumerable<double> samples, ElementRange range = null)
        //{
        //    //var data = samples.Skip(range?.Index ?? 0).Take(range?.Count ?? samples.Count());
        //    return samples.GetRms(range);
        //}
    }

    public struct RmsReading
    {
        public double Rms { get; set; }
        public double Peak { get; set; }

        public RmsReading(double rms, double peak)
        {
            Rms = rms;
            Peak = peak;
        }
    }
}