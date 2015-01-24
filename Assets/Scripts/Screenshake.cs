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

	public IEnumerator Screenshaker(float strength, float length)
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

	public IEnumerator Screenshaker()
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
}
