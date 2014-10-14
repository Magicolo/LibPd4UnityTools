using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[ExecuteInEditMode]
	public class AudioInfo : MonoBehaviour, INamable {

		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}

		public float fadeIn;
		public AnimationCurve fadeInCurve = new AnimationCurve(new []{ new Keyframe(0, 0), new Keyframe(1, 1) });
		public float fadeOut = 0.1F;
		public AnimationCurve fadeOutCurve = new AnimationCurve(new []{ new Keyframe(0, 1), new Keyframe(1, 0) });
		[Range(0, 1)] public float randomVolume;
		[Range(0, 6)] public float randomPitch;
		public bool doNotKill;
	
		AudioSource source;
		public AudioSource Source {
			get {
				if (source == null) {
					source = this.GetOrAddComponent<AudioSource>();
					source.playOnAwake = false;
				}
				return source;
			}
		}

		public AudioClip Clip {
			get {
				return Source.clip;
			}
			set {
				Source.clip = value;
			}
		}

		public ClipInfo clipInfo;
		
		void Awake() {
			clipInfo = new ClipInfo(this);
		}
		
		void Update() {
			if (!Application.isPlaying) {
				clipInfo.Update();
			}
		}
	}
}
