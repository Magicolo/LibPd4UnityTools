
namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDModule : Magicolo.AudioTools.MultipleAudioItem {

		public PDSpatializer spatializer;
		public PDPlayer pdPlayer;
		
		public PDModule(string name, int id, PDSpatializer spatializer, PDAudioItemManager itemManager, PDPlayer pdPlayer)
			: base(name, id, itemManager, pdPlayer) {
			this.spatializer = spatializer;
			this.pdPlayer = pdPlayer;
			
			Initialize();
		}

		public PDModule(string name, int id, PDEditorModule editorModule, PDAudioItemManager itemManager, PDPlayer pdPlayer)
			: base(name, id, itemManager, pdPlayer) {
			this.Volume = editorModule.Volume;
			this.spatializer = new PDSpatializer(name, editorModule, pdPlayer);
			this.pdPlayer = pdPlayer;
			
			Initialize();
		}
		
		public PDModule(int id, PDEditorModule editorModule, PDAudioItemManager itemManager, PDPlayer pdPlayer)
			: base(editorModule.Name, id, itemManager, pdPlayer) {
			this.Volume = editorModule.Volume;
			this.spatializer = new PDSpatializer(editorModule, pdPlayer);
			this.pdPlayer = pdPlayer;
			
			Initialize();
		}
		
		public void Initialize() {
			spatializer.Initialize(Volume * player.audioSettings.masterVolume);
		}
		
		public override void Update() {
			UpdateAudioItems();
			RemoveStoppedAudioItems();
			
			if (State == States.Playing) {
				spatializer.Update();
			}
		}

		public override void UpdateVolume() {
			base.UpdateVolume();
			
			spatializer.SetVolume(Volume * player.audioSettings.masterVolume);
		}
		
		public override void Play() {
			base.Play();
			
			pdPlayer.communicator.SendValue(Name + "_Play", 1);
		}
		
		public override void Pause() {
			base.Pause();
			
			pdPlayer.communicator.SendValue(Name + "_Pause", 0);
		}
	
		public override void Stop() {
			base.Stop();
			
			pdPlayer.communicator.SendValue(Name + "_Stop", 0);
		}
		
		public override void StopImmediate() {
			base.StopImmediate();
			
			pdPlayer.communicator.SendValue(Name + "_Stop", 0);
		}
	}
}
