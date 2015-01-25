using UnityEngine;
using System.Collections;

public class KillTrigger : MonoBehaviour
{
	[SerializeField]
	private Transform m_Checkpoint;

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (m_Checkpoint == null) return;

		if (collider.tag == "Player")
		{
			Player player = collider.gameObject.GetComponent<Player>();
			player.TakeDamage(1);
			player.Teleport(m_Checkpoint.position);
			//Respawn at the checkpoint
		}
	}
}
