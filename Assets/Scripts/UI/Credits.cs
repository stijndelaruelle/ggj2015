using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour 
{
	public GameObject creditsPanel;
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyUp(KeyCode.Escape))
		{
			if(!creditsPanel.activeSelf)
				creditsPanel.SetActive(true);
			else
				creditsPanel.SetActive(false);
		}
	}
}
