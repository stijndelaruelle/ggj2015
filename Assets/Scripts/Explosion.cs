using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour 
{
	public float lifetime;

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(Explodification());
	}

	private IEnumerator Explodification()
	{
		float timer = lifetime;

		while (timer > 0.0f)
		{
			timer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		Destroy(gameObject);
	}
}
