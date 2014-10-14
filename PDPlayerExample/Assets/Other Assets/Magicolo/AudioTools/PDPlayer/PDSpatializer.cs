using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDSpatializer {

		public enum RolloffMode {
			Logarithmic,
			Linear
		}
		
		string moduleName;
		public string ModuleName {
			get {
				return moduleName;
			}
			set {
				moduleName = value;
				Spatialize();
			}
		}

		GameObject source;
		public GameObject Source {
			get {
				return source;
			}
			set {
				source = value;
				Spatialize();
			}
		}

		RolloffMode volumeRolloff;
		public RolloffMode VolumeRolloff {
			get {
				return volumeRolloff;
			}
			set {
				volumeRolloff = value;
				Spatialize();
			}
		}

		float minDistance = 1;
		public float MinDistance {
			get {
				return minDistance;
			}
			set {
				minDistance = value;
				Spatialize();
			}
		}

		float maxDistance = 500;
		public float MaxDistance {
			get {
				return maxDistance;
			}
			set {
				maxDistance = value;
				Spatialize();
			}
		}

		float panLevel = 0.75F;
		public float PanLevel {
			get {
				return panLevel;
			}
			set {
				panLevel = value;
				Spatialize();
			}
		}
		
		protected PDPlayer pdPlayer;
		
		public PDSpatializer(string moduleName, GameObject source, PDPlayer pdPlayer) {
			this.moduleName = moduleName;
			this.source = source;
			this.pdPlayer = pdPlayer;
		}

		public PDSpatializer(string moduleName, PDEditorModule editorModule, PDPlayer pdPlayer) {
			this.moduleName = moduleName;
			this.source = editorModule.Source;
			this.volumeRolloff = editorModule.VolumeRolloff;
			this.minDistance = editorModule.MinDistance;
			this.maxDistance = editorModule.MaxDistance;
			this.panLevel = editorModule.PanLevel;
			this.pdPlayer = pdPlayer;
		}
		
		public PDSpatializer(PDEditorModule editorModule, PDPlayer pdPlayer) {
			this.moduleName = editorModule.Name;
			this.source = editorModule.Source;
			this.volumeRolloff = editorModule.VolumeRolloff;
			this.minDistance = editorModule.MinDistance;
			this.maxDistance = editorModule.MaxDistance;
			this.panLevel = editorModule.PanLevel;
			this.pdPlayer = pdPlayer;
		}
		
		public void Initialize(float volume) {
			pdPlayer.communicator.SendValue(ModuleName + "_HRFLeft", 20000);
			pdPlayer.communicator.SendValue(ModuleName + "_HRFRight", 20000);
			pdPlayer.communicator.SendValue(ModuleName + "_PanLeft", 1);
			pdPlayer.communicator.SendValue(ModuleName + "_PanRight", 1);
			pdPlayer.communicator.SendValue(ModuleName + "_Attenuation", 1);
			pdPlayer.communicator.SendValue(ModuleName + "_Volume", volume);
		}
		
		public void Update() {
			if (CheckForChanges()) {
				Spatialize();
			}
		}
		
		public void Spatialize() {
			if (Source != null) {
				const float fullFrequencyRange = 20000;
				const float hrfFactor = 1500;
				const float curveDepth = 3.5F;
			
				Vector3 listenerToSource = Source.transform.position - pdPlayer.listener.transform.position;
				float angle = Vector3.Angle(pdPlayer.listener.transform.right, listenerToSource);
				float panLeft = (1 - PanLevel) + PanLevel * Mathf.Sin(Mathf.Max(180 - angle, 90) * Mathf.Deg2Rad);
				float panRight = (1 - PanLevel) + PanLevel * Mathf.Sin(Mathf.Max(angle, 90) * Mathf.Deg2Rad);
				
				float behindFactor = 1 + 4 * (Mathf.Clamp(Vector3.Angle(listenerToSource, pdPlayer.listener.transform.forward), 90, 135) - 90) / (135 - 90);
				float hrfLeft = Mathf.Pow(panLeft, 2) * (fullFrequencyRange - hrfFactor) / behindFactor + hrfFactor;
				float hrfRight = Mathf.Pow(panRight, 2) * (fullFrequencyRange - hrfFactor) / behindFactor + hrfFactor;
				float distance = Vector3.Distance(Source.transform.position, pdPlayer.listener.transform.position);
				float adjustedDistance = Mathf.Clamp01(Mathf.Max(distance - MinDistance, 0) / Mathf.Max(MaxDistance - MinDistance, 0.001F));
				
				float attenuation;
				if (VolumeRolloff == RolloffMode.Linear) {
					attenuation = 1F - adjustedDistance;
				}
				else {
					attenuation = Mathf.Pow((1F - Mathf.Pow(adjustedDistance, 1F / curveDepth)), curveDepth);
				}
			
				pdPlayer.communicator.SendValue(ModuleName + "_HRFLeft", hrfLeft);
				pdPlayer.communicator.SendValue(ModuleName + "_HRFRight", hrfRight);
				pdPlayer.communicator.SendValue(ModuleName + "_PanLeft", panLeft);
				pdPlayer.communicator.SendValue(ModuleName + "_PanRight", panRight);
				pdPlayer.communicator.SendValue(ModuleName + "_Attenuation", attenuation);
				pdPlayer.communicator.SendValue(ModuleName + "_SourcePosition", Source.transform.position);
				pdPlayer.communicator.SendValue(ModuleName + "_ListenerAngle", angle);
				pdPlayer.communicator.SendValue(ModuleName + "_ListenerDistance", distance);
			}
		}
	
		public void SetVolume(float volume) {
			pdPlayer.communicator.SendValue(ModuleName + "_Volume", volume);
		}

		public bool CheckForChanges() {
			bool changed = false;
			
			if (Source != null && (Source.transform.hasChanged || pdPlayer.listener.transform.hasChanged)) {
				changed = true;
				pdPlayer.SetTransformHasChanged(Source.transform, false);
				pdPlayer.SetTransformHasChanged(pdPlayer.listener.transform, false);
			}
			
			return changed;
		}
	}
}
