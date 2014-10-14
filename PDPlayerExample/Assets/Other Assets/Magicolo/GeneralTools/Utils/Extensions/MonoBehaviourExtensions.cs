using UnityEngine;
using System.Collections;

public static class MonoBehaviourExtensions {

	public static void SetExecutionOrder(this MonoBehaviour script, int order) {
		#if UNITY_EDITOR
		foreach (UnityEditor.MonoScript s in UnityEditor.MonoImporter.GetAllRuntimeMonoScripts()) {
			if (s.name == script.GetType().Name){
				if (UnityEditor.MonoImporter.GetExecutionOrder(s) != order){
					UnityEditor.MonoImporter.SetExecutionOrder(s, order);
				}
			}
		}
		#endif
	}
	
	public static void SetTransformHasChanged(this MonoBehaviour script, Transform transform, bool hasChanged) {
		script.StartCoroutine(SetHasChanged(transform, hasChanged));
	}
	
	static IEnumerator SetHasChanged(Transform transform, bool hasChanged) {
		yield return new WaitForEndOfFrame();
		transform.hasChanged = hasChanged;
	}
}
