using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{
	public float moveSpeed = 2f;
	public int enemyDamage = 1;
	public float repeatDamageTime = 2f;
	public float killTime = .5f;
	public Transform frontCheck;
	
	private bool animationOverride = false;
	private Animator spriteAnim;

	[SerializeField]
	private float m_SightRadius = 0.0f;

	[SerializeField]
	private float m_AttackCooldownTimer = 0.0f;

	[SerializeField]
	private float m_RandomPositionTimer = 0.0f;

	private Transform m_TargetPlayer;
	private Vector3 m_TargetPosition;
	private bool m_CanAttack = true;
	private bool m_CanChooseRandomPosition = true;

	// Use this for initialization
	void Start () 
	{
		spriteAnim = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		SetAnimation();
		FindTarget();

		//Move the enemy
		if (m_TargetPlayer == null && m_TargetPosition == Vector3.zero)
		{
			rigidbody2D.velocity = new Vector2(0.0f, rigidbody2D.velocity.y);
		}
		else
		{
			Vector3 target = m_TargetPosition;
			if (m_TargetPlayer != null) target = m_TargetPlayer.position;

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
				m_TargetPosition = Vector3.zero;
			}
		}

		// Create an array of all the colliders in front of the enemy.
		Collider2D[] frontHits = Physics2D.OverlapPointAll(frontCheck.position);

		// Check each of the colliders.
		foreach(Collider2D frontColliding in frontHits)
		{
			// If any of the colliders is an Obstacle...
			if(frontColliding.tag == "Obstacle" || frontColliding.gameObject.layer == 8)
			{
				break;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D collidingObject)
	{
		if(collidingObject.gameObject.tag == "Player")
		{
			if(collidingObject.gameObject.GetComponent<Player>().m_IsDashing)
			{
				Vector2 throwVector = transform.position - collidingObject.transform.position + Vector3.up * .1f;
				gameObject.rigidbody2D.AddForce(throwVector * Mathf.Abs(collidingObject.rigidbody.velocity.x) * 50);
			
				collider2D.enabled = false;
				StartCoroutine(AttackCooldownRoutine());

				KillCountdown();
			}
			else
			{
				//Let player take damage
				collidingObject.gameObject.GetComponent<Player>().TakeDamage(enemyDamage);

				//Make player jump from damage
				Vector3 hurtVector = collidingObject.transform.position - this.transform.position + Vector3.up * 5f;
				collidingObject.gameObject.rigidbody2D.AddForce(hurtVector * 30);
			}
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

		//Ga naar random positie
		if (m_CanChooseRandomPosition && m_TargetPlayer == null)
		{
			float offset = Random.Range (-5.0f, 5.0f);
			m_TargetPosition = new Vector3(transform.position.x + offset, transform.position.y, transform.position.z);

			StartCoroutine(RandomPositionCooldownRoutine());
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

	private IEnumerator RandomPositionCooldownRoutine()
	{
		m_CanChooseRandomPosition = false;
		float timer = Random.Range(m_RandomPositionTimer - 1.0f, m_RandomPositionTimer + 1.0f);
		
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		m_CanChooseRandomPosition = true;
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
