using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{
	public float moveSpeed = 2f;
	public int enemyHealth = 1;
	public float repeatDamageTime = 2f;
	public Transform frontCheck;
	public Transform topCheck;

	private SpriteRenderer enemySprite;
	private float lastHitTime;

	// Use this for initialization
	void Start () 
	{
		enemySprite = this.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		//Move the enemy
		rigidbody2D.velocity = new Vector2(transform.localScale.x * moveSpeed, rigidbody2D.velocity.y);

		// Create an array of all the colliders in front of the enemy.
		Collider2D[] frontHits = Physics2D.OverlapPointAll(frontCheck.position, 1);	
		// Check each of the colliders.
		foreach(Collider2D frontColliding in frontHits)
		{
			// If any of the colliders is an Obstacle...
			if(frontColliding.tag == "Obstacle")
			{
				// ... Flip the enemy and stop checking the other colliders.
				Flip ();
				break;
			}
		}

		// Create an array of all the colliders in front of the enemy.
		Collider2D[] topHits = Physics2D.OverlapPointAll(topCheck.position, 1);	
		// Check each of the colliders.
		foreach(Collider2D topColliding in topHits)
		{
			// If any of the colliders is an Obstacle...
			if(topColliding.tag == "Player")
			{
				Die();
				break;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D collidingObject)
	{
		if(collidingObject.gameObject.tag == "Player" && Time.time > lastHitTime + repeatDamageTime)
		{
			lastHitTime = Time.time;

			//Let player take damage

			//Make player jump from damage
			Vector3 hurtVector = collidingObject.transform.position - this.transform.position + Vector3.up * 5f;
			collidingObject.gameObject.rigidbody2D.AddForce(hurtVector * 30);
			
			Flip ();
		}
	}

	void Flip()
	{
		Vector3 enemyScale = transform.localScale;
		enemyScale.x *= -1;
		transform.localScale = enemyScale;
	}

	void Die()
	{
		//Play death animation?
		Destroy(this.gameObject);
	}
}
