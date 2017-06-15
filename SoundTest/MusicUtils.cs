using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundTester {
	class MusicUtils {
		static public float freq2pitch(float freq) {
			if (freq == 0) return float.NaN;
			return (float)(Math.Log(freq, 2.0) - Math.Log(440.0f, 2.0)) * 12.0f + 69.0f;
		}

		static public float pitch2freq(float pitch) {
			return 440.0f * (float)Math.Pow(2.0f, (pitch - 69.0f) / 12.0f);
		}

	}
}
