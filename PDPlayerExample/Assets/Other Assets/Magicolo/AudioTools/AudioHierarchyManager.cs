using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioHierarchyManager : IStartable {

		public string audioClipsPath;
		
		public AudioSource[] audioSources = new AudioSource[0];
		public AudioClip[] currentAudioClips = new AudioClip[0];
		public AudioClip[] audioClips = new AudioClip[0];
		public List<GameObject> folderStructure = new List<GameObject>();
		
		public AudioPlayer audioPlayer;
		public GeneralAudioSettings audioSettings;

		Dictionary<string, Magicolo.AudioTools.AudioInfo> audioInfos;
		
		public AudioHierarchyManager(AudioPlayer audioPlayer) {
			this.audioPlayer = audioPlayer;
			audioSettings = audioPlayer.audioSettings;
		}
		
		public void Start() {
			if (Application.isPlaying) {
				BuildAudioInfoDict();
				audioPlayer.SetChildrenActive(false);
			}
		}
		
		public void Update() {
			UpdateHierarchy();
		}

		void UpdateHierarchy() {
			SetCurrentAudioClips();
			CreateHierarchy();
			EnsureUniqueNames();
			RemoveEmptyFolders();
			FreezeHierarchy();
			audioPlayer.gameObject.SortChildrenRecursive();
		}

		void CreateHierarchy() {
			foreach (AudioClip audioClip in audioClips) {
				string audioClipPath = UnityEditor.AssetDatabase.GetAssetPath(audioClip).TrimStart(("Assets/Resources/" + audioClipsPath).ToCharArray());
				string audioClipDirectory = Path.GetDirectoryName(audioClipPath);
				GameObject parent = GetOrAddFolder(audioClipDirectory);
				GameObject child = audioPlayer.gameObject.FindChildRecursive(audioClip.name);
				if (child == null) {
					child = new GameObject(audioClip.name);
					Magicolo.AudioTools.AudioInfo audioInfo = child.GetOrAddComponent<Magicolo.AudioTools.AudioInfo>();
					audioInfo.Clip = audioClip;
				}
				child.transform.parent = parent.transform;
				child.transform.Reset();
			}
		}
		
		GameObject GetOrAddFolder(string directory) {
			string[] folderNames = directory.Split(Path.AltDirectorySeparatorChar);
			GameObject parent = audioPlayer.gameObject;
			GameObject folder = audioPlayer.gameObject;
			
			foreach (string folderName in folderNames) {
				if (string.IsNullOrEmpty(folderName)) {
					continue;
				}
				
				folder = parent.FindChild(folderName);
				if (folder == null) {
					folder = new GameObject(folderName);
					folder.transform.parent = parent.transform;
					folderStructure.Add(folder);
				}
				parent = folder;
			}
			return parent;
		}

		void RemoveEmptyFolders() {
			foreach (GameObject folder in folderStructure.ToArray()) {
				if (folder != null) {
					if (folder.transform.childCount == 0) {
						RemoveEmptyFolder(folder);
					}
				}
			}
		}
		
		void RemoveEmptyFolder(GameObject folder) {
			Transform parent = folder.transform.parent;
			
			if (parent != null && parent.childCount == 1 && parent != audioPlayer.transform) {
				folderStructure.Remove(folder);
				RemoveEmptyFolder(folder.transform.parent.gameObject);
			}
			else {
				folderStructure.Remove(folder);
				folder.Remove();
			}
		}

		void EnsureUniqueNames() {
			Magicolo.AudioTools.AudioInfo[] audioInfos = audioPlayer.GetComponentsInChildren<Magicolo.AudioTools.AudioInfo>();
			foreach (Magicolo.AudioTools.AudioInfo audioInfo in audioInfos) {
				audioInfo.SetUniqueName(audioInfo.Name, "", audioInfos);
			}
		}

		void FreezeHierarchy() {
			audioPlayer.transform.hideFlags = HideFlags.HideInInspector;
			audioPlayer.transform.Reset();
			foreach (GameObject child in audioPlayer.gameObject.GetChildrenRecursive()) {
				child.transform.hideFlags = HideFlags.HideInInspector;
				child.transform.Reset();
			}
		}
		
		void SetCurrentAudioClips() {
			audioClips = Resources.LoadAll<AudioClip>(audioClipsPath);
			audioSources = audioPlayer.GetComponentsInChildren<AudioSource>();
			currentAudioClips = new AudioClip[audioSources.Length];
			
			for (int i = 0; i < audioSources.Length; i++) {
				currentAudioClips[i] = audioSources[i].clip;
			}
			
			audioPlayer.SortChildrenRecursive();
		}
	
		void BuildAudioInfoDict() {
			audioInfos = new Dictionary<string, Magicolo.AudioTools.AudioInfo>();
			
			foreach (Magicolo.AudioTools.AudioInfo audioInfo in Object.FindObjectsOfType<Magicolo.AudioTools.AudioInfo>()) {
				audioInfos[audioInfo.gameObject.name] = audioInfo;
			}
		}
		
		public Magicolo.AudioTools.AudioInfo GetAudioInfo(string key) {
			return audioInfos[key];
		}
	}
}
