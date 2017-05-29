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
		public SpecAna() {
			InitializeComponent();
		}

		WaveIn wi = null;
		float[] recorded;
		float[] spectrum;

		const int FREQ_GRAPH_SIZE = 400;
		float[] freq_graph = new float[FREQ_GRAPH_SIZE];
		int freq_graph_pos = 0;
		float THRESHOLD = 0.001f;

		List<Complex32> _buffer = new List<Complex32>();

		static int SAMPLE_RATE = 8000;
		static int BUFFER_SIZE = 2000;
		float UNIT_TIME = 1.0f;

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
			var fourier_v = _buffer.ToArray();
			Fourier.Forward(fourier_v);
			spectrum = fourier_v.Take(fourier_v.Length / 2).Select(x => (float)x.Norm()).ToArray();
			List<KeyValuePair<float, float>> peaks = new List<KeyValuePair<float, float>>();
			float prev = -1.0f;
			bool prev_up = true;
			float max_fq = 0.0f;
			float max_v = 0.0f;

			for (int i = 0; i < spectrum.Length; i++) {
				float v = spectrum[i];
				float fq = i / UNIT_TIME;
				if (fq < 95.0) continue;
				bool up = false;
				if (v > prev) {
					up = true;
				}
				if ( (!up) && prev_up) {
					peaks.Add(new KeyValuePair<float, float>(fq, v));
				}
				if (max_v < v) {
					max_fq = fq;
					max_v = v;
				}
				prev = v;
				prev_up = up;
			}
			Console.WriteLine(String.Format("{0,12:F4},{1,12:F4}", max_fq, max_v));
			if (max_v > THRESHOLD) {
				freq_graph[freq_graph_pos++] = max_fq;
			} else {
				freq_graph[freq_graph_pos++] = float.NaN;
			}
			if (freq_graph_pos >= FREQ_GRAPH_SIZE) freq_graph_pos = 0;
        }
		public void _DataAvailable(object sender , WaveInEventArgs e) {
			var buff = e.Buffer;
            Console.WriteLine(e.BytesRecorded);
			recorded = new float[(buff.Length / 2)];
			int REC_LEN = recorded.Length;
			int NUMCHUNKS = 1;
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
				for (int k = 0; k < chunk_len; k++,i++) {
					short val = (short)((buff[2 * i + 1] << 8) | buff[2 * i]);
					recorded[i] = val / 65536.0f; // (val - 32768.0f) / 32768.0f ;
					_buffer.Add(new Complex32(recorded[i], 0));
				}

				this.RecalcSpectrum();
			}
		}

		private void SpecAna_Load(object sender, EventArgs e) {
			Init();
		}

		private void SpectrumBox_Paint(object sender, PaintEventArgs e) {
			if(recorded != null) Curve_Spec(e.Graphics,spectrum);
        }

		private void PitchGraphBox_Paint(object sender, PaintEventArgs e) {
			Graph painter = new Graph(PitchGraphBox.Width, PitchGraphBox.Height);
			painter.InitDraw();
			painter.Clear();
			painter.Curve(freq_graph);
			painter.EndDraw(e.Graphics);
		}

		public void Curve_Spec(Graphics g, float[] yl) {
			float prev_x = float.NaN;
			float prev_y = float.NaN;
			float real_x, real_y;
			if (UNIT_TIME == 0) return;
			for (int i = 1; i < yl.Length; i++) {
				//real_x = (float) i / yl.Length * g.ClipBounds.Width; 
				float fq = i / UNIT_TIME;
				real_x =  (float) (( Math.Log(fq) / Math.Log(10))-1.0f) / 3.0f  * g.ClipBounds.Width ;

				real_y = g.ClipBounds.Bottom - (yl[i] * g.ClipBounds.Height);
				real_y = (float) - ( Math.Log10(yl[i]) / 5.0f * g.ClipBounds.Height);

				if ((!float.IsNaN(prev_x)) && (!float.IsNaN(prev_y))) {
					g.DrawLine(Pens.White, new PointF(prev_x, prev_y), new PointF(real_x, real_y));
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
		}

	}
}
