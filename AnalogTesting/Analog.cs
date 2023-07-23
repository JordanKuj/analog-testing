using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenLayers.Base;


namespace AnalogTesting
{
    public class AnalogClient
    {
        private readonly DeviceMgr DeviceManager = DeviceMgr.Get();
        private Device Device { get; set; }
        private AnalogInputSubsystem ainSS;
        private OlBuffer[] daqBuffers;
        private int Stopping;
        private int DurationSeconds;
        System.IO.FileStream OutFile { get; set; }

        public AnalogClient()
        {
        }

        private void SetupChannels(params (CouplingType couplingType, ExcitationCurrentSource excitation)[] configs)
        {
            ainSS.ChannelList.Clear();
            for (var i = 0; i < configs.Length; i++)
            {
                var config = configs[i];
                var ch = ainSS.SupportedChannels.GetChannelInfo(SubsystemType.AnalogInput, i);
                ch.Coupling = config.couplingType;
                ch.ExcitationCurrentSource = config.excitation;
                ainSS.ChannelList.Add(new ChannelListEntry(ch));
            }
            foreach (var ch in ainSS.ChannelList)
            {
                ((ChannelListEntry)ch).Gain = 0;
            }
            ainSS.Config();

        }
        private void SetupBuffers(int sampleRate, int channelCount)
        {
            if (daqBuffers != null)
            {
                foreach (var buffer in daqBuffers)
                {
                    buffer?.Dispose();
                }
            }

            daqBuffers = new OlBuffer[10];

            for (int i = 0; i < daqBuffers.Length; ++i)
            {
                daqBuffers[i] = new OlBuffer(sampleRate * channelCount, ainSS);

                ainSS.BufferQueue.QueueBuffer(daqBuffers[i]);
            }

            foreach (var buff in daqBuffers)
            {
                if (buff.State == OlBuffer.BufferState.Idle || buff.State == OlBuffer.BufferState.Completed)
                {
                    ainSS.BufferQueue.QueueBuffer(buff);
                }
            }
        }

        public void Start(int sampleRate, TimeSpan duration)
        {
            Debug.WriteLine("Stopped");

            OutFile = File.OpenWrite($"{DateTimeOffset.Now.ToUnixTimeSeconds()}_{sampleRate}.raw");
            DurationSeconds = (int)duration.TotalSeconds;
            var devices = DeviceManager.GetDeviceNames();
            //DeviceName = devices.First();
            //Logger.LogTrace($"Device <{DeviceName}> starting.");
            Interlocked.Exchange(ref Stopping, 0);
            var DeviceName = devices.First();
            var hardwareAvail = DeviceManager.HardwareAvailable();
            Device = DeviceManager.GetDevice(DeviceName);
            ainSS = Device.AnalogInputSubsystem(0);
            ainSS.DriverRunTimeErrorEvent += new DriverRunTimeErrorEventHandler(HandleDriverRunTimeErrorEvent);
            ainSS.BufferDoneEvent += new BufferDoneHandler(HandleBufferDone);
            ainSS.QueueDoneEvent += new QueueDoneHandler(HandleQueueDone);
            ainSS.QueueStoppedEvent += new QueueStoppedHandler(HandleQueueStopped);

            ainSS.BufferQueue.FreeAllQueuedBuffers();
            ainSS.Trigger.ThresholdTriggerChannel = 0;
            ainSS.Trigger.TriggerType = TriggerType.Software;



            ainSS.DataFlow = DataFlow.Continuous;
            //ainSS.VoltageRangeC
            ainSS.Clock.Frequency = sampleRate;


            SetupBuffers(sampleRate, 2);
            SetupChannels((CouplingType.AC, ExcitationCurrentSource.Disabled));


            ainSS.Start();
            //Logger.LogTrace($"Device <{DeviceName}> started.");
        }

        public void Stop()
        {
            ainSS.DriverRunTimeErrorEvent -= new DriverRunTimeErrorEventHandler(HandleDriverRunTimeErrorEvent);
            ainSS.BufferDoneEvent -= new BufferDoneHandler(HandleBufferDone);
            ainSS.QueueDoneEvent -= new QueueDoneHandler(HandleQueueDone);
            ainSS.QueueStoppedEvent -= new QueueStoppedHandler(HandleQueueStopped);
            ainSS.Stop();
            OutFile.Dispose();
            Debug.WriteLine("Stopped recording");
        }

        private void HandleQueueStopped(object sender, GeneralEventArgs eventData)
        {
            var msg = $"Queue Stopped received on subsystem {eventData.Subsystem} element {eventData.Subsystem.Element} at time {eventData.DateTime.ToString("T")}";
            System.Diagnostics.Debug.WriteLine(msg);
        }

        private void HandleQueueDone(object sender, GeneralEventArgs eventData)
        {
            System.Diagnostics.Debug.WriteLine($"Device got handleQueueDone");
        }

        private void HandleBufferDone(object sender, BufferDoneEventArgs bufferDoneData)
        {
            try
            {
                var buffer = bufferDoneData.OlBuffer;
                var subSystem = bufferDoneData.Subsystem;

                double[] GetData(int channelIndex)
                {
                    return buffer.GetDataAsVolts(subSystem.ChannelList[channelIndex]);
                }

                if (buffer.ValidSamples > 0)
                {
                    double[] microphoneBuffer = GetData(0);

                    if (Stopping == 0)
                    {
                        ainSS.BufferQueue.QueueBuffer(buffer);
                    }

                    var sampleCount = buffer.ValidSamples / ainSS.ChannelList.Count;

                    if (sampleCount > 0)
                    {
                        byte[] GetBytes(double[] values)
                        {
                            var result = new byte[values.Length * sizeof(double)];
                            Buffer.BlockCopy(values, 0, result, 0, result.Length);
                            return result;
                        }

                        var micData = GetBytes(microphoneBuffer);


                        OutFile.Write(micData, 0, micData.Length);

                        var valuesWritten = OutFile.Position / sizeof(double);
                        var freq = bufferDoneData.Subsystem.Clock.Frequency;

                        var seconds = valuesWritten / freq;
                        if (seconds > DurationSeconds)
                        {
                            Stop();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "Got error in handle buffer in analog client.");
            }
        }

        private void HandleDriverRunTimeErrorEvent(object sender, DriverRunTimeErrorEventArgs eventData)
        {
            var msg = string.Format("<{0}> Occurred on subsystem {1} element {2} at time {3}",
                eventData.Message,
                eventData.Subsystem,
                eventData.Subsystem.Element,
                eventData.DateTime.ToString("T"));
            Debug.WriteLine(msg);
        }
    }
}
