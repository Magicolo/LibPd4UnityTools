using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;
using Magicolo.GeneralTools;
using Candlelight;

[System.Serializable]
public abstract class AudioItem : Magicolo.GeneralTools.INamable {

	public enum States {
		StandingBy,
		Playing,
		Paused,
		Stopped,
		FadingIn,
		FadingOut
	}
	
	[SerializeField, PropertyBackingField(typeof(AudioItem), "Name")]
	string name;
	public string Name { 
		get { 
			return name; 
		}
		set { 
			name = value;
		}
	}
	
	[SerializeField, PropertyBackingField(typeof(AudioItem), "Id")]
	int id;
	public int Id {
		get {
			return id;
		}
		set {
			id = value;
		}
	}
	
	[SerializeField, PropertyBackingField(typeof(AudioItem), "Volume", typeof(RangeAttribute), 0, 5)]
	float volume = 1;
	public float Volume {
		get {
			return volume;
		}
		set {
			volume = value;
		}
	}
	
	[SerializeField, PropertyBackingField(typeof(AudioItem), "State")]
	States state = States.StandingBy;
	public States State {
		get {
			return state;
		}
		set {
			state = value;
		}
	}

	protected AudioItemManager itemManager;
	protected Magicolo.AudioTools.Player player;
	
	protected AudioItem(string name, int id, float volume, AudioItem.States state, AudioItemManager itemManager, Magicolo.AudioTools.Player player) {
		this.Name = name;
		this.Id = id;
		this.Volume = volume;
		this.State = state;
		this.itemManager = itemManager;
		this.player = player;
	}
	
	protected AudioItem(string name, int id, AudioItemManager itemManager, Magicolo.AudioTools.Player player) {
		this.Name = name;
		this.Id = id;
		this.itemManager = itemManager;
		this.player = player;
	}
	
	public abstract void Update();
		
	public virtual void Play() {
		State = States.Playing;
	}
	
	public virtual void Pause() {
		State = States.Paused;
	}
	
	public virtual void Stop() {
		State = States.Stopped;
	}

	public virtual void StopImmediate() {
		State = States.Stopped;
	}

	public virtual float GetVolume() {
		return Volume;
	}

	public abstract void SetVolume(float targetVolume, float time);
	
	public abstract void SetVolume(float targetVolume);
}
