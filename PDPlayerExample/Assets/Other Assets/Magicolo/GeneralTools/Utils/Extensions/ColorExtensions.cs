using UnityEngine;
using System.Collections;

public static class ColorExtensions {

	public static Color Mult(this Color color, Color otherColor, string channels) {
		return ((Vector4)color).Mult((Vector4)otherColor, HelperFunctions.ColorChannelsToVectorAxis(channels));
	}
	
	public static Color Mult(this Color color, Color otherColor) {
		return color.Mult(otherColor, "RGBA");
	}
	
	public static Color Div(this Color color, Color otherColor, string channels) {
		return ((Vector4)color).Div((Vector4)otherColor, HelperFunctions.ColorChannelsToVectorAxis(channels));
	}
	
	public static Color Div(this Color color, Color otherColor) {
		return color.Div(otherColor, "RGBA");
	}
	
	public static Color Pow(this Color color, double power, string channels) {
		return ((Vector4)color).Pow(power, HelperFunctions.ColorChannelsToVectorAxis(channels));
	}
	
	public static Color Pow(this Color color, double power) {
		return color.Pow(power, "RGBA");
	}
	
	public static Color Round(this Color color, double step, string channels) {
		if (step <= 0)
			return color;
		if (channels.Contains("R")) {
			color.r = (float)(Mathf.Round((float)(color.r * (1D / step))) / (1D / step));
		}
		if (channels.Contains("G")) {
			color.g = (float)(Mathf.Round((float)(color.g * (1D / step))) / (1D / step));
		}
		if (channels.Contains("B")) {
			color.b = (float)(Mathf.Round((float)(color.b * (1D / step))) / (1D / step));
		}
		if (channels.Contains("A")) {
			color.a = (float)(Mathf.Round((float)(color.a * (1D / step))) / (1D / step));
		}
		return color;
	}
	
	public static Color Round(this Color color, double step) {
		return color.Round(step, "RGBA");
	}
	
	public static Color Round(this Color color) {
		return color.Round(1, "RGBA");
	}
	
	public static float Average(this Color color, string channels) {
		float average = 0;
		int axisCount = 0;
		if (channels.Contains("R")) {
			average += color.r;
			axisCount += 1;
		}
		if (channels.Contains("G")) {
			average += color.g;
			axisCount += 1;
		}
		if (channels.Contains("B")) {
			average += color.b;
			axisCount += 1;
		}
		if (channels.Contains("A")) {
			average += color.a;
			axisCount += 1;
		}
		return average / axisCount;
	}
	
	public static float Average(this Color color) {
		return color.Average("RGBA");
	}
	
	public static Color ToHSV(this Color RGBColor) {
		float R = RGBColor.r;
		float G = RGBColor.g;
		float B = RGBColor.b;
		float H = 0;
		float S = 0;
		float V = 0;
		float d = 0;
		float h = 0;
		
		float minRGB = Mathf.Min(R, Mathf.Min(G, B));
		float maxRGB = Mathf.Max(R, Mathf.Max(G, B));
	
		if (minRGB == maxRGB)
			return new Color(0, 0, minRGB, RGBColor.a);

		if (R == minRGB)
			d = G - B;
		else if (B == minRGB)
			d = R - G;
		else
			d = B - R;
			
		if (R == minRGB)
			h = 3;
		else if (B == minRGB)
			h = 1;
		else
			h = 5;
			
		H = (60 * (h - d / (maxRGB - minRGB))) / 360;
		S = (maxRGB - minRGB) / maxRGB;
		V = maxRGB;
		
		return new Color(H, S, V, RGBColor.a);
	}
	
	public static Color ToRGB(this Color HSVColor) {
		float H = HSVColor.r;
		float S = HSVColor.g;
		float V = HSVColor.b;
		float R = 0;
		float G = 0;
		float B = 0;
		float maxHSV = 255 * V;
		float minHSV = maxHSV * (1 - S);
		float h = H * 360;
		float z = (maxHSV - minHSV) * (1 - Mathf.Abs((h / 60) % 2 - 1));
		
		if (0 <= h && h < 60) {
			R = maxHSV;
			G = z + minHSV;
			B = minHSV;
		}
		else if (60 <= h && h < 120) {
			R = z + minHSV;
			G = maxHSV;
			B = minHSV;
		}
		else if (120 <= h && h < 180) {
			R = minHSV;
			G = maxHSV;
			B = z + minHSV;	
		}
		else if (180 <= h && h < 240) {
			R = minHSV;
			G = z + minHSV;
			;
			B = maxHSV;
		}
		else if (240 <= h && h < 300) {
			R = z + minHSV;
			G = minHSV;
			B = maxHSV;
		}
		else if (300 <= h && h < 360) {
			R = maxHSV;
			G = minHSV;
			B = z + minHSV;
		}
		return new Color(R / 255, G / 255, B / 255, HSVColor.a);
	}
}
