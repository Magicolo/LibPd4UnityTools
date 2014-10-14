using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class SingleAudioItem : AudioItem {

		public AudioSource audioSource;
		public Magicolo.AudioTools.AudioInfo audioInfo;
		public GameObject gameObject;
		public CoroutineHolder coroutineHolder;
		public GainManager gainManager;
		
		States pausedState;
		
		public SingleAudioItem(string name, int id, AudioSource audioSource, AudioInfo audioInfo, GameObject gameObject, CoroutineHolder coroutineHolder, GainManager gainManager, AudioItemManager itemManager, Magicolo.AudioTools.Player player)
			: base(name, id, itemManager, player) {
			
			this.audioSource = audioSource;
			this.audioInfo = audioInfo;
			this.gameObject = gameObject;
			this.coroutineHolder = coroutineHolder;
			this.gainManager = gainManager;
		}
		
		public override void Update() {
			if (!audioSource.loop) {
				if ((audioSource.pitch > 0 && audioSource.time >= audioSource.clip.length - audioInfo.fadeOut) || (audioSource.pitch < 0 && audioSource.time <= audioInfo.fadeOut)) {
					Stop();
				}
			}
			gameObject.name = string.Format("{0} ({1})", Name, State);
		}
		
		public virtual void UpdateVolume() {
			gainManager.volume = Volume * player.audioSettings.masterVolume;
		}
		
		public override void Play() {
			if (State == States.StandingBy) {
				//HACK Trick to deal with reversed sounds.
				if (audioSource.pitch < 0) {
					audioSource.time = audioSource.clip.length - 0.00001f;
				}
				coroutineHolder.AddCoroutine("FadeIn", FadeIn(audioSource.volume, audioInfo.fadeIn, audioInfo.fadeInCurve));
			}
			else if (State == States.Paused) {
				audioSource.Play();
				coroutineHolder.ResumeCoroutines("FadeIn");
				coroutineHolder.ResumeCoroutines("FadeVolume");
				State = pausedState;
			}
		}

		public override void Pause() {
			if (State == States.Playing || State == States.FadingIn) {
				audioSource.Pause();
				coroutineHolder.PauseCoroutines("FadeIn");
				coroutineHolder.PauseCoroutines("FadeVolume");
				pausedState = State;
				base.Pause();
			}
		}

		public override void Stop() {
			if (State != States.Stopped || State != States.FadingOut) {
				coroutineHolder.AddCoroutine("FadeOut", FadeOut(0, audioInfo.fadeOut, audioInfo.fadeOutCurve));
			}
		}

		public override void StopImmediate() {
			if (State != States.Stopped) {
				base.Stop();
				audioSource.Stop();
				gainManager.Deactivate();
				itemManager.Deactivate(this);
				coroutineHolder.RemoveAllCoroutines();
			}
		}
		
		public override void SetVolume(float targetVolume, float time) {
			coroutineHolder.RemoveCoroutines("FadeVolume");
			coroutineHolder.AddCoroutine("FadeVolume", FadeVolume(gainManager.volume, Mathf.Clamp(targetVolume, 0, 10), time));
		}

		public override void SetVolume(float targetVolume) {
			coroutineHolder.RemoveCoroutines("FadeVolume");
			
			Volume = targetVolume;
			gainManager.volume = Volume;
		}

		#region IEnumerators
		public virtual IEnumerator FadeVolume(float startVolume, float targetVolume, float time) {
			float counter = 0;
			
			while (counter < time) {
				Volume = ((counter / time) * (targetVolume - startVolume) + startVolume);
				gainManager.volume = Volume * player.audioSettings.masterVolume;
				counter += Time.deltaTime;
				yield return new WaitForSeconds(0);
			}
			
			Volume = targetVolume;
			gainManager.volume = Volume * player.audioSettings.masterVolume;
		}
		
		public virtual IEnumerator FadeIn(float targetVolume, float time, AnimationCurve curve) {
			State = States.FadingIn;
			audioSource.Play();
			gainManager.Activate();
			
			IEnumerator fade = Fade(audioSource.volume, targetVolume, time, curve);
			while (fade.MoveNext()) {
				yield return fade.Current;
			}
			
			base.Play();
		}
		
		public virtual IEnumerator FadeOut(float targetVolume, float time, AnimationCurve curve) {
			State = States.FadingOut;
			coroutineHolder.RemoveCoroutines("FadeIn");
			
			IEnumerator fade = Fade(audioSource.volume, targetVolume, time, curve);
			while (fade.MoveNext()) {
				yield return fade.Current;
			}
			
			base.Stop();
			audioSource.Stop();
			gainManager.Deactivate();
			itemManager.Deactivate(this);
			coroutineHolder.RemoveAllCoroutines();
		}

		public virtual IEnumerator Fade(float startVolume, float targetVolume, float time, AnimationCurve curve) {
			float counter = 0;
			
			while (counter < time) {
				float fadeVolume = curve.Evaluate(counter / time);
				audioSource.volume = fadeVolume * startVolume;
				counter += Time.deltaTime;
				yield return new WaitForSeconds(0);
			}
			audioSource.volume = targetVolume;
		}
		#endregion
	}
}
