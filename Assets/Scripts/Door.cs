using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
	[SerializeField]
	private string m_LevelName;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			//Make the player auto walk
			other.gameObject.GetComponent<Player>().AutoWalk(1.0f);

			LevelSwapper.Instance.NextLevel = m_LevelName;
			AbilityMenu.Instance.SetActive(true);
		}
	}
}
