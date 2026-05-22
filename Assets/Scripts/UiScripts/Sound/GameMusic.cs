using UnityEngine;
using System.Collections;

public class GameMusic : MonoBehaviour
{
    [SerializeField] private AudioClip[] musicTracks;
    [Range(0f, 1f)][SerializeField] private float volume = 0.5f;

    private AudioSource audioSource;
    private int currentTrack = 0;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
        audioSource.loop = false;

        if (musicTracks.Length > 0)
        {
            PlayNextTrack();
        }
    }

    void Update()
    {
        if (!audioSource.isPlaying && musicTracks.Length > 0)
        {
            PlayNextTrack();
        }
    }

    void PlayNextTrack()
    {
        audioSource.clip = musicTracks[currentTrack];
        audioSource.Play();

        currentTrack++;
        if (currentTrack >= musicTracks.Length)
        {
            currentTrack = 0;
        }
    }
}