using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class MultipleAudioItem : AudioItem {
		
		public List<AudioItem> audioItems = new List<AudioItem>();
		
		public MultipleAudioItem(string name, int id, AudioItemManager itemManager, Magicolo.AudioTools.Player player)
			: base(name, id, itemManager, player) {
		}
		
		public override void Update() {
			UpdateAudioItems();
			
			if (RemoveStoppedAudioItems() && State != States.Stopped) {
				Stop();
			}
		}

		public virtual void UpdateAudioItems() {
			foreach (AudioItem audioItem in audioItems) {
				audioItem.Update();
			}
		}
		
		public virtual void UpdateVolume() {
			foreach (AudioItem audioItem in audioItems) {
				audioItem.SetVolume(Volume);
			}
		}
		
		public virtual void AddAudioItem(AudioItem audioItem) {
			audioItems.Add(audioItem);
			UpdateVolume();
		}

		public virtual bool RemoveStoppedAudioItems() {
			bool allStopped = true;
			
			foreach (AudioItem audioItem in audioItems.ToArray()) {
				if (audioItem != null) {
					if (audioItem.State == States.Stopped) {
						audioItems.Remove(audioItem);
					}
					else {
						allStopped = false;
					}
				}
			}
			return allStopped;
		}
		
		public override void Play() {
			base.Play();
				
			foreach (AudioItem audioItem in audioItems) {
				audioItem.Play();
			}
		}

		public override void Pause() {
			base.Pause();
				
			foreach (AudioItem audioItem in audioItems) {
				audioItem.Pause();
			}
		}

		public override void Stop() {
			base.Stop();
				
			foreach (AudioItem audioItem in audioItems) {
				audioItem.Stop();
			}
		}

		public override void StopImmediate() {
			base.StopImmediate();
			
			foreach (AudioItem audioItem in audioItems) {
				audioItem.StopImmediate();
			}
		}
		
		public override void SetVolume(float targetVolume, float time) {
			player.coroutineHolder.RemoveCoroutines("FadeVolume" + Name + Id);
			player.coroutineHolder.AddCoroutine("FadeVolume" + Name + Id, FadeVolume(Volume, targetVolume, time));
		}
		
		public override void SetVolume(float targetVolume) {
			player.coroutineHolder.RemoveCoroutines("FadeVolume" + Name + Id);
			Volume = targetVolume;
			UpdateVolume();
		}
		
		public virtual IEnumerator FadeVolume(float startVolume, float targetVolume, float time) {
			float counter = 0;
			
			while (counter < time) {
				Volume = (counter / time) * (targetVolume - startVolume) + startVolume;
				UpdateVolume();
				counter += Time.deltaTime;
				yield return new WaitForSeconds(0);
			}
			
			Volume = targetVolume;
			UpdateVolume();
		}
	}
}
