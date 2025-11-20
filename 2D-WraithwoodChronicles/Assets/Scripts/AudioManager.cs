using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource soundEffectsSource;

    [Header("Audio Clips")]
    public List<AudioClip> musicTrackClips;
    public List<AudioClip> soundEffectClips;

    [Header("Music Fade Settings")]
    public float musicFadeDuration = 1.5f;

    private Coroutine currentMusicCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string clipName)
    {
        AudioClip clip = musicTrackClips.Find(c => c.name == clipName);

        if (musicSource.isPlaying && musicSource.clip == clip)
        {
            //Already playing this track
            return;
        }

        if (currentMusicCoroutine != null)
        {
            StopCoroutine(currentMusicCoroutine);
        }

        currentMusicCoroutine = StartCoroutine(FadeInNewMusic(clip));
    }

    private IEnumerator FadeInNewMusic(AudioClip newClip)
    {
        float originalVolume = musicSource.volume;

        if (musicSource.isPlaying && musicSource.clip != null)
        {
            for (float t = 0; t < musicFadeDuration; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(originalVolume, 0f, t / musicFadeDuration);
                yield return null;
            }

            musicSource.Stop();
        }

        musicSource.clip = newClip;
        musicSource.Play();

        for (float t = 0; t < musicFadeDuration; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, originalVolume, t / musicFadeDuration);
            yield return null;
        }

        musicSource.volume = originalVolume;
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySound(string clipName, GameObject target = null)
    {
        AudioClip clip = soundEffectClips.Find(c => c.name == clipName);

        if (target != null)
        {
            AudioSource.PlayClipAtPoint(clip, target.transform.position);
        }
        else
        {
            soundEffectsSource.PlayOneShot(clip);
        }
    }
}
