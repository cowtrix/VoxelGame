using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInTime : MonoBehaviour
{
    public bool ScaleOverTime = true;
    public float DestroyTime = 5f;

    private float m_timer;

	private void Start()
	{
        m_timer = DestroyTime;
    }

	void Update()
    {
        m_timer -= Time.deltaTime;
        if(m_timer < 0)
		{
            Destroy(gameObject);
		}
    }
}
