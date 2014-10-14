using UnityEngine;
using System.Collections;
using LibPDBinding;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDCommunicator : Magicolo.GeneralTools.IStartable, Magicolo.GeneralTools.IStoppable {
		
		public PDPlayer pdPlayer;
	
		public PDCommunicator(PDPlayer pdPlayer) {
			this.pdPlayer = pdPlayer;
		}
	
		public void Initialize() {
			SendValue("BufferSize", pdPlayer.bridge.bufferSize);
			SendValue("BufferAmount", pdPlayer.bridge.bufferAmount);
			SendValue("SampleRate", pdPlayer.bridge.sampleRate);
			SendValue("UMasterPlay", 1);
		}
	
		public void SubscribeDebug() {
			LibPD.Subscribe("Debug");
			LibPD.Bang += ReceiveDebugBang;
			LibPD.Float += ReceiveDebugFloat;
			LibPD.Symbol += ReceiveDebugSymbol;
			LibPD.List += ReceiveDebugList;
			LibPD.Message += ReceiveDebugMessage;
		}
	
		public void UnsubscribeDebug() {
			LibPD.Bang -= ReceiveDebugBang;
			LibPD.Float -= ReceiveDebugFloat;
			LibPD.Symbol -= ReceiveDebugSymbol;
			LibPD.List -= ReceiveDebugList;
			LibPD.Message -= ReceiveDebugMessage;
			LibPD.Unsubscribe("Debug");
		}
	
		public bool SendValue(string receiverName, object toSend) {
			int success = -1;
			
			if (toSend is int)
				success = LibPD.SendFloat(receiverName, (float)((int)toSend));
			else if (toSend is int[])
				success = LibPD.SendList(receiverName, ((int[])toSend).ToFloatArray());
			else if (toSend is float)
				success = LibPD.SendFloat(receiverName, (float)toSend);
			else if (toSend is float[])
				success = LibPD.SendList(receiverName, (float[])toSend);
			else if (toSend is double)
				success = LibPD.SendFloat(receiverName, (float)((double)toSend));
			else if (toSend is double[])
				success = LibPD.SendList(receiverName, ((double[])toSend).ToFloatArray());
			else if (toSend is bool)
				success = LibPD.SendFloat(receiverName, (float)((bool)toSend).GetHashCode());
			else if (toSend is bool[])
				success = LibPD.SendList(receiverName, ((bool[])toSend).ToFloatArray());
			else if (toSend is char)
				success = LibPD.SendSymbol(receiverName, ((char)toSend).ToString());
			else if (toSend is char[])
				success = LibPD.SendSymbol(receiverName, new string((char[])toSend));
			else if (toSend is string)
				success = LibPD.SendSymbol(receiverName, (string)toSend);
			else if (toSend is string[])
				success = LibPD.SendList(receiverName, (string[])toSend);
			else if (toSend is System.Enum)
				success = LibPD.SendFloat(receiverName, (float)(toSend.GetHashCode()));
			else if (toSend is System.Enum[])
				success = LibPD.SendList(receiverName, ((System.Enum[])toSend).ToFloatArray());
			else if (toSend is Vector2)
				success = LibPD.SendList(receiverName, ((Vector2)toSend).x, ((Vector2)toSend).y);
			else if (toSend is Vector3)
				success = LibPD.SendList(receiverName, ((Vector3)toSend).x, ((Vector3)toSend).y, ((Vector3)toSend).z);
			else if (toSend is Vector4)
				success = LibPD.SendList(receiverName, ((Vector4)toSend).x, ((Vector4)toSend).y, ((Vector4)toSend).z, ((Vector4)toSend).w);
			else if (toSend is Quaternion)
				success = LibPD.SendList(receiverName, ((Quaternion)toSend).x, ((Quaternion)toSend).y, ((Quaternion)toSend).z, ((Quaternion)toSend).w);
			else if (toSend is Rect)
				success = LibPD.SendList(receiverName, ((Rect)toSend).x, ((Rect)toSend).y, ((Rect)toSend).width, ((Rect)toSend).height);
			else if (toSend is Bounds)
				success = LibPD.SendList(receiverName, ((Bounds)toSend).center.x, ((Bounds)toSend).center.y, ((Bounds)toSend).size.x, ((Bounds)toSend).size.y);
			else if (toSend is Color)
				success = LibPD.SendList(receiverName, ((Color)toSend).r, ((Color)toSend).g, ((Color)toSend).b, ((Color)toSend).a);
			else {
				Debug.LogError("Invalid type to send to Pure Data: " + toSend);
			}
			
			return success == 0;
		}
	
		public bool SendBang(string receiverName) {
			return LibPD.SendBang(receiverName) == 0;
		}
	
		public bool SendMessage<T>(string receiverName, string message, params T[] arguments) {
			return LibPD.SendMessage(receiverName, message, arguments) == 0;
		}
	
		public bool SendAftertouch(int channel, int value) {
			return LibPD.SendAftertouch(channel, value) == 0;
		}
	
		public bool SendControlChange(int channel, int controller, int value) {
			return LibPD.SendControlChange(channel, controller, value) == 0;
		}
	
		public bool SendMidiByte(int port, int value) {
			return LibPD.SendMidiByte(port, value) == 0;
		}
	
		public bool SendNoteOn(int channel, int pitch, int velocity) {
			return LibPD.SendNoteOn(channel, pitch, velocity) == 0;
		}
	
		public bool SendPitchbend(int channel, int value) {
			return LibPD.SendPitchbend(channel, value) == 0;
		}
	
		public bool SendPolyAftertouch(int channel, int pitch, int value) {
			return LibPD.SendPolyAftertouch(channel, pitch, value) == 0;
		}
	
		public bool SendProgramChange(int channel, int value) {
			return LibPD.SendProgramChange(channel, value) == 0;
		}
	
		public bool SendSysex(int port, int value) {
			return LibPD.SendSysex(port, value) == 0;
		}
	
		public bool SendSysRealtime(int port, int value) {
			return LibPD.SendSysRealtime(port, value) == 0;
		}
	
		public bool WriteArray(string arrayName, int offset, float[] data, int amountOfValues) {
			if (!LibPD.Exists(arrayName)) {
				return false;
			}
			
			if (LibPD.ArraySize(arrayName) < amountOfValues) {
				ResizeArray(arrayName, amountOfValues);
			}
			
			int success = LibPD.WriteArray(arrayName, offset, data, amountOfValues);
			return success == 0;
		}
	
		public bool WriteArray(string arrayName, int offset, float[] data) {
			return WriteArray(arrayName, offset, data, data.Length);
		}
	
		public bool WriteArray(string arrayName, float[] data) {
			return WriteArray(arrayName, 0, data, data.Length);
		}
	
		public bool WriteArray(string arrayName, string soundName) {
			AudioClip clip = pdPlayer.infoManager.GetAudioInfo(soundName).Clip;
			float[] data = new float[clip.samples * clip.channels];
			clip.GetData(data, 0);
			return WriteArray(arrayName, 0, data, data.Length);
		}
	
		public bool ReadArray(string arrayName, float[] data) {
			return LibPD.ReadArray(data, arrayName, 0, data.Length) == 0;
		}

		public bool ResizeArray(string arrayName, int size) {
			return SendMessage(arrayName, "resize", size);
		}
		
		public void GetArraySize(string arrayName) {
			LibPD.ArraySize(arrayName);
		}
	
		void ReceiveDebugBang(string sendName) {
			if (sendName == "Debug")
				Debug.Log(string.Format("{0} received Bang", sendName));
		}
	
		void ReceiveDebugFloat(string sendName, float value) {
			if (sendName == "Debug")
				Debug.Log(string.Format("{0} received Float: {1}", sendName, value));
		}
	
		void ReceiveDebugSymbol(string sendName, string value) {
			if (sendName == "Debug")
				Debug.Log(string.Format("{0} received Symbol: {1}", sendName, value));
		}
	
		void ReceiveDebugList(string sendName, object[] value) {
			if (sendName == "Debug")
				Debug.Log(string.Format("{0} received List: {1}", sendName, Logger.ObjectToString(value)));
		}
	
		void ReceiveDebugMessage(string sendName, string message, object[] value) {
			if (sendName == "Debug")
				Debug.Log(string.Format("{0} received Message: {1} {2}", sendName, message, Logger.ObjectToString(value)));
		}
	
	
		public void Start() {
			SubscribeDebug();
			SendValue("UMasterPlay", 1);
		}
	
		public void Stop() {
			SendValue("UMasterStop", 0);
			UnsubscribeDebug();
		}
	}
}
