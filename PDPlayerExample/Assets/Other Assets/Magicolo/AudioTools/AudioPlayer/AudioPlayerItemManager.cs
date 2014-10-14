using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioPlayerItemManager : AudioItemManager {
		
		public AudioPlayer audioPlayer;
		
		public AudioPlayerItemManager(AudioPlayer audioPlayer)
			: base(audioPlayer.listener, audioPlayer.infoManager, audioPlayer) {
			
			this.audioPlayer = audioPlayer;
		}

		public AudioItem Play(string soundName, GameObject source = null) {
			AudioItem audioItem = GetAudioItem(soundName, source);
			LimitVoices();
			audioItem.Play();
			return audioItem;
		}
		
		public AudioItem GetAudioItem(string soundName, GameObject source = null) {
			Magicolo.AudioTools.AudioInfo audioInfo = infoManager.GetAudioInfo(soundName);
			AudioSource audioSource = GetAudioSource(audioInfo, source);
			
			GameObject gameObject = audioSource.gameObject;
			CoroutineHolder coroutineHolder = gameObject.GetOrAddComponent<CoroutineHolder>();
			
			GainManager gainManager = gameObject.GetOrAddComponent<GainManager>();
			idCounter += 1;
			SingleAudioItem audioItem = new SingleAudioItem(soundName, idCounter, audioSource, audioInfo, gameObject, coroutineHolder, gainManager, this, audioPlayer);
			
			gainManager.Initialize(audioItem, audioPlayer);
			activeAudioItems.Add(audioItem);
			
			return audioItem;
		}
	}
}
