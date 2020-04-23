using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelescopeCreak : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip creakZoomIn;
    public AudioClip creakZoomOut;
    public AudioClip creakZoomReturn;

    public AudioClip lensCapSlideClose;
    public AudioClip lensCapSlideOpen;

    public void zoomInCreak()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.PlayOneShot(creakZoomIn);
    }

    public void zoomOutCreak()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.PlayOneShot(creakZoomOut);
    }

    public void returnCreak()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.PlayOneShot(creakZoomReturn);
    }
}
