//using Accord.Audio.Windows;
//using Accord.Audio;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Numerics;
//using System.Text;
//using System.Threading.Tasks;

//namespace AnalogTesting.signal
//{
//    public class BlackmanWindow : WindowBase
//    {

//        /// <summary>
//        ///   Constructs a new Blackman window.
//        /// </summary>
//        /// 
//        /// <param name="length">The length for the window.</param>
//        /// 
//        public BlackmanWindow(int length)
//            : this(0.16, length)
//        {
//        }

       
//        /// <summary>
//        ///   Constructs a new Blackman window.
//        /// </summary>
//        /// 
//        /// <param name="alpha">Blackman's alpha</param>
//        /// <param name="length">The length for the window.</param>
//        /// 
//        public BlackmanWindow(double alpha, int length) : base(length)
//        {
//            double a0 = (1.0 - alpha) / 2.0;
//            double a1 = 0.5;
//            double a2 = alpha / 2.0;


//            for (int i = 0; i < length; i++)
//                this[i] = (float)(a0 - a1 * Math.Cos((2.0 * System.Math.PI * i) / (length - 1))
//                    + a2 * Math.Cos((4.0 * System.Math.PI * i) / (length - 1)));
//        }


//    }

//    public class RaisedCosineWindow : WindowBase
//    {
//        /// <summary>
//        ///   Constructs a new Raised Cosine Window
//        /// </summary>
//        /// 
//        public RaisedCosineWindow(double alpha, double duration, int sampleRate)
//            : this(alpha, (int)duration * sampleRate, sampleRate)
//        {
//        }

//        /// <summary>
//        ///   Constructs a new Raised Cosine Window
//        /// </summary>
//        /// 
//        public RaisedCosineWindow(double alpha, int length)
//            : this(alpha, length, 0)
//        {
//        }

//        /// <summary>
//        ///   Constructs a new Raised Cosine Window
//        /// </summary>
//        /// 
//        public RaisedCosineWindow(double alpha, int length, int sampleRate)
//            : base(length, sampleRate)
//        {

//            double beta = 1 - alpha;

//            for (int i = 0; i < length; i++)
//                this[i] = (float)(beta - (alpha * System.Math.Cos((2 * System.Math.PI * i) / (length - 1))));
//        }


//        /// <summary>
//        ///   Creates a new Hamming Window.
//        /// </summary>
//        /// 
//        public static RaisedCosineWindow Hamming(int length)
//        {
//            return new RaisedCosineWindow(0.46, length);
//        }

//        /// <summary>
//        ///   Creates a new Hann Window.
//        /// </summary>
//        /// 
//        public static RaisedCosineWindow Hann(int length)
//        {
//            return new RaisedCosineWindow(0.5, length);
//        }

//        /// <summary>
//        ///   Creates a new Hann Window.
//        /// </summary>
//        /// 
//        public static RaisedCosineWindow Hann(double length, int sampleRate)
//        {
//            return new RaisedCosineWindow(0.5, length, sampleRate);
//        }

//        /// <summary>
//        ///   Creates a new Rectangular Window.
//        /// </summary>
//        /// 
//        /// <param name="length">The size of the window.</param>
//        /// 
//        public static RaisedCosineWindow Rectangular(int length)
//        {
//            return new RaisedCosineWindow(0, length);
//        }

//    }



//    public abstract class WindowBase : IWindow
//    {
//        private int sampleRate;
//        private float[] window;

//        /// <summary>
//        ///   Gets the window length.
//        /// </summary>
//        /// 
//        public int Length
//        {
//            get { return window.Length; }
//        }

//        /// <summary>
//        ///   Gets the Window duration.
//        /// </summary>
//        /// 
//        public double Duration
//        {
//            get { return sampleRate * window.Length; }
//        }

//        /// <summary>
//        ///   Constructs a new Window.
//        /// </summary>
//        /// 
//        protected WindowBase(double duration, int sampleRate)
//            : this((int)duration * sampleRate, sampleRate)
//        {

//        }

//        /// <summary>
//        ///   Constructs a new Window.
//        /// </summary>
//        /// 
//        protected WindowBase(int length)
//            : this(length, 0)
//        {
//        }

//        /// <summary>
//        ///   Constructs a new Window.
//        /// </summary>
//        /// 
//        protected WindowBase(int length, int sampleRate)
//        {
//            this.window = new float[length];
//            this.sampleRate = sampleRate;
//        }

//        /// <summary>
//        ///   Gets or sets values for the Window function.
//        /// </summary>
//        /// 
//        public float this[int index]
//        {
//            get { return window[index]; }
//            protected set { window[index] = value; }
//        }

//        /// <summary>
//        ///   Splits a signal using the window.
//        /// </summary>
//        /// 
//        public unsafe virtual Signal Apply(Signal signal, int sampleIndex)
//        {
//            int channels = signal.Channels;
//            int samples = signal.Length;

//            int minLength = System.Math.Min(signal.Length, Length);

//            Signal result = new Signal(channels, Length, signal.SampleRate, signal.SampleFormat);

//            if (signal.SampleFormat == SampleFormat.Format32BitIeeeFloat)
//            {
//                for (int c = 0; c < channels; c++)
//                {
//                    float* dst = (float*)result.Data.ToPointer() + c;
//                    float* src = (float*)signal.Data.ToPointer() + c + c * sampleIndex;

//                    for (int i = 0; i < minLength; i++, dst += channels, src += channels)
//                    {
//                        *dst = window[i] * (*src);
//                    }
//                }
//            }
//            else if (signal.SampleFormat == SampleFormat.Format64BitIeeeFloat)
//            {
//                for (int c = 0; c < channels; c++)
//                {
//                    double* dst = (double*)result.Data.ToPointer() + c;
//                    double* src = (double*)signal.Data.ToPointer() + c + c * sampleIndex;

//                    for (int i = 0; i < minLength; i++, dst += channels, src += channels)
//                    {
//                        *dst = window[i] * (*src);
//                    }
//                }
//            }
//            else
//            {
//                throw new UnsupportedSampleFormatException("Sample format is not supported by the filter.");
//            }

//            return result;
//        }

//        /// <summary>
//        ///   Splits a signal using the window.
//        /// </summary>
//        /// 
//        public virtual ComplexSignal Apply(ComplexSignal complexSignal, int sampleIndex)
//        {
//            Complex[,] resultData = new Complex[Length, complexSignal.Channels];
//            ComplexSignal result = ComplexSignal.FromArray(resultData, complexSignal.SampleRate);

//            int channels = result.Channels;

//            unsafe
//            {
//                for (int c = 0; c < complexSignal.Channels; c++)
//                {
//                    Complex* dst = (Complex*)result.Data.ToPointer() + c;
//                    Complex* src = (Complex*)complexSignal.Data.ToPointer() + c + c * sampleIndex;

//                    for (int i = 0; i < Length; i++, dst += channels, src += channels)
//                    {
//                        *dst = window[i] * (*src);
//                    }
//                }
//            }

//            return result;
//        }

//        public double[] Apply(double[] signal, int sampleIndex)
//        {
//            throw new NotImplementedException();
//        }

//        public double[][] Apply(double[][] signal, int sampleIndex)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
