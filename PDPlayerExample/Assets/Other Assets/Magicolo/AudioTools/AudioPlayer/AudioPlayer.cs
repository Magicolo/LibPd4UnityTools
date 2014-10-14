using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;

[ExecuteInEditMode]
public class AudioPlayer : Magicolo.AudioTools.Player {

	static AudioPlayer instance;
	static AudioPlayer Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<AudioPlayer>();
			}
			return instance;
		}
	}
	
	#region Components
	public AudioHierarchyManager hierarchyManager;
	public AudioPlayerEditorHelper editorHelper;
	public AudioPlayerItemManager itemManager;
	#endregion
	
	protected override void Awake() {
		base.Awake();
		
		this.SetExecutionOrder(-11);
		editorHelper = editorHelper ?? new AudioPlayerEditorHelper(Instance);
		editorHelper.Subscribe();
		
		audioSettings = new GeneralAudioSettings(Instance);
		hierarchyManager = hierarchyManager ?? new AudioHierarchyManager(Instance);
		
		audioSettings.Start();
		
		if (Application.isPlaying) {
			hierarchyManager.Start();
			itemManager = new AudioPlayerItemManager(Instance);
		}
	}
	
	protected override void Update() {
		base.Update();
		
		if (Application.isPlaying) {
			itemManager.Update();
		}
		else {
			audioSettings.Update();
			hierarchyManager.Update();
			editorHelper.Update();
		}
	}
	
	/// <summary>
	/// Plays an audio source spatialized around the <paramref name="source"/>.
	/// </summary>
	/// <param name="soundName">The name of sound to be played.</param>
	/// <param name="source">The source around which the audio source will be spatialized.</param>
	/// <returns>The AudioItem that will let you control the audio source.</returns>
	public static AudioItem Play(string soundName, GameObject source) {
		return instance.itemManager.Play(soundName, source);
	}
	
	/// <summary>
	/// Plays an audio source spatialized around the listener.
	/// </summary>
	/// <param name="soundName">The name of sound to be played.</param>
	/// <returns>The AudioItem that will let you control the audio source.</returns>
	public static AudioItem Play(string soundName) {
		return instance.itemManager.Play(soundName, null);
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
}
