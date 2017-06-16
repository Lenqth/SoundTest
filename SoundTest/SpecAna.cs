using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dxgtest {
	public partial class SpecAna : Form {
		public static Font FONT = new Font(FontFamily.GenericMonospace, 8);
		public SpecAna() {
			InitializeComponent();
		}

		WaveIn wi = null;
		float[] recorded;
		float[] spectrum;
		double[] test_graph;

		const int FREQ_GRAPH_SIZE = 400;
		float CUTOFF = 120.0f;
		float THRESHOLD = 0.0001f;
		float[] freq_graph = new float[FREQ_GRAPH_SIZE];
		int freq_graph_pos = 0;

		List<Complex32> _buffer = new List<Complex32>();
		List<KeyValuePair<int, float>> peaks = new List<KeyValuePair<int, float>>();

		static int SAMPLE_RATE = 48000;
		static int BUFFER_SIZE = 4800 * 2;
		float UNIT_TIME = 1.0f;

		double test_pitch = double.NaN;

		public void Init() {

			for (int i = 0; i < WaveIn.DeviceCount; i++) {
				var deviceInfo = WaveIn.GetCapabilities(i);
				Console.WriteLine(  String.Format("Device {0}: {1}, {2} channels",
					i, deviceInfo.ProductName, deviceInfo.Channels) );
			}

			wi = new WaveIn();
			wi.DeviceNumber = 0;
			
			wi.WaveFormat = new WaveFormat(sampleRate: SAMPLE_RATE, channels: 1);
			wi.DataAvailable += _DataAvailable;
			wi.StartRecording();
		}
		public void End() {
			wi.StopRecording();
		}
		public void RecalcSpectrum() {
			if (_buffer.Count > BUFFER_SIZE) {
				_buffer.RemoveRange(0, _buffer.Count - BUFFER_SIZE);
			}
			UNIT_TIME = (_buffer.Count * 1.0f / SAMPLE_RATE);
			int _buffer_len = _buffer.Count;
			var windowfunc = Window.Hann(_buffer_len);

			float ScaleFactor = 1.0f;

			var fourier_v = new Complex32[_buffer_len];
			for (int i = 0; i < _buffer_len; i++) {
				fourier_v[i] = new Complex32(ScaleFactor * _buffer[i].Real * (float)windowfunc[i], 0);
			}

			Fourier.Forward(fourier_v);
			spectrum = fourier_v.Take(fourier_v.Length / 2).Select(x => (float)x.Norm()).ToArray();
			/*
			Complex32[] cepstrum = fourier_v.Take(fourier_v.Length / 2).Select(x => Complex32.Log(x) ).ToArray();
			Fourier.Inverse(cepstrum);
			for (int i = 120; i < cepstrum.Length; i++) {
				cepstrum[i] = 0;
			}
			Fourier.Forward(cepstrum);
			test_graph = cepstrum.Take(cepstrum.Length / 2).Select(x => (float) x.Norm() ).ToArray(); //cepstrum.Select(x => (float)x.Norm()).ToArray();
			*/
			if(_buffer.Count == BUFFER_SIZE) {
				int l = recorded.Length;
				int CORR_LEN = 2000;
				test_graph = new double[CORR_LEN];
				for (int j = 0; j < l; j++) {
					test_graph[0] += (recorded[j] * recorded[j]);// / (l- i);
				}
				for (int i = 1; i < CORR_LEN; i++) {
					for (int j = 0; j + i < l; j++) {
						test_graph[i] += (recorded[j] * recorded[i + j]); // / (l - i);
					}
					test_graph[i] /= test_graph[0] * l / (l - i);
					double q = 0.0;
					int k = 0;
					for (k = 0; k < 5; k++) {
						if (i - k <= 0) break;
						q += test_graph[i - k];
                    }
					test_graph[i] = q / k;
                }
				List<double> corr_peaks = new List<double>();
				for (int i = 2; i < CORR_LEN - 1; i++) {
					if (test_graph[i] > 0.2) {
						if (test_graph[i - 1] < test_graph[i] && test_graph[i] > test_graph[i + 1]) {
							corr_peaks.Add(i);
						}
					}
				}
				if (corr_peaks.Count >= 3) {
					test_pitch = SAMPLE_RATE * (1.0) / (corr_peaks[2] - corr_peaks[1]);
					if (test_pitch < 100)
						test_pitch = double.NaN;
                } else {
					test_pitch = double.NaN;
				}
			}
			freq_graph[freq_graph_pos++] = (float)test_pitch;
			/*
			float prev = -1.0f;
			bool prev_up = true;
			float max_fq = 0.0f;
			float max_v = 0.0f;

			peaks.Clear();
            for (int i = 0; i < spectrum.Length; i++) {
				float v = spectrum[i];
				float fq = i / UNIT_TIME;
				if (fq < CUTOFF) continue;
				bool up = false;
				if (v > prev) {
					up = true;
				}
				if ( (!up) && prev_up && prev > THRESHOLD ) {
					peaks.Add(new KeyValuePair<int, float>(i-1, prev));
				}
				if (max_v < v) {
					max_fq = fq;
					max_v = v;
				}
				prev = v;
				prev_up = up;
			}
			float freq = float.NaN;
			if (peaks.Count > 0) freq = peaks.Select( x=> x.Key).Min() / UNIT_TIME;
            freq_graph[freq_graph_pos++] = (float) test_pitch;

			//Console.WriteLine(String.Format("{0,12:F4},{1,12:F4}", max_fq, max_v));
/*			if (max_v > THRESHOLD) {
				freq_graph[freq_graph_pos++] = max_fq;
			} else {
				freq_graph[freq_graph_pos++] = float.NaN;
			} */



			if (freq_graph_pos >= FREQ_GRAPH_SIZE) freq_graph_pos = 0;
        }
		public void _DataAvailable(object sender , WaveInEventArgs e) {
			var buff = e.Buffer;
            //Console.WriteLine(e.BytesRecorded);
			recorded = new float[(buff.Length / 2)];
			int REC_LEN = recorded.Length;
			int NUMCHUNKS = 2;
			int chunk_len = REC_LEN / NUMCHUNKS;
			//			var fourier_v = new Complex32[(buff.Length / 2)];
			/*
						for (int i = 0; i < recorded.Length; i ++){
							short val = (short) ( (buff[2 * i + 1] << 8 )| buff[2 * i ] ) ;
							recorded[i] =  val / 65536.0f; // (val - 32768.0f) / 32768.0f ;
							_buffer.Add(new Complex32(recorded[i], 0)) ;
						}

						this.RecalcSpectrum();
						*/
			int i = 0;
			for (int j = 0; j < NUMCHUNKS; j++) {
				for (int k = 0; k < chunk_len; k++, i++) {
					short val = (short)((buff[2 * i + 1] << 8) | buff[2 * i]);
					recorded[i] = val / 65536.0f; // (val - 32768.0f) / 32768.0f ;
					_buffer.Add(new Complex32(recorded[i], 0));
				}
				RecalcSpectrum();
			}
		}

		private void SpecAna_Load(object sender, EventArgs e) {
			Init();
		}

		private void SpectrumBox_Paint(object sender, PaintEventArgs e) {
			if(spectrum != null) Curve_Spec(e.Graphics,spectrum);
        }

		private void PitchGraphBox_Paint(object sender, PaintEventArgs e) {
			Graph painter = new Graph(PitchGraphBox.Width, PitchGraphBox.Height);
			painter.InitDraw();
			painter.Clear();
			painter.Curve(freq_graph);
			painter.EndDraw(e.Graphics);
		}
		private void CepsBox_Paint(object sender, PaintEventArgs e) {
			if (test_graph != null) Curve_Test(e.Graphics, test_graph);
		}

		public void Curve_Spec(Graphics g, float[] yl) {
			float prev_x = float.NaN;
			float prev_y = float.NaN;
			float real_x, real_y;
			if (UNIT_TIME == 0) return;
			for (int i = 1; i < yl.Length; i++) {
				//real_x = (float) i / yl.Length * g.ClipBounds.Width;
				float fq = i / UNIT_TIME;

				real_x = (float)((Math.Log(fq) / Math.Log(10)) - 1.0f) / 3.0f * g.ClipBounds.Width;

				//real_y = g.ClipBounds.Bottom - (yl[i] * g.ClipBounds.Height);
				real_y = (float)-(Math.Log10(yl[i]) / 5.0f * g.ClipBounds.Height);

				if (float.IsInfinity(real_x) || float.IsInfinity(real_y)) {
					prev_x = prev_y = float.NaN;
					continue;
				}
				if (peaks.Count > 0 && peaks[0].Key == i) {
					peaks.RemoveAt(0);
					g.FillPie(Brushes.Red, real_x - 4, real_y - 4, 8, 8, 0, 360);
					g.DrawString(String.Format("{0,4:F0}", fq), FONT, Brushes.Red, new PointF(real_x - 12, real_y - 16));
				}

				if ((!float.IsNaN(real_y)) && (!float.IsNaN(prev_y)) &&
					(!float.IsInfinity(real_y)) && (!float.IsInfinity(prev_y))) {
					g.DrawLine(Pens.White, new PointF(prev_x, prev_y), new PointF(real_x, real_y));
				}
				prev_x = real_x;
				prev_y = real_y;
			}
			if (!double.IsNaN(test_pitch)) {
				float tp_x = (float)((Math.Log(test_pitch) / Math.Log(10)) - 1.0f) / 3.0f * g.ClipBounds.Width;
				g.DrawLine(Pens.GreenYellow, new PointF(tp_x, g.ClipBounds.Top), new PointF(tp_x, g.ClipBounds.Bottom));
			}

		}
		public void Curve_Test(Graphics g, double[] yl) {
			double prev_x = double.NaN;
			double prev_y = double.NaN;
			double real_x, real_y;
			if (UNIT_TIME == 0) return;
			for (int i = 1; i < yl.Length; i++) {
				//real_x = (float) i / yl.Length * g.ClipBounds.Width;
				double fq = SAMPLE_RATE * ( 1.0 / i);

				real_x = ((Math.Log(fq) / Math.Log(10)) - 1.0f) / 3.0f * g.ClipBounds.Width;

				//real_y = g.ClipBounds.Bottom - (yl[i] * g.ClipBounds.Height);
				real_y = ( (1.0 - yl[i] )/2.0 * g.ClipBounds.Height);

				if (double.IsInfinity(real_x) || double.IsInfinity(real_y)) {
					prev_x = prev_y = float.NaN;
					continue;
				}
				
				if ((!double.IsNaN(real_y)) && (!double.IsNaN(prev_y)) &&
					(!double.IsInfinity(real_y)) && (!double.IsInfinity(prev_y))) {
					g.DrawLine(Pens.White, new PointF((float)prev_x, (float)prev_y), new PointF((float)real_x, (float)real_y));
				}
				prev_x = real_x;
				prev_y = real_y;
			}
		}

		private void SpecAna_FormClosed(object sender, FormClosedEventArgs e) {
			End();
		}

		private void timer1_Tick(object sender, EventArgs e) {
			SpectrumBox.Refresh();
			PitchGraphBox.Refresh();
			CepsBox.Refresh();
		}

	}
}
