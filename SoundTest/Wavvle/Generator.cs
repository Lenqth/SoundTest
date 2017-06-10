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
using SoundTester;

namespace dxgtest {
	public partial class Generator : Form {
		public Generator() {
			InitializeComponent();
		}

		Graph painter;
		Synth synth;

		private void pictureBox1_Paint(object sender, PaintEventArgs e) {
			painter.InitDraw();
            painter.Clear();
			int prev_pos = graph_pos;
			//graph_pos += timer1.Interval / 10; 4
			graph_pos =  (int)(synth.player.GetPosition() / 441 / 2);
			for (int i = prev_pos; i <= graph_pos ; i++) {
				freq_graph[i % Synth.FREQ_BUFF_SIZE] = synth.freq_buff[i % Synth.FREQ_BUFF_SIZE];
            }
			painter.Curve(freq_graph , sample_interval:0.04f );

			float cur_x = (0.04f * graph_pos - painter.offset_x) * painter.scale_x;
            painter.g.DrawLine(Pens.BlueViolet, new PointF(cur_x, 0), new PointF(cur_x, painter.g.ClipBounds.Bottom));

			;
			painter.EndDraw(e.Graphics);
	}

		private void Generator_Load(object sender, EventArgs e) {
			painter = new Graph(pictureBox1.Size.Width, pictureBox1.Size.Height);
			synth = new Synth();
			synth.Play(Frequency,Waveform);
        }
		
		float[] freq_graph = new float[Synth.FREQ_BUFF_SIZE];
		int graph_pos = 0;

		bool sndkey1 = false;
		bool sndkey2 = false;
		bool sndkey3 = false;

		int sn_pitch = 64;
		int sn_pitch2 = 0;
		float snd1freq = Graph.pitch2freq(64);
		float snd2freq = Graph.pitch2freq(70);
		float snd3freq = Graph.pitch2freq(71);

		bool vib_switch = true;
		bool vibflg = false;

		/*config*/

		float vibFreq = 5.0f;
		float vibWidth = 30.0f;


		float shkDur = 0.2f;
		float shkWidth = 100.0f;

		double shkTime = 0.0;

		double prev_t = 9e99;
		protected double Frequency(double t) {
			double delta = 0.0;
			if (prev_t < t) {
				delta = t - prev_t;
			}
			prev_t = t;
			double basefreq = snd1freq;
			basefreq -= shkTime / shkDur * shkWidth;
			shkTime = Math.Max(shkTime-delta,0);

			if (vibflg) {
				basefreq += (vibWidth * Math.Sin(t * vibFreq * 2 * Math.PI));
			}
			return basefreq;
        }


		static double Pow2(double x, double y) {
			return Math.Sign(x) * Math.Pow(Math.Abs(x), y);
		}

		//protected double Waveform(double phase) {
		//	return Pow2(Math.Sin(phase), 0.2);
		//}


		protected double Waveform(double phase) {
			return Math.Sin(phase) * 0.4 + Math.Sin(phase * 2.0) * 0.3 + Math.Sin(phase * 3.0) * 0.2 + Math.Sin(phase * 4.0) * 0.1 ;
		}

		private void Generator_FormClosed(object sender, FormClosedEventArgs e) {
			synth.Stop();
		}

		private void timer1_Tick(object sender, EventArgs e) {
			this.pictureBox1.Refresh();
		}

		private void Generator_KeyDown(object sender, KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Z:
					sndkey1 = true;
					synth.Volume = 1.0f;
					break;
				case Keys.X:
					sndkey2 = true;
					break;
				case Keys.C:
					sndkey3 = true;
					break;
				case Keys.A:
					if (sndkey1) break;
					synth.wp.ClearBuffer();
					shkTime = shkDur;
					sndkey1 = true;
					synth.Volume = 1.0f;
					break;
				case Keys.ShiftKey:
					vibflg = true;
					break;

			}
		}

		private void Generator_KeyUp(object sender, KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Z:
					sndkey1 = false;
					synth.Volume = 0.0f;
					break;
				case Keys.X:
                    sndkey2 = false;
					break;
				case Keys.C:
					sndkey3 = false;
					break;
				case Keys.A:
					shkTime = 0.0;
					sndkey1 = false;
					synth.Volume = 0.0f;
					break;
				case Keys.Up:
					sn_pitch += 1;
					snd1freq = Graph.pitch2freq( sn_pitch + sn_pitch2 / 8.0f);
					break;
				case Keys.Down:
					sn_pitch -= 1;
					snd1freq = Graph.pitch2freq(sn_pitch + sn_pitch2 / 8.0f);
					break;
				case Keys.ShiftKey:
					vibflg = false;
					break;
			}
		}


	}
}
