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
		NextLevel = "level0";
		SwapLevel(0.0f);
	}

	private void Update()
	{
		if (!Debug.isDebugBuild) return;

		if (Input.GetKeyDown(KeyCode.F1))
		{
			m_NextLevel = "level0";
			SwapLevel(0.0f);
		}

		if (Input.GetKeyDown(KeyCode.F2))
		{
			m_NextLevel = "level1";
			SwapLevel(0.0f);
		}

		if (Input.GetKeyDown(KeyCode.F3))
		{
			m_NextLevel = "level2";
			SwapLevel(0.0f);
		}

		if (Input.GetKeyDown(KeyCode.F4))
		{
			m_NextLevel = "level3";
			SwapLevel(0.0f);
		}

		if (Input.GetKeyDown(KeyCode.F5))
		{
			m_NextLevel = "level4";
			SwapLevel(0.0f);
		}

		if (Input.GetKeyDown(KeyCode.F6))
		{
			m_NextLevel = "level5";
			SwapLevel(0.0f);
		}

		if (Input.GetKeyDown(KeyCode.F7))
		{
			m_NextLevel = "level6";
			SwapLevel(0.0f);
		}

		if (Input.GetKeyDown(KeyCode.F8))
		{
			m_NextLevel = "level7";
			SwapLevel(0.0f);
		}

		if (Input.GetKeyDown(KeyCode.F9))
		{
			m_NextLevel = "level8";
			SwapLevel(0.0f);
		}
	}

	private void OnLevelWasLoaded()
	{
		StartCoroutine(FadeIn());
	}
	
	public void SwapLevel(float fadeTime = 2.0f)
	{
		if (fadeTime <= 0.0f)
		{
			Application.LoadLevel(NextLevel);
			return;
		}

		StartCoroutine(SwapLevelRoutine(fadeTime));
	}

	public IEnumerator FadeIn()
	{
		float timer = m_FadeTime;
		m_FadeImage.gameObject.SetActive(true);

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
		m_FadeImage.gameObject.SetActive(false);
	}

	public IEnumerator SwapLevelRoutine(float fadeTime)
	{
		float timer = fadeTime;
		m_FadeImage.gameObject.SetActive(true);

		while (timer > 0.0f)
		{
			//Fade the texture
			if (m_FadeImage != null)
			{
				Color newColor = m_FadeImage.color;
				newColor.a = Mathf.Lerp(0.0f, 1.0f, (fadeTime / timer) - 1.0f);
				
				m_FadeImage.color = newColor;
			}
			timer -= Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}
			
		m_FadeImage.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		m_FadeImage.gameObject.SetActive(false);

		//Swap level
		Application.LoadLevel(NextLevel);
	}
}
