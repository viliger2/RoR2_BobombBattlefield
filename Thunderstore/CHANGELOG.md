<details>
<summary>1.1.0 </summary>

* 1.3.9 update.
* Added King Bob-omb boss item: Royal Crown.
* Removed separate stage spawn configs for Bob-ombs, now there is a single config where you list stages that it can spawn in by coma.
</details>
<details>
<summary>1.0.8 </summary>

* Fixed an issue with MiniMapMod dropping performance to a crawl.
</details>
<details>
<summary>1.0.7 </summary>

* Removed diorama under the stage.
	* _Some people thought it was an easter egg, its not. I just forgot to remove it from the scene after I made it._
* Added sound effect on using teleporter to leave the stage and added music exit cue on teleporter being charged, similar to vanilla stages.
* Added missing dependencies.
</details>
<details>
<summary>1.0.6 </summary>

* Added log entry for the stage.
* Fixed some localization strings.
* Added "better" diorama.
* Added elite displays for Bob-omb and King Bob-omb for SoTS elites.
</details>
<details>
<summary>1.0.5 </summary>

* SoTS update.
* Rebalanced tree drops and exposed drop chances to config.
* 1UP and Coin now have models. I would love to still have sprites but Gearbox\Unity changes made so they have a very ugly box around them.
* Removed StageAPI dependency.
* Music might be a bit quiet compared to vanilla stages due to Wwise update. Let me know how it feels.
</details>
<details>
<summary>1.0.4 </summary>

* Exposed Bob-omb and King Bob-omb character spawn cards.
	* _You can find them in `SM64BBF.SM64BBFContent.CharacterSpawnCards`. Do note however that due to fact that I used ContentProvider csc are filled rather late, they will be null on plug-in load. Also King Bob-omb spawn card will be null if Regigigas is not installed._
* Added option to add Bob-ombs to any stage, including modded ones and Simulacrum.	
* Lowered volume and range of Bob-omb fuse sound. 
	* _You still should be able to hear them approaching you from behind. Just they would no longer block every other sound in 100m radius._
</details>
<details>
<summary>1.0.3 </summary>

* Removed SoundAPI dependency.
* Regigigas is now fully optional.
	* _Yes, that means if you don't have Regigigas installed King Bob-omb won't spawn._
</details>
<details>
<summary>1.0.2 </summary>

* Made OneUp world unique so it would no longer appear in printers.
</details>
<details>
<summary>1.0.1 </summary>

* Interactable trees now resemble their SM64 counterparts more closely.
* Bob-omb's "notice player" animation and sound now scale with attack speed.
* Bob-ombs now gain armor buff when they start exploding after noticing the player. Explosion behavior on low health is unchanged.
* Replaced game's jumppads with SM64 cannons with new sound.
* Maybe fixed "Look rotation viewing vector is zero" log spam.
* Fixed King Bob-omb having wild arms because animations broke somehow?
* Added elite displays to King Bob-omb.
* Added Bob-omb and King Bob-omb icons.
</details>
<details>
<summary>1.0.0 </summary>

* Initial release
</details>
