using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class AbilityMenu : MonoBehaviour
{
	public enum Ability
	{
		Run,
		Dash,
		Jump,
		DoubleJump,
		Health,
		HealthRegen,
		GatlingGun,
		GrenadeLauncher
	}

	//Singleton
	private static AbilityMenu m_Instance;
	public static AbilityMenu Instance
	{
		get
		{
			if(m_Instance == null)
			{
				m_Instance = GameObject.FindObjectOfType<AbilityMenu>();
				
				//Tell unity not to destroy this object when loading a new scene!
				DontDestroyOnLoad(m_Instance.gameObject);
			}
			
			return m_Instance;
		}
	}

	[SerializeField]
	private Player m_Player = null;

	[SerializeField]
	private List<Button> m_Buttons = null;

	private int m_DeletedAbilities = 0;
	
	void Awake()
	{
		if(m_Instance == null)
		{
			//If I am the first instance, make me the Singleton
			m_Instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
			if(this != m_Instance)
				Destroy(this.gameObject);
		}

		m_Buttons = new List<Button>();
		for (int i = 0; i < transform.childCount; ++i)
		{
			Button btn = transform.GetChild(i).gameObject.GetComponent<Button>();
			if (btn != null) m_Buttons.Add(btn);
		}

		SetActive(false);
	}

	public void FillMenu()
	{
		foreach(Button btn in m_Buttons)
		{
			btn.interactable = false;
		}

		//Movement
		if (m_Player.m_Acceleration > 0)  { m_Buttons[0].interactable = true; }
		if (m_Player.m_DashDuration > 0 ) { m_Buttons[1].interactable = true; }

		//Jumping
		if (m_Player.m_JumpAmount > 0) { m_Buttons[2].interactable = true; }
		if (m_Player.m_JumpAmount > 1) { m_Buttons[3].interactable = true; }

		//Health
		if (m_Player.MaxHealth > 1)       	{ m_Buttons[4].interactable = true; }
		if (m_Player.m_HealthRegenRate > 0) { m_Buttons[5].interactable = true; }

		//Weapons
		if (m_Player.m_GatlingGun != null)  	 { m_Buttons[6].interactable = true; }
		if (m_Player.m_GrenadeLauncher != null ) {m_Buttons[7].interactable = true; }
	}
	
	public void RemoveAbility(int abilityID)
	{
		Ability ability = (Ability)abilityID;
		m_Buttons[abilityID].interactable = false;

		//We remove an ability
		switch (ability)
		{
			case Ability.Run:
				Debug.Log ("We removed the run ability!");
				m_Player.m_Acceleration = 0.0f;
				break;
				
			case Ability.Dash:
				Debug.Log ("We removed the dash ability!");
				m_Player.m_DashDuration = 0.0f;
				break;

			case Ability.Jump:
				Debug.Log ("We removed the jump ability!");
				m_Player.m_JumpAmount -= 1;
				break;
				
			case Ability.DoubleJump:
				Debug.Log ("We removed the doublejump ability!");
				m_Player.m_JumpAmount -= 1;
				break;

			case Ability.Health:
				Debug.Log ("We removed the health ability!");
				m_Player.Health = 1;
				m_Player.MaxHealth = 1;
				break;

			case Ability.HealthRegen:
				Debug.Log ("We removed the heathregen ability!");
				m_Player.m_HealthRegenRate = -1.0f;
				break;

			case Ability.GatlingGun:
				Debug.Log ("We removed the gatlingun ability!");
				m_Player.m_GatlingGun = null;
				break;

			case Ability.GrenadeLauncher:
				Debug.Log ("We removed the grenadelauncher ability!");
				m_Player.m_GrenadeLauncher = null;
				break;

			default:
				break;
		}

		//We move on to the next level
		++m_DeletedAbilities;
		if (m_DeletedAbilities == 2)
		{
			//Hide ourselves
			SetActive(false);

			LevelSwapper.Instance.SwapLevel();
			m_DeletedAbilities = 0;
		}
	}

	public void SetActive(bool value)
	{
		if (value == true) FillMenu();
		gameObject.SetActive(value);
	}

}
