using Accord.Audio.Windows;
using Accord.Math;
using AnalogTesting.signal;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AnalogTesting.FileAccess;
using static AnalogTesting.SignalAnalsys;

namespace AnalogTesting
{
    public partial class Form1 : Form
    {
        private FileAccess FileAccess = new FileAccess();
        private AnalogClient analogClient = new AnalogClient();

        private FileAccess.FileInfo[] Files = new FileAccess.FileInfo[] { };
        private readonly int[] SampleSizes = Enumerable.Range(4, 10).Select(x => (int)Math.Pow(2, x)).ToArray();


        public Form1()
        {
            InitializeComponent();

            this.radioButtonA.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            this.radioButtonB.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            this.radioButtonC.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            this.radioButtonZ.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);

            SetOptionComboboxes();

        }

        private string SelectedFile => (string)comboBoxRecordings.SelectedItem;

        private WeightMode SelectedWeightMode
        {
            get
            {
                if (radioButtonA.Checked) return WeightMode.A;
                if (radioButtonB.Checked) return WeightMode.B;
                if (radioButtonC.Checked) return WeightMode.C;
                return WeightMode.Z;
            }
        }

        private SignalAnalsys.WindowFunction SelectedFunction
        {
            get
            {
                return (WindowFunction)Enum.Parse(typeof(WindowFunction), comboBoxWindowMode.SelectedItem.ToString());


                //if (radioButtonBlackman.Checked) return SignalAnalsys.WindowFunction.BlackMan;
                //return SignalAnalsys.WindowFunction.Cosine;
            }
        }

        private double Alpha
        {
            get
            {
                return (double)numericUpDown1.Value;
            }
        }

        public int SampleSize
        {
            get
            {
                return int.Parse(comboBoxSampleSize.SelectedItem.ToString());
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            analogClient.Start(40000, TimeSpan.FromSeconds(30));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReloadFileOptions();

        }

        private void ReloadFileOptions()
        {
            Files = FileAccess.GetFiles().ToArray();
            comboBoxRecordings.Items.Clear();
            comboBoxRecordings.Items.AddRange(Files.Select(x => Path.GetFileName(x.Path)).ToArray());
        }

        private void SetOptionComboboxes()
        {
            comboBoxSampleSize.Items.AddRange(SampleSizes.Select(x => x.ToString()).ToArray());
            comboBoxSampleSize.SelectedItem = "2048";

            object[] items = Enum.GetValues(typeof(WindowFunction)).Cast<WindowFunction>().Select(x => x.ToString()).ToArray();
            comboBoxWindowMode.Items.AddRange(items);
            comboBoxWindowMode.SelectedItem = WindowFunction.BlackMan.ToString();
        }

        private void comboBoxRecordings_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
        }

        private (FileAccess.FileInfo info, double[] fileData) ReadFile()
        {
            var info = FileAccess.ParseInfo(SelectedFile);
            var fileData = FileAccess.ReadFile(SelectedFile).ToArray();
            return (info: info, fileData: fileData);
        }

        private static PlotModel RedrawPlot(FFTSProcessSignal process, (FileAccess.FileInfo info, double[] fileData) file, SignalAnalsys options)
        {
            //var sampleSize = 2048;
            var (info, _) = file;

            var sampleSize = options.SampleSize;

            var results = process.ProcessDataSetsAsync(info.SampleRate, options).ToArray();


            var model = new PlotModel();

            var data = new double[sampleSize / 2, results.Length];

            //ArrayBuilder.Evaluate((x,y)=>)

            for (var timeIndex = 0; timeIndex < results.Length - 1; timeIndex++)
            {
                var item = results[timeIndex];
                for (var fIdx = 0; fIdx < (sampleSize / 2) - 1; fIdx++)
                {
                    var frequency = item.Values[fIdx];
                    data[fIdx, timeIndex] = frequency.Magnitude;

                    //heatMapSeries.Data.add(1);
                }

            }

            var freqAxis = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = sampleSize / 2,
                Key = "freqAxis"
            };

            model.Axes.Add(freqAxis);

            var colorAxis = new LinearColorAxis()
            {
                Palette = OxyPalettes.Inferno(),
                Minimum = data.Min(),
                Maximum = data.Max()
            };

            model.Axes.Add(colorAxis);

            //model.Axes.Add(new LinearAxis()
            //{
            //    Position = AxisPosition.Left
            //});


            freqAxis.Minimum = ProcessSignal.IndexToFrequency(0, info.SampleRate, sampleSize);
            freqAxis.Maximum = ProcessSignal.IndexToFrequency(sampleSize / 2, info.SampleRate, sampleSize);

