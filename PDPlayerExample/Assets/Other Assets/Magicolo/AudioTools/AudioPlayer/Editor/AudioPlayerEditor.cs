#if UNITY_EDITOR
using Magicolo.EditorTools;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Magicolo.AudioTools{
	[CustomEditor(typeof(AudioPlayer))]
	public class AudioPlayerEditor : CustomEditorBase {
		
		AudioPlayer audioPlayer;
		SerializedProperty audioSettingsProperty;
		
		public override void OnInspectorGUI(){
			audioPlayer = (AudioPlayer) target;
			audioSettingsProperty = serializedObject.FindProperty("audioSettings");
			
			Begin();
			
			ShowGeneralSettings();
			ShowButtons();
			Separator();
			
			End();
		}

		void ShowGeneralSettings() {
			EditorGUILayout.PropertyField(audioSettingsProperty.FindPropertyRelative("masterVolume"));
			EditorGUILayout.PropertyField(audioSettingsProperty.FindPropertyRelative("maxVoices"));
		}	
		
		void ShowButtons() {
			PDPlayer pdPlayer = FindObjectOfType<PDPlayer>();
			
			if (pdPlayer != null) {
				return;
			}
			
			Separator();
			
			if (pdPlayer == null) {
				if (LargeAddElementButton("Add Pure Data Player".ToGUIContent())) {
					audioPlayer.AddComponent<PDPlayer>();
				}
			}
		}
	}
}
#endif