using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace dxgtest {
	public partial class Generator : Form {
		public Generator() {
			InitializeComponent();
		}

		Graph painter; 
		private void pictureBox1_Paint(object sender, PaintEventArgs e) {
			painter.InitDraw();
            painter.Clear();
			int prev_pos = graph_pos;
			//graph_pos += timer1.Interval / 10; 
			graph_pos =  (int)(player.GetPosition() / 441 / 2);
			Console.Error.WriteLine(graph_pos);
			for (int i = prev_pos; i <= graph_pos ; i++) {
				freq_graph[i % FREQ_BUFF_SIZE] = freq_buff[i % FREQ_BUFF_SIZE];
            }
			painter.Curve(freq_graph , sample_interval:0.01f );
			painter.EndDraw(e.Graphics);
	}

		private void Generator_Load(object sender, EventArgs e) {
			painter = new Graph(pictureBox1.Size.Width, pictureBox1.Size.Height);
			this.Init();
			this.Play();
        }

		const int FREQ_BUFF_SIZE = 100000;
		float[] freq_buff = new float[FREQ_BUFF_SIZE];
		float[] freq_graph = new float[FREQ_BUFF_SIZE];
		int buff_pos = 0;
		int graph_pos = 0;
		BufferedWaveProvider wp;
		WaveOut player;

		protected void Init() {
			wp = new BufferedWaveProvider(new WaveFormat(44100, 16, 1));
			var mmDevice = new MMDeviceEnumerator()
				.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
			player = new WaveOut(); //  new WasapiOut(mmDevice, AudioClientShareMode.Shared, false, 200);

		}
		public void Play() {
			Task t = test_sin(delegate (float x) {
					return (float)(330.0f + 40f * Math.Sin(x * 8 * Math.PI));
			});
			player.Init(wp);
			player.Play();
			
		}

		public void Stop() {
			player.Stop();
		}
		static float PHASE_PER_SAMPLE = (float)(2 * Math.PI / 44100.0);
		static float PHASE_LARGE = (float)(100 * 2 * Math.PI);
		async Task test_sin(Func<float, float> freq) {
			int WAVE_BYTES = (int)(44100 * 5) * 2;
			int bufsize = 20000;
			float phase = 0;
			byte[] data = new byte[WAVE_BYTES];
			int t = 0;
			while (true) {
				for (int i = 0; i < WAVE_BYTES; i += 2) {
					float fq = freq( (t++) / 44100.0f);
                    phase += fq * PHASE_PER_SAMPLE;

					if ((t % 441) == 440) {
						freq_buff[buff_pos] = fq;
						buff_pos++;
						if (buff_pos >= freq_buff.Length) buff_pos = 0;
					}

					if (phase > PHASE_LARGE) phase -= PHASE_LARGE;

					ushort val = (ushort)(Math.Sin(phase) * (1 << 14) + (1 << 14));
					data[i] = (byte)(val & 0xff);
					data[i + 1] = (byte)(val >> 8);
				}
				//Console.WriteLine(phase);
				//Console.WriteLine(wp.BufferLength);
				//Console.WriteLine(wp.BufferedBytes);
				//Console.WriteLine(DateTime.Now);
				int pos = 0;
				for (pos = 0; pos + bufsize < data.Length; pos += bufsize) {
					wp.AddSamples(data, pos, bufsize);
					while (wp.BufferedBytes + bufsize > wp.BufferLength) {
						await Task.Delay(100);
					}
				}
				wp.AddSamples(data, pos, data.Length - pos);
			}
		}

		private void Generator_FormClosed(object sender, FormClosedEventArgs e) {
			this.Stop();
		}

		private void timer1_Tick(object sender, EventArgs e) {
			this.pictureBox1.Refresh();
		}
	}
}
