using Actors.NPC.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Actors
{
	public class PlayerMovementController : MovementController
	{
        public Jetpack Jetpack;
        public float FreeFloatTime = .2f;

        protected override void FixedUpdate()
        {
            Jetpack.Active = false;
            base.FixedUpdate();
        }

        protected override Vector3 MutateUngroundedVelocity(Vector3 worldVelocityDirection)
        {
            if (TimeUngrounded > FreeFloatTime && State.TryAdd(eStateKey.Fuel, Jetpack.FuelEfficiency, "Jetpack"))
            {
                Jetpack.Active = true;
                worldVelocityDirection *= Jetpack.ThrusterStrength;
                if (m_isJumping)
                {
                    worldVelocityDirection *= .5f;
                    worldVelocityDirection += Vector3.up * Jetpack.ThrusterStrength * .5f;
                }
                Jetpack.ThrustersFiring = worldVelocityDirection.magnitude > 0;
                return worldVelocityDirection;
            }
            return base.MutateUngroundedVelocity(worldVelocityDirection);
        }

        private bool m_isJumping;

        public void JumpInput(InputAction.CallbackContext context)
        {
            m_isJumping = !context.canceled;
        }
    }
}