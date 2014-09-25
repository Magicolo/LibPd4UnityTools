using UnityEngine;
using LibPDBinding;
using System.Collections;

public class AudioSendToLibPdExample : MonoBehaviour {

	void Awake() {
		int sampleRate;
		int bufferSize;
		int bufferAmount;
		
		sampleRate = AudioSettings.outputSampleRate;
		AudioSettings.GetDSPBufferSize(out bufferSize, out bufferAmount);
		
		LibPD.SendFloat("BufferSize", bufferSize);
		LibPD.SendFloat("BufferAmount", bufferAmount);
		LibPD.SendFloat("SampleRate", sampleRate);
	}
	
	void OnAudioFilterRead(float[] data, int channels) {
		LibPD.SendList("Test", data);
		
		for (int i = 0; i < data.Length; i++) {
			data[i] = 0;
		}
	}
}
