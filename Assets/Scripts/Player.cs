﻿using UnityEngine;
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
	private int m_JumpAmount = 0;
	private int m_CurrentJump = 0;

	[SerializeField]
	private Transform m_GroundChecker = null;

	bool m_IsJumping = false;

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

		//Running
		if(Mathf.Abs(rigidbody2D.velocity.x) < m_MaxSpeed || Mathf.Sign(horizInput) != Mathf.Sign(rigidbody2D.velocity.x))
		{
			rigidbody2D.AddForce(Vector2.right * horizInput * m_Acceleration);
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
	}

	private void HandleAnimations()
	{
		Vector3 theScale = transform.localScale;
		theScale.x = transform.localScale.x * Mathf.Sign(rigidbody2D.velocity.x);
		transform.localScale = theScale;
	}
}