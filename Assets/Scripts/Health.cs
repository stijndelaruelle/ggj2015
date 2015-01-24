using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour 
{
	public int health = 3;
	public int maxHealth = 3;
	public float regenTime = 2;

	private float timer = 2;
	private SpriteRenderer spriteRen;
	private bool invincible = false;

	void Start()
	{
		spriteRen = gameObject.GetComponent<SpriteRenderer>();
	}

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
		if(!invincible)
			health -= damage;

		StartCoroutine(Blinking());
	}

	private IEnumerator Blinking()
	{
		//Flash flash Invincibility!
		invincible = true;

		spriteRen.enabled = false;
		yield return new WaitForSeconds(.1f);
		spriteRen.enabled = true;
		spriteRen.material.color = Color.red;
		yield return new WaitForSeconds(.1f);
		spriteRen.enabled = false;
		yield return new WaitForSeconds(.2f);
		spriteRen.material.color = Color.white;
		spriteRen.enabled = true;
		yield return new WaitForSeconds(.2f);
		spriteRen.enabled = false;
		yield return new WaitForSeconds(.2f);
		spriteRen.enabled = true;


		invincible = false;

	}
}
