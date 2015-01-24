using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	//-----------------
	// Datamembers
	//-----------------

	//Run
	[SerializeField] public  float m_Acceleration = 0.0f;
	[SerializeField] private float m_MaxSpeed = 0.0f;

	//Dash
	[SerializeField] private float m_DashAcceleration = 0.0f;
	[SerializeField] public  float m_DashDuration = 0.0f;
	[SerializeField] private float m_DashCooldown = 0.0f;

	//Jump
	[SerializeField] private float m_JumpAcceleration = 0.0f;
	[SerializeField] private float m_MaxJumpSpeed = 0.0f;
	[SerializeField] public  int m_JumpAmount = 0;

	//Weapons
	[SerializeField] public  Gun m_GatlingGun = null;
	[SerializeField] public  Gun m_GrenadeLauncher = null;

	//Health
	[SerializeField] public float m_Health = 0;
	[SerializeField] public float m_MaxHealth = 0;
	[SerializeField] public float m_HealthRegen = 0;

	[SerializeField] private Transform m_GroundChecker = null;

	private bool m_IsJumping = false;
	private bool m_IsDashing = false;
	private bool m_IsAutoWalking = false;
	public  bool m_CanDash = true;
	private int m_CurrentJump = 0;

	private float m_HorizDirection = 1.0f;
	
	//-----------------
	// Functions
	//-----------------
	private void OnLevelWasLoaded()
	{
		m_IsJumping = false;
		m_IsDashing = false;
		m_IsAutoWalking = false;
		m_CanDash = true;
		
		m_HorizDirection = 1.0f;

		//Look for spawn position
		GameObject spawn = GameObject.FindGameObjectWithTag("Spawn");
		if (spawn != null)
		{
			transform.position = spawn.transform.position;
		}
	}

	// Update is called once per frame
	private void Update ()
	{
		bool isOnGround = Physics2D.Linecast(transform.position, m_GroundChecker.position, 1 << LayerMask.NameToLayer("Ground")); 

		HandleHealth();

		//Start jumping
		if(Input.GetButtonDown("Jump") && m_JumpAmount > 0)
		{
			++m_CurrentJump;
			if (isOnGround || m_CurrentJump < m_JumpAmount)
			{
				m_IsJumping = true;
			}
		}

		//Stop jumping
		if (Input.GetButtonUp("Jump"))
		{
			m_IsJumping = false;
		}

		if (isOnGround)
		{
			m_CurrentJump = 0;
		}
	}

	//We use FixedUpdate for any physics related stuff
	private void FixedUpdate()
	{
		HandleMovement();
		HandleShooting();
		HandleAnimations();
	}

	private void HandleHealth()
	{
		//Health
		if(m_Health <= 0)
		{
			Destroy (this.gameObject);
		}
		
		//Regenerate health
		m_Health += Time.deltaTime * m_HealthRegen;
		m_Health = Mathf.Clamp(m_Health, 0, m_MaxHealth);
	}

	private void HandleMovement()
	{
		if (m_IsAutoWalking) return;

		//Dashing
		if(m_CanDash && Input.GetButtonDown("Fire1"))
		{
			StartCoroutine(DashRoutine());

			//Start cooldown
			StartCoroutine(DashCooldownRoutine());
		}
		else if (!m_IsDashing)
		{
			float horizInput = Input.GetAxis("Horizontal");
			if (Mathf.Abs(horizInput) > 0.0f) m_HorizDirection = Mathf.Sign(horizInput);

			//Running
			if(Mathf.Abs(rigidbody2D.velocity.x) < m_MaxSpeed || Mathf.Sign(horizInput) != Mathf.Sign(rigidbody2D.velocity.x))
			{
				rigidbody2D.AddForce(Vector2.right * horizInput * m_Acceleration);
			}

			//Clamp
			if (Mathf.Abs(rigidbody2D.velocity.x) > m_MaxSpeed)
			{
				rigidbody2D.velocity = new Vector2(Vector2.right.x * Mathf.Sign(rigidbody2D.velocity.x) * m_MaxSpeed, rigidbody2D.velocity.y);
			}
		}

		//Jumping
		if (m_IsJumping)
		{
			rigidbody2D.AddForce(new Vector2(0.0f, m_JumpAcceleration));

			if (rigidbody2D.velocity.y > m_MaxJumpSpeed)
			{
				m_IsJumping = false;
			}
		}

		//Keep ourselves within the screen bounts
		Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
		
		if(pos.x < 0.0f || pos.x > 1.0f)
		{
			rigidbody2D.velocity = new Vector2(0.0f, rigidbody2D.velocity.y);
		}
	}

	private void HandleShooting()
	{
		if((m_GatlingGun != null) && Input.GetButton("Fire2"))
		{
			m_GatlingGun.Fire(m_HorizDirection);
			StartCoroutine(Camera.main.GetComponent<Screenshake>().Screenshaker());
		}

		if ((m_GrenadeLauncher != null) && Input.GetButton("Fire3"))
		{
			m_GrenadeLauncher.Fire (m_HorizDirection);
		}
	}

	private void HandleAnimations()
	{
		if (m_HorizDirection != Mathf.Sign(transform.localScale.x))
		{
			Vector3 newScale = transform.localScale;
			newScale.x = transform.localScale.x * -1.0f;
			transform.localScale = newScale;
		}
	}

	#region Dashing
	private IEnumerator DashRoutine()
	{
		m_IsDashing = true;
		float timer = m_DashDuration;

		//Ignore gaviry & put y velocity at 0
		rigidbody2D.gravityScale = 0.0f;
		rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0.0f);

		while (timer > 0.0f)
		{
			timer -= Time.deltaTime;
			rigidbody2D.AddForce(Vector2.right * m_HorizDirection * m_DashAcceleration);
			yield return new WaitForEndOfFrame();
		}

		rigidbody2D.gravityScale = 3.0f;
		m_IsDashing = false;
	}

	private IEnumerator DashCooldownRoutine()
	{
		m_CanDash = false;
		float timer = m_DashCooldown;

		while (timer > 0.0f)
		{
			timer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		m_CanDash = true;
	}
	#endregion

	#region AutoWalk
	public void AutoWalk(float duration)
	{
		StartCoroutine(AutoWalkRoutine(duration));
	}

	private IEnumerator AutoWalkRoutine(float duration)
	{
		m_IsAutoWalking = true;
		float timer = duration;
		
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime;
			rigidbody2D.AddForce(Vector2.right * Mathf.Sign(rigidbody2D.velocity.x) * (m_Acceleration / 5.0f));

			//Clamp
			if (Mathf.Abs(rigidbody2D.velocity.x) > (m_MaxSpeed / 5.0f))
			{
				rigidbody2D.velocity = new Vector2(Vector2.right.x * Mathf.Sign(rigidbody2D.velocity.x) * (m_MaxSpeed / 5.0f), rigidbody2D.velocity.y);
			}

			yield return new WaitForEndOfFrame();
		}
		
		//m_IsAutoWalking = false;
	}
	#endregion
}
