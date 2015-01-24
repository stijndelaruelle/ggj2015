using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour 
{
	public int health = 3;
	public int maxHealth = 3;
	public float regenTime = 2;

	private float timer = 2;

	// Update is called once per frame
	void Update () 
	{
		if(health <= 0)
		{
			Destroy (this.gameObject);
		}

		//Regenerate health
		if(health < maxHealth && regenTime > 0)
		{
			if(timer > 0)
			{
				timer -= Time.deltaTime;
			}
			else
			{
				health++;
				timer = regenTime;
			}
		}
	}

	public void TakeDamage(int damage)
	{
		health -= damage;

		//Flash flash Invincibility!

	}
}
