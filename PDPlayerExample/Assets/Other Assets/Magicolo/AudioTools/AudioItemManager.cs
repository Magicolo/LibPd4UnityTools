using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioItemManager {
		
		public List<SingleAudioItem> activeAudioItems = new List<SingleAudioItem>();
		public List<GameObject> activeAudioObjects = new List<GameObject>();
		public List<GameObject> inactiveAudioObjects = new List<GameObject>();
		
		public int idCounter;
		public AudioListener listener;
		public Magicolo.AudioTools.AudioHierarchyManager infoManager;
		public Magicolo.AudioTools.Player player;

		public AudioItemManager(AudioListener listener, Magicolo.AudioTools.AudioHierarchyManager infoManager, Magicolo.AudioTools.Player player) {
			this.listener = listener;
			this.infoManager = infoManager;
			this.player = player;
		}
		
		public virtual void Update() {
			foreach (AudioItem audioItem in activeAudioItems.ToArray()) {
				audioItem.Update();
			}
		}
		
		public virtual void UpdateVolume() {
			foreach (SingleAudioItem audioItem in activeAudioItems) {
				audioItem.UpdateVolume();
			}
		}
		
		public virtual void Deactivate(SingleAudioItem audioItem) {
			activeAudioItems.Remove(audioItem);
			activeAudioObjects.Remove(audioItem.gameObject);
			inactiveAudioObjects.Add(audioItem.gameObject);
			audioItem.gameObject.transform.parent = player.transform;
			audioItem.gameObject.SetActive(false);
		}
		
		public virtual void SetMasterVolume(float targetVolume, float time) {
			player.coroutineHolder.RemoveCoroutines("FadeMasterVolume");
			player.coroutineHolder.AddCoroutine("FadeMasterVolume", FadeMasterVolume(player.audioSettings.masterVolume, targetVolume, time));
		}
		
		public virtual void SetMasterVolume(float targetVolume) {
			player.coroutineHolder.RemoveCoroutines("FadeMasterVolume");
			player.audioSettings.masterVolume = targetVolume;
			UpdateVolume();
		}
		
		public void LimitVoices() {
			if (activeAudioItems.Count > player.audioSettings.maxVoices) {
				foreach (SingleAudioItem audioItem in activeAudioItems.ToArray()) {
					if (!audioItem.audioInfo.doNotKill) {
						audioItem.StopImmediate();
						
						if (activeAudioItems.Count <= player.audioSettings.maxVoices) {
							break;
						}
					}
				}
			}
		}
		
		public AudioSource GetAudioSource(Magicolo.AudioTools.AudioInfo audioInfo, GameObject source) {
			GameObject gameObject = GetGameObject(source);
			return SetAudioSource(gameObject.GetOrAddComponent<AudioSource>(), audioInfo);
		}
		
		public GameObject GetGameObject(GameObject source) {
			GameObject gameObject;
			
			gameObject = inactiveAudioObjects.Count == 0 ? new GameObject() : inactiveAudioObjects.Pop();
			gameObject.transform.parent = source == null ? listener.transform : source.transform;
			gameObject.transform.Reset();
			gameObject.SetActive(true);
			activeAudioObjects.Add(gameObject);
			
			return gameObject;
		}
	
		public AudioSource SetAudioSource(AudioSource audioSource, Magicolo.AudioTools.AudioInfo audioInfo) {
			audioSource.Copy(audioInfo.Source);
			audioSource.volume += Random.Range(-audioInfo.randomVolume, audioInfo.randomVolume);
			audioSource.pitch += Random.Range(-audioInfo.randomPitch, audioInfo.randomPitch);
			return audioSource;
		}
	
		public virtual IEnumerator FadeMasterVolume(float startVolume, float targetVolume, float time) {
			float counter = 0;
			
			while (counter < time) {
				player.audioSettings.masterVolume = (counter / time) * (targetVolume - startVolume) + startVolume;
				counter += Time.deltaTime;
				UpdateVolume();
				yield return new WaitForSeconds(0);
			}
			
			player.audioSettings.masterVolume = targetVolume;
		}
	}
}
