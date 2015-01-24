using UnityEngine;
using System.Collections;

public class Grenade : Projectile 
{
	[SerializeField]
	private float m_Damage = 1.0f;

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
			if(bombHits[i].tag == "Enemy" || bombHits[i].tag == "Player")
			{
				Vector2 throwVector = bombHits[i].transform.position - this.transform.position + Vector3.up * 5f;

				bombHits[i].gameObject.rigidbody2D.AddForce(throwVector * m_ThrowForce);

				//Do damage to player
				bombHits[i].gameObject.GetComponent<Health>().health -= m_Damage;
			}
		}

		Destroy (this.gameObject);
	}
}
