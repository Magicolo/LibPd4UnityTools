#if UNITY_EDITOR
using Magicolo.AudioTools;
using Magicolo.EditorTools;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(PDPlayer))]
public class PDPlayerEditor : CustomEditorBase {

	PDPlayer pdPlayer;
	PDEditorHelper editorHelper;
	SerializedProperty editorHelperProperty;
	SerializedProperty modulesProperty;
	PDEditorModule currentModule;
	SerializedProperty currentModuleProperty;
	PDEditorModule defaultModule;
	SerializedProperty defaultModuleProperty;
	
	public override void OnInspectorGUI(){
		pdPlayer = (PDPlayer) target;
		editorHelper = pdPlayer.editorHelper;
		editorHelperProperty = serializedObject.FindProperty("editorHelper");
		modulesProperty = editorHelperProperty.FindPropertyRelative("modules");
		defaultModule = editorHelper.defaultModule;
		defaultModuleProperty = editorHelperProperty.FindPropertyRelative("defaultModule");
		
		Begin();
		
		ShowGeneralSettings();
		Separator();
		ShowDefaultSettings();
		Separator();
		ShowModules();
		End();
	}
	
	void ShowGeneralSettings(){
		EditorGUI.BeginDisabledGroup(Application.isPlaying);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("patchesPath"), new GUIContent("Patches Path", "The path where the Pure Data patches are relative to Assets/Resources/."));
		EditorGUI.EndDisabledGroup();
	}

	void ShowDefaultSettings() {
		BeginBox();
		
		EditorGUILayout.LabelField(new GUIContent("Default Module Settings", "Default settings for new modules."), EditorStyles.boldLabel);
		
		EditorGUI.indentLevel += 1;
		
		EditorGUILayout.PropertyField(defaultModuleProperty.FindPropertyRelative("volume"));
			
		BeginBox();
		defaultModule.spatializerShowing = EditorGUILayout.Foldout(defaultModule.spatializerShowing, "Spatializer");
		if (defaultModule.spatializerShowing){
			EditorGUI.indentLevel += 1;
			
			PropertyObjectField<GameObject>(defaultModuleProperty.FindPropertyRelative("source"), "Source".ToGUIContent());
				
			EditorGUILayout.PropertyField(defaultModuleProperty.FindPropertyRelative("volumeRolloff"));
			EditorGUILayout.PropertyField(defaultModuleProperty.FindPropertyRelative("minDistance"));
			EditorGUILayout.PropertyField(defaultModuleProperty.FindPropertyRelative("maxDistance"));
			EditorGUILayout.PropertyField(defaultModuleProperty.FindPropertyRelative("panLevel"));
				
			EditorGUI.indentLevel -= 1;
		}
		EndBox();
		
		EditorGUI.indentLevel -= 1;
		
		EndBox();
	}	
	
	void ShowModules(){
		if (LargeAddElementButton(modulesProperty, new GUIContent("Add New Module"))){
			editorHelper.modules[editorHelper.modules.Count - 1] = new PDEditorModule("", defaultModule, pdPlayer);
			editorHelper.modules[editorHelper.modules.Count - 1].SetUniqueName("default", editorHelper.modules);
		}
	
		for (int i = 0; i < editorHelper.modules.Count; i++) {
			currentModule = editorHelper.modules[i];
			currentModuleProperty = modulesProperty.GetArrayElementAtIndex(i);
			
			BeginBox();
			if (DeleteElementFoldOutWithArrows(modulesProperty, i, new GUIContent(currentModule.Name, "Sets specific settings for a module."), GetStateColorStyle())){
				break;
			}
			
			ShowModule();
			EndBox();
		}
	}

	void ShowModule(){
		if (currentModuleProperty.isExpanded){
			EditorGUI.indentLevel += 1;
			
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			currentModule.SetUniqueName(EditorGUILayout.TextField(currentModule.Name), currentModule.Name, "default", editorHelper.modules);
			EditorGUI.EndDisabledGroup();
			
			EditorGUILayout.PropertyField(currentModuleProperty.FindPropertyRelative("volume"), new GUIContent("Volume", "Sets the volume of this module."));
			
			BeginBox();
			currentModule.spatializerShowing = EditorGUILayout.Foldout(currentModule.spatializerShowing, new GUIContent("Spatializer", "These settings specify how the module should be spatialize via the [uspatialize~] object in Pure Data."));
			if (currentModule.spatializerShowing){
				EditorGUI.indentLevel += 1;
				
				PropertyObjectField<GameObject>(currentModuleProperty.FindPropertyRelative("source"), new GUIContent("Source", "The source around which the module will be spatialized.\nIf left empty, the module will not be panned or attenuated.\n\nThe source's position can be recuperated with a [receive 'moduleName'_SourcePosition] in Pure Data.\nAdditionnaly, the listener's angle and distance in relationship to the source can be recuperated with a [receive 'moduleName'_ListenerAngle] and [receive 'moduleName'_ListenerDistance]."));

				EditorGUILayout.PropertyField(currentModuleProperty.FindPropertyRelative("volumeRolloff"), new GUIContent("Volume Rolloff", "The curve of the distance attenuation."));
				EditorGUILayout.PropertyField(currentModuleProperty.FindPropertyRelative("minDistance"), new GUIContent("Min Distance", "Distance at which the module starts to be attenuated."));
				EditorGUILayout.PropertyField(currentModuleProperty.FindPropertyRelative("maxDistance"), new GUIContent("Max Distance", "Distance at which the module has been completely."));
				EditorGUILayout.PropertyField(currentModuleProperty.FindPropertyRelative("panLevel"), new GUIContent("Pan Level", "Controls how much panning is applied."));
					
				EditorGUI.indentLevel -= 1;
			}
			EndBox();
			Separator();
			EditorGUI.indentLevel -= 1;
		}
	}

	GUIStyle GetStateColorStyle() {
		if (Application.isPlaying){
			GUIStyle style = new GUIStyle("foldout");
			style.fontStyle = FontStyle.Bold;
			Color textColor = style.normal.textColor;
			AudioItem.States state = currentModule.State;
			
			switch (state) {
				case AudioItem.States.FadingIn:
					textColor = new Color(1, 1, 0, 1);
					break;
				case AudioItem.States.FadingOut:
					textColor = new Color(1, 1, 0.5F, 1);
					break;
				case AudioItem.States.Paused:
					textColor = new Color(0, 0, 1, 1);
					break;
				case AudioItem.States.Playing:
					textColor = new Color(0, 1, 0, 1);
					break;
				case AudioItem.States.StandingBy:
					textColor = new Color(1, 0, 0, 1);
					break;
				case AudioItem.States.Stopped:
					textColor = new Color(1, 0, 0, 1);
					break;
			}
			
			style.normal.textColor = textColor * 0.8F;
			style.onNormal.textColor = textColor * 0.8F;
			style.focused.textColor = textColor;
			style.onFocused.textColor = textColor;
			style.active.textColor = textColor;
			style.onActive.textColor = textColor;
			return style;
		}
		
		return new GUIStyle("foldout");
	}
}
#endif
