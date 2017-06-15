using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace SoundTester {
	class Note {
		public int Pitch = 60;
		public int Velocity = 127;
		internal double _CurrentPhase = 0;
		internal double _CurrentTime = 0;
		public Note(int pitch , int velocity) {
			this.Pitch = pitch;
			this.Velocity = velocity;
		}
	}

	class MultiSynth {
		public BufferedWaveProvider wp;
		public WaveOut player;
		public const int FREQ_BUFF_SIZE = 1000;
		public int SampleRate = 44100 ;
		public double Volume = 1.0;

		protected bool isAlive = true;

		public List<Note>[] CurrentNotes;
		public Func<double, double , double>[] WaveForms = new Func<double, double, double>[256];
		public int[] WaveType = new int[16];
		public event EventHandler OnTick ;

		protected double SinWave(double phase,double time) {
			return Math.Sin(phase) * 1.0;
		}
		protected double SawWave(double phase, double time) {
			return ( (phase / (Math.PI * 2.0) ) % (1.0) )*2.0-1.0;
		}
		protected double SqWave(double phase, double time) {
			return ((phase / (Math.PI * 2.0) ) % 1) < 0.5 ? 0 : 1;
		}
		protected Random random;
		protected double WhiteNoise(double phase, double time) {
			double r = 0.0;
			int N = 10;
			for (int i = 0; i < 10; i++) {
				r += random.NextDouble();
            }
			return 2*r/N - 0.5;
		}
		protected double TriWave(double phase, double time) {
			double x = (phase / (Math.PI * 2.0));
			return ((x % 1) < 0.5 ? 2 * x : 2 - 2 * x)*2.0-1.0;
		}
		protected Func<double, double, double> SqWaveGen(double duty) {
			return delegate (double phase, double time) {
				double x = (phase / (Math.PI * 2.0));
				return (x % 1) < duty ? 1 : 0;
			};
		}
		protected Func<double, double, double> TriWaveGen(double duty) {
			return delegate (double phase, double time) {
				double x = (phase / (Math.PI * 2.0));
				return ((x % 1.0) < duty ? (x / duty) : (1.0-x)/(1.0-duty) )*2.0-1.0;
			};
		}
		protected Func<double, double, double> FMWaveGen(double mod_freq, double mod_amp) {
			return ((phase, time) => SqWave(phase + mod_amp * Math.Sin(mod_freq * time), 0));
		}
		public MultiSynth() {
			CurrentNotes = new List<Note>[16];
			for (int i = 0; i < 16; i++) {
				CurrentNotes[i] = new List<Note>();
			}
			random = new Random();
            WaveForms[0] = SinWave;
			WaveForms[1] = TriWaveGen(0.5);
			//WaveForms[1] = SawWave;
			WaveForms[23] = TriWave;
			WaveForms[24] = SqWaveGen(0.8);
			WaveForms[14] = SawWave;
			WaveForms[80] = SqWave;
			WaveForms[3] = TriWaveGen(0.7);
			var RANDOM_WAVEFORMS = false;
            if (RANDOM_WAVEFORMS) {
				var rand = new Random(142857);
				for (int i = 0; i < 256; i++) {
					var f = 5 + rand.NextDouble() * 50;
					var a = rand.NextDouble() * 20;
					if (WaveForms[i] == null) {
						WaveForms[i] = FMWaveGen(f, a);
					}
				}
			}
		}

		public void NoteOn(int channel,Note note) {
			lock(CurrentNotes[channel])
				CurrentNotes[channel].Add(note);
		}
		public void NoteOff(int channel, Note note) {
			lock (CurrentNotes[channel])
				CurrentNotes[channel].RemoveAll(n => (n.Pitch == note.Pitch));
		}

		public void ProgramChange(int channel, int n) {
			if (WaveForms[n] != null) {
				WaveType[channel] = n;
			} else {
				Console.WriteLine(String.Format("channel {0}:ProgramChange {1} failed.", channel,n)) ;
			}
		}






		WaveFileWriter writer;
		public void Play() {
			wp = new BufferedWaveProvider(new WaveFormat(SampleRate, 16, 1));
			var mmDevice = new MMDeviceEnumerator()
				.GetDefaultAudioEndpoint(DataFlow.Render , Role.Multimedia);
			player = new WaveOut( ); //  new WasapiOut(mmDevice, AudioClientShareMode.Shared, false, 200);
									 // writer = new WaveFileWriter("out.wav", new WaveFormat(SampleRate, 16, 1));
			player.DesiredLatency = 100;
			player.Init(wp);
			isAlive = true;
			Task t = task();
			player.Play();
			player.Volume = 1.0f;
        }

		public void Stop() {
			isAlive = false;
			if (player != null)
				player.Stop();
			if (writer != null)
				writer.Close();
		}
		

		static double PHASE_PER_SAMPLE = (2 * Math.PI / 44100.0);
		async Task task() {
			const int WAVE_BYTES = (int)10;
			const int bufferlen = 441*10 ;
			byte[] data = new byte[WAVE_BYTES];
			int tick = 0;
			while (isAlive) {
				for (int i = 0; i < WAVE_BYTES; i += 2) {
					double t = tick / (double)SampleRate;
					double val = 0.0;
					OnTick(this,new EventArgs());
					
					for (int j = 0; j < 16; j++) {
						lock (CurrentNotes[j]) {
							foreach (var x in CurrentNotes[j]) {
								double fq = MusicUtils.pitch2freq(x.Pitch);
								val += 0.1 * WaveForms[WaveType[j]](x._CurrentPhase,x._CurrentTime);
								x._CurrentPhase += fq * PHASE_PER_SAMPLE;
								x._CurrentTime += 1 / 44100.0;
							}
						}
					}
					if (val >= 1.0) val = 1.0;
					if (val <= -1.0) val = -1.0;
					ushort usval = (ushort)( Math.Round(val  * ( 1 << 14 )) + (1 << 14));
					if(writer!=null)writer.WriteSample((float)val);
					data[i] = (byte)(usval & 0xff);
					data[i + 1] = (byte)(usval >> 8);
					tick += 1;
				}
				int pos = 0;
				wp.AddSamples(data, pos, WAVE_BYTES);
				while (wp.BufferedBytes > bufferlen - WAVE_BYTES ) {					
					await Task.Delay(1);
				}
			}
		}

	}
}
