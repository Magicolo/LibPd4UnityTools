LibPd4UnityTools
================

Set of tools to facilitate the communication between LibPd and the Unity game engine.


---PDPlayer---

Features:
	-Gives access to simple controls to interact with LibPD.
	-Lets you send Unity AudioSources to Pure Data.
	-Lets you spatialize audio signals generated in Pure Data.
	-Uses pooled objects to minimize memory allocation.
	-Clean UI in the hierarchy window/inspector to preset settings for a module or settings for an AudioSource.
	-Multiple AudioSource settings editing.
	-Everything is static, so no need to instantiate anything (ex: PDPlayer.Play(something))
	-Most of the useful documentation is present
	-Help patches for custom Pure Data objects
	-Example scene with an example Pure Data patch

Notes:
	-Depends on the Property Backing Field package from Candlelight
	-Has only been tested on Windows