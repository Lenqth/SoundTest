using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundTester {
	public partial class MidiPiano : Form {
		public MidiPiano() {
			InitializeComponent();
		}
		MidiLoader mid;
		private void MidiPiano_Load(object sender, EventArgs e) {
			mid = new MidiLoader();
			mid.player.ChannelMessagePlayed += HandleChannelMessagePlayed;
        }

		private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e) {
			pianoControl1.Send(e.Message);
		}

		private void MidiPiano_FormClosed(object sender, FormClosedEventArgs e) {
			mid.Dispose();
		}
	}
}
