using UnityEngine;
using UnityEngine.InputSystem;

namespace Actors
{
	public class PlayerMovementController : MovementController
	{
		public void OnMove(InputAction.CallbackContext context)
		{
			m_moveDirection = context.ReadValue<Vector2>();
		}

		public void OnJump(InputAction.CallbackContext context)
		{
			var val = context.ReadValue<float>();
			if (val > .5f)
			{
				m_inputJump = true;
			}
			if (context.canceled)
			{
				m_inputJump = false;
			}
			Debug.Log($"Jump: {m_inputJump}");
		}
	}
}