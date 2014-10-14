using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;
using Candlelight;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDEditorModule : Magicolo.GeneralTools.INamable {
		
		[SerializeField]
		string name;
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		AudioItem.States state = AudioItem.States.StandingBy;
		public AudioItem.States State {
			get {
				return Application.isPlaying ? pdPlayer.itemManager.GetModule(name).State : state;
			}
		}
		
		[SerializeField, PropertyBackingField(typeof(PDEditorModule), "Volume", typeof(RangeAttribute), 0F, 5F)]
		float volume = 1;
		public float Volume {
			get {
				return volume;
			}
			set {
				volume = value;
				if (Application.isPlaying) {
					pdPlayer.itemManager.SetVolume(name, volume);
				}
			}
		}

		[SerializeField, PropertyBackingField(typeof(PDEditorModule), "Source")]
		GameObject source;
		public GameObject Source {
			get {
				return source;
			}
			set {
				source = value;
				if (Application.isPlaying) {
					pdPlayer.itemManager.GetModule(name).spatializer.Source = source;
				}
			}
		}

		[SerializeField, PropertyBackingField(typeof(PDEditorModule), "VolumeRolloff")]
		PDSpatializer.RolloffMode volumeRolloff;
		public PDSpatializer.RolloffMode VolumeRolloff {
			get {
				return volumeRolloff;
			}
			set {
				volumeRolloff = value;
				if (Application.isPlaying) {
					pdPlayer.itemManager.GetModule(name).spatializer.VolumeRolloff = volumeRolloff;
				}
			}
		}

		[SerializeField, PropertyBackingField(typeof(PDEditorModule), "MinDistance", typeof(MinAttribute))]
		float minDistance = 1;
		public float MinDistance {
			get {
				return minDistance;
			}
			set {
				minDistance = value;
				if (Application.isPlaying) {
					pdPlayer.itemManager.GetModule(name).spatializer.MinDistance = minDistance;
				}
			}
		}

		[SerializeField, PropertyBackingField(typeof(PDEditorModule), "MaxDistance", typeof(MinAttribute))]
		float maxDistance = 500;
		public float MaxDistance {
			get {
				return maxDistance;
			}
			set {
				maxDistance = value;
				if (Application.isPlaying) {
					pdPlayer.itemManager.GetModule(name).spatializer.MaxDistance = maxDistance;
				}
			}
		}

		[SerializeField, PropertyBackingField(typeof(PDEditorModule), "PanLevel", typeof(RangeAttribute), 0F, 1F)]
		float panLevel = 1;
		public float PanLevel {
			get {
				return panLevel;
			}
			set {
				panLevel = value;
				if (Application.isPlaying) {
					pdPlayer.itemManager.GetModule(name).spatializer.PanLevel = panLevel;
				}
			}
		}

		public PDPlayer pdPlayer;
		public bool spatializerShowing = true;
		
		public PDEditorModule(string name, PDPlayer pdPlayer) {
			this.name = name;
			this.pdPlayer = pdPlayer;
			
			volume = 1;
			minDistance = 1;
			maxDistance = 500;
			panLevel = 1;
		}
		
		public PDEditorModule(PDModule module, PDPlayer pdPlayer) {
			this.name = module.Name;
			this.volume = module.Volume;
			this.source = module.spatializer.Source;
			this.volumeRolloff = module.spatializer.VolumeRolloff;
			this.minDistance = module.spatializer.MinDistance;
			this.maxDistance = module.spatializer.MaxDistance;
			this.panLevel = module.spatializer.PanLevel;
			this.pdPlayer = pdPlayer;
		}
		
		public PDEditorModule(string name, PDEditorModule editorModule, PDPlayer pdPlayer) {
			this.name = name;
			this.volume = editorModule.Volume;
			this.source = editorModule.Source;
			this.volumeRolloff = editorModule.VolumeRolloff;
			this.minDistance = editorModule.MinDistance;
			this.maxDistance = editorModule.MaxDistance;
			this.panLevel = editorModule.PanLevel;
			this.pdPlayer = pdPlayer;
		}
	
		public PDEditorModule(string name, AudioItem.States state, float volume, GameObject source, PDSpatializer.RolloffMode volumeRolloff, float minDistance, float maxDistance, float panLevel) {
			this.name = name;
			this.state = state;
			this.volume = volume;
			this.source = source;
			this.volumeRolloff = volumeRolloff;
			this.minDistance = minDistance;
			this.maxDistance = maxDistance;
			this.panLevel = panLevel;
		}
	
		public PDEditorModule() {
		}
	}
}
