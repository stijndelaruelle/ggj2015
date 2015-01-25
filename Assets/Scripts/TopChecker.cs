using UnityEngine;
using System.Collections;

public class TopChecker : MonoBehaviour 
{
	public GameObject parentObject;

	void OnTriggerEnter2D(Collider2D enteredCollider)
	{
		if(enteredCollider.tag == "Player")
		{
			enteredCollider.gameObject.rigidbody2D.AddForce(Vector2.up * Mathf.Abs(enteredCollider.rigidbody2D.velocity.x) * 50);

			Destroy(parentObject);
		}
	}
}
