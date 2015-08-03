using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	public AudioSource source;

	public AudioClip click;
	public AudioClip attackHit;
	public AudioClip drawWeapon;
	public AudioClip sheathWeapon;
	public AudioClip deathSound;
	public AudioClip footstep;

	public AudioClip inGameMusic;
	public AudioClip inMenuMusic;
	public AudioClip victoryMusic;

	public void loopMenuMusic()
	{
		source.clip = inMenuMusic;
		source.Play ();
		source.loop = true;
		//Invoke ("inMenuMusic", inMenuMusic.length);
	}

	public void loopGameMusic()
	{
		source.clip = inGameMusic;
		source.Play ();
		source.loop = true;
		//Invoke ("loopGameMusic", inGameMusic.length);
	}

	public void loopVictoryMusic()
	{
		source.clip = victoryMusic;
		source.Play ();
		source.loop = true;
		//Invoke ("loopVictoryMusic", victoryMusic.length);
	}
	
	public void playClick()
	{
		source.PlayOneShot (click);
	}

	public void playAttackHit()
	{
		source.PlayOneShot (attackHit);
	}
	public void playDrawWeapon()
	{
		source.PlayOneShot (drawWeapon);
	}
	public void playSheathWeapon()
	{
		source.PlayOneShot (sheathWeapon);
	}
	public void playDeathSound()
	{
		source.PlayOneShot (deathSound);
	}
	public void playFootstep()
	{
		source.PlayOneShot (footstep);
	}
	public void playFootsteps()
	{
		source.PlayOneShot (footstep);
		Invoke ("playFootstep", footstep.length);
	}
}
