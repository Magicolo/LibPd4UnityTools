using System.Collections.Generic;
using UnityEngine;
using Candlelight;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	public class Metronome {

		double beatsPerMinute = 120;
		public double BeatsPerMinute {
			get {
				return beatsPerMinute;
			}
			set {
				beatsPerMinute = value;
				beatDuration = 60D / beatsPerMinute;
				measureDuration = (60D / beatsPerMinute) * beatsPerMeasure;
			}
		}
		
		int beatsPerMeasure = 4;
		public int BeatsPerMeasure {
			get {
				return beatsPerMeasure;
			}
			set {
				beatsPerMeasure = value;
				beatDuration = 60D / beatsPerMinute;
				measureDuration = (60D / beatsPerMinute) * beatsPerMeasure;
			}
		}

		int currentBeat;
		public int CurrentBeat { 
			get {
				return currentBeat;
			}
		}
		
		int currentMeasure;
		public int CurrentMeasure { 
			get {
				return currentMeasure;
			}
		}
	
		bool isPlaying;
		public bool IsPlaying {
			get {
				return isPlaying;
			}
		}
		
		double nextBeatTime;
		double nextMeasureTime;
		double beatDuration;
		double measureDuration;
		List<ITickable> tickables = new List<ITickable>();
		IEnumerator ticker;
	
		public Metronome(double beatsPerMinute, int beatsPerMeasure) {
			this.BeatsPerMinute = beatsPerMinute;
			this.BeatsPerMeasure = beatsPerMeasure;
		}

		public void Start() {
			isPlaying = true;
			
			currentBeat = 0;
			currentMeasure = 0;
			nextBeatTime = AudioSettings.dspTime;
			nextMeasureTime = AudioSettings.dspTime;
			ticker = Tick();
		}
		
		public void Stop() {
			isPlaying = false;
			
		}
		
		public void Update() {
			if (isPlaying) {
				ticker.MoveNext();
			}
		}
		
		public void Subscribe(ITickable tickable) {
			tickables.Add(tickable);
		}
		
		public void Unsubscribe(ITickable tickable) {
			tickables.Remove(tickable);
		}
		
		void BeatEvent() {
			foreach (ITickable tickable in tickables) {
				tickable.BeatEvent(CurrentBeat);
			}
		}

		void MeasureEvent() {
			foreach (ITickable tickable in tickables) {
				tickable.MeasureEvent(CurrentMeasure);
			}
		}
	
		IEnumerator Tick() {
			while (true) {
				double currentTime = AudioSettings.dspTime;
				if (currentTime >= nextBeatTime) {
					if (CurrentBeat == 0) {
						currentMeasure += 1;
						nextMeasureTime += measureDuration;
						MeasureEvent();
					}
				
					currentBeat = (CurrentBeat + 1) % beatsPerMeasure;
					nextBeatTime += beatDuration;
					BeatEvent();
				}
				yield return null;
			}
		}
	}
}
