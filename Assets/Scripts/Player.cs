using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController2D))]
public class Player : MonoBehaviour
{
    //TODO STIJN
    //-Change dash functionality
    //-Change npc movement
    //-fix level overgang
    //-Variable jump height
    //-fix death sequence

	//-----------------
	// Events
	//-----------------
	public delegate void BoringDelegate();
	public event BoringDelegate OnHealthChanged;
	public event BoringDelegate OnMaxHealthChanged;

	//-----------------
	// Datamembers
	//-----------------
    private CharacterController2D m_Controller;
    private Vector2 m_ExternalForce;

    //Running
    [SerializeField] private float m_Gravity = -25f;
    [SerializeField] public  float m_RunSpeed = 8f;
    [SerializeField] private float m_GroundDamping = 20f; // how fast do we change direction? higher means faster
                     private float m_HorizDirection = 1.0f;
                     private bool m_IsAutoWalking = false;

    //Jumping    
    [SerializeField] private float m_JumpHeight = 3f;
    [SerializeField] private float m_InAirDamping = 5f;
    [SerializeField] public  int   m_JumpAmount = 0;
                     private int   m_CurrentJump = 0;

	//Dashing
	[SerializeField] private float m_DashSpeed = 0.0f;
	[SerializeField] public  float m_DashDuration = 0.0f;
	[SerializeField] private float m_DashCooldown = 0.0f;
                     private bool  m_IsDashing = false;
                     private bool  m_CanDash = true;

	//Health(-ing)
	[SerializeField] private int   m_Health = 0;
	[SerializeField] private int   m_MaxHealth = 0;
	[SerializeField] public  float m_HealthRegenRate = 0; //Time in seconds for a heart to refil
                     private bool  m_IsInvincible = false;
                     private float m_HealthRegenTimer = 0.0f;

    public int Health
    {
        get { return m_Health; }
        set
        {
            m_Health = value;
            if (OnHealthChanged != null) OnHealthChanged();
        }
    }
    public int MaxHealth
    {
        get { return m_MaxHealth; }
        set
        {
            m_MaxHealth = value;
            if (OnMaxHealthChanged != null) OnMaxHealthChanged();
        }
    }
    private bool IsDead
    {
        get { return m_Health <= 0; }
    }

	//Weapons
	[SerializeField] public  Gun m_GatlingGun = null;
	[SerializeField] public  Gun m_GrenadeLauncher = null;

    //Animation
    private bool m_AnimationOverride = false;
    private Animator m_SpriteAnimation = null;
    private SpriteRenderer m_SpriteRenderer = null;

	//Cashed values, in case we respawn
	private float m_CachedRunSpeed = 0.0f;
	private float m_CachedDashDuration = 0.0f;
	private int   m_CachedJumpAmount = 0;
	private int   m_CachedMaxHealth = 0;
	private float m_CachedHealthRegen = 0;
	private Gun   m_CachedGatlingGun = null;
	private Gun   m_CachedGrenadeLauncher = null;

    //-------------------
    // Private Functions
    //-------------------
	private void Awake()
	{
        m_Controller = GetComponent<CharacterController2D>();
        m_ExternalForce = new Vector2();

        // listen to some events for illustration purposes
        //_controller.onControllerCollidedEvent += onControllerCollider;
        //_controller.onTriggerEnterEvent += onTriggerEnterEvent;
        //_controller.onTriggerExitEvent += onTriggerExitEvent;

        m_CachedRunSpeed        = m_RunSpeed;
		m_CachedDashDuration    = m_DashDuration;
		m_CachedJumpAmount      = m_JumpAmount;
		m_CachedMaxHealth       = m_MaxHealth;
		m_CachedHealthRegen     = m_HealthRegenRate;
		m_CachedGatlingGun      = m_GatlingGun;
		m_CachedGrenadeLauncher = m_GrenadeLauncher;

		Health = MaxHealth;
		m_HealthRegenTimer = m_HealthRegenRate;

		//Animator Reference
		m_SpriteAnimation = gameObject.GetComponent<Animator>();
        m_SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}

