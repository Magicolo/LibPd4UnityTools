using System.IO;
using UnityEngine;
using System.Collections;
using Candlelight;

namespace Magicolo.AudioTools {
	public class Player : MonoBehaviour {
		
		public string folderPath;
		public AudioPlayer audioPlayer;
		public GeneralAudioSettings audioSettings;
		public AudioHierarchyManager infoManager;
		public CoroutineHolder coroutineHolder;
		public AudioListener listener;

		protected virtual void Awake() {
			if (Application.isPlaying) {
				audioPlayer = gameObject.GetOrAddComponent<AudioPlayer>();
				audioSettings = audioPlayer.audioSettings;
				infoManager = audioPlayer.hierarchyManager;
				coroutineHolder = gameObject.GetOrAddComponent<CoroutineHolder>();
				listener = FindObjectOfType<AudioListener>();
				if (listener == null) {
					GameObject newListener = new GameObject("Listener");
					listener = newListener.AddComponent<AudioListener>();
					listener.transform.Reset();
					Debug.LogWarning("No listener was found in the scene. One was automatically created.");
				}
			}
		}
		
		protected virtual void Start() {
			if (!Application.isPlaying) {
				if (FindObjectsOfType(GetType()).Length > 1) {
					Debug.LogError(string.Format("There can only be one {0}.", GetType().Name));
					this.Remove();
				}
			}
		}
		
		protected virtual void Update() {
			if (!Application.isPlaying) {
				folderPath = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + Path.AltDirectorySeparatorChar + HelperFunctions.GetFolderPath("Magicolo" + Path.AltDirectorySeparatorChar + "AudioTools") + Path.AltDirectorySeparatorChar;
			}
		}
	}
}
