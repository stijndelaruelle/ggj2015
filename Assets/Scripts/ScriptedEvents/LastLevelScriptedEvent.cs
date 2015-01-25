using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class EventPhase
{
	public float m_Strength;
	public float m_Duration;
}

public class LastLevelScriptedEvent : MonoBehaviour
{
	[SerializeField] private GameObject m_Spawner;
	[SerializeField] private List<EventPhase> m_Phases;

	// Use this for initialization
	void Start()
	{
		StartPhase(0);
	}

	void StartPhase(int phase)
	{
		StartCoroutine(StartPhaseRoutine(phase));
	}

	private IEnumerator StartPhaseRoutine(int phase)
	{
		float timer = m_Phases[phase].m_Duration;

		//Start screenshake!
		if (m_Phases[phase].m_Strength != 0)
		{
			Camera.main.GetComponent<Screenshake>().ScreenShake(m_Phases[phase].m_Strength, timer);
		}

		if (phase == 1)
		{
			m_Spawner.SetActive(true);
		}

		while (timer > 0.0f)
		{
			timer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		if (phase != (m_Phases.Count - 1)) StartPhase(++phase);
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.tag == "Player")
		{
			m_Spawner.SetActive(false);
			m_Spawner.GetComponent<Spawner>().StopSpawning();
		}
	}
}