	private void Respawn()
	{
        m_RunSpeed        = m_CachedRunSpeed;
		m_DashDuration    = m_CachedDashDuration;
		m_JumpAmount      = m_CachedJumpAmount;
		m_MaxHealth       = m_CachedMaxHealth;
		m_HealthRegenRate = m_CachedHealthRegen;
		m_GatlingGun      = m_CachedGatlingGun;
		m_GrenadeLauncher = m_CachedGrenadeLauncher;

		m_Health = MaxHealth;
		m_HealthRegenTimer = m_HealthRegenRate;
	}

	private void OnLevelWasLoaded()
	{
		m_IsDashing = false;
		m_IsAutoWalking = false;

		m_CanDash = true;
		m_CurrentJump = 0;
		gameObject.layer = 9;

		m_HorizDirection = 1.0f;

		OnHealthChanged();
		OnMaxHealthChanged();

		//Look for spawn position
		GameObject spawn = GameObject.FindGameObjectWithTag("Spawn");
		if (spawn != null)
		{
			transform.position = spawn.transform.position;
		}
	}

	// Update is called once per frame
	private void Update ()
	{
		if (IsDead) return;

		HandleHealth();
        HandleMovement();
        HandleShooting();
	}

	private void HandleMovement()
	{
		if (m_IsAutoWalking) return;
        if (m_IsDashing) return;

        // grab our current _velocity to use as a base for all calculations
        Vector3 velocity = m_Controller.velocity;

        //Ground checks
        if (m_Controller.isGrounded)
        {
            m_CurrentJump = 0; //Allow us to jump again
            velocity.y = 0;
        }

		float horizInput = Input.GetAxis("Horizontal");
		if (Mathf.Abs(horizInput) > 0.0f) m_HorizDirection = Mathf.Sign(horizInput);

        //Jumping
        if (Input.GetButtonDown("Jump") && m_JumpAmount > 0)
        {
            if (m_Controller.isGrounded || m_CurrentJump < m_JumpAmount)
            {
                velocity.y = Mathf.Sqrt(2f * m_JumpHeight * -m_Gravity);
                ++m_CurrentJump;
            }
        }

        //Dashing
        if (m_CanDash && Input.GetButton("Dash"))
        {
            StartCoroutine(DashRoutine());
            StartCoroutine(DashCooldownRoutine());
            return;
        }

        //Calculate new velocity
        var smoothedMovementFactor = m_Controller.isGrounded ? m_GroundDamping : m_InAirDamping; // how fast do we change direction?
        velocity.x = Mathf.Lerp(velocity.x, horizInput * m_RunSpeed, Time.deltaTime * smoothedMovementFactor);

        // apply gravity before moving
        velocity.y += m_Gravity * Time.deltaTime;

        //Add External force
        velocity.x += m_ExternalForce.x;
        velocity.y += m_ExternalForce.y;

        m_Controller.move(velocity * Time.deltaTime);

        //Reset external force
        m_ExternalForce.x = 0.0f;
        m_ExternalForce.y = 0.0f;

        //Animations
        HandleAnimations(horizInput);
	}

	private void HandleShooting()
	{
		if (m_IsAutoWalking) return;
        if (m_IsDashing) return;

		if((m_GatlingGun != null) && Input.GetButton("Weapon1"))
		{
			m_GatlingGun.Fire(m_HorizDirection);

			//Play Animation
			StartCoroutine(PlayAnimationRoutine(3, .167f));

			//Play Sound
			if(!audio.isPlaying && Input.GetButton("Weapon1"))
				audio.Play();

		}
		else if(audio.isPlaying)
			audio.Stop();

		if ((m_GrenadeLauncher != null) && Input.GetButtonUp("Weapon2"))
		{
			//Play Animation
			if(m_GrenadeLauncher.m_CanShoot)
				StartCoroutine(PlayAnimationRoutine(4, .167f));

			m_GrenadeLauncher.Fire (m_HorizDirection);
		}
	}

