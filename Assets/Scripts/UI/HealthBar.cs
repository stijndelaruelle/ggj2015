using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthBar : MonoBehaviour
{
	[SerializeField]
	private Player m_Player;

	[SerializeField]
	private Image m_HeartPrefab;

	List<Image> m_Hearts;

	// Use this for initialization
	void Start ()
	{
		m_Hearts = new List<Image>();

		m_Player.OnHealthChanged += OnHealthChanged;
		m_Player.OnMaxHealthChanged += OnMaxHealthChanged;

		OnMaxHealthChanged();
	}

	void OnHealthChanged()
	{
		for (int i = 0; i < m_Hearts.Count; ++i)
		{
			float alpha = 1.0f;
			if (i >= m_Player.Health) alpha = 0.25f;

			m_Hearts[i].color = new Color(1.0f, 1.0f, 1.0f, alpha);
		}
	}

	void OnMaxHealthChanged()
	{
		if (m_Hearts.Count == m_Player.MaxHealth) return;

		for (int i = 0; i < m_Hearts.Count; ++i)
		{
			m_Hearts[i].transform.SetParent(null);
			Destroy(m_Hearts[i]);
		}
		m_Hearts.Clear();

		for (int i = 0; i < m_Player.MaxHealth; ++i)
		{
			Image heart = GameObject.Instantiate(m_HeartPrefab) as Image;
			heart.gameObject.transform.SetParent(gameObject.transform);
			m_Hearts.Add(heart);
		}
	}
}
