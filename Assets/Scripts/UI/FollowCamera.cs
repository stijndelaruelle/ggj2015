using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
	[SerializeField]
	private Transform m_Target;

	[SerializeField]
	private float m_MaxOffset;

	void Update()
	{
		float xOffset = m_Target.position.x - transform.position.x;
		float yOffset = m_Target.position.y - transform.position.y;

		//Character moved to far to the left
		if (xOffset < -m_MaxOffset)
		{
			Vector3 newPos = new Vector3(transform.position.x - (Mathf.Abs(xOffset) - m_MaxOffset),
			                             transform.position.y,
			                             transform.position.z);
			transform.position = newPos;
		}

		//Character moved to far to the right
		if (xOffset > m_MaxOffset)
		{
			Vector3 newPos = new Vector3(transform.position.x + (xOffset - m_MaxOffset),
			                             transform.position.y,
			                             transform.position.z);

			transform.position = newPos;
		}

		//Character moved to far to the top
		if (yOffset < -m_MaxOffset)
		{
			Vector3 newPos = new Vector3(transform.position.x,
			                             transform.position.y - (Mathf.Abs(yOffset) - m_MaxOffset),
			                             transform.position.z);
			transform.position = newPos;
		}
		
		//Character moved to far to the right
		if (yOffset > m_MaxOffset)
		{
			Vector3 newPos = new Vector3(transform.position.x,
			                             transform.position.y + (yOffset - m_MaxOffset),
			                             transform.position.z);
			
			transform.position = newPos;
		}
	}
}
