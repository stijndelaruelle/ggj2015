using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
	[SerializeField]
	private Transform m_Target;

	[SerializeField]
	private float m_MaxOffset;

	private float m_LeftBorder;
	private float m_RightBorder;

	private float m_TopBorder;
	private float m_BottomBorder;

	private void OnLevelWasLoaded()
	{
		GameObject bottomLeft = GameObject.Find("BottomLeft");
		GameObject topRight = GameObject.Find("TopRight");

		if (bottomLeft == null)
		{
			Debug.LogWarning("Missing BottomLeft object for the camera!!!");
			return;
		}

		if (topRight == null)
		{
			Debug.LogWarning("Missing TopRight object for the camera!!!");
			return;
		}

		//Set borders
		m_LeftBorder = bottomLeft.transform.position.x;
		m_BottomBorder = bottomLeft.transform.position.y;
		m_RightBorder = topRight.transform.position.x;
		m_TopBorder = topRight.transform.position.y;
	}

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

		CheckBorders();
	}

	private void CheckBorders()
	{
		if (transform.position.x < m_LeftBorder)   transform.position = new Vector3(m_LeftBorder, transform.position.y, transform.position.z);
		if (transform.position.x > m_RightBorder)  transform.position = new Vector3(m_RightBorder, transform.position.y, transform.position.z);

		if (transform.position.y < m_BottomBorder) transform.position = new Vector3(transform.position.x, m_BottomBorder, transform.position.z);
		if (transform.position.y > m_TopBorder)    transform.position = new Vector3(transform.position.x, m_TopBorder, transform.position.z);
	}
}