            var heatMapSeries = new HeatMapSeries
            {
                X0 = ProcessSignal.IndexToFrequency(0, info.SampleRate, sampleSize),
                X1 = ProcessSignal.IndexToFrequency(sampleSize / 2, info.SampleRate, sampleSize),
                Y0 = 0,
                Y1 = data.GetLength(1),
                Interpolate = false,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = data,
                XAxisKey = "freqAxis",

            };

            //heatMapSeries.

            model.Series.Add(heatMapSeries);

            return model;
        }

        private void buttonRunGraph_Click(object sender, EventArgs e)
        {
            var fileData = ReadFile();
            var process = new FFTSProcessSignal();
            var options = new SignalAnalsys()
            {
                Alpha = Alpha,
                DbWeightMode = SelectedWeightMode,
                Function = SelectedFunction,
                SampleSize = SampleSize
            };
            process.SetData(fileData.fileData, SampleSize);
            var model = RedrawPlot(process, fileData, options);
            plotView1.Model = model;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            var (info, fileData) = ReadFile();
            fileData.SaveWaveFile(info.SampleRate, info.Name);
            ReloadFileOptions();
        }

        private async void buttonSaveAllGraphs_Click(object sender, EventArgs e)
        {
            buttonSaveAllGraphs.Enabled = false;

            var analogFile = ReadFile();
            var screenshotDirectory = analogFile.info.Name;
            if (Directory.Exists(screenshotDirectory))
                Directory.Delete(screenshotDirectory, true);
            Directory.CreateDirectory(screenshotDirectory);
            var tasks = new List<Task>();

            var dbWeights = Enum.GetValues(typeof(WeightMode)).Cast<WeightMode>();
            var windowFunctions = Enum.GetValues(typeof(WindowFunction)).Cast<WindowFunction>();

            foreach (var sampleSize in SampleSizes)
            {
                var process = new FFTSProcessSignal();
                process.SetData(analogFile.fileData, sampleSize);

                foreach (var dbWeightMode in dbWeights)
                    foreach (var windowFunction in windowFunctions)
                    {
                        var options = new SignalAnalsys()
                        {
                            DbWeightMode = dbWeightMode,
                            Function = windowFunction,
                            SampleSize = sampleSize
                        };

                        var task = Task.Run(() =>
                        {
                            var model = RedrawPlot(process, analogFile, options);
                            var fileName = $"{dbWeightMode}_{windowFunction}_{sampleSize}";
                            var tempFilePath = Path.Combine(screenshotDirectory, fileName + ".tmp");
                            var filePath = Path.Combine(screenshotDirectory, fileName + ".png");
                            PngExporter.Export(model, tempFilePath, 600, 600);

                            using (var image = Image.FromFile(tempFilePath))
                            {
                                using (var gfx = Graphics.FromImage(image))
                                using (var font = new Font("Cascadia Mono", 20, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Pixel))
                                {
                                    var heightOffset = 30;
                                    var newHeight = image.Height - heightOffset;


                                    gfx.DrawImage(image, new RectangleF(0, 0, image.Width, newHeight), new RectangleF(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                                    gfx.FillRectangle(Brushes.WhiteSmoke, new Rectangle(0, newHeight, image.Width, heightOffset));
                                    gfx.DrawString(fileName, font, Brushes.Black, new PointF(0, image.Height - heightOffset + 5));
                                }
                                image.Save(filePath);
                            }

                        });
                        tasks.Add(task);
                    }
            }


            await Task.WhenAll(tasks);

            foreach (var file in Directory.EnumerateFiles(screenshotDirectory, "*.tmp"))
            {
                File.Delete(file);
            }

            for (var dbIdx = 0; dbIdx < dbWeights.Count(); dbIdx++)
            {

                for (var funcIdx = 0; funcIdx < windowFunctions.Count(); funcIdx++)
                {
                    using (var finalImage = new Bitmap(SampleSizes.Length * 600, 600))
                    using (var gfx = Graphics.FromImage(finalImage))
                    {
                        var weight = dbWeights.ElementAt(dbIdx);
                        var func = windowFunctions.ElementAt(funcIdx);

                        for (var sampleIdx = 0; sampleIdx < SampleSizes.Count(); sampleIdx++)
                        {
                            var sampleSize = SampleSizes.ElementAt(sampleIdx);

                            var x = sampleIdx * 600;
                            var y = 0;// ((dbIdx * dbWeights.Count()) + funcIdx) * 600;
                            using (var image = Bitmap.FromFile(Path.Combine(screenshotDirectory, $"{weight}_{func}_{sampleSize}.png")))
                                gfx.DrawImage(image, x, y);




                        }
                        finalImage.Save(Path.Combine(screenshotDirectory, $"output_{weight}_{func}.png"));
                    }
                }
            }
            foreach (var file in Directory.EnumerateFiles(screenshotDirectory, "*.png"))
            {
                if (file.Contains("output")) continue;
                File.Delete(file);
            }

            buttonSaveAllGraphs.Enabled = true;
        }
    }
}
