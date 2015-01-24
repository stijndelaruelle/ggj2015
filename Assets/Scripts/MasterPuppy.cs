using UnityEngine;
using System.Collections;

public class MasterPuppy : MonoBehaviour 
{
	public float moveSpeed = 2f;
	public int enemyDamage = 1;
	public float repeatDamageTime = 2f;
	public float dashSpeedThreshold = 10f;
	public float killTime = .5f;
	public Transform frontCheck;
	public Transform topCheck;
	
	//private SpriteRenderer enemySprite;
	private float lastHitTime;
	public bool canDamage = true;
	
	// Use this for initialization
	void Start () 
	{
		//enemySprite = this.GetComponent<SpriteRenderer>();
		rigidbody2D.velocity = new Vector2(transform.localScale.x * moveSpeed, rigidbody2D.velocity.y);
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{	
		Debug.Log("3 " + rigidbody2D.velocity);
		// Create an array of all the colliders in front of the enemy.
		Collider2D[] frontHits = Physics2D.OverlapPointAll(frontCheck.position, 1);	
		// Check each of the colliders.
		foreach(Collider2D frontColliding in frontHits)
		{
			// If any of the colliders is an Obstacle...
			if(frontColliding.tag == "Obstacle" || frontColliding.gameObject.layer == 8)
			{
				// ... Flip the enemy and stop checking the other colliders.
				StartCoroutine(IdleRoutine());
				break;
			}
		}
		
		Vector2 startPos = new Vector2(transform.position.x + transform.localScale.x, transform.position.y);
		Vector2 endPos = new Vector2(startPos.x, startPos.y - transform.localScale.y);
		
		RaycastHit2D bottomCheck = Physics2D.Linecast(startPos, endPos);

		Debug.Log("1 " + rigidbody2D.velocity);
		if(!bottomCheck && Time.time > lastHitTime + repeatDamageTime)
			Flip ();
		
		// Create an array of all the colliders in front of the enemy.
		Collider2D[] topHits = Physics2D.OverlapPointAll(topCheck.position);	
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
		if(collidingObject.gameObject.tag == "Player")
		{
			if(!collidingObject.gameObject.GetComponent<Player>().m_CanDash && 
			   Mathf.Abs (collidingObject.rigidbody.velocity.x) > dashSpeedThreshold)
			{
				Vector2 throwVector = transform.position - collidingObject.transform.position + Vector3.up * .1f;
				gameObject.rigidbody2D.AddForce(throwVector * Mathf.Abs(collidingObject.rigidbody.velocity.x) * 250);
				
				canDamage = false;
				collider2D.enabled = false;
				
				KillCountdown();
			}
			else if(canDamage && Time.time > lastHitTime + repeatDamageTime)
			{
				lastHitTime = Time.time;
				
				//Let player take damage
				collidingObject.gameObject.GetComponent<Health>().TakeDamage(enemyDamage);
				
				//Make player jump from damage
				Vector3 hurtVector = collidingObject.transform.position - this.transform.position + Vector3.up * 5f;
				collidingObject.gameObject.rigidbody2D.AddForce(hurtVector * 30);
				
				Flip ();
			}
		}
	}
	
	void Flip()
	{
		Vector3 enemyScale = transform.localScale;
		enemyScale.x *= -1;
		transform.localScale = enemyScale;
	}
	
	public void KillCountdown()
	{
		StartCoroutine(KillCountdownRoutine());
	}
	
	public IEnumerator KillCountdownRoutine()
	{
		float timer = killTime;
		
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		Die();
	}
	
	void Die()
	{
		//Play death animation?
		collider2D.enabled = false;
		Destroy(this.gameObject);
	}

	void Move()
	{
		//Move the enemy
		rigidbody2D.velocity = new Vector2(transform.localScale.x * moveSpeed, rigidbody2D.velocity.y);
	}

	private IEnumerator IdleRoutine()
	{
		Flip();
		rigidbody2D.velocity = Vector2.zero;

		//Set timer to animationlength
		float timer = 2f;
		
		while (timer > 0.0f)
		{
			Debug.Log("Check");
			timer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		Move();
	}
}
