using Common;
using Interaction.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;

public class AutoProperty<T>
{
    public T Value
    {
        get
        {
            if (m_value == null || m_value.Equals(null))
            {
                m_value = Getter.Invoke(Context);
                if (m_value == null || m_value.Equals(null))
                {
                    throw new Exception("AutoProperty getter failed to return object");
                }
            }
            return m_value;
        }
    }
    private T m_value;
    public Func<GameObject, T> Getter { get; private set; }
    public GameObject Context { get; private set; }
    public AutoProperty(GameObject context, Func<GameObject, T> getter)
    {
        Context = context;
        Getter = getter;
    }
}


namespace Actors
{
    public interface ILookAdapter
    {
        Transform transform { get; }
        bool CanSee(Vector3 worldPosition);
    }

    public class MovementController : ExtendedMonoBehaviour, IMovementController
    {
        public bool IsGrounded { get; private set; }
        public float TimeUngrounded { get; private set; }
        public Vector2 MoveDirection { get; private set; }
        public Vector3 CurrentGravity { get; private set; }
        public AutoProperty<ILookAdapter> LookAdapter { get; private set; }
        public Rigidbody Rigidbody => GetComponent<Rigidbody>();
        public ActorState State => GetComponent<ActorState>();
        public Actor Actor => GetComponent<Actor>();
        private GravityManager GravityManager => GravityManager.Instance;

        [Header("Parameters")]
        public float MovementSpeed = 2000;
        public float JumpForce = 300;
        public LayerMask CollisionMask = 1 << 8;
        public float RotateTowardGravitySpeed = 10;
        public Transform GroundingPoint;
        public float PushOutSpeed = 10;

        protected bool m_inputJump;

        protected virtual void Start()
        {
            LookAdapter = new AutoProperty<ILookAdapter>(gameObject, (go) => go.GetComponentByInterfaceInChildren<ILookAdapter>());
            Rigidbody.useGravity = false;
        }

        protected virtual void FixedUpdate()
        {
            var dt = Time.fixedDeltaTime;
            CurrentGravity = GravityManager.GetGravityForce(transform.position);

            var groundingDistance = Vector3.Distance(GroundingPoint.position, transform.position);
            IsGrounded = Physics.Raycast(transform.position, CurrentGravity, out var groundHit, groundingDistance * 1.01f, CollisionMask, QueryTriggerInteraction.Ignore);
            var isGroundedForward = Physics.Raycast(transform.position + LookAdapter.Value.transform.forward, CurrentGravity, out var forwardHit, groundingDistance, CollisionMask, QueryTriggerInteraction.Ignore);
            Debug.DrawLine(transform.position, transform.position + CurrentGravity * dt, Color.green);

            if (!IsGrounded)
            {
                Rigidbody.AddForce(CurrentGravity * dt * Rigidbody.mass);
                TimeUngrounded += dt;
            }
            else
            {
                TimeUngrounded = 0;
            }

            {
                // Straighten up
                var straightenQuat = Quaternion.Euler(Quaternion.FromToRotation(Vector3.up, -CurrentGravity.normalized)
                    .eulerAngles.xy().x0z(Rigidbody.rotation.eulerAngles.y));
                var straightenLerp = RotateTowardGravitySpeed * dt * (CurrentGravity.magnitude / 1000f);
                Rigidbody.rotation = Quaternion.Lerp(Rigidbody.rotation, straightenQuat, straightenLerp);
                //Rigidbody.rotation = straightenQuat;
                Debug.DrawLine(transform.position, transform.position + straightenQuat * transform.forward, Color.yellow);
            }

            {
                // Push out
                if (groundHit.distance < groundingDistance)
                {
                    Rigidbody.MovePosition(Rigidbody.position + groundHit.normal * (groundingDistance - groundHit.distance) * dt * PushOutSpeed);
                }
            }

            {
                // Move from input
                if (m_inputJump && IsGrounded)
                {
                    Rigidbody.AddForce(transform.localToWorldMatrix.MultiplyVector(JumpForce * transform.up) * Rigidbody.mass);
                    m_inputJump = false;
                }

                var movement = MoveDirection;
                var localVelocityDirection = new Vector3(movement.x, 0, movement.y);
                var worldVelocityDirection = LookAdapter.Value.transform.localToWorldMatrix.MultiplyVector(localVelocityDirection);

                if (IsGrounded)
                {
                    var normal = isGroundedForward ? forwardHit.normal : groundHit.normal;
                    worldVelocityDirection -= normal * Vector3.Dot(worldVelocityDirection, normal);
                    worldVelocityDirection = worldVelocityDirection.normalized * MovementSpeed;
                }
                else
                {
                    worldVelocityDirection = MutateUngroundedVelocity(worldVelocityDirection);
                }

                Debug.DrawLine(transform.position, transform.position + worldVelocityDirection, Color.cyan, 5);
                Rigidbody.AddForce(worldVelocityDirection * dt);
            }
            if (Actor.Animator)
            {
                Actor.Animator.SetFloat("VelocityX", Rigidbody.velocity.x);
                Actor.Animator.SetFloat("VelocityY", Rigidbody.velocity.y);
                Actor.Animator.SetFloat("VelocityZ", Rigidbody.velocity.z);
            }
        }

        public void Jump()
        {
            m_inputJump = true;
        }

        protected virtual Vector3 MutateUngroundedVelocity(Vector3 worldVelocityDirection)
        {
            worldVelocityDirection *= 0.01f;
            return worldVelocityDirection;
        }

        private void OnDrawGizmosSelected()
        {
            var gravityVec = GravityManager.GetGravityForce(transform.position);
            Gizmos.DrawLine(transform.position, GroundingPoint.position);
            Gizmos.DrawWireCube(GroundingPoint.position, Vector3.one * .05f);
        }

        public void Move(Vector2 dir)
        {
            //Debug.Log($"Move: {dir}");
            MoveDirection = dir;
        }
    }
}