using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanford.Multimedia.Midi;
namespace SoundTester {
	class MidiLoader : IDisposable {
		
		OutputDevice midi_out;
		MultiSynth synth_out;
		Sequence seq;
		public Sequencer player;



		public MidiLoader() {
			seq = new Sequence("tester.mid");
			midi_out = new OutputDevice(0);
			synth_out = new MultiSynth();
			player = new Sequencer();
			player.Sequence = seq;
			player.ChannelMessagePlayed += HandleChannelMessagePlayed;
			synth_out.OnTick += Synth_out_OnTick;
			synth_out.Play();
			player.Start();
		}

		private void Synth_out_OnTick(object sender, EventArgs e) {
		}

		private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e) {
			if (e.Message.Command == ChannelCommand.NoteOn) {
				synth_out.NoteOn(e.Message.MidiChannel,new Note(e.Message.Data1, e.Message.Data2));
			} else if (e.Message.Command == ChannelCommand.NoteOff) {
				synth_out.NoteOff(e.Message.MidiChannel, new Note(e.Message.Data1, e.Message.Data2));
			} else if (e.Message.Command == ChannelCommand.ProgramChange) {
				synth_out.ProgramChange(e.Message.MidiChannel, e.Message.Data1);

			}
			// midi_out.Send(e.Message);
		}

		#region IDisposable Support
		private bool disposedValue = false; // 重複する呼び出しを検出するには

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					// TODO: マネージ状態を破棄します (マネージ オブジェクト)。
				}
				if(player != null)	player.Stop();
				if (midi_out != null) midi_out.Close();
				if (synth_out != null) synth_out.Stop();
				// TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
				// TODO: 大きなフィールドを null に設定します。

				disposedValue = true;
			}
		}

		// TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
		// ~MidiLoader() {
		//   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
		//   Dispose(false);
		// }

		// このコードは、破棄可能なパターンを正しく実装できるように追加されました。
		public void Dispose() {
			// このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
			Dispose(true);
			// TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
			// GC.SuppressFinalize(this);
		}
		#endregion

	}
}
