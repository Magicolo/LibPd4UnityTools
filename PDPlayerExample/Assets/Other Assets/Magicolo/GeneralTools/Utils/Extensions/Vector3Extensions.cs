using UnityEngine;
using System.Collections;

public static class Vector3Extensions {

	public static Vector3 Rotate(this Vector3 vector, float angle) {
		return vector.Rotate(angle, Vector3.forward);
	}
	
	public static Vector3 Rotate(this Vector3 vector, float angle, Vector3 axis) {
		angle %= 360;
		return Quaternion.AngleAxis(-angle, axis) * vector;
	}
		
	public static Vector3 SquareClamp(this Vector3 vector, float size = 1) {
		return vector.RectClamp(size, size);
	}
	
	public static Vector3 RectClamp(this Vector3 vector, float width = 1, float height = 1) {
		float clamped;
		if (vector.x < -width || vector.x > width) {
			clamped = Mathf.Clamp(vector.x, -width, width);
			vector.y *= clamped / vector.x;
			vector.x = clamped;
		}
		if (vector.y < -height || vector.y > height) {
			clamped = Mathf.Clamp(vector.y, -height, height);
			vector.x *= clamped / vector.y;
			vector.y = clamped;
		}
		return vector;
	}
	
	public static Vector3 Mult(this Vector3 vector, Vector3 otherVector, string axis) {
		return ((Vector4)vector).Mult(otherVector, axis);
	}
	
	public static Vector3 Mult(this Vector3 vector, Vector3 otherVector) {
		return vector.Mult(otherVector, "XYZ");
	}
	
	public static Vector3 Mult(this Vector3 vector, Vector2 otherVector, string axis) {
		return vector.Mult((Vector3)otherVector, axis);
	}
	
	public static Vector3 Mult(this Vector3 vector, Vector2 otherVector) {
		return vector.Mult((Vector3)otherVector, "XY");
	}
	
	public static Vector3 Mult(this Vector3 vector, Vector4 otherVector, string axis) {
		return vector.Mult((Vector3)otherVector, axis);
	}
	
	public static Vector3 Mult(this Vector3 vector, Vector4 otherVector) {
		return vector.Mult((Vector3)otherVector, "XYZ");
	}
	
	public static Vector3 Div(this Vector3 vector, Vector3 otherVector, string axis) {
		return ((Vector4)vector).Div(otherVector, axis);
	}
	
	public static Vector3 Div(this Vector3 vector, Vector3 otherVector) {
		return vector.Div(otherVector, "XYZ");
	}
	
	public static Vector3 Div(this Vector3 vector, Vector2 otherVector, string axis) {
		return vector.Div((Vector3)otherVector, axis);
	}
	
	public static Vector3 Div(this Vector3 vector, Vector2 otherVector) {
		return vector.Div((Vector3)otherVector, "XY");
	}
	
	public static Vector3 Div(this Vector3 vector, Vector4 otherVector, string axis) {
		return vector.Div((Vector3)otherVector, axis);
	}
	
	public static Vector3 Div(this Vector3 vector, Vector4 otherVector) {
		return vector.Div((Vector3)otherVector, "XYZ");
	}
	
	public static Vector3 Pow(this Vector3 vector, double power, string axis) {
		return ((Vector4)vector).Pow(power, axis);
	}
	
	public static Vector3 Pow(this Vector3 vector, double power) {
		return vector.Pow(power, "XYZ");
	}
	
	public static Vector3 Round(this Vector3 vector, double step, string axis) {
		return ((Vector4)vector).Round(step, axis);
	}
	
	public static Vector3 Round(this Vector3 vector, double step) {
		return vector.Round(step, "XYZ");
	}
	
	public static Vector3 Round(this Vector3 vector) {
		return vector.Round(1, "XYZ");
	}
	
	public static float Average(this Vector3 vector, string axis) {
		return ((Vector4)vector).Average(axis);
	}
	
	public static float Average(this Vector3 vector) {
		return ((Vector4)vector).Average("XYZ");
	}
}
