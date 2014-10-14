using UnityEngine;
using System.Collections;
using Candlelight;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class GeneralAudioSettings : IStartable {

		[PropertyBackingField(typeof(GeneralAudioSettings), "MasterVolume", typeof(RangeAttribute), 0f, 1f)]
		public float masterVolume = 1;
		public float MasterVolume {
			get {
				return masterVolume;
			}
			set {
				masterVolume = value;
				if (Application.isPlaying) {
					if (audioPlayer != null) {
						audioPlayer.itemManager.SetMasterVolume(masterVolume);
					}
					if (pdPlayer != null) {
						pdPlayer.itemManager.SetMasterVolume(masterVolume);
					}
				}
			}
		}
		
		[Range(1, 64)] public int maxVoices = 32;
		
		public AudioPlayer audioPlayer;
		public PDPlayer pdPlayer;
		public AudioHierarchyEditorHelper editorHelper;
		
		public GeneralAudioSettings(AudioPlayer audioPlayer) {
			this.audioPlayer = audioPlayer;
		}
		
		public void Start() {
			editorHelper = editorHelper ?? new AudioHierarchyEditorHelper();
			editorHelper.Subscribe();
			pdPlayer = Object.FindObjectOfType<PDPlayer>();
		}
		
		public void Update() {
			#if UNITY_EDITOR
			editorHelper.Update();
			#endif
		}
	}
}
