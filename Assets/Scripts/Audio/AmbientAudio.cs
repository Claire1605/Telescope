using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AmbientAudio
{
    public static bool isFading = false;
    public static AudioClip currentTrack;

    private static Coroutine currentFadeCoroutine;
    private static AudioSource ambience1;
    private static AudioSource ambience2;

    private static float speed = 1.0f;
    private static float maxVolume = 0.25f;

    public static void Initialise()
    {
        ambience1 = GameObject.FindGameObjectWithTag("Ambience1").GetComponent<AudioSource>();
        ambience2 = GameObject.FindGameObjectWithTag("Ambience2").GetComponent<AudioSource>();
        currentTrack = ambience1.clip;
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

        bool ambience1FadeOut = true;
        float currentVolume1 = 0;
        float currentVolume2 = 0;

        if (ambience1.volume > 0)
        {
            currentVolume1 = ambience1.volume;
            currentVolume2 = ambience2.volume;
            ambience1FadeOut = true;

            if (ambience2.clip != newClip)
            {
                ambience2.clip = newClip;
                ambience2.Play();
            }
        }
        else
        {
            currentVolume1 = ambience1.volume;
            currentVolume2 = ambience2.volume;
            ambience1FadeOut = false;

            if (ambience1.clip != newClip)
            {
                ambience1.clip = newClip;
                ambience1.Play();
            }
        }

        if (ambience1FadeOut)
        {
            while (i < 1)
            {
                i += Time.deltaTime * speed;
                ambience1.volume = Mathf.Lerp(currentVolume1, 0, i);
                ambience2.volume = Mathf.Lerp(currentVolume2, maxVolume, i);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (i < 1)
            {
                i += Time.deltaTime * speed;
                ambience1.volume = Mathf.Lerp(currentVolume1, maxVolume, i);
                ambience2.volume = Mathf.Lerp(currentVolume2, 0, i);
                yield return new WaitForEndOfFrame();
            }
        }

        isFading = false;
    }
}