	private void HandleAnimations(float horizMovement)
	{
		if (m_HorizDirection != Mathf.Sign(transform.localScale.x))
		{
			Vector3 newScale = transform.localScale;
			newScale.x = transform.localScale.x * -1.0f;
			transform.localScale = newScale;
		}

		if(!m_AnimationOverride)
		{
            if (Mathf.Abs(horizMovement) > .1f)
				m_SpriteAnimation.SetInteger("AnimID", 2); //Walk
			else
				m_SpriteAnimation.SetInteger("AnimID", 1); //Idle
		}
	}

    private void HandleHealth()
    {
        //Health
        if (m_Health <= 0)
        {
            StartCoroutine(PlayAnimationRoutine(5, 2.0f));
            gameObject.layer = 15;

            Respawn();
            LevelSwapper.Instance.NextLevel = "level0";
            StartCoroutine(WaitForFadeRoutine(1.0f));
        }

        //Regeneration
        if (m_HealthRegenRate > 0.0f && m_Health != m_MaxHealth)
        {
            m_HealthRegenTimer -= Time.deltaTime;
            if (m_HealthRegenTimer <= 0.0f)
            {
                m_Health += 1;
                Health = Mathf.Clamp(m_Health, 0, m_MaxHealth);
                m_HealthRegenTimer = m_HealthRegenRate;
            }
        }
    }

    //-------------------
    // Public Functions
    //-------------------
    public void TakeDamage(int damage)
    {
        if (!m_IsInvincible)
        {
            m_Health -= damage;
            m_HealthRegenTimer = m_HealthRegenRate + 2.0f; //Reset regen timer and add 2 seconds extra

            StartCoroutine(InvincibilityRoutine());
            OnHealthChanged();
        }
    }

    public void Teleport(Vector3 position)
    {
        transform.position = position;

    }

    public void AddExternalForce(float x, float y)
    {
        m_ExternalForce.x += x;
        m_ExternalForce.y += y;
    }

    //-------------------
    // Coroutines
    //-------------------
	#region Dashing
	private IEnumerator DashRoutine()
	{
		m_IsDashing = true;
		float timer = m_DashDuration;

		//Play Animation
		m_AnimationOverride = true;
		m_SpriteAnimation.SetInteger("AnimID", 6);

		while (timer > 0.0f)
		{
            timer -= Time.deltaTime;

            //Move in our last direction, ignoring graviry
            Vector3 velocity = m_Controller.velocity;
            velocity.x = m_HorizDirection * m_DashSpeed;
            velocity.y = 0.0f;

            m_Controller.move(velocity * Time.deltaTime);

			//Repeat
			yield return new WaitForEndOfFrame();
		}

		m_AnimationOverride = false;
        m_IsDashing = false;
	}

	private IEnumerator DashCooldownRoutine()
	{
		m_CanDash = false;
        yield return new WaitForSeconds(m_DashCooldown);
		m_CanDash = true;
	}
	#endregion

	private IEnumerator PlayAnimationRoutine(int ID, float animationLength)
	{
		m_AnimationOverride = true;
		m_SpriteAnimation.SetInteger("AnimID", ID);

        yield return new WaitForSeconds(animationLength);

		m_AnimationOverride = false;
	}

	private IEnumerator WaitForFadeRoutine(float fadeTime)
	{
        yield return new WaitForSeconds(fadeTime);
		LevelSwapper.Instance.SwapLevel();
	}

    private IEnumerator InvincibilityRoutine()
	{
		//Flash flash Invincibility!
		m_IsInvincible = true;
		gameObject.layer = 15;		//Put the player on a layer that doesn't collide with enemies

        m_SpriteRenderer.material.color = Color.red;

        for (int i = 0; i < 15; ++i)
        {
            m_SpriteRenderer.enabled = !m_SpriteRenderer.enabled;
            yield return new WaitForSeconds(.1f);
        }
        
		m_SpriteRenderer.enabled = true;
        m_SpriteRenderer.material.color = Color.white;
		yield return new WaitForSeconds(.4f);
		
		m_IsInvincible = false;
		gameObject.layer = 9;		//Put the player back on the player layer...
	}
}
