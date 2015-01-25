using UnityEngine;
using System.Collections;

public class EnemyAngry : Enemy 
{
	// Use this for initialization
	void Start () 
	{
		spriteAnim = gameObject.GetComponent<Animator>();
		m_TargetPlayer = GameObject.Find("Player").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		SetAnimation();

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
	
	private IEnumerator KillCountdownRoutine()
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
