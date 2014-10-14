#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	public static class CustomMenus {
	
		[MenuItem("Magicolo's Tools/Create/Audio Player")]
		static void CreateAudioPlayer() {
			GameObject gameObject;
			AudioPlayer existingAudioPlayer = Object.FindObjectOfType<AudioPlayer>();
		
			if (existingAudioPlayer == null) {
				gameObject = new GameObject();
				gameObject.name = "AudioPlayer";
				gameObject.AddComponent<AudioPlayer>();
			}
			else {
				gameObject = existingAudioPlayer.gameObject;
			}
			Selection.activeGameObject = gameObject;
		}
	}
}
#endif
