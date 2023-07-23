//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Numerics;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Tm2b.Tools
//{
//    public static class SignalProcessing
//    {
//        private static Task<IEnumerable<Complex>> ProcessDataSetsAsync(IEnumerable<double> rawAnalog, int sampleSize, System.Threading.CancellationToken token)
//        {
//            if (!ValidPow2(sampleSize))
//            {
//                throw new ArgumentException("Must be a power of 2", nameof(sampleSize));
//            }
//            if (!rawAnalog.Any())
//            {
//                throw new ArgumentNullException(nameof(rawAnalog));
//            }

//            return Task.Run(() =>
//            {
//                var dataSets = new List<Complex[]>();
//                //TODO: remove stop watches
//                var sw = Stopwatch.StartNew();

//                for (var i = 0; i < (rawAnalog.Count() / sampleSize) + 1; i++)
//                {
//                    var startIdx = i * sampleSize;
//                    var remainingValues = rawAnalog.Count() - startIdx;
//                    Complex[] dataSet = null;
//                    if (remainingValues == 0)
//                    {
//                        break;
//                    }
//                    if (remainingValues < sampleSize)
//                    {
//                        var items = rawAnalog
//                            .Skip(startIdx)
//                            .Take(remainingValues)
//                            .Select(x => new Complex(x, 0))
//                            .Concat(Enumerable.Repeat(new Complex(), sampleSize - remainingValues));
//                        dataSet = items.ToArray();
//                    }
//                    else
//                    {
//                        dataSet = rawAnalog.Skip(startIdx).Take(sampleSize).Select(x => new Complex(x, 0)).ToArray();
//                    }
//                    Accord.Math.FourierTransform.FFT(dataSet, Accord.Math.FourierTransform.Direction.Forward);
//                    token.ThrowIfCancellationRequested();
//                    dataSets.Add(dataSet);
//                }

//                sw.Stop();
//                System.Diagnostics.Debug.WriteLine($"FFT A: {sw.ElapsedMilliseconds}ms");
//                IEnumerable<Complex> Combine(IEnumerable<Complex> c1, IEnumerable<Complex> c2)
//                {
//                    return c1.Zip(c2, (a, b) =>
//                    {
//                        if (a.Magnitude >= b.Magnitude)
//                        {
//                            return a;
//                        }
//                        return b;
//                    });
//                }

//                IEnumerable<Complex> Dft = null;
//                sw.Restart();

//                if (dataSets.Count() == 1)
//                {
//                    Dft = dataSets.First();
//                }
//                else
//                {
//                    Dft = Combine(dataSets[0], dataSets[1]);
//                    for (var i = 2; i < dataSets.Count(); i++)
//                    {
//                        Dft = Combine(Dft, dataSets[i]);
//                        token.ThrowIfCancellationRequested();
//                    }
//                }
//                sw.Stop();
//                System.Diagnostics.Debug.WriteLine($"FFT B: {sw.ElapsedMilliseconds}ms");

//                return Dft;
//            }, token);
//        }

//        //public static async Task<FftResult[]> GetFftResultsAsync(RecordingSegment recordingSegment, ElementRange largestRange, Configuration.DbCalibration dbCalibration, System.Threading.CancellationToken token, int sampleSize)
//        //{
//        //    var range = recordingSegment.Data.Skip(largestRange.Index).Take(largestRange.Count);
//        //    return await GetFftResultsAsync(range.Select(x => x.Microphone).ToArray(), recordingSegment.SamplingRate, dbCalibration, token, sampleSize);
//        //}

//        public static async Task<FftResult[]> GetFftResultsAsync(double[] recordingSegment, int samplingRate, Configuration.DbCalibration dbCalibration, System.Threading.CancellationToken token, int sampleSize)
//        {
//            var fft = await Tools.SignalProcessing.ProcessDataSetsAsync(recordingSegment, sampleSize, token);
//            var results = fft.Take(fft.Count() / 2)
//                .Select((v, i) => new FftResult(Tools.SignalProcessing.IndexToFrequency(i, samplingRate, sampleSize), MathTools.VoltsToDb(v.Magnitude, dbCalibration)))
//                .Skip(1)
//                .ToArray();
//            foreach (var result in results)
//            {
//                var offset = DbWeightingTools.Weighting(dbCalibration.DbWeightingMode, result.Frequency);
//                result.Magnitude = Math.Max(0, result.Magnitude - offset.Offset);
//            }

//            return results;
//        }

//        public static async Task<AudioResults> ProcessAudioResults(IEnumerable<double> recordingSegment,
//            int samplingRate,
//            ElementRange range,
//            IEnumerable<FftSetpoint> setpoints,
//            Configuration.DbCalibration dbCalibration,
//            CancellationToken token,
//            int samples,
//            bool runFFT = true)
//        {
//            var minFreq = setpoints.Min(x => x.Frequency);
//            var maxFreq = setpoints.Max(x => x.Frequency);

