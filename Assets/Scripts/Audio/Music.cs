using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Music
{
    public static bool isFading = false;
    public static AudioClip currentTrack;

    private static Coroutine currentFadeCoroutine;
    private static AudioSource music1;
    private static AudioSource music2;

    private static float speed = 1.0f;
    private static float maxVolume = 0.25f;

    public static void Initialise()
    {
        music1 = GameObject.FindGameObjectWithTag("Music1").GetComponent<AudioSource>();
        music2 = GameObject.FindGameObjectWithTag("Music2").GetComponent<AudioSource>();
        currentTrack = music1.clip;
    }

    public static void SetFadeCoroutine(Coroutine coroutine)
    {
        currentFadeCoroutine = coroutine;
    }

    public static Coroutine GetFadeCoroutine()
    {
        return currentFadeCoroutine;
    }

    public static IEnumerator FadeAudio(AudioClip newClip)
    {
        currentTrack = newClip;

        isFading = true;
        float i = 0;

        bool music1FadeOut = true;
        float currentVolume1 = 0;
        float currentVolume2 = 0;

        if (music1.volume > 0)
        {
            currentVolume1 = music1.volume;
            currentVolume2 = music2.volume;
            music1FadeOut = true;

            if (music2.clip != newClip)
            {
                music2.clip = newClip;
                music2.Play();
            }
        }
        else
        {
            currentVolume1 = music1.volume;
            currentVolume2 = music2.volume;
            music1FadeOut = false;

            if (music1.clip != newClip)
            {
                music1.clip = newClip;
                music1.Play();
            }
        }

        if (music1FadeOut)
        {
            while (i < 1)
            {
                i += Time.deltaTime * speed;
                music1.volume = Mathf.Lerp(currentVolume1, 0, i);
                music2.volume = Mathf.Lerp(currentVolume2, maxVolume, i);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (i < 1)
            {
                i += Time.deltaTime * speed;
                music1.volume = Mathf.Lerp(currentVolume1, maxVolume, i);
                music2.volume = Mathf.Lerp(currentVolume2, 0, i);
                yield return new WaitForEndOfFrame();
            }
        }

        isFading = false;
    }
}
