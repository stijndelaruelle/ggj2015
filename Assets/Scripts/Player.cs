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
	private float m_MaxDashSpeed = 0.0f;

	[SerializeField]
	private float m_DashCooldown = 0.0f;

	[SerializeField]
	private int m_JumpAmount = 0;
	private int m_CurrentJump = 0;

	[SerializeField]
	private Transform m_GroundChecker = null;

	bool m_IsJumping = false;
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
		HandleAnimations();
	}

	private void HandleMovement()
	{
		float horizInput = Input.GetAxis("Horizontal");
		if (Mathf.Abs(horizInput) > 0.0f) m_HorizDirection = Mathf.Sign(horizInput);

		//Dashing
		if(m_CanDash && Input.GetButtonDown("Fire1"))
		{
			//Dash!
			rigidbody2D.AddForce(Vector2.right * m_HorizDirection * m_DashAcceleration);

			//Start cooldown
			StartCoroutine(DashCooldownRoutine());
		}
		else
		{
			//Running
			if(Mathf.Abs(rigidbody2D.velocity.x) < m_MaxSpeed || Mathf.Sign(horizInput) != Mathf.Sign(rigidbody2D.velocity.x))
			{
				rigidbody2D.AddForce(Vector2.right * horizInput * m_Acceleration);
			}
		}

		//Jumping
		if (m_IsJumping)
		{
			rigidbody2D.AddForce(new Vector2(0.0f, m_JumpAcceleration));

			if (Mathf.Abs(rigidbody2D.velocity.y) > m_MaxJumpSpeed)
			{
				m_IsJumping = false;
			}
		}

		//Clamp horizontal speed
		float maxSpeed = m_MaxSpeed;
		if (!m_CanDash) maxSpeed = m_MaxDashSpeed;

		if (Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
		{
			rigidbody2D.velocity = new Vector2(m_HorizDirection * maxSpeed, rigidbody2D.velocity.y);
		}
	}

	private void HandleAnimations()
	{
		Vector3 theScale = transform.localScale;
		theScale.x = transform.localScale.x * Mathf.Sign(rigidbody2D.velocity.x);
		transform.localScale = theScale;
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
