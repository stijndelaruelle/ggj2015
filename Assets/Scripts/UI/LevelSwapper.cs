using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSwapper : MonoBehaviour
{
	[SerializeField]
	private float m_FadeTime = 0.0f;

	[SerializeField]
	private Image m_FadeImage = null;
	
	private string m_NextLevel = "";
	public string NextLevel
	{
		get { return m_NextLevel; }
		set { m_NextLevel = value; }
	}

	//Singleton
	private static LevelSwapper m_Instance;
	public static LevelSwapper Instance
	{
		get
		{
			if(m_Instance == null)
			{
				m_Instance = GameObject.FindObjectOfType<LevelSwapper>();
				
				//Tell unity not to destroy this object when loading a new scene!
				DontDestroyOnLoad(m_Instance.gameObject);
			}
			
			return m_Instance;
		}
	}
	
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
	}

	void Start()
	{
		NextLevel = "level1";
		SwapLevel(true);
	}

	private void OnLevelWasLoaded()
	{
		StartCoroutine(FadeIn());
	}
	
	public void SwapLevel(bool instant = false)
	{
		if (instant)
		{
			Application.LoadLevel(NextLevel);
			return;
		}

		StartCoroutine(SwapLevelRoutine());
	}

	public IEnumerator FadeIn()
	{
		float timer = m_FadeTime;
		
		while (timer > 0.0f)
		{
			//Fade the texture
			if (m_FadeImage != null)
			{
				Color newColor = m_FadeImage.color;
				newColor.a = Mathf.Lerp(1.0f, 0.0f, (m_FadeTime / timer) - 1.0f);

				m_FadeImage.color = newColor;
			}

			timer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		m_FadeImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
	}

	public IEnumerator SwapLevelRoutine()
	{
		float timer = m_FadeTime;
		
		while (timer > 0.0f)
		{
			//Fade the texture
			if (m_FadeImage != null)
			{
				Color newColor = m_FadeImage.color;
				newColor.a = Mathf.Lerp(0.0f, 1.0f, (m_FadeTime / timer) - 1.0f);
				
				m_FadeImage.color = newColor;
			}
			timer -= Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}
			
		m_FadeImage.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);

		//Swap level
		Application.LoadLevel(NextLevel);
	}
}
