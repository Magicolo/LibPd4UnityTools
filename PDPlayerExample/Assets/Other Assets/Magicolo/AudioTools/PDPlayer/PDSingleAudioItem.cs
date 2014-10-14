using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDSingleAudioItem : Magicolo.AudioTools.SingleAudioItem {
		
		public PDPlayer pdPlayer;
		
		public PDSingleAudioItem(string name, int id, AudioSource audioSource, Magicolo.AudioTools.AudioInfo audioInfo, GameObject gameObject, CoroutineHolder coroutineHolder, PDGainManager gainManager, PDAudioItemManager itemManager, PDPlayer pdPlayer)
			: base(name, id, audioSource, audioInfo, gameObject, coroutineHolder, gainManager, itemManager, pdPlayer) {
			
			this.pdPlayer = pdPlayer;
			pdPlayer.communicator.SendValue(Name + "_Volume", Volume);
		}

		public override void UpdateVolume() {
			base.UpdateVolume();
			
			pdPlayer.communicator.SendValue(Name + "_Volume", Mathf.Clamp(Volume, 0, 10));
		}
		
		public override void SetVolume(float targetVolume) {
			base.SetVolume(targetVolume);
			
			pdPlayer.communicator.SendValue(Name + "_Volume", Mathf.Clamp(Volume, 0, 10));
		}
		
		public override IEnumerator FadeVolume(float startVolume, float targetVolume, float time) {
			IEnumerator fade = base.FadeVolume(startVolume, targetVolume, time);
			
			while (fade.MoveNext()) {
				pdPlayer.communicator.SendValue(Name + "_Volume", Volume);
				yield return fade.Current;
			}
		}
	}
}
