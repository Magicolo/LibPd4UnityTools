using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

namespace Candlelight.Examples
{
	[CustomEditor(typeof(ArrayPropertySetterExample)), CanEditMultipleObjects]
	public class ArrayPropertySetterExampleEditor : Editor
	{
		private ReorderableList array;
		private ReorderableList list;

		private void DrawElementCallback(Rect position, ReorderableList reorderableList, int index)
		{
			if (index < reorderableList.serializedProperty.arraySize)
			{
				EditorGUI.PropertyField(position, reorderableList.serializedProperty.GetArrayElementAtIndex(index));
			}
		}

		void OnEnable()
		{
			array = new ReorderableList(serializedObject, serializedObject.FindProperty("m_ArrayProperty"));
			array.drawHeaderCallback = position => EditorGUI.LabelField(position, "Array Property");
			array.drawElementCallback =
				(position, index, isActive, isFocused) => DrawElementCallback(position, array, index);
			list = new ReorderableList(serializedObject, serializedObject.FindProperty("m_ListProperty"));
			list.drawHeaderCallback = position => EditorGUI.LabelField(position, "List Property");
			list.drawElementCallback =
				(position, index, isActive, isFocused) => DrawElementCallback(position, list, index);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			array.DoLayoutList();
			list.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}
	}
}