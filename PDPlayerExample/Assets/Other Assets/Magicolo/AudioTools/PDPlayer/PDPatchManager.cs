using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using LibPDBinding;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDPatchManager : Magicolo.GeneralTools.IStartable, Magicolo.GeneralTools.IStoppable {

		public PDPlayer pdPlayer;
	
		Dictionary<string, int> patches = new Dictionary<string, int>();

		public PDPatchManager(PDPlayer pdPlayer) {
			this.pdPlayer = pdPlayer;
		}
	
		public void Open(params string[] patchesName) {
			foreach (string patchName in patchesName) {
				string path = GetPatchPath(patchName);
				patches[Path.GetFileName(patchName)] = LibPD.OpenPatch(path);
				pdPlayer.communicator.Initialize();
				pdPlayer.itemManager.Initialize();
			}
			LibPD.ComputeAudio(true);
		}
	
		public void Close(params string[] patchesName) {
			foreach (string patchName in patchesName) {
				if (patches.ContainsKey(patchName)) {
					LibPD.ClosePatch(patches[patchName]);
					patches.Remove(patchName);
				}
			}
		}
		
		public void CloseAll() {
			foreach (string key in new List<string>(patches.Keys)) {
				Close(key);
			}
		}

		public bool IsOpened(string patchName) {
			return patches.ContainsKey(patchName);
		}
		
		string GetPatchPath(string patchName) {
			string path = Application.streamingAssetsPath + Path.AltDirectorySeparatorChar + pdPlayer.patchesPath + Path.AltDirectorySeparatorChar + patchName + ".pd";
		
			#if UNITY_ANDROID && !UNITY_EDITOR
			string patchJar = Application.persistentDataPath + "/" + patchName;
			
			if (File.Exists(patchJar))
			{
				Debug.Log("Patch already unpacked");
				File.Delete(patchJar);
				
				if (File.Exists(patchJar))
				{
					Debug.Log("Couldn't delete");				
				}
			}
			
			WWW dataStream = new WWW(path);
			
			
			// Hack to wait till download is done
			while(!dataStream.isDone) 
			{
			}
			
			if (!String.IsNullOrEmpty(dataStream.error))
			{
				Debug.Log("### WWW ERROR IN DATA STREAM:" + dataStream.error);
			}
			
			File.WriteAllBytes(patchJar, dataStream.bytes);
			
			path = patchJar;
			#endif
		
			return path;
		}

		public void Start() {
			LibPD.OpenPatch(pdPlayer.folderPath + "PDPlayer" + Path.AltDirectorySeparatorChar + "initialize~.pd");
			LibPD.ComputeAudio(true);
		}
		
		public void Stop() {
			CloseAll();
		}
	}
}