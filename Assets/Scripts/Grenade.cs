using UnityEngine;
using System.Collections;

public class Grenade : Projectile 
{
	[SerializeField]
	private int m_Damage = 1;

	[SerializeField]
	private float m_ThrowForce = 50.0f;

	[SerializeField]
	private float m_DetonationTime = 2.0f;

	[SerializeField]
	private float m_Range = 2.0f;

	override protected void Start()
	{
		base.Start();
		StartCoroutine(DetonateRoutine());
	}

	private IEnumerator DetonateRoutine()
	{
		float timer = m_DetonationTime;
		
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		Detonate();
	}

	// Update is called once per frame
	private void Detonate () 
	{
		Collider2D[] bombHits = Physics2D.OverlapCircleAll(transform.position, m_Range);

		for(int i = 0 ; i < bombHits.Length; i++)
		{
			if(bombHits[i].gameObject.layer == 12 || bombHits[i].tag == "Player")
			{
				Vector2 throwVector = bombHits[i].transform.position - this.transform.position + Vector3.up * 5f;

				bombHits[i].gameObject.rigidbody2D.AddForce(throwVector * m_ThrowForce);

				if(bombHits[i].tag == "Player")
				{
					//Do damage to player
					bombHits[i].gameObject.GetComponent<Player>().TakeDamage(m_Damage);
				}
				else
				{
					bombHits[i].gameObject.GetComponent<Enemy>().canDamage = false;
					bombHits[i].gameObject.GetComponent<Enemy>().KillCountdown();
				}
			}
		}
		
		Camera.main.GetComponent<Screenshake>().ScreenShake();

		Destroy(gameObject);
	}

	override protected void OnCollisionEnter2D(Collision2D collision)
	{
		//Do nothing
	}
}
