using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	[SerializeField]
	private float m_Speed;
	
	// Use this for initialization
	void Start ()
	{
		rigidbody2D.AddForce(Vector2.right * m_Speed);
	}
}
