using UnityEngine;
using Magicolo.AudioTools;
using System.Collections;

//TODO Add sends in the inspector.

[RequireComponent(typeof(AudioPlayer))]
[ExecuteInEditMode]
public class PDPlayer : Magicolo.AudioTools.Player {

	public string patchesPath = "Patches";
	
	static PDPlayer instance;
	static PDPlayer Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<PDPlayer>();
			}
			return instance;
		}
	}

	#region Components
	public PDEditorHelper editorHelper;
	public PDBridge bridge;
	public PDCommunicator communicator;
	public PDPatchManager patchManager;
	public PDAudioItemManager itemManager;
	public PDAudioFilterRead filterRead;
	#endregion

	protected override void Awake() {
		base.Awake();
		
		this.SetExecutionOrder(-10);
		editorHelper = editorHelper ?? new PDEditorHelper(Instance);
		editorHelper.Subscribe();
		
		if (Application.isPlaying) {
			bridge = new PDBridge(Instance);
			communicator = new PDCommunicator(Instance);
			patchManager = new PDPatchManager(Instance);
			itemManager = new PDAudioItemManager(Instance);
			
			listener.enabled = false;
			filterRead = listener.GetOrAddComponent<PDAudioFilterRead>();
			filterRead.pdPlayer = Instance;
			listener.enabled = true;
		
			bridge.Start();
			communicator.Start();
			patchManager.Start();
		}
	}
	
	protected override void Update() {
		base.Update();
		
		if (Application.isPlaying) {
			itemManager.Update();
		}
		else {
			editorHelper.Update();
		}
	}
	
	void OnApplicationQuit() {
		if (Application.isPlaying) {
			patchManager.Stop();
			bridge.Stop();
			communicator.Stop();
		}
	}
	
	void OnDrawGizmos() {
		editorHelper.DrawGizmos();
	}
	
	/// <summary>
	/// Plays a module with an audio source spatialized around the <paramref name="source"/>. In Pure Data, you can receive the play command (float 1) with a <c>[receive <paramref name="moduleName"/>_Play]</c>.
	/// </summary>
	/// <param name="moduleName">The name of the module to be played. If the module doesn't exist, one will be created with the default settings.</param>
	/// <param name="soundName">The name of sound to be played. In Pure Data, use <c>[ureceive~ <paramref name="moduleName"/>_<paramref name="soundName"/>]</c> to receive the audio. Do not send this audio signal through a <c>[uspatialize~]</c> because it is already spatialized.</param>
	/// <param name="source">The source around which the module and audio source will be spatialized. If a source is already provided in the inspector, it will be overriden. In Pure Data, send the audio that you want spatialized through a <c>[uspatialize~ <paramref name="moduleName"/>]</c>.</param>
	/// <returns>The AudioItem that will let you control the module.</returns>
	public static AudioItem Play(string moduleName, string soundName, GameObject source) {
		return instance.itemManager.Play(moduleName, soundName, source);
	}
	
	/// <summary>
	/// Plays a module with an audio source spatialized around the listener. In Pure Data, you can receive the play command (float 1) with a <c>[receive <paramref name="moduleName"/>_Play]</c>.
	/// </summary>
	/// <param name="moduleName">The name of the module to be played. If the module doesn't exist, one will be created with the default settings.</param>
	/// <param name="soundName">The name of sound to be played. In Pure Data, use <c>[ureceive~ <paramref name="moduleName"/>_<paramref name="soundName"/>]</c> to receive the audio. Do not send this audio signal through a <c>[uspatialize~]</c> because it is already spatialized.</param>
	/// <returns>The AudioItem that will let you control the module.</returns>
	public static AudioItem Play(string moduleName, string soundName) {
		return instance.itemManager.Play(moduleName, soundName);
	}
	
	/// <summary>
	/// Plays a module with an audio source spatialized around the <paramref name="source"/>. In Pure Data, you can receive the play command (float 1) with a <c>[receive <paramref name="moduleName"/>_Play]</c>.
	/// </summary>
	/// <param name="moduleName">The name of the module to be played. If the module doesn't exist, one will be created with the default settings.</param>
	/// <param name="source">The source around which the module will be spatialized. If a source is already provided in the inspector, it will be overriden. In Pure Data, send the audio that you want spatialized through a <c>[uspatialize~ <paramref name="moduleName"/>]</c>.</param>
	/// <returns>The AudioItem that will let you control the module.</returns>
	public static AudioItem Play(string moduleName, GameObject source) {
		return instance.itemManager.Play(moduleName, source);
	}
	
	/// <summary>
	/// Plays a module with an audio source spatialized around the listener. In Pure Data, you can receive the play command (float 1) with a <c>[receive <paramref name="moduleName"/>_Play]</c>.
	/// </summary>
	/// <param name="moduleName">The name of the module to be played. If the module doesn't exist, one will be created with the default settings.</param>
	/// <returns>The AudioItem that will let you control the module.</returns>
	public static AudioItem Play(string moduleName) {
		return instance.itemManager.Play(moduleName);
	}

	/// <summary>
	/// Pauses any audio signals going through a <c>[uspatialize~ <paramref name="moduleName"/>]</c> and any AudioSource connected to the module. In Pure Data, you can receive the pause command (float 0) with a <c>[receive <paramref name="moduleName"/>_Pause]</c>.
	/// </summary>
	/// <param name="moduleName">The name of the module to be paused.</param>
	public static void Pause(string moduleName) {
		instance.itemManager.Pause(moduleName);
	}

	/// <summary>
	/// Stops any audio signals going through a <c>[uspatialize~ <paramref name="moduleName"/>]</c> and any AudioSource connected to the module. In Pure Data, you can receive the stop command (float 0) with a <c>[receive <paramref name="moduleName"/>_Stop]</c>.
	/// </summary>
	/// <param name="moduleName">The name of the module to be stopped.</param>
	public static void Stop(string moduleName) {
		instance.itemManager.Stop(moduleName);
	}

	/// <summary>
	/// Gets the volume of a module.
	/// </summary>
	/// <param name="moduleName">The name of the module from which to get the volume.</param>
	/// <returns>The volume.</returns>
	public static float GetVolume(string moduleName) {
		return instance.itemManager.GetVolume(moduleName);
	}

	/// <summary>
	/// Ramps the volume of a module. In Pure Data, you can receive the volume command (float <paramref name="targetVolume"/>) with a <c>[receive <paramref name="moduleName"/>_Volume]</c>.
	/// </summary>
	/// <param name="moduleName">The name of the module from which to ramp the volume.</param>
	/// <param name="targetVolume">The target to which the volume will be ramped.</param>
	/// <param name="time">The time it will take for the volume to reach the <paramref name="targetVolume"/>.</param>
	public static void SetVolume(string moduleName, float targetVolume, float time) {
		instance.itemManager.SetVolume(moduleName, targetVolume, time);
	}

	/// <summary>
	/// Sets the volume of a module. In Pure Data, you can receive the volume command (float <paramref name="targetVolume"/>) with a <c>[receive <paramref name="moduleName"/>_Volume]</c>.
	/// </summary>
	/// <param name="moduleName">The name of the module from which to set the volume.</param>
	/// <param name="targetVolume">The target to which the volume will be set.</param>
	public static void SetVolume(string moduleName, float targetVolume) {
		instance.itemManager.SetVolume(moduleName, targetVolume);
	}

	/// <summary>
	/// Gets the master volume of the PDPlayer.
	/// </summary>
	/// <returns>The master volume.</returns>
	public static float GetMasterVolume() {
		return instance.audioSettings.masterVolume;
	}
	
	/// <summary>
	/// Ramps the master volume of the PDPlayer. In Pure Data, you can receive the volume command (float <paramref name="targetVolume"/>) with a <c>[receive <paramref name="moduleName"/>_Volume]</c>.
	/// </summary>
	/// <param name="targetVolume">The target to which the volume will be ramped.</param>
	/// <param name="time">The time it will take for the volume to reach the <paramref name="targetVolume"/>.</param>
	public static void SetMasterVolume(float targetVolume, float time) {
		instance.itemManager.SetMasterVolume(targetVolume, time);
	}
	
	/// <summary>
	/// Sets the master volume of the PDPlayer. In Pure Data, you can receive the volume command (float <paramref name="targetVolume"/>) with a <c>[receive <paramref name="moduleName"/>_Volume]</c>.
	/// </summary>
	/// <param name="targetVolume">The target to which the volume will be set.</param>
	public static void SetMasterVolume(float targetVolume) {
		instance.itemManager.SetMasterVolume(targetVolume);
	}
	
	#region Send
	/// <summary>
	/// Converts and sends a value to Pure Data. In Pure Data, you can receive the value with a <c>[receive <paramref name="receiverName"/>]</c>.
	/// </summary>
	/// <param name="receiverName">The name of to be used in Pure Data to receive the value.</param>
	/// <param name="toSend">The value to be sent. Valid types include int, int[] float, float[], double, double[], bool, bool[], char, char[], string, string[], Enum, Enum[], Vector2, Vector3, Vector4, Quaternion, Rect, Bounds and Color.</param>
	/// <returns>True if the value has been successfully sent and received.</returns>
	public static bool SendValue(string receiverName, object toSend) {
		return Instance.communicator.SendValue(receiverName, toSend);
	}
	
	/// <summary>
	/// Sends a bang to Pure Data. In Pure Data, you can receive the bang with a <c>[receive <paramref name="receiverName"/>]</c>.
	/// </summary>
	/// <param name="receiverName">The name of to be used in Pure Data to receive the value.</param>
	/// <returns>True if the bang has been successfully sent and received.</returns>
	public static bool SendBang(string receiverName) {
		return Instance.communicator.SendBang(receiverName);
	}
	
	/// <summary>
	/// Sends a message to Pure Data. In Pure Data, you can receive the message with a <c>[receive <paramref name="receiverName"/>]</c>.
	/// </summary>
	/// <param name="receiverName">The name of to be used in Pure Data to receive the value.</param>
	/// <param name="message">The message to be sent.</param>
	/// <param name="arguments">Additional arguments can be added to the message. Valid types include int, float, string.</param>
	/// <returns>True if the message has been successfully sent and received.</returns>
	public static bool SendMessage<T>(string receiverName, string message, params T[] arguments) {
		return Instance.communicator.SendMessage(receiverName, message, arguments);
	}
	
	/// <summary>
	/// Sends a aftertouch event to Pure Data. In Pure Data, you can receive the aftertouch event with a <c>[touchin]</c>.
	/// </summary>
	/// <param name="channel">The channel to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the aftertouch event has been successfully sent and received.</returns>
	public static bool SendAftertouch(int channel, int value) {
		return Instance.communicator.SendAftertouch(channel, value);
	}
	
	/// <summary>
	/// Sends a control change event to Pure Data. In Pure Data, you can receive the control change event with a <c>[ctlin]</c>.
	/// </summary>
	/// <param name="channel">The channel to be sent.</param>
	/// <param name="controller">The controller to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the control change event has been successfully sent and received.</returns>
	public static bool SendControlChange(int channel, int controller, int value) {
		return Instance.communicator.SendControlChange(channel, controller, value);
	}
	
	/// <summary>
	/// Sends a raw midi byte to Pure Data. In Pure Data, you can receive the raw midi byte with a <c>[midiin]</c>.
	/// </summary>
	/// <param name="port">The port to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the raw midi byte has been successfully sent and received.</returns>
	public static bool SendMidiByte(int port, int value) {
		return Instance.communicator.SendMidiByte(port, value);
	}
	
	/// <summary>
	/// Sends a note on event to Pure Data. In Pure Data, you can receive the note on event with a <c>[notein]</c>.
	/// </summary>
	/// <param name="channel">The channel to be sent.</param>
	/// <param name="pitch">The pitch to be sent.</param>
	/// <param name="velocity">The velocity to be sent.</param>
	/// <returns>True if the note on event has been successfully sent and received.</returns>
	public static bool SendNoteOn(int channel, int pitch, int velocity) {
		return Instance.communicator.SendNoteOn(channel, pitch, velocity);
	}
	
	/// <summary>
	/// Sends a pitch bend event to Pure Data. In Pure Data, you can receive the pitch bend event with a <c>[bendin]</c>.
	/// </summary>
	/// <param name="channel">The channel to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the pitch bend event has been successfully sent and received.</returns>
	public static bool SendPitchbend(int channel, int value) {
		return Instance.communicator.SendPitchbend(channel, value);
	}
	
	/// <summary>
	/// Sends a polyphonic aftertouch event to Pure Data. In Pure Data, you can receive the polyphonic aftertouch event with a <c>[polytouchin]</c>.
	/// </summary>
	/// <param name="channel">The channel to be sent.</param>
	/// <param name="pitch">The pitch to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the polyphonic aftertouch event has been successfully sent and received.</returns>
	public static bool SendPolyAftertouch(int channel, int pitch, int value) {
		return Instance.communicator.SendPolyAftertouch(channel, pitch, value);
	}
	
	/// <summary>
	/// Sends a program change event to Pure Data. In Pure Data, you can receive the program change event with a <c>[pgmin]</c>.
	/// </summary>
	/// <param name="channel">The channel to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the program change event has been successfully sent and received.</returns>
	public static bool SendProgramChange(int channel, int value) {
		return Instance.communicator.SendProgramChange(channel, value);
	}
	
	/// <summary>
	/// Sends a byte of a sysex message to Pure Data. In Pure Data, you can receive the byte of the sysex message with a <c>[sysexin]</c>.
	/// </summary>
	/// <param name="port">The port to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the byte of the sysex message has been successfully sent and received.</returns>
	public static bool SendSysex(int port, int value) {
		return Instance.communicator.SendSysex(port, value);
	}
	
	/// <summary>
	/// Sends a byte to Pure Data. In Pure Data, you can receive the byte with a <c>[realtimein]</c>.
	/// </summary>
	/// <param name="port">The port to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the byte has been successfully sent and received.</returns>
	public static bool SendSysRealtime(int port, int value) {
		return Instance.communicator.SendSysRealtime(port, value);
	}
	
	/// <summary>
	/// Writes an audio clip to a Pure Data array (the array will be resized if needed). In Pure Data, you can receive the data with a <c>[table <paramref name="arrayName"/>]</c>.
	/// </summary>
	/// <param name="arrayName">The name of the array that will receive the data.</param>
	/// <param name="soundName">The name of the sound to be written to the array.</param>
	/// <returns>True if the data has been successfully sent and received.</returns>
	public static bool WriteArray(string arrayName, string soundName) {
		return Instance.communicator.WriteArray(arrayName, soundName);
	}
	
	/// <summary>
	/// Writes an audio clip to a Pure Data array (the array will be resized if needed). In Pure Data, you can receive the data with a <c>[table <paramref name="arrayName"/>]</c>.
	/// </summary>
	/// <param name="arrayName">The name of the array that will receive the data.</param>
	/// <param name="data">The data to be written to the array.</param>
	/// <returns>True if the data has been successfully sent and received.</returns>
	public static bool WriteArray(string arrayName, float[] data) {
		return Instance.communicator.WriteArray(arrayName, data);
	}
	
	/// <summary>
	/// Resizes a Pure Data array to a new size. Can be used to free memory.
	/// </summary>
	/// <param name="arrayName">The name of the array to be resized.</param>
	/// <param name="size">The target size of the array.</param>
	/// <returns>True if the array has been successfully resized.</returns>
	public static bool ResizeArray(string arrayName, int size) {
		return Instance.communicator.ResizeArray(arrayName, size);
	}
	#endregion
	
	/// <summary>
	/// Opens a patch and starts the DSP.
	/// </summary>
	/// <param name="patchName">The name of the patch (without the extension) to be opened relative to <c>Assets/Resources/<paramref name="patchesPath"/>/</c></param>.
	public static void OpenPatch(string patchName) {
		Instance.patchManager.Open(patchName);
	}
	
	/// <summary>
	/// Opens patches and starts the DSP.
	/// </summary>
	/// <param name="patchesName">The name of the patches (without the extension) to be opened relative to <c>Assets/Resources/<paramref name="patchesPath"/>/</c></param>
	public static void OpenPatches(params string[] patchesName) {
		Instance.patchManager.Open(patchesName);
	}
	
	/// <summary>
	/// Closes a patch.
	/// </summary>
	/// <param name="patchName">The name of the patch (without the extension and directory) to be closed.</param>
	public static void ClosePatch(string patchName) {
		Instance.patchManager.Close(patchName);
	}
	
	/// <summary>
	/// Closes patches.
	/// </summary>
	/// <param name="patchesName">The name of the patches (without the extension and directory) to be closed.</param>
	public static void ClosePatches(params string[] patchesName) {
		Instance.patchManager.Close(patchesName);
	}
	
	/// <summary>
	/// Closes all opened patches.
	/// </summary>
	public static void CloseAllPatches() {
		Instance.patchManager.CloseAll();
	}
}
