using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]


public class PlayerAudioController : MonoBehaviour
{
	private AudioSource playerAudio;
    public AudioClip vacuumSound;
    public AudioClip vacuumStartSound;

    // Start is called before the first frame update
    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
		{
			playerAudio.PlayOneShot(vacuumSound, 1.0f);
			playerAudio.PlayOneShot(vacuumStartSound, 1.0f);
		}
		if (Input.GetKeyUp(KeyCode.Space))
		{
			playerAudio.Stop();
		}
    }
}
