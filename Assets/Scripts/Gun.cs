using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
	[SerializeField]
	private Projectile m_Projectile;

	[SerializeField]
	private float m_AimAngle;

	[SerializeField]
	private float m_SpreadAngle;

	[SerializeField]
	private float m_ReloadTime;

	[SerializeField]
	private float m_RecoilForce;

	[SerializeField]
	private float m_ScreenShakeStrength;

	[SerializeField]
	private float m_ScreenShakeLength;

	public bool m_CanShoot = true;		//Made public to use for animator on player
	public bool isGrenade = false;

	public void Fire(float horizDirection)
	{
		if (!m_Projectile || !m_CanShoot) return;

		Quaternion angle = Quaternion.Euler(new Vector3(0.0f, 0.0f, -horizDirection * 90.0f));

		//Add aim angle
		Quaternion quat = Quaternion.Euler(new Vector3(0.0f, 0.0f, m_AimAngle));
		angle *= quat;

		//Add random spread
		float randAngle = Random.Range(-m_SpreadAngle, m_SpreadAngle);
		Quaternion spread = Quaternion.Euler(new Vector3(0.0f, 0.0f, randAngle));

		angle *= spread;

		//Instantiate the bullet
		Instantiate(m_Projectile, transform.position, angle);
		StartCoroutine(ReloadCoroutine());

		//Add recoil for the player
		transform.parent.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-horizDirection * m_RecoilForce, 0.0f), ForceMode2D.Impulse);

		//Screenshake, huj huj huj!
		Camera.main.GetComponent<Screenshake>().ScreenShake(m_ScreenShakeStrength, m_ScreenShakeLength);
	}

	private IEnumerator ReloadCoroutine()
	{
		m_CanShoot = false;
		float timer = m_ReloadTime;
		
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		m_CanShoot = true;
	}
}
