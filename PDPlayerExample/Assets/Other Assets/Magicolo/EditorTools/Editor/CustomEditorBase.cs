using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Magicolo.EditorTools {
	[ExecuteInEditMode]
	public class CustomEditorBase : Editor {
	
		protected bool deleteBreak;
		protected delegate void AddElementCallback(SerializedProperty addedElement);
		protected delegate void DeleteElementCallback();
		protected Event currentEvent;
		protected Dictionary<string, bool> pressedDict = new Dictionary<string, bool>();
		protected int selectedIndex;
		protected Vector2 lastClickPosition;

		protected virtual void Begin(bool space = true) {
			currentEvent = Event.current;
		
			if (space) {
				EditorGUILayout.Space();
			}
		
			deleteBreak = false;
			EditorGUI.BeginChangeCheck();
			serializedObject.Update();
		}
	
		protected virtual void End(bool space = true) {
			if (space) {
				EditorGUILayout.Space();
			}
		
			serializedObject.ApplyModifiedProperties();
		
			if (EditorGUI.EndChangeCheck()) {
				EditorUtility.SetDirty(target);
			}
		}

		protected void BeginBox(GUIStyle style) {
			Rect rect = EditorGUILayout.BeginVertical();
			rect.width -= EditorGUI.indentLevel * 16 - 1;
			rect.height += 1;
			rect.x += EditorGUI.indentLevel * 16;
			GUI.Box(rect, "", style);
		}
	
		protected void BeginBox() {
			BeginBox(new GUIStyle("box"));
		}
	
		protected void EndBox() {
			EditorGUILayout.EndVertical();
		}
	
		protected bool AddElementFoldOut(SerializedProperty property, GUIContent label, GUIStyle style, int overrideArraySize, AddElementCallback addElementCallback = null) {
			int arraySize = property.arraySize;
			if (overrideArraySize >= 0) {
				arraySize = overrideArraySize;
			}
			label.text += string.Format(" ({0})", arraySize);
		
			EditorGUILayout.BeginHorizontal();
		
			if (property.isExpanded && arraySize == 0) {
				property.isExpanded = false;
			}
		
			property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label, style);
		
			bool pressed = false;
			if (property.isExpanded && arraySize == 0) {
				AddElement(property, addElementCallback);
				pressed = true;
			}
		
			if (AddElementButton(property, addElementCallback)) {
				pressed = true;
			}
		
			EditorGUILayout.EndHorizontal();
			return pressed;
		}
	
		protected bool AddElementFoldOut(SerializedProperty property, GUIContent label, int overrideArraySize, AddElementCallback addElementCallback = null) {
			return AddElementFoldOut(property, label, EditorStyles.foldout, overrideArraySize, addElementCallback);
		}
	
		protected bool AddElementFoldOut(SerializedProperty property, GUIContent label, GUIStyle style, AddElementCallback addElementCallback = null) {
			return AddElementFoldOut(property, label, style, -1, addElementCallback);
		}
	
		protected bool AddElementFoldOut(SerializedProperty property, GUIContent label, AddElementCallback addElementCallback = null) {
			return AddElementFoldOut(property, label, EditorStyles.foldout, -1, addElementCallback);
		}
	
		[System.Obsolete]
		protected bool AddElementFoldOut(SerializedProperty property, bool showing, GUIContent label, int overrideArraySize, AddElementCallback addElementCallback = null) {
			int arraySize = property.arraySize;
			if (overrideArraySize >= 0) {
				arraySize = overrideArraySize;
			}
			label.text += string.Format(" ({0})", arraySize);
		
			EditorGUILayout.BeginHorizontal();
			if (showing && arraySize == 0) {
				showing = false;
			}
			showing = EditorGUILayout.Foldout(showing, label);
			if (showing && arraySize == 0) {
				AddElement(property, addElementCallback);
			}
			AddElementButton(property, addElementCallback);
			EditorGUILayout.EndHorizontal();
			return showing;
		}
	
		[System.Obsolete]
		protected bool AddElementFoldOut(SerializedProperty property, bool showing, GUIContent label, AddElementCallback addElementCallback = null) {
			return AddElementFoldOut(property, showing, label, -1, addElementCallback);
		}
	
		[System.Obsolete]
		protected bool AddElementFoldOut(SerializedProperty property, bool showing, string label, int overrideArraySize, AddElementCallback addElementCallback = null) {
			return AddElementFoldOut(property, showing, new GUIContent(label), overrideArraySize, addElementCallback);
		}
	
		[System.Obsolete]
		protected bool AddElementFoldOut(SerializedProperty property, bool showing, string label, AddElementCallback addElementCallback = null) {
			return AddElementFoldOut(property, showing, new GUIContent(label), -1, addElementCallback);
		}
	
		protected bool LargeAddElementButton(GUIContent label, AddElementCallback addElementCallback = null) {
			bool pressed = false;
			GUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			GUILayout.Space(EditorGUI.indentLevel * 16);
			GUIStyle style = new GUIStyle("MiniToolbarButton");
			if (GUILayout.Button(label, style)) {
				pressed = true;
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
			return pressed;
		}
	
		protected bool LargeAddElementButton(SerializedProperty property, GUIContent label, AddElementCallback addElementCallback = null) {
			bool pressed = false;
			if (LargeAddElementButton(label, addElementCallback)) {
				AddElement(property, addElementCallback);
				pressed = true;
			}
			return pressed;
		}
	
		protected bool AddElementButton() {
			bool pressed = false;
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			GUIStyle style = new GUIStyle("toolbarbutton");
			style.clipping = TextClipping.Overflow;
			style.fontSize = 10;
			if (GUILayout.Button("+", style, GUILayout.Width(16))) {
				pressed = true;
			}
			EditorGUI.EndDisabledGroup();
			return pressed;
		}
	
		protected bool AddElementButton(SerializedProperty property, AddElementCallback addElementCallback = null) {
			bool pressed = false;
			if (AddElementButton()) {
				AddElement(property, addElementCallback);
				pressed = true;
			}
			return pressed;
		}
	
		protected bool DeleteElementFoldOutWithArrows(SerializedProperty property, int index, GUIContent label, GUIStyle style, DeleteElementCallback deleteElementCallback = null) {
			EditorGUILayout.BeginHorizontal();
		
			SerializedProperty elementProperty = property.GetArrayElementAtIndex(index);
			elementProperty.isExpanded = EditorGUILayout.Foldout(elementProperty.isExpanded, label, style);
			bool pressed = DeleteElementButtonWithArrows(property, index, deleteElementCallback);
		
			EditorGUILayout.EndHorizontal();
			return pressed;
		}
	
		protected bool DeleteElementFoldOutWithArrows(SerializedProperty property, int index, GUIContent label, DeleteElementCallback deleteElementCallback = null) {
			return DeleteElementFoldOutWithArrows(property, index, label, EditorStyles.foldout, deleteElementCallback);
		}
	
		protected bool DeleteElementFoldOut(SerializedProperty property, int index, GUIContent label, GUIStyle style, DeleteElementCallback deleteElementCallback = null) {
			EditorGUILayout.BeginHorizontal();
		
			SerializedProperty elementProperty = property.GetArrayElementAtIndex(index);
			elementProperty.isExpanded = EditorGUILayout.Foldout(elementProperty.isExpanded, label, style);
			bool pressed = DeleteElementButton(property, index, deleteElementCallback);
		
			EditorGUILayout.EndHorizontal();
		
			return pressed;
		}
	
		protected bool DeleteElementFoldOut(SerializedProperty property, int index, GUIContent label, DeleteElementCallback deleteElementCallback = null) {
			return DeleteElementFoldOut(property, index, new GUIContent(label), EditorStyles.foldout, deleteElementCallback);
		}
	
		[System.Obsolete]
		protected bool DeleteElementFoldOut(SerializedProperty property, int index, bool showing, GUIContent label, DeleteElementCallback deleteElementCallback = null) {
			EditorGUILayout.BeginHorizontal();
			showing = EditorGUILayout.Foldout(showing, label);
			DeleteElementButton(property, index, deleteElementCallback);
			EditorGUILayout.EndHorizontal();
		
			return showing;
		}
	
		[System.Obsolete]
		protected bool DeleteElementFoldOut(SerializedProperty property, int index, bool showing, string label, DeleteElementCallback deleteElementCallback = null) {
			return DeleteElementFoldOut(property, index, showing, new GUIContent(label), deleteElementCallback);
		}
	
		[System.Obsolete]
		protected bool DeleteElementFoldOutWithArrows(SerializedProperty property, int index, bool showing, GUIContent label, DeleteElementCallback deleteElementCallback = null) {
			EditorGUILayout.BeginHorizontal();
			showing = EditorGUILayout.Foldout(showing, label);
			DeleteElementButtonWithArrows(property, index, deleteElementCallback);
			EditorGUILayout.EndHorizontal();
			return showing;
		}
	
		[System.Obsolete]
		protected bool DeleteElementFoldOutWithArrows(SerializedProperty property, int index, bool showing, string label, DeleteElementCallback deleteElementCallback = null) {
			return DeleteElementFoldOutWithArrows(property, index, showing, new GUIContent(label), deleteElementCallback);
		}
	
		protected bool DeleteElementButton() {
			bool pressed = false;
		
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			GUIStyle style = new GUIStyle("MiniToolbarButtonLeft");
			style.clipping = TextClipping.Overflow;
			if (GUILayout.Button("−", style, GUILayout.Width(16))) {
				pressed = true;
			}
			EditorGUI.EndDisabledGroup();
			return pressed;
		}
	
		protected bool DeleteElementButton(SerializedProperty property, int indexToRemove, DeleteElementCallback deleteElementCallback = null) {
			bool pressed = false;
			if (DeleteElementButton()) {
				DeleteElement(property, indexToRemove, deleteElementCallback);
				pressed = true;
			}
			return pressed;
		}
	
		protected bool DeleteElementButtonWithArrows(SerializedProperty property, int indexToRemove, DeleteElementCallback deleteElementCallback = null) {
			bool pressed = false;
			EditorGUILayout.BeginHorizontal();
			MoveElementArrows(property, indexToRemove);
			pressed = DeleteElementButton(property, indexToRemove, deleteElementCallback);
			EditorGUILayout.EndHorizontal();
			return pressed;
		}
	
		protected void MoveElementArrows(SerializedProperty property, int indexToMove) {
			EditorGUILayout.Space();
			if (property.arraySize <= 1) return;
		
			pressedDict = pressedDict ?? new Dictionary<string, bool>();
			string upArrowName = "UpArrow" + property.name + property.depth + indexToMove;
			string downArrowName = "DownArrow" + property.name + property.depth + indexToMove;

			EditorGUI.BeginDisabledGroup(Application.isPlaying);
		
			GUIStyle referenceStyle = new GUIStyle("boldLabel");
			GUIStyle upArrowStyle = new GUIStyle("boldLabel");
			upArrowStyle.fontSize = 9;
			upArrowStyle.alignment = TextAnchor.MiddleCenter;
			if (pressedDict.ContainsKey(upArrowName)) {
				if (pressedDict[upArrowName]) upArrowStyle.normal.textColor = Color.gray;
			}
			else pressedDict[upArrowName] = false;
		
			GUIStyle downArrowStyle = new GUIStyle("boldLabel");
			downArrowStyle.fontSize = 9;
			downArrowStyle.alignment = TextAnchor.MiddleCenter;
			if (pressedDict.ContainsKey(downArrowName)) {
				if (pressedDict[downArrowName]) downArrowStyle.normal.textColor = Color.gray;
			}
			else pressedDict[downArrowName] = false;
		
			bool isInsideRect;
			Rect position;
			Event e = Event.current;
			position = EditorGUILayout.BeginHorizontal();
			position.x += position.width - 16;
			position.y += 1;
			position.height = 8;
			position.width = 14;
			isInsideRect = e.mousePosition.Intersects(new Rect(position.x, position.y, position.width, position.height - 1));
		
			if (e.isMouse) {
				if (e.button == 0) {
					if (e.type == EventType.MouseDown) {
						if (isInsideRect) {
							upArrowStyle.normal.textColor = Color.gray;
							pressedDict[upArrowName] = true;
						}
						else {
							upArrowStyle.normal.textColor = referenceStyle.normal.textColor;
							pressedDict[upArrowName] = false;
						}
					}
					else if (e.type == EventType.MouseUp) {
						upArrowStyle.normal.textColor = referenceStyle.normal.textColor;
						pressedDict[upArrowName] = false;
					}
				}
			}
		
			if (GUI.Button(position, "▲", upArrowStyle)) {
				upArrowStyle.normal.textColor = referenceStyle.normal.textColor;
				pressedDict[upArrowName] = false;
				property.MoveArrayElement(indexToMove, Mathf.Clamp(indexToMove - 1, 0, property.arraySize - 1));
			}
		
			position.y += 7;
			isInsideRect = e.mousePosition.Intersects(new Rect(position.x, position.y, position.width, position.height - 1));
			if (e.isMouse) {
				if (e.button == 0) {
					if (e.type == EventType.MouseDown) {
						if (isInsideRect) {
							downArrowStyle.normal.textColor = Color.gray;
							pressedDict[downArrowName] = true;
						}
						else {
							downArrowStyle.normal.textColor = referenceStyle.normal.textColor;
							pressedDict[downArrowName] = false;
						}
					}
					else if (e.type == EventType.MouseUp) {
						downArrowStyle.normal.textColor = referenceStyle.normal.textColor;
						pressedDict[downArrowName] = false;
					}
				}
			}
		
			if (GUI.Button(position, "▼", downArrowStyle)) {
				downArrowStyle.normal.textColor = referenceStyle.normal.textColor;
				pressedDict[downArrowName] = false;
				property.MoveArrayElement(indexToMove, Mathf.Clamp(indexToMove + 1, 0, property.arraySize - 1));
			}
			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
		}
	
		protected SerializedProperty AddElement(SerializedProperty property, AddElementCallback addElementCallback = null) {
			property.arraySize += 1;
			property.serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(property.serializedObject.targetObject);
		
			SerializedProperty newProperty = property.GetArrayElementAtIndex(property.arraySize - 1);
			if (addElementCallback != null) addElementCallback(newProperty);
		
			return newProperty;
		}
	
		protected void DeleteElement(SerializedProperty property, int indexToRemove, DeleteElementCallback deleteElementCallback = null) {
			property.DeleteArrayElementAtIndex(indexToRemove);
			property.serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(property.serializedObject.targetObject);
			deleteBreak = true;
		
			if (deleteElementCallback != null) deleteElementCallback();
		}
	
		protected void PropertyObjectField<T>(SerializedProperty property, GUIContent label, bool allowSceneObjects = true) {
			label = label ?? GUIContent.none;
			property.objectReferenceValue = EditorGUILayout.ObjectField(label, property.objectReferenceValue, typeof(T), allowSceneObjects);
		}
	
		protected void Separator(bool reserveVerticalSpace = true) {
			if (reserveVerticalSpace) {
				GUILayout.Space(4);
				EditorGUILayout.LabelField(GUIContent.none, new GUIStyle("RL DragHandle"), GUILayout.Height(4));
				GUILayout.Space(4);
			}
			else {
				Rect position = EditorGUILayout.BeginVertical();
				position.y += 7;
				EditorGUI.LabelField(position, GUIContent.none, new GUIStyle("RL DragHandle"));
				EditorGUILayout.EndVertical();
			}
		}

		//	bool Reorderable(SerializedProperty property, int index, bool showing, GUIContent label) {
		//		pressedDict = pressedDict ?? new Dictionary<string, bool>();
		//		showingDict = showingDict ?? new Dictionary<string, bool>();
		//		string name = "Reorderable" + property.name + property.depth;
		//		bool isInsideRect;
		//		Rect position = EditorGUILayout.BeginHorizontal();
		//		EditorGUILayout.EndHorizontal();
		//		position.width = label.text.GetWidth(EditorStyles.standardFont) + 10;
		//		position.height = 17;
		//		isInsideRect = currentEvent.mousePosition.Intersects(position);
		//
		//		if (currentEvent.isMouse) {
		//			if (currentEvent.button == 0) {
		//				if (currentEvent.type == EventType.MouseDown) {
		//					if (isInsideRect) {
		//						selectedIndex = index;
		//						pressedDict[name + index] = true;
		//						showingDict[name + index] = showing;
		//						showing = false;
		//						lastClickPosition = currentEvent.mousePosition;
		//					}
		//				}
		//				else if (currentEvent.type == EventType.MouseUp) {
		//					pressedDict.Clear();
		//				}
		//			}
		//		}
		//
		//		if (selectedIndex == -1) {
		//			if (!pressedDict.ContainsKey(name + index))
		//				pressedDict[name + index] = false;
		//			showingDict[name + index] = showing;
		//		}
		//		else {
		//			if (pressedDict.Count == 0) {
		//				if (showingDict.ContainsKey(name + index)) {
		//					showing = showingDict[name + index];
		//					showingDict.Remove(name + index);
		//				}
		//				if (showingDict.Count == 0) {
		//					selectedIndex = -1;
		//				}
		//			}
		//			else if (pressedDict.ContainsValue(true)) {
		//				if (index != selectedIndex)
		//					showing = false;
		//			}
		//		}
		//
		//		if (pressedDict.ContainsKey(name + index) && pressedDict[name + index]) {
		//			position.x -= 6;
		//			position.y += 6;
		//			position.width += 3;
		//			GUI.Label(position, "", new GUIStyle("ColorPickerVertThumb"));
		//			position.x += 1;
		//			GUI.Label(position, "", new GUIStyle("ColorPickerVertThumb"));
		//			position.x += 1;
		//			GUI.Label(position, "", new GUIStyle("ColorPickerVertThumb"));
		//			position.x += 1;
		//			GUI.Label(position, "", new GUIStyle("ColorPickerVertThumb"));
		//
		//			float diff = currentEvent.mousePosition.y - lastClickPosition.y;
		//			if (diff > 9 || diff < -9) {
		//				selectedIndex = Mathf.Clamp(index + (int)(diff / Mathf.Abs(diff)), 0, property.arraySize - 1);
		//
		//				if (index != selectedIndex) {
		//					property.MoveArrayElement(index, selectedIndex);
		//					lastClickPosition.y += (selectedIndex - index) * 18;
		//					pressedDict[name + selectedIndex] = true;
		//					pressedDict[name + index] = false;
		//					showingDict.SwitchKeys(name + index, name + selectedIndex);
		//					showing = false;
		//					property.serializedObject.ApplyModifiedProperties();
		//					EditorUtility.SetDirty(property.serializedObject.targetObject);
		//				}
		//			}
		//		}
		//
		//		return showing;
		//	}
	}
}