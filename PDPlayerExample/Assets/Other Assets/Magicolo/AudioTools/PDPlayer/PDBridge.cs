using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using LibPDBinding;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDBridge : Magicolo.GeneralTools.IStartable, Magicolo.GeneralTools.IStoppable {
	
		public int sampleRate;
		public int bufferSize;
		public int bufferAmount;
		public bool initialized;
		public int ticks;
		public PDPlayer pdPlayer;
	
	
		public PDBridge(PDPlayer pdPlayer) {
			this.pdPlayer = pdPlayer;
		}
	
		public void StartLibPD() {
			ResolvePath();
			SetAudioSettings();
			OpenAudio();
		}
	
		public void StopLibPD() {
			initialized = false;
			LibPD.Release();
		}

		void OpenAudio() {
			if (LibPD.OpenAudio(2, 2, sampleRate) == 0) {
				initialized = true;
			}
			else {
				Debug.LogError("Failed to start LibPD.");
			}
		}

		void SetAudioSettings() {
			AudioSettings.GetDSPBufferSize(out bufferSize, out bufferAmount);
			sampleRate = AudioSettings.outputSampleRate;
			ticks = bufferSize / LibPD.BlockSize;
		}
	
		void ResolvePath() {
			string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
			string dllPath = Application.dataPath + "/" + "Plugins";
			dllPath.Replace('/', Path.DirectorySeparatorChar);
		
			if (!currentPath.Contains(dllPath)) {
				Environment.SetEnvironmentVariable("PATH", currentPath + Path.PathSeparator + dllPath, EnvironmentVariableTarget.Process);
			}
		}
	
	
		public void Start() {
			StartLibPD();
		}

		public void Stop() {
			StopLibPD();
		}
	}
}