//            FftResult[] results = Array.Empty<FftResult>();
//            //TODO: remove stopwatch and debug writelines
//            var sw = Stopwatch.StartNew();
//            if (runFFT)
//            {
//                //  results = await Task.Run(() =>
//                //{
//                var recordingData = recordingSegment.Skip(range.Index).Take(range.Count).ToArray();
//                sw.Stop();
//                System.Diagnostics.Debug.WriteLine($"ProcessAudioResults A: {sw.ElapsedMilliseconds}ms {recordingData.Length}");
//                sw.Restart();
//                results = await GetFftResultsAsync(recordingData, samplingRate, dbCalibration, token, samples);
//                sw.Stop();
//                System.Diagnostics.Debug.WriteLine($"ProcessAudioResults B: {sw.ElapsedMilliseconds}ms {recordingData.Length}");
//                sw.Reset();
//                //});
//            }
//            var largestReading = results.Where(x => x.Frequency >= minFreq && x.Frequency <= maxFreq).OrderBy(x => x.Frequency).LastOrDefault();
//            sw.Restart();

//            var amplitude = recordingSegment.GetAmplitude(range);
//            sw.Stop();
//            System.Diagnostics.Debug.WriteLine($"ProcessAudioResults C: {sw.ElapsedMilliseconds}ms");
//            var db = CalculateDb(results);
//            return new AudioResults()
//            {
//                FftResult = results,
//                PeakFrequency = largestReading?.Frequency ?? 0,
//                PeakFrequencyAmplitude = largestReading?.Magnitude ?? 0,
//                Db = db,
//                Sones = MathTools.DbToSones(db),
//                FrequenciesOverSetpoints = results.GetFrequenciesOverSetpoints(setpoints).ToArray(),
//                Range = range
//            };
//        }

//        private static bool ValidPow2(int value)
//        {
//            for (var i = 2; i < 15; i++)
//            {
//                var v = (2 << i);
//                if (value == v)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        private static double IndexToFrequency(int index, double samplingRate, int samples)
//        {
//            if (index == 0)
//            {
//                return 0;
//            }
//            return index * samplingRate / (samples * 1.0);
//        }

//        public static void DeleteOldAudioFiles(string directory, TimeSpan maxAge)
//        {
//            var files = Directory.GetFiles(directory);
//            var now = DateTime.Now;
//            foreach (var f in files)
//            {
//                var created = File.GetCreationTime(f);
//                if (Math.Abs((now - created).TotalHours) > Math.Abs(maxAge.TotalHours))
//                {
//                    try
//                    {
//                        File.Delete(f);
//                    }
//                    catch (Exception ex)
//                    {
//                        //Should log
//                    }
//                }
//            }
//        }

//        public static void SaveWaveFile(RecordingSegment recordingSegment, string wavFileName, string rawFileName)
//        {
//            if (recordingSegment == null)
//            {
//                return;
//            }

//            byte[] Convert(AnalogSample value)
//            {
//                var v = value.Microphone * Int16.MaxValue;
//                Int16 result;
//                if (v > Int16.MaxValue)
//                {
//                    result = Int16.MaxValue;
//                }
//                else if (v < Int16.MinValue)
//                {
//                    result = Int16.MinValue;
//                }
//                else
//                {
//                    result = (Int16)v;
//                }
//                return BitConverter.GetBytes(result);
//            }
//            var data = recordingSegment.Data.ToArray();

//            using (var ms = new System.IO.MemoryStream(data.Select(Convert).SelectMany(x => x).ToArray()))
//            using (var rs = new RawSourceWaveStream(ms, new WaveFormat(recordingSegment.SamplingRate, 16, 1)))
//            {
//                WaveFileWriter.CreateWaveFile(wavFileName, rs);
//            }

//            if (!string.IsNullOrEmpty(rawFileName))
//            {
//                using (var fs = File.Create(rawFileName))
//                using (var utf = new System.Text.Json.Utf8JsonWriter(fs))
//                {
//                    System.Text.Json.JsonSerializer.Serialize(writer: utf, value: data);
//                    fs.Flush();
//                }
//            }
//        }

//        public static async Task<AudioAnalysis> GetAudioDataAsync(CycleResults result,
//            IEnumerable<FftSetpoint> fftSetpoints,
//             Configuration.DbCalibration dbCfg,
//             int fftSamples)
//        {
//            //var cfg = Options.CurrentValue;
//            //var dbCfg = cfg.DecibelsCalibration;
//            var fileName = result.RawFileName;
//            if (string.IsNullOrEmpty(fileName) || !System.IO.File.Exists(fileName))
//            {
//                throw new Exception($"Cannot find audio file for test <audio file name:{fileName}>.");
//            }

//            var aUpStart = result.A_Up_Audio_Start;
//            var aUpDuration = result.A_Up_Audio_Duration;

//            var aDownStart = result.A_Down_Audio_Start;
//            var aDownDuration = result.A_Down_Audio_Duration;

