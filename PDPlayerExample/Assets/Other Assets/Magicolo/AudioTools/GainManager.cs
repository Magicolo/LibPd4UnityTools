using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	public class GainManager : MonoBehaviour {

		[Min] public float volume = 1;
		
		[HideInInspector] public SingleAudioItem audioItem;
		[HideInInspector] public Magicolo.AudioTools.Player player;
	
		public virtual void Initialize(SingleAudioItem audioItem, Magicolo.AudioTools.Player player){
			this.audioItem = audioItem;
			this.player = player;
			
			volume = audioItem.Volume * player.audioSettings.masterVolume;
		}
		
		public virtual void Activate(){
			enabled = true;
		}
		
		public virtual void Deactivate() {
			enabled = false;
		}
		
		public virtual void OnAudioFilterRead(float[] data, int channels) {
			for (int i = 0; i < data.Length; i++) {
				data[i] *= volume;
			}
		}
	}
}
