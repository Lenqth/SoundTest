namespace dxgtest {
	partial class SpecAna {
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
			this.components = new System.ComponentModel.Container();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.SpectrumBox = new System.Windows.Forms.PictureBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.PitchGraphBox = new System.Windows.Forms.PictureBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.CepsBox = new System.Windows.Forms.PictureBox();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.SpectrumBox)).BeginInit();
			this.tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PitchGraphBox)).BeginInit();
			this.tabPage3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.CepsBox)).BeginInit();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(664, 357);
			this.tabControl1.TabIndex = 1;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.SpectrumBox);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(656, 331);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "スペクトル";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// SpectrumBox
			// 
			this.SpectrumBox.BackColor = System.Drawing.Color.DimGray;
			this.SpectrumBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SpectrumBox.Location = new System.Drawing.Point(3, 3);
			this.SpectrumBox.Name = "SpectrumBox";
			this.SpectrumBox.Size = new System.Drawing.Size(650, 325);
			this.SpectrumBox.TabIndex = 1;
			this.SpectrumBox.TabStop = false;
			this.SpectrumBox.Paint += new System.Windows.Forms.PaintEventHandler(this.SpectrumBox_Paint);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.PitchGraphBox);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(656, 331);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "音程グラフ";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// PitchGraphBox
			// 
			this.PitchGraphBox.BackColor = System.Drawing.Color.DimGray;
			this.PitchGraphBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PitchGraphBox.Location = new System.Drawing.Point(3, 3);
			this.PitchGraphBox.Name = "PitchGraphBox";
			this.PitchGraphBox.Size = new System.Drawing.Size(650, 325);
			this.PitchGraphBox.TabIndex = 0;
			this.PitchGraphBox.TabStop = false;
			this.PitchGraphBox.Paint += new System.Windows.Forms.PaintEventHandler(this.PitchGraphBox_Paint);
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.CepsBox);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(656, 331);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "テスト用";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// CepsBox
			// 
			this.CepsBox.BackColor = System.Drawing.Color.LightBlue;
			this.CepsBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CepsBox.Location = new System.Drawing.Point(0, 0);
			this.CepsBox.Name = "CepsBox";
			this.CepsBox.Size = new System.Drawing.Size(656, 331);
			this.CepsBox.TabIndex = 0;
			this.CepsBox.TabStop = false;
			this.CepsBox.Paint += new System.Windows.Forms.PaintEventHandler(this.CepsBox_Paint);
			// 
			// SpecAna
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(664, 357);
			this.Controls.Add(this.tabControl1);
			this.Name = "SpecAna";
			this.Text = "Form2";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SpecAna_FormClosed);
			this.Load += new System.EventHandler(this.SpecAna_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.SpectrumBox)).EndInit();
			this.tabPage2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.PitchGraphBox)).EndInit();
			this.tabPage3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.CepsBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.PictureBox SpectrumBox;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.PictureBox PitchGraphBox;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.PictureBox CepsBox;
	}
}