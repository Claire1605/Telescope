using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInVolume : MonoBehaviour
{
	public float maxVolume = 0.05f;
	AudioSource source;

	private void Start()
	{
		source = GetComponent<AudioSource>();
	}

	public void FadeIn()
	{
		StartCoroutine(FadeVolume());
	}

	public IEnumerator FadeVolume()
	{
		float t = 0;

		while (t < 1)
		{
			t += Time.deltaTime;

			source.volume = Mathf.Lerp(0.0f, maxVolume, t);

			yield return new WaitForEndOfFrame();
		}
	}
}
