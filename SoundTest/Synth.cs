using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace SoundTester {
	class Synth {
		public BufferedWaveProvider wp;
		public WaveOut player;
		public const int FREQ_BUFF_SIZE = 1000;
		public float[] freq_buff = new float[FREQ_BUFF_SIZE];
		public int buff_pos = 0;
		public double Volume = 1.0;
		
		protected double Waveform(double phase) {
			return Math.Sin(phase) * 0.4 + Math.Sin(phase * 2.0) * 0.3 + Math.Sin(phase * 3.0) * 0.2 + Math.Sin(phase * 4.0) * 0.1;
		}

		public void Play(Func<double, double> freq, Func<double, double> wave) {
			wp = new BufferedWaveProvider(new WaveFormat(44100, 16, 1));
			var mmDevice = new MMDeviceEnumerator()
				.GetDefaultAudioEndpoint(DataFlow.Render , Role.Multimedia);
			player = new WaveOut(); //  new WasapiOut(mmDevice, AudioClientShareMode.Shared, false, 200);
			player.Init(wp);
			Task t = task(freq, wave);
			player.Play();
			player.Volume = 1.0f;
        }
		public void Play(Func<double, double> freq) {
			Play(freq, Waveform);
		}
		public void Play(double freq) {
			Play(t => freq, Waveform);
		}

		public void Stop() {
			player.Stop();
		}

		static double PHASE_PER_SAMPLE = (2 * Math.PI / 44100.0);
		async Task task(Func<double, double> freq, Func<double, double> wave) {
			int WAVE_BYTES = (int)(441) * 2;
			int bufferlen = 441 * 30;
			double phase = 0;
			byte[] data = new byte[WAVE_BYTES];
			int t = 0;
			while (true) {
				for (int i = 0; i < WAVE_BYTES; i += 2) {
					double fq = freq((t++) / 44100.0);
					phase += fq * PHASE_PER_SAMPLE;
					if ((t % 441) == 440) {
						freq_buff[buff_pos] = (float)fq;
						//if (!sndkey1)
						//	freq_buff[buff_pos] = float.NaN;
						buff_pos++;
						if (buff_pos >= freq_buff.Length) buff_pos = 0;
					}
					ushort val = (ushort)( (wave(phase) * Volume )  * (1 << 14) + (1 << 14));
					//if (!sndkey1) val = 0;
					data[i] = (byte)(val & 0xff);
					data[i + 1] = (byte)(val >> 8);
				}
				//Console.WriteLine(phase);
				//Console.WriteLine(wp.BufferLength);
				//Console.WriteLine(wp.BufferedBytes);
				//Console.WriteLine(DateTime.Now);
				int pos = 0;
				while (wp.BufferedBytes + data.Length > bufferlen) {
					await Task.Delay(1);
				}
				wp.AddSamples(data, pos, data.Length);
			}
		}

	}
}
