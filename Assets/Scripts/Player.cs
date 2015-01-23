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
	private float m_JumpSpeed = 0.0f;

	[SerializeField]
	private Transform m_GroundChecker = null;
	
 	bool m_IsOnGround = false;

	//-----------------
	// Functions
	//-----------------

	// Update is called once per frame
	private void Update ()
	{
		m_IsOnGround = Physics2D.Linecast(transform.position, m_GroundChecker.position, 1 << LayerMask.NameToLayer("Ground")); 
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
		if(Input.GetButtonDown("Jump") && m_IsOnGround)
		{
			rigidbody2D.AddForce(new Vector2(0.0f, m_JumpSpeed));
		}
	}

	private void HandleAnimations()
	{
		Vector3 theScale = transform.localScale;
		theScale.x = transform.localScale.x * Mathf.Sign(rigidbody2D.velocity.x);
		transform.localScale = theScale;
	}
}
