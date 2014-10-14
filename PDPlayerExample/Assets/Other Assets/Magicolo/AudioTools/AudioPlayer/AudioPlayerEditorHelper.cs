using UnityEngine;
using System.Collections;
using Magicolo.EditorTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioPlayerEditorHelper : EditorHelper {

		public Texture icon;
		public AudioPlayer audioPlayer;
		
		public AudioPlayerEditorHelper(AudioPlayer audioPlayer) {
			this.audioPlayer = audioPlayer;
		}
		
		public override void OnHierarchyWindowItemGUI(int instanceid, Rect selectionrect) {
			#if UNITY_EDITOR
			icon = icon ?? UnityEditor.EditorGUIUtility.ObjectContent(null, typeof(AudioClip)).image;
			GameObject gameObject = UnityEditor.EditorUtility.InstanceIDToObject(instanceid) as GameObject;
			
			if (gameObject == null || icon == null) return;
			
			float width = selectionrect.width;
			selectionrect.width = 16;
			selectionrect.height = 16;
			
			AudioPlayer player = gameObject.GetComponent<AudioPlayer>();
			if (player != null) {
				selectionrect.x = width - 40 + gameObject.GetHierarchyDepth() * 14;
				GUI.DrawTexture(selectionrect, icon);
			}
			#endif
		}
	}
}
