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
}
