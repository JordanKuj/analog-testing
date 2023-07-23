using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AnalogTesting
{
    public class FileAccess
    {


        public static FileInfo ParseInfo(string path)
        {
            var fileName = Path.GetFileName(path);

            var idx1 = fileName.IndexOf("_");
            var idx2 = fileName.IndexOf(".");
            var sampleRate = int.Parse(fileName.Substring(idx1 + 1, idx2 - idx1 - 1));
            var samples = 0;
            using (var fs = File.OpenRead(path))
            {
                samples = (int)(fs.Length / sizeof(double));
            }
            var duration = TimeSpan.FromSeconds(samples / sampleRate);

            return new FileInfo()
            {
                Name = Path.GetFileNameWithoutExtension(fileName),
                Path = path,
                SampleRate = sampleRate,
                Duration = duration
            };
        }

        public IEnumerable<FileInfo> GetFiles()
        {
            var files = Directory.EnumerateFiles(Environment.CurrentDirectory).Where(x => x.EndsWith(".raw"));
            return files.Select(x => ParseInfo(x));
        }

        public IEnumerable<double> ReadFile(string path)
        {
            var info = ParseInfo(path);
            var blockSize = info.SampleRate * sizeof(double);

            using (var fs = File.OpenRead(path))
            {
                var data = new byte[blockSize];
                var values = new double[info.SampleRate];
                for (var i = 0; i < fs.Length / blockSize; i++)
                {
                    fs.Read(data, 0, blockSize);
                    Buffer.BlockCopy(data, 0, values, 0, blockSize);

                    foreach (var v in values)
                    {
                        yield return v;
                    }
                }
            }
        }



        public class FileInfo
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public int SampleRate { get; set; }
            public TimeSpan Duration { get; set; }
        }
    }
}
