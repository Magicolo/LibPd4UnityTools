using UnityEngine;

public static class IntExtensions {

	public static float Pow(this int i, double power = 2) {
		return Mathf.Pow(i, (float)power);
	}
	
	public static int Round(this int i, double step = 1) {
		return step <= 0 ? i : (int)(Mathf.Round((float)(i * (1D / step))) / (1D / step));
	}
}
