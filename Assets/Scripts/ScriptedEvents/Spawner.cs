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
		StartCoroutine(SpawnRoutine());
	}

	private void Update()
	{

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
			pos.x -= 5.0f;
			pos.z = 0.0f;

			GameObject.Instantiate(m_Prefab, pos, transform.rotation);
		}

		SpawnBatch();
	}
}
