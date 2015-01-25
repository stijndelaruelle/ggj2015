using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
	[SerializeField]
	private GameObject m_Prefab;

	[SerializeField]
	private float m_ReloadTime;

	[SerializeField]
	private int m_MinInBatch;

	[SerializeField]
	private int m_MaxInBatch;

	// Use this for initialization
	private void Start()
	{
		SpawnBatch();
	}

	private void SpawnBatch()
	{
		++m_MinInBatch;
		++m_MaxInBatch;
		StartCoroutine(SpawnRoutine());
	}

	public void StopSpawning()
	{
		StopCoroutine("SpawnRoutine");
	}
	
	private IEnumerator SpawnRoutine()
	{
		float timer = m_ReloadTime;

		while (timer > 0.0f)
		{
			timer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		int num = Random.Range (m_MinInBatch, m_MaxInBatch);

		for (int i = 0; i < num; ++i)
		{
			Vector3 pos = GameObject.Find ("Player").transform.position;
			int offset = 5;

			int rand = Random.Range (0, 100);
			if (rand > 50) offset *= -1;

			pos.x += offset;
			pos.z = 0.0f;

			GameObject.Instantiate(m_Prefab, pos, transform.rotation);
			yield return new WaitForSeconds(0.1f);
		}

		SpawnBatch();
	}
}
