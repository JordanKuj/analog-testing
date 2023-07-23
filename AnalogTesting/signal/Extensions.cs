using Accord.Audio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AnalogTesting.signal
{
    public static class Extensions
    {

        public static void ToWav(this double[] data, int sampleRate)
        {

            var audioSignal = Signal.FromArray(data.Select(x => (float)x).ToArray(), sampleRate, SampleFormat.Format64BitIeeeFloat);


        }

        public static void SaveWaveFile(this double[] recordingSegment, int sampleRate, string wavFileName)
        {
            if (recordingSegment == null)
            {
                return;
            }

            byte[] Convert(double value)
            {
                var v = value * Int16.MaxValue;
                Int16 result;
                if (v > Int16.MaxValue)
                {
                    result = Int16.MaxValue;
                }
                else if (v < Int16.MinValue)
                {
                    result = Int16.MinValue;
                }
                else
                {
                    result = (Int16)v;
                }
                return BitConverter.GetBytes(result);
            }
            var data = recordingSegment;

            using (var ms = new System.IO.MemoryStream(data.Select(Convert).SelectMany(x => x).ToArray()))
            using (var rs = new RawSourceWaveStream(ms, new WaveFormat(sampleRate, 16, 1)))
            {
                WaveFileWriter.CreateWaveFile( $"{wavFileName}.wav", rs);
            }

            //if (!string.IsNullOrEmpty(rawFileName))
            //{
            //    using (var fs = File.Create(rawFileName))
            //    using (var utf = new System.Text.Json.Utf8JsonWriter(fs))
            //    {
            //        System.Text.Json.JsonSerializer.Serialize(writer: utf, value: data);
            //        fs.Flush();
            //    }
            //}
        }

    }
}
