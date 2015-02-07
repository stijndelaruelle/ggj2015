using UnityEngine;
using System.Collections;

public class AbilityMenuOpener : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.tag == "Player")
		{
			//collider.gameObject.GetComponent<Player>().m_IsAutoWalking = true;
			AbilityMenu.Instance.SwapLevel = false;
			AbilityMenu.Instance.gameObject.SetActive(true);
		}
	}
}
