using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace dxgtest {
	class Pitches {
		int hiA = 69; // 440hz
	}
	class Graph {

		Image backgimg;
		Graphics g;

		public Graph(int width,int height){
			this.backgimg = new Bitmap(width, height);
		}

		public void InitDraw() {
			g = Graphics.FromImage(backgimg);
			g.SetClip(new RectangleF(PointF.Empty , backgimg.Size ));
		}

		public void EndDraw(Graphics winG) {
//			winG.DrawImage(backgimg, winG.ClipBounds);
            winG.DrawImage(backgimg, winG.ClipBounds , new RectangleF(new PointF(0,0),backgimg.Size) , GraphicsUnit.Pixel );
		}


		float offset_x = 0.0f;
		int offset_y = 49;

		/// <summary>
		/// true :無印式
		/// false:球場式
		/// </summary>
		bool oldstyle = false;


		static float scale_x = 15;
		static float scale_y = 12;

		static public float freq2pitch(float freq) {
			if (freq == 0) return float.NaN;
			return (float)(Math.Log(freq,2.0) - Math.Log(440.0f, 2.0) ) * 12.0f + 69.0f;
		}

		static public float pitch2freq(float pitch) {
			return 440.0f * (float) Math.Pow( 2.0f , (pitch-69.0f)/12.0f);
		}

		public void Bar(int level, float x = 0.0f, float len = 1.0f) {
			
		}
		public void Curve(float[] freq, int index = 0, int length = -1 , float x_first = 0.0f ,float sample_interval = 0.1f ) {
			if (length == -1) length = freq.Length;
			float prev_x = float.NaN;
			float prev_y = float.NaN;
			float real_x,real_y;
            for (int i= index ; i < index + length; i++) {
				float pitch = freq2pitch(freq[i]);
				if (float.IsNaN(pitch)) {
					real_x = real_y = float.NaN;
                } else {
					real_x = (x_first + sample_interval * i - this.offset_x) * scale_x;
					real_y = g.ClipBounds.Bottom - ( (pitch - this.offset_y) * scale_y) ;
					if ((!float.IsNaN(prev_x)) && (!float.IsNaN(prev_y))) {
						g.DrawLine(Pens.YellowGreen, new PointF(prev_x, prev_y), new PointF(real_x, real_y));
					}
				}
				prev_x = real_x;
				prev_y = real_y;
			}
		}

		public void Clear() {
			g.Clear(System.Drawing.Color.DimGray);
			var centre_format = new StringFormat();
			centre_format.LineAlignment = StringAlignment.Center;
			centre_format.Alignment = StringAlignment.Center;

			for (int i = 40; i < 99; i++) {
				g.DrawLine(Pens.LightGray,
					new PointF(0, g.ClipBounds.Bottom - ((i - this.offset_y) * scale_y)),
					new PointF(g.ClipBounds.Right, g.ClipBounds.Bottom - ((i - this.offset_y) * scale_y))
                    );
				g.DrawString(i.ToString(), new Font(FontFamily.GenericMonospace,8), Brushes.Red,
					new PointF(10 + (i % 4) * 5, g.ClipBounds.Bottom - ((i - this.offset_y) * scale_y)  ),centre_format) ;
			}
			/*			float[] testdata = new float[210];

						for (int i = 0; i < 100; i++) {
							testdata[i] = (float)(261.625 + i);
						}
						for (int i = 0; i < 100; i++) {
							testdata[i+110] = (float)(261.625 + i);
						}
			*/
		}


	}
}
