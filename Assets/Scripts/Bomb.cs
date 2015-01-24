using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour 
{
	public float bombDamage = 1f;
	public float bombForce = 50f;
	public float detonationTime = 2f;
	public float bombRange = 2f;

	private float spawnTime;

	// Use this for initialization
	void Start () 
	{
		spawnTime = Time.time;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(Time.time > spawnTime + detonationTime)
		{
			Collider2D[] bombHits = Physics2D.OverlapCircleAll(transform.position, bombRange);

			for(int i = 0 ; i < bombHits.Length; i++)
			{
				if(bombHits[i].tag == "Enemy" || bombHits[i].tag == "Player")
				{
					Vector2 throwVector = bombHits[i].transform.position - this.transform.position + Vector3.up * 5f;

					bombHits[i].gameObject.rigidbody2D.AddForce(throwVector * bombForce);

					//Do damage to player
					bombHits[i].gameObject.GetComponent<Health>().health -= bombDamage;
				}
			}

			Destroy (this.gameObject);
		}
	}
}
