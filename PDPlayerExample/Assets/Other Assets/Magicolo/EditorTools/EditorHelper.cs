using UnityEngine;
using System.Collections;

namespace Magicolo.EditorTools {
	[System.Serializable]
	public class EditorHelper {

		public void Update() {
			#if UNITY_EDITOR
			Unsubscribe();
			Subscribe();
			#endif
		}
		
		public void Subscribe() {
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.hierarchyWindowChanged += OnHierarchyWindowChanged;
			UnityEditor.EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemGUI;
			UnityEditor.EditorApplication.modifierKeysChanged += OnModifierKeysChanged;
			UnityEditor.EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
			UnityEditor.EditorApplication.projectWindowChanged += OnProjectWindowChanged;
			UnityEditor.EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
			UnityEditor.EditorApplication.searchChanged += OnSearchChanged;
			UnityEditor.EditorApplication.update += OnUpdate;
			#endif
		}
		
		public void Unsubscribe() {
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.hierarchyWindowChanged -= OnHierarchyWindowChanged;
			UnityEditor.EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemGUI;
			UnityEditor.EditorApplication.modifierKeysChanged -= OnModifierKeysChanged;
			UnityEditor.EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;
			UnityEditor.EditorApplication.projectWindowChanged -= OnProjectWindowChanged;
			UnityEditor.EditorApplication.projectWindowItemOnGUI -= OnProjectWindowItemGUI;
			UnityEditor.EditorApplication.searchChanged -= OnSearchChanged;
			UnityEditor.EditorApplication.update -= OnUpdate;
			#endif
		}
		
		public virtual void OnHierarchyWindowChanged() {
		}
		
		public virtual void OnHierarchyWindowItemGUI(int instanceid, Rect selectionrect) {
		}
		
		public virtual void OnModifierKeysChanged() {
		}
		
		public virtual void OnPlaymodeStateChanged() {
		}
		
		public virtual void OnProjectWindowChanged() {
		}
		
		public virtual void OnProjectWindowItemGUI(string guid, Rect selectionRect) {
		}
		
		public virtual void OnSearchChanged() {
		}
		
		public virtual void OnUpdate() {
		}
	}
}
