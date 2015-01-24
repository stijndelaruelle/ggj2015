using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
	private Button m_Button = null;

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

		SetActive(false);
	}

	public void FillMenu()
	{
		//Clear all our buttons
		for(int i = transform.childCount - 1; i >= 0; --i)
		{
			Destroy(transform.GetChild(i).gameObject);
		}

		//Movement
		if (m_Player.m_Acceleration > 0)  { AddButton((int)Ability.Run, "Run"); }
		if (m_Player.m_DashDuration > 0 ) { AddButton((int)Ability.Dash, "Dash"); }

		//Jumping
		if (m_Player.m_JumpAmount > 0) { AddButton((int)Ability.Jump, "Jump"); }
		if (m_Player.m_JumpAmount > 1) { AddButton((int)Ability.DoubleJump, "Double Jump"); }

		//Health
		if (m_Player.MaxHealth > 1)       	{ AddButton((int)Ability.Health, "Health"); }
		if (m_Player.m_HealthRegenRate > 0) { AddButton((int)Ability.HealthRegen, "health regen"); }

		//Weapons
		if (m_Player.m_GatlingGun != null)  	 { AddButton((int)Ability.GatlingGun, "Gatling gun"); }
		if (m_Player.m_GrenadeLauncher != null ) { AddButton((int)Ability.GrenadeLauncher, "Grenade launcher"); }
	}
	
	public void RemoveAbility(int abilityID)
	{
		Ability ability = (Ability)abilityID;

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

		//Hide ourselves
		SetActive(false);

		//We move on to the next level
		LevelSwapper.Instance.SwapLevel();
	}

	private void AddButton(int abilityID, string text)
	{
		Button button = GameObject.Instantiate(m_Button) as Button;
		
		RectTransform btnTransform = button.gameObject.GetComponent<RectTransform>();
		
		btnTransform.SetParent(transform);
		btnTransform.GetChild(0).GetComponent<Text>().text = text;
		btnTransform.sizeDelta = new Vector2(1.0f, 1.0f);
		
		button.onClick.AddListener(() => RemoveAbility(abilityID));
	}

	public void SetActive(bool value)
	{
		if (value == true) FillMenu();
		gameObject.SetActive(value);
	}

}
