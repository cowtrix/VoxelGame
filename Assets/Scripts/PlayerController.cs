using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public sbyte Layer;
    private PlayerInput m_input;

	private InputAction m_sizeUp, m_sizeDown;

	private void Awake()
	{
		m_input = GetComponent<PlayerInput>();
		m_sizeUp = m_input.actions.Single(a => a.name == "SizeUp");
		m_sizeDown = m_input.actions.Single(a => a.name == "SizeDown");
	}

	// Update is called once per frame
	void Update()
    {
        if (m_sizeUp.triggered && Layer < sbyte.MaxValue)
		{
            transform.localScale *= 3;
		}
        else if(m_sizeDown.triggered && Layer > sbyte.MinValue)
		{
            transform.localScale /= 3;
        }
    }
}