//            var bUpStart = result.B_Up_Audio_Start;
//            var bUpDuration = result.B_Up_Audio_Duration;

//            var bDownStart = result.B_Down_Audio_Start;
//            var bDownDuration = result.B_Down_Audio_Duration;

//            ElementRange CalculatePositions(int? start, int? duration)
//            {
//                if (start == null || duration == null)
//                {
//                    return null;
//                }

//                return new ElementRange(start.Value, duration.Value);
//            }

//            var fs = File.ReadAllText(fileName);
//            var samples = ((AnalogSample[])System.Text.Json.JsonSerializer.Deserialize(fs, typeof(AnalogSample[]))).Select(x => x.Microphone).ToArray();
//            var rate = result.SampleRate.Value;

//            var aUpPos = CalculatePositions(aUpStart, aUpDuration);
//            var bUpPos = CalculatePositions(bUpStart, bUpDuration);
//            var aDownPos = CalculatePositions(aDownStart, aDownDuration);
//            var bDownPos = CalculatePositions(bDownStart, bDownDuration);

//            var dataSamples = new float[samples.Length / 4];


//            var audioResult = new AudioAnalysis();

//            //var fftSamples = cfg.SoundAnalysis.FftSamples;

//            Task<AudioResults> RunProcessing(ElementRange range)
//            {
//                if (range == null)
//                {
//                    return Task.FromResult<AudioResults>(null);
//                }
//                return SignalProcessing.ProcessAudioResults(samples, rate, range, fftSetpoints, dbCfg, System.Threading.CancellationToken.None, fftSamples);
//            }

//            var aupT = RunProcessing(aUpPos);
//            var adownT = RunProcessing(aDownPos);
//            var bupT = RunProcessing(bUpPos);
//            var bdownT = RunProcessing(bDownPos);
//            audioResult.AUp = await aupT;
//            audioResult.ADown = await adownT;
//            audioResult.BUp = await bupT;
//            audioResult.BDown = await bdownT;
//            return audioResult;
//        }

//        public static IEnumerable<FftResult> GetFrequenciesOverSetpoints(this FftResult[] recording, IEnumerable<FftSetpoint> setpoints)
//        {
//            if (!recording.Any())
//            {
//                return Array.Empty<FftResult>();
//            }

//            var badSamples = new List<FftResult>();
//            foreach (var sample in recording)
//            {
//                var frequency = sample.Frequency;

//                if (setpoints.All(x => x.Frequency > frequency) || setpoints.All(x => x.Frequency < frequency))
//                {
//                    continue;
//                }
//                if (setpoints.Any(x => x.Frequency == frequency))
//                {
//                    var setpoint = setpoints.Single(x => x.Frequency == frequency).Max;
//                    if (setpoint < sample.Magnitude)
//                    {
//                        badSamples.Add(sample);
//                    }
//                    continue;
//                }
//                var leftSetpoint = setpoints.Where(x => x.Frequency < frequency).OrderBy(x => x.Frequency).Last();
//                var rightSetpoint = setpoints.Where(x => x.Frequency > frequency).OrderBy(x => x.Frequency).First();

//                var slope = (rightSetpoint.Max - leftSetpoint.Max) / (rightSetpoint.Frequency - leftSetpoint.Frequency);
//                var b = leftSetpoint.Max - (slope * leftSetpoint.Frequency);

//                var maxThreshold = (slope * frequency) + b;

//                if (maxThreshold < sample.Magnitude)
//                {
//                    badSamples.Add(sample);
//                }
//            }
//            return badSamples;
//        }

//        public static double CalculateDb(FftResult[] results)
//        {
//            var octaves = new[] { 31.25, 62.5, 125, 250, 500, 1000, 2000, 4000, 8000 };

//            var db = 0d;
//            for (var i = 0; i < results.Length - 1; i++)
//            {
//                var result = results[i];
//                var nextResult = results[i + 1];

//                for (var o = 0; o < octaves.Length; o++)
//                {
//                    var octave = octaves[o];
//                    if (result.Frequency <= octave && nextResult.Frequency >= octave)
//                    {
//                        //find interpolated value
//                        var slope = (nextResult.Magnitude - result.Magnitude) / (nextResult.Frequency - result.Frequency);
//                        var b = result.Magnitude - (slope * result.Frequency);

//                        var value = (slope * octave) + b;
//                        db += Math.Pow(10, (value / 10));

//                        break;
//                    }
//                }
//            }
//            return Math.Log10(db) * 10;
//        }

//        //public static double MaxFrequency(double samplingRate, int samples)
//        //{
//        //    if (!ValidPow2(samples))
//        //    {
//        //        throw new ArgumentException("Parameter must be a power of 2", nameof(samples));
//        //    }

//        //    var bins = samples / 2.0;
//        //    var frequencyPerBin = (samplingRate / 2.0) / bins;

//        //    return frequencyPerBin * samplingRate;
//        //}
//    }
//}