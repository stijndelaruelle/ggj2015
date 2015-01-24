using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	//-----------------
	// Datamembers
	//-----------------
	[SerializeField]
	private float m_Acceleration = 0.0f;

	[SerializeField]
	private float m_MaxSpeed = 0.0f;

	[SerializeField]
	private float m_JumpAcceleration = 0.0f;

	[SerializeField]
	private float m_MaxJumpSpeed = 0.0f;

	[SerializeField]
	private float m_DashAcceleration = 0.0f;

	[SerializeField]
	private float m_DashDuration = 0.0f;

	[SerializeField]
	private float m_DashCooldown = 0.0f;

	[SerializeField]
	private int m_JumpAmount = 0;
	private int m_CurrentJump = 0;

	[SerializeField]
	private Gun m_GatlingGun = null;

	[SerializeField]
	private Gun m_GrenadeLauncher = null;

	[SerializeField]
	private Transform m_GroundChecker = null;

	bool m_IsJumping = false;
	bool m_IsDashing = false;
	float m_HorizDirection = 1.0f;
	bool m_CanDash = true;

	//-----------------
	// Functions
	//-----------------

	// Update is called once per frame
	private void Update ()
	{
		bool isOnGround = Physics2D.Linecast(transform.position, m_GroundChecker.position, 1 << LayerMask.NameToLayer("Ground")); 

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

	private void HandleMovement()
	{
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
	}

	private void HandleShooting()
	{
		if((m_GatlingGun != null) && Input.GetButton("Fire2"))
		{
			m_GatlingGun.Fire(m_HorizDirection);
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
}
