using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	[SerializeField]
	private float m_Speed;
	
	// Use this for initialization
	virtual protected void Start ()
	{
		Vector2 newFwd = transform.rotation * Vector3.up;
		rigidbody2D.AddForce(newFwd * m_Speed);
	}

	private void Update()
	{
		Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

		if(pos.x < 0.0f || pos.x > 1.0f || pos.y < 0.0f || pos.y > 1.0f)
		{
			Destroy(gameObject);
		}
	}

	virtual protected void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.layer == 12)
		{
			Vector2 throwVector = collision.transform.position - transform.position + Vector3.up * .1f;
			gameObject.rigidbody2D.AddForce(throwVector * Mathf.Abs(collision.rigidbody.velocity.x) * 250);

			collision.gameObject.collider2D.enabled = false;
			collision.gameObject.GetComponent<Enemy>().KillCountdown();
		}

		Destroy(gameObject);
	}
}
