using UnityEngine;
using System.Collections;

public class Screenshake : MonoBehaviour 
{
	public float shakeStrength = .1f;
	public float shakeLength = .1f;

	private Vector3 startingPosition;

	void Start()
	{
		startingPosition = transform.position;
	}

	public void ScreenShake()
	{
		StartCoroutine(ScreenshakeRoutine());
	}

	public void ScreenShake(float strength, float length)
	{
		StartCoroutine(ScreenshakeRoutine(strength, length));
	}

	private IEnumerator ScreenshakeRoutine()
	{
		float timer = shakeLength;
		
		while(timer > 0)
		{
			timer -= Time.deltaTime;
			
			transform.position = new Vector3(startingPosition.x + Random.Range(-shakeStrength, shakeStrength),
			                                 startingPosition.y + Random.Range(-shakeStrength, shakeStrength),
			                                 startingPosition.z);
			yield return new WaitForEndOfFrame();
		}
		
		transform.position = startingPosition;
	}

	private IEnumerator ScreenshakeRoutine(float strength, float length)
	{
		float timer = length;

		while(timer > 0)
		{
			timer -= Time.deltaTime;

			transform.position = new Vector3(startingPosition.x + Random.Range(-strength, strength),
			                                 startingPosition.y + Random.Range(-strength, strength),
			                                 startingPosition.z);
			yield return new WaitForEndOfFrame();
		}
	}
}
