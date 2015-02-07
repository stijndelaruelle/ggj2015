using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{
	public float moveSpeed = 2f;
	public int enemyDamage = 1;
	public float repeatDamageTime = 2f;
	public float killTime = .5f;

	[SerializeField]
	private Transform m_FrontCheck;

	[SerializeField]
	private Transform m_GroundCheck;

	protected bool animationOverride = false;
	protected Animator spriteAnim;

	[SerializeField]
	private float m_SightRadius = 0.0f;

	[SerializeField]
	private float m_AttackCooldownTimer = 0.0f;

	[SerializeField]
	private float m_MinWanderTimer = 0.0f;

	[SerializeField]
	private float m_MaxWanderTimer = 0.0f;

	protected Transform m_TargetPlayer;
	private bool m_CanAttack = false;

	private bool m_IsWandering = false;
	private bool m_CanWander = true;

	// Use this for initialization
	void Start () 
	{
		spriteAnim = gameObject.GetComponent<Animator>();

		StartCoroutine(AttackCooldownRoutine());
		StartCoroutine(WanderCooldownRoutine());
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		SetAnimation();
		FindTarget();

		//Move the enemy
		if (m_TargetPlayer == null && !m_IsWandering)
		{
			rigidbody2D.velocity = new Vector2(0.0f, rigidbody2D.velocity.y);
		}

		if (m_TargetPlayer)
		{
			Vector3 target = m_TargetPlayer.position;

			Vector3 dir = transform.position - target;
			rigidbody2D.velocity = new Vector2(-Mathf.Sign(dir.x) * moveSpeed, rigidbody2D.velocity.y);

			if (dir.magnitude > 0.5f)
			{
				Vector3 enemyScale = transform.localScale;
				enemyScale.x = -Mathf.Sign(dir.x);
				transform.localScale = enemyScale;
			}
			else
			{
				m_TargetPlayer = null;
			}
		}
		else if (m_IsWandering)
		{
			rigidbody2D.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * moveSpeed, rigidbody2D.velocity.y);
		}

		// Create an array of all the colliders in front of the enemy.
		Collider2D[] frontHits = Physics2D.OverlapPointAll(m_FrontCheck.position);

		// Check each of the colliders.
		foreach(Collider2D frontColliding in frontHits)
		{
			// If any of the colliders is an Obstacle...
			if(frontColliding.tag == "Obstacle" || frontColliding.gameObject.layer == 8)
			{
				Flip();
				rigidbody2D.velocity = new Vector2(0.0f, rigidbody2D.velocity.y);
				break;
			}
		}

		// Create an array of all the colliders in front of the enemy.
		Collider2D[] groundHits = Physics2D.OverlapPointAll(m_GroundCheck.position);
		
		// Check each of the colliders.
		bool foundGround = false;
		foreach(Collider2D frontColliding in groundHits)
		{
			// If any of the colliders is an Obstacle...
			if(frontColliding.tag == "Obstacle" || frontColliding.gameObject.layer == 8)
			{
				foundGround = true;
				break;
			}
		}

		if (!foundGround)
		{
			Flip();
			rigidbody2D.velocity = new Vector2(0.0f, rigidbody2D.velocity.y);
		}
	}

	private void Flip()
	{
		Vector3 enemyScale = transform.localScale;
		enemyScale.x *= -1;
		transform.localScale = enemyScale;
	}

	void OnCollisionEnter2D(Collision2D collidingObject)
	{
		if(collidingObject.gameObject.tag == "Player")
		{
            //if(collidingObject.gameObject.GetComponent<Player>().m_IsDashing)
            //{
            //    Vector2 throwVector = transform.position - collidingObject.transform.position + Vector3.up * .1f;
            //    gameObject.rigidbody2D.AddForce(throwVector * Mathf.Abs(collidingObject.rigidbody.velocity.x) * 50);
			
            //    collider2D.enabled = false;
            //    StartCoroutine(AttackCooldownRoutine());

            //    KillCountdown();
            //}
            //else
            //{
				//Let player take damage
				collidingObject.gameObject.GetComponent<Player>().TakeDamage(enemyDamage);

				//Make player jump from damage
				Vector3 hurtVector = collidingObject.transform.position - this.transform.position + Vector3.up * 5f;
				collidingObject.gameObject.rigidbody2D.AddForce(hurtVector * 30);
			//}
		}
	}

	public void KillCountdown()
	{
		StartCoroutine(KillCountdownRoutine());
	}

	public IEnumerator KillCountdownRoutine()
	{
		float timer = killTime;
		animationOverride = true;
		spriteAnim.SetInteger("AnimID", 2);

		while (timer > 0.0f)
		{
			timer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		Die();
	}

	void Die()
	{
		Destroy(gameObject);
	}

	private void FindTarget()
	{
		if (m_CanAttack)
		{
			Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, m_SightRadius);
			
			m_TargetPlayer = null;
			for(int i = 0 ; i < objectsInRange.Length; i++)
			{
				if(objectsInRange[i].tag == "Player")
				{
					m_TargetPlayer = objectsInRange[i].gameObject.transform;
					StartCoroutine(AttackCooldownRoutine());
				}
			}
		}

		//Wander around a bit
		if (!m_IsWandering && m_CanWander && m_TargetPlayer == null)
		{
			StartCoroutine(WanderRoutine());
			StartCoroutine(WanderCooldownRoutine());
		}
	}
	
	private IEnumerator AttackCooldownRoutine()
	{
		m_CanAttack = false;
		float timer = m_AttackCooldownTimer;
		
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		m_CanAttack = true;
	}

	private IEnumerator WanderRoutine()
	{
		m_IsWandering = true;
		float timer = Random.Range(m_MinWanderTimer, m_MaxWanderTimer);

		//One in 2 that we change direction
		float rand = Random.Range(0, 100);
		if (rand > 50) Flip ();

		while (timer > 0.0f)
		{
			timer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		m_IsWandering = false;
	}

	private IEnumerator WanderCooldownRoutine()
	{
		m_CanWander = false;
		float timer = Random.Range(m_MinWanderTimer * 3.0f, m_MaxWanderTimer * 3.0f);
		
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		m_CanWander = true;
	}

	void SetAnimation()
	{
		if(!animationOverride)
		{
			if(Mathf.Abs(rigidbody2D.velocity.x) > .1f)
				spriteAnim.SetInteger("AnimID", 1);
			else
				spriteAnim.SetInteger("AnimID", 0);
		}
	}
}
