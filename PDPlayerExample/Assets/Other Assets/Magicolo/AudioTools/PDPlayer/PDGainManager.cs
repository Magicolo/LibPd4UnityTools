using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	public class PDGainManager : Magicolo.AudioTools.GainManager {

		[HideInInspector] public int index;
		[HideInInspector] public int voice;
		[HideInInspector] public int offset;
		[HideInInspector] public PDPlayer pdPlayer;
		
		public static int indexCounter;
		public static int voiceCounter;
		public static Dictionary<int, PDSingleAudioItem> indexAudioItem = new Dictionary<int, PDSingleAudioItem>();
		public static Dictionary<string, int> soundNameVoice = new Dictionary<string, int>();
		
		public virtual void Initialize(PDSingleAudioItem audioItem, PDPlayer pdPlayer) {
			base.Initialize(audioItem, pdPlayer);
			
			this.pdPlayer = pdPlayer;
			
			index = GetUnusedIndex();
			if (index == -1) {
				Debug.LogWarning(string.Format("No available voice for audio item {0} of id {1}.", audioItem.Name, audioItem.Id));
				audioItem.StopImmediate();
				return;
			}
			indexAudioItem[index] = audioItem;
			
			if (soundNameVoice.ContainsKey(audioItem.Name)) {
				voice = soundNameVoice[audioItem.Name];
			}
			else {
				voice = voiceCounter;
				soundNameVoice[audioItem.Name] = voice;
				voiceCounter += 1;
				voiceCounter %= pdPlayer.audioSettings.maxVoices;
			}
			
			offset = index * pdPlayer.bridge.bufferSize * 2;
		}
		
		public override void Activate() {
			base.Activate();
			
			pdPlayer.communicator.SendValue("UVoice" + index + "Switch", 1);
			pdPlayer.communicator.SendValue("UVoice" + index, voice);
			pdPlayer.communicator.SendValue(audioItem.Name + "_Voice", voice);
		}
		
		public override void Deactivate() {
			base.Deactivate();
			
			indexAudioItem.Remove(index);
			pdPlayer.communicator.SendValue("UVoice" + index + "Switch", 0);
			pdPlayer.communicator.SendValue("UVoice" + index, -1);
		}

		public int GetUnusedIndex() {
			for (int i = 0; i < pdPlayer.audioSettings.maxVoices; i++) {
				indexCounter += 1;
				indexCounter %= pdPlayer.audioSettings.maxVoices;
				if (indexAudioItem.ContainsKey(indexCounter) && indexAudioItem[indexCounter].audioInfo.doNotKill) {
					continue;
				}
				return indexCounter;
			}
			return -1;
		}
		
		public override void OnAudioFilterRead(float[] data, int channels) {
			if (pdPlayer.filterRead.initialized) {
				data.CopyTo(PDAudioFilterRead.dataSum, offset);
				data.Clear();
			}
		}
	}
}
