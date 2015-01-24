using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour 
{
	public float health = 3;
	public float maxHealth = 3;
	public float regenRate = 0;

	// Update is called once per frame
	void Update () 
	{
		if(health <= 0)
		{
			Destroy (this.gameObject);
		}

		//Regenerate health
		health += Time.deltaTime * regenRate;
		health = Mathf.Clamp(health, 0, maxHealth);
	}
}
