using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class ClipInfo {

		public string name;
		public int channels;
		public int frequency;
		public float length;
		public int samples;
		
		[HideInInspector] public AudioInfo audioInfo;
		
		public ClipInfo(Magicolo.AudioTools.AudioInfo audioInfo) {
			this.audioInfo = audioInfo;
		}
		
		public void Update() {
			name = audioInfo.Clip.name;
			channels = audioInfo.Clip.channels;
			frequency = audioInfo.Clip.frequency;
			length = audioInfo.Clip.length / audioInfo.Source.pitch;
			samples = audioInfo.Clip.samples;
		}
	}
}
