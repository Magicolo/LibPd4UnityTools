using UnityEngine;

/* 
 * You will need to first follow these steps in order to make everything work properly:
 * 
 * 1- In the menus, click Magicolo's Tools/Create/AudioPlayer
 * 
 * 2- Add sounds to the Resources folder (Assets/Resources/) Notice that the AudioPlayer will
 * create a hierarchy reprensenting your sounds where you can change their settings.
 * 
 * 3- Check out the example.pd patch for more information.
*/

public class PDPlayerExample : MonoBehaviour {

	public string soundName;
	AudioItem audioItem;
	
	void Awake () {
		// You first need to open a patch.
		PDPlayer.OpenPatch("example");
		
		// You will need to change the soundName part of the [ureceive~ Test_soundName] object in 
		// the test.pd patch to correspond to the soundName of this method.
		// The audioItem variable will let us control the module that is playing.
		audioItem = PDPlayer.Play("Test", soundName, gameObject);
		
		// Because the module is spatialized around the GameObject that holds this script, you
		// can move it around to test the spatialization settings.
		
		// This is just to show how you can control a module using the AudioItem returned from the
		// PDPlayer.Play method.
		Invoke("StopAudioItem", 5);
	}
	
	void StopAudioItem(){
		audioItem.Stop();
		
		// You can close a patch if it is no longer needed.
		PDPlayer.ClosePatch("example");
	}
}
