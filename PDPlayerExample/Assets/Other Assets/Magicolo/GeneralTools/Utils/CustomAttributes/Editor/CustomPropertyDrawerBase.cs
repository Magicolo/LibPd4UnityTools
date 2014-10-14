#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CustomPropertyDrawerBase : PropertyDrawer {
	
	protected bool noFieldLabel;
	protected bool noPrefixLabel;
	protected bool noIndex;
	protected string prefixLabel;
	protected int index;
	
	protected bool drawPrefixLabel = true;
	protected float scrollbarThreshold;
	protected Rect currentPosition;
	protected GUIContent currentLabel = GUIContent.none;
	
	protected SerializedProperty arrayProperty;
	protected delegate void AddElementCallback(SerializedProperty addedElement);
	protected delegate void DeleteElementCallback();
		
	int indentLevel;
	
	protected Rect Begin(Rect position, SerializedProperty property, GUIContent label){
		noFieldLabel = ((CustomAttributeBase) attribute).NoFieldLabel;
		noPrefixLabel = ((CustomAttributeBase) attribute).NoPrefixLabel;
		noIndex = ((CustomAttributeBase) attribute).NoIndex;
		prefixLabel = ((CustomAttributeBase) attribute).PrefixLabel;
		scrollbarThreshold = Screen.width - position.width > 19 ? 298 : 313;
		indentLevel = EditorGUI.indentLevel;
			
		EditorGUI.BeginChangeCheck();
		
		if (fieldInfo.FieldType.IsArray){
 			index = AttributeUtility.GetIndexFromLabel(label);
 			arrayProperty = property.serializedObject.FindProperty(fieldInfo.Name);
 			
 			if (noIndex){
 				if (string.IsNullOrEmpty(prefixLabel)){
					label.text = label.text.Substring(0, label.text.Length - 2);
				}
 			}
 			else if (!string.IsNullOrEmpty(prefixLabel)){
 				prefixLabel += " " + index;
 			}
		}
		
		
		if (drawPrefixLabel){
			if (!noPrefixLabel){
				if (!string.IsNullOrEmpty(prefixLabel)) {
					label.text = prefixLabel;
				}
				position = EditorGUI.PrefixLabel(position, label);
			}
		}
		else {
			if (noPrefixLabel)
				label.text = "";
			else if 
				(!string.IsNullOrEmpty(prefixLabel)) label.text = prefixLabel;
		}
		currentPosition = position;
		currentLabel = label;
		return position;
	}
	
	protected void End(SerializedProperty property){
		if (fieldInfo.FieldType.IsArray && index == 0){
			if (AttributeUtility.toRemove.ContainsKey(fieldInfo.DeclaringType + arrayProperty.name)){
				arrayProperty.DeleteArrayElementAtIndex(AttributeUtility.toRemove[fieldInfo.DeclaringType + arrayProperty.name]);
				AttributeUtility.toRemove.Remove(fieldInfo.DeclaringType + arrayProperty.name);
			}
		}
		EditorGUI.indentLevel = indentLevel;
		if (EditorGUI.EndChangeCheck())
			EditorUtility.SetDirty(property.serializedObject.targetObject);
	}
	
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return EditorGUI.GetPropertyHeight(property, label, true);
	}
	
	protected bool AddElementButton(Rect position) {
		bool pressed = false;
		EditorGUI.BeginDisabledGroup(Application.isPlaying);
		GUIStyle style = new GUIStyle("MiniToolbarButtonLeft");
		style.clipping = TextClipping.Overflow;
		style.fontSize = 10;
		if (GUI.Button(new Rect(position.x, position.y, 16, 16), "+", style)) {
			pressed = true;
		}
		EditorGUI.EndDisabledGroup();
		return pressed;
	}
	
	protected bool AddElementButton(Rect position, SerializedProperty property, AddElementCallback addElementCallback = null) {
		bool pressed = false;
		if (AddElementButton(position)) {
			AddElement(property, addElementCallback);
			pressed = true;
		}
		return pressed;
	}
	
	protected bool DeleteElementButton(Rect position) {
		bool pressed = false;
		
		EditorGUI.BeginDisabledGroup(Application.isPlaying);
		GUIStyle style = new GUIStyle("MiniToolbarButtonLeft");
		style.clipping = TextClipping.Overflow;
		if (GUI.Button(new Rect(position.x, position.y, 16, 16), "−", style)) {
			pressed = true;
		}
		EditorGUI.EndDisabledGroup();
		return pressed;
	}
	
	protected bool DeleteElementButton(Rect position, SerializedProperty property, int indexToRemove, DeleteElementCallback deleteElementCallback = null) {
		bool pressed = false;
		if (DeleteElementButton(position)) {
			DeleteElement(property, indexToRemove, deleteElementCallback);
			pressed = true;
		}
		return pressed;
	}
	
	protected bool DeleteElementButtonWithArrows(Rect position, SerializedProperty property, int indexToRemove, DeleteElementCallback deleteElementCallback = null) {
		bool pressed = false;
		MoveElementArrows(new Rect(position.x - 13, position.y, position.width, position.height), property, indexToRemove);
		pressed = DeleteElementButton(position, property, indexToRemove, deleteElementCallback);
		return pressed;
	}
	
	protected void MoveElementArrows(Rect position, SerializedProperty property, int indexToMove) {
		if (property.arraySize <= 1)
			return;
		
		string upArrowName = "UpArrow" + property.name + property.depth + indexToMove;
		string downArrowName = "DownArrow" + property.name + property.depth + indexToMove;

		EditorGUI.BeginDisabledGroup(Application.isPlaying);
		
		GUIStyle referenceStyle = new GUIStyle("boldLabel");
		GUIStyle upArrowStyle = new GUIStyle("boldLabel");
		upArrowStyle.fontSize = 9;
		upArrowStyle.alignment = TextAnchor.MiddleCenter;
		if (AttributeUtility.pressedDict.ContainsKey(upArrowName)) {
			if (AttributeUtility.pressedDict[upArrowName])
				upArrowStyle.normal.textColor = Color.gray;
		}
		else
			AttributeUtility.pressedDict[upArrowName] = false;
		
		GUIStyle downArrowStyle = new GUIStyle("boldLabel");
		downArrowStyle.fontSize = 9;
		downArrowStyle.alignment = TextAnchor.MiddleCenter;
		if (AttributeUtility.pressedDict.ContainsKey(downArrowName)) {
			if (AttributeUtility.pressedDict[downArrowName])
				downArrowStyle.normal.textColor = Color.gray;
		}
		else
			AttributeUtility.pressedDict[downArrowName] = false;
		
		bool isInsideRect;
		Event e = Event.current;
		position.height = 8;
		position.width = 14;
		isInsideRect = e.mousePosition.Intersects(new Rect(position.x, position.y, position.width, position.height - 1));
		
		if (e.isMouse) {
			if (e.button == 0) {
				if (e.type == EventType.MouseDown) {
					if (isInsideRect) {
						upArrowStyle.normal.textColor = Color.gray;
						AttributeUtility.pressedDict[upArrowName] = true;
					}
					else {
						upArrowStyle.normal.textColor = referenceStyle.normal.textColor;
						AttributeUtility.pressedDict[upArrowName] = false;
					}
				}
				else if (e.type == EventType.MouseUp) {
					upArrowStyle.normal.textColor = referenceStyle.normal.textColor;
					AttributeUtility.pressedDict[upArrowName] = false;
				}
			}
		}
		
		if (GUI.Button(position, "▲", upArrowStyle)) {
			upArrowStyle.normal.textColor = referenceStyle.normal.textColor;
			AttributeUtility.pressedDict[upArrowName] = false;
			property.MoveArrayElement(indexToMove, Mathf.Clamp(indexToMove - 1, 0, property.arraySize - 1));
		}
		
		position.y += 7;
		isInsideRect = e.mousePosition.Intersects(new Rect(position.x, position.y, position.width, position.height - 1));
		if (e.isMouse) {
			if (e.button == 0) {
				if (e.type == EventType.MouseDown) {
					if (isInsideRect) {
						downArrowStyle.normal.textColor = Color.gray;
						AttributeUtility.pressedDict[downArrowName] = true;
					}
					else {
						downArrowStyle.normal.textColor = referenceStyle.normal.textColor;
						AttributeUtility.pressedDict[downArrowName] = false;
					}
				}
				else if (e.type == EventType.MouseUp) {
					downArrowStyle.normal.textColor = referenceStyle.normal.textColor;
					AttributeUtility.pressedDict[downArrowName] = false;
				}
			}
		}
		
		if (GUI.Button(position, "▼", downArrowStyle)) {
			downArrowStyle.normal.textColor = referenceStyle.normal.textColor;
			AttributeUtility.pressedDict[downArrowName] = false;
			property.MoveArrayElement(indexToMove, Mathf.Clamp(indexToMove + 1, 0, property.arraySize - 1));
		}
		EditorGUI.EndDisabledGroup();
	}
	
	protected SerializedProperty AddElement(SerializedProperty property, AddElementCallback addElementCallback = null) {
		property.arraySize += 1;
		property.serializedObject.ApplyModifiedProperties();
		EditorUtility.SetDirty(property.serializedObject.targetObject);
		
		SerializedProperty newProperty = property.GetArrayElementAtIndex(property.arraySize - 1);
		if (addElementCallback != null)
			addElementCallback(newProperty);
		
		return newProperty;
	}
	
	protected void DeleteElement(SerializedProperty property, int indexToRemove, DeleteElementCallback deleteElementCallback = null) {
		AttributeUtility.toRemove[fieldInfo.DeclaringType + property.name] = indexToRemove;
		if (deleteElementCallback != null)
			deleteElementCallback();
	}
}
#endif