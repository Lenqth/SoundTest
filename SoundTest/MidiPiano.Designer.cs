namespace SoundTester {
	partial class MidiPiano {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.pianoControl1 = new Sanford.Multimedia.Midi.UI.PianoControl();
			this.SuspendLayout();
			// 
			// pianoControl1
			// 
			this.pianoControl1.HighNoteID = 109;
			this.pianoControl1.Location = new System.Drawing.Point(12, 12);
			this.pianoControl1.LowNoteID = 21;
			this.pianoControl1.Name = "pianoControl1";
			this.pianoControl1.NoteOnColor = System.Drawing.Color.SkyBlue;
			this.pianoControl1.Size = new System.Drawing.Size(686, 63);
			this.pianoControl1.TabIndex = 0;
			this.pianoControl1.Text = "pianoControl1";
			// 
			// MidiPiano
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(710, 331);
			this.Controls.Add(this.pianoControl1);
			this.Name = "MidiPiano";
			this.Text = "MidiPiano";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MidiPiano_FormClosed);
			this.Load += new System.EventHandler(this.MidiPiano_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private Sanford.Multimedia.Midi.UI.PianoControl pianoControl1;
	}
}