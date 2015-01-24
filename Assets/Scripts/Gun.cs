using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
	[SerializeField]
	private Projectile m_Projectile;

	[SerializeField]
	private float m_SpreadAngle;

	[SerializeField]
	private float m_ReloadTime;

	private bool m_CanShoot = true;

	public void Fire(float horizDirection)
	{
		if (!m_Projectile || !m_CanShoot) return;

		Quaternion angle = Quaternion.Euler(new Vector3(0.0f, 0.0f, -horizDirection * 90.0f));

		//Add random spread
		float randAngle = Random.Range(-m_SpreadAngle, m_SpreadAngle);
		Quaternion spread = Quaternion.Euler(new Vector3(0.0f, 0.0f, randAngle));

		angle *= spread;

		//Instantiate the bullet
		Instantiate(m_Projectile, transform.position, angle);

		StartCoroutine(ReloadCoroutine());
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
