#if UNITY_EDITOR
using Magicolo.EditorTools;
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Magicolo.AudioTools {
	[CustomEditor(typeof(Magicolo.AudioTools.AudioInfo)), CanEditMultipleObjects]
	public class AudioInfoEditor : CustomEditorBase {
	
		Magicolo.AudioTools.AudioInfo audioInfo;
		ClipInfo clipInfo;
		SerializedProperty clipInfoProperty;
		
		public override void OnInspectorGUI() {
			audioInfo = (Magicolo.AudioTools.AudioInfo)target;
			clipInfo = audioInfo.clipInfo;
			clipInfoProperty = serializedObject.FindProperty("clipInfo");
			
			
			Begin();
			
			ShowFadeIn();
			ShowFadeInCurve();
			ShowFadeOut();
			ShowFadeOutCurve();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("randomVolume"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("randomPitch"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("doNotKill"));
			ShowClipInfo();
			
			End();
		}
	
		void ShowFadeIn(){
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeIn"));
			if (clipInfo.length <= 0) {
				return;
			}
			if (EditorGUI.EndChangeCheck()){
				serializedObject.ApplyModifiedProperties();
				audioInfo.fadeOut = Mathf.Clamp(audioInfo.fadeOut, 0, clipInfo.length - audioInfo.fadeIn);
			}
		}
		
		void ShowFadeOut(){
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeOut"));
			if (clipInfo.length <= 0) {
				return;
			}
			if (EditorGUI.EndChangeCheck()){
				serializedObject.ApplyModifiedProperties();
				audioInfo.fadeIn = Mathf.Clamp(audioInfo.fadeIn, 0, clipInfo.length - audioInfo.fadeOut);
			}
			audioInfo.fadeIn = Mathf.Clamp(audioInfo.fadeIn, 0, clipInfo.length);
			audioInfo.fadeOut = Mathf.Clamp(audioInfo.fadeOut, 0, clipInfo.length);
		}
		
		void ShowFadeInCurve() {
			audioInfo.fadeInCurve = EditorGUILayout.CurveField("Fade In Curve".ToGUIContent(), audioInfo.fadeInCurve, Color.cyan, new Rect(0, 0, 1, 1));
			if (audioInfo.fadeInCurve.keys.Length < 2) {
				audioInfo.fadeInCurve.keys = new []{ new Keyframe(0, 0), new Keyframe(1, 1) };
			}
			audioInfo.fadeInCurve.MoveKey(0, new Keyframe(0, 0));
			audioInfo.fadeInCurve.MoveKey(audioInfo.fadeInCurve.keys.Length - 1, new Keyframe(1, 1));
		}
		
		void ShowFadeOutCurve() {
			audioInfo.fadeOutCurve = EditorGUILayout.CurveField("Fade Out Curve".ToGUIContent(), audioInfo.fadeOutCurve, Color.cyan, new Rect(0, 0, 1, 1));
			if (audioInfo.fadeOutCurve.keys.Length < 2) {
				audioInfo.fadeOutCurve.keys = new []{ new Keyframe(0, 1), new Keyframe(1, 0) };
			}
			audioInfo.fadeOutCurve.MoveKey(0, new Keyframe(0, 1));
			audioInfo.fadeOutCurve.MoveKey(audioInfo.fadeOutCurve.keys.Length - 1, new Keyframe(1, 0));
		}
	
		void ShowClipInfo() {
			BeginBox();
			clipInfoProperty.isExpanded = EditorGUILayout.Foldout(clipInfoProperty.isExpanded, "Clip Info");
			
			if (clipInfoProperty.isExpanded){
				EditorGUI.indentLevel += 1;
				GUIStyle style = EditorStyles.boldLabel;
				EditorGUILayout.LabelField("Name:", clipInfo.name, style);
				EditorGUILayout.LabelField("Channels:", clipInfo.channels.ToString(), style);
				EditorGUILayout.LabelField("Frequency:", clipInfo.frequency.ToString(), style);
				EditorGUILayout.LabelField("Length:", clipInfo.length.ToString(), style);
				EditorGUILayout.LabelField("Samples:", clipInfo.samples.ToString(), style);
				EditorGUI.indentLevel -= 1;
			}
			EndBox();
		}
	}
}
#endif
