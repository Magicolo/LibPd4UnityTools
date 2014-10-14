using UnityEngine;
using System.Collections;

public static class DoubleExtensions {

	public static float Pow(this double d, double power = 2) {
		return Mathf.Pow((float)d, (float)power);
	}
	
	public static double Round(this double d, double step = 1) {
		return step <= 0 ? d : (double)(Mathf.Round((float)(d * (1D / step))) / (1D / step));
	}
}
