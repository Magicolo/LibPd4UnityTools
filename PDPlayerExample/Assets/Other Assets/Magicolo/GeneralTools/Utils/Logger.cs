using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Logger {
	static Dictionary<System.Type, int> instanceDict = new Dictionary<System.Type, int>();
	
	public static double RoundPrecision = 0.001;
	
	public static void Log(params object[] toLog){
		string log = "";
		
		if (toLog != null)
		{
			foreach (object item in toLog)
			{
				log += ObjectToString(item);
				log += ", ";
			}
			if (!string.IsNullOrEmpty(log))
				log = log.Substring(0, log.Length - 2);
		}
		Debug.Log(log);
	}
		
	public static void LogWarning(params object[] toLog){
		string log = "";
		foreach (object item in toLog){
			log += ObjectToString(item);
			log += ", ";
		}
		if (!string.IsNullOrEmpty(log)) log = log.Substring(0, log.Length - 2);
		Debug.LogWarning(log);
	}
		
	public static void LogError(params object[] toLog){
		string log = "";
		foreach (object item in toLog){
			log += ObjectToString(item);
			log += ", ";
		}
		if (!string.IsNullOrEmpty(log)) log = log.Substring(0, log.Length - 2);
		Debug.LogError(log);
	}
		
	public static void LogSingleInstance(Object instanceToLog, params object[] toLog){
		if (instanceDict.ContainsKey(instanceToLog.GetType())){
			if (instanceDict[instanceToLog.GetType()] == instanceToLog.GetInstanceID()){
				Log(toLog);
			}
		}
		else {
			instanceDict[instanceToLog.GetType()] = instanceToLog.GetInstanceID();
			Log(toLog);
		}
	}
	
	public static string ObjectToString(object obj){
		string str = "";
		
		if (obj is System.Array){
			str += "(";
			foreach (object item in (System.Array) obj) str += ObjectToString(item) + ", ";
			if (str.Length > 1) str = str.Substring(0, str.Length - 2);
			str += ")";
		}
		else if (obj is IList){
			str += "[";
			foreach (object item in (IList) obj) str += ObjectToString(item) + ", ";
			if (str.Length > 1) str = str.Substring(0, str.Length - 2);
			str += "]";
		}
		else if (obj is IDictionary){
			str += "{";
			foreach (object key in ((IDictionary) (IDictionary) obj).Keys) str += ObjectToString(key) + " : " + ObjectToString(((IDictionary) obj)[key]) + ", ";
			if (str.Length > 1) str = str.Substring(0, str.Length - 2);
			str += "}";
		}
		else if (obj is IEnumerator) str += ObjectToString(((IEnumerator) obj).Current);
		else if (obj is Vector2 || obj is Vector3 || obj is Vector4 || obj is Color || obj is Quaternion || obj is Rect) str += VectorToString(obj);
		else if (obj is LayerMask) str += ((LayerMask) obj).value.ToString();
		else if (obj != null) str += obj.ToString();
		else str += "null";
		return str;
	}
		
	public static string VectorToString(object v){
		string str = "";
		
		if (v is Vector2){
			Vector2 v2 = (Vector2) v;
//			v2 = v2.Round(RoundPrecision);
			str += "Vector2(" + v2.x + ", " + v2.y;
		}
		else if (v is Vector3){
			Vector3 v3 = (Vector3) v;
//			v3 = v3.Round(RoundPrecision);
			str += "Vector3(" + v3.x + ", " + v3.y + ", " + v3.z;
		}
		else if (v is Vector4){
			Vector4 v4 = (Vector4) v;
//			v4 = v4.Round(RoundPrecision);
			str += "Vector4(" + v4.x + ", " + v4.y + ", " + v4.z + ", " + v4.w;
		}
		else if (v is Quaternion){
			Quaternion q = (Quaternion) v;
			q = q.Round(RoundPrecision);
			str += "Quaternion(" + q.x + ", " + q.y + ", " + q.z + ", " + q.w;
		}
		else if (v is Color){
			Color c = (Color) v;
//			c = c.Round(RoundPrecision);
			str += "Color(" + c.r + ", " + c.g + ", " + c.b + ", " + c.a;
		}
		else if (v is Rect){
			Rect r = (Rect) v;
			r = r.Round(RoundPrecision);
			str += "Rect(" + r.x + ", " + r.y + ", " + r.width + ", " + r.height;
		}
		return str + ")";
	}
}
