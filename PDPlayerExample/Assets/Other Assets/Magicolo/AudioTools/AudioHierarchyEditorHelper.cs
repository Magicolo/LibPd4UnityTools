using UnityEngine;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioHierarchyEditorHelper : Magicolo.EditorTools.EditorHelper {
		
		public override void OnHierarchyWindowItemGUI(int instanceid, Rect selectionrect) {
			#if UNITY_EDITOR
			Texture icon = UnityEditor.EditorGUIUtility.ObjectContent(null, typeof(AudioSource)).image;
			GameObject gameObject = UnityEditor.EditorUtility.InstanceIDToObject(instanceid) as GameObject;
			
			if (gameObject == null || icon == null)
				return;
			
			float width = selectionrect.width;
			selectionrect.width = 16;
			selectionrect.height = 16;
			
			Magicolo.AudioTools.AudioInfo audioInfo = gameObject.GetComponent<Magicolo.AudioTools.AudioInfo>();
			if (audioInfo != null){
				selectionrect.x = width - 8 + gameObject.GetHierarchyDepth() * 14;
				GUI.DrawTexture(selectionrect, icon);
			}
			#endif
		}
	}
}