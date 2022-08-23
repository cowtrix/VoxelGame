using Actors;
using Common;
using Interaction.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Voxul;

namespace Interaction.Activities
{
    public class ClawMachine : Activity
    {
        public int PlayCost = 1;

        public override string DisplayName => name;
        public float MovementSpeed = .01f;
        public float AutoMoveSpeed = .5f;
        public float ChaseSpeed = 1;
        public float ClawSpeed = 1;

        public VoxelColorTint ButtonTint;
        [ColorUsage(false, true)]
        public Color TintColor;
        public float m_buttonTintAmount;

        public float JoystickRotationAmount = 20;
        public float JoystickReturnSpeed = 1;
        public Transform Claw, ClawOrigin, JoyStick;

        public Item[] Prizes;
        public int PrizeCount = 20;

        [Range(0, 1)]
        public float X;
        [Range(0, 1)]
        public float Y;
        [Range(0, 1)]
        public float Z;
        [Range(0, 1)]
        public float ClawOpen;
        public Bounds ClawBounds;
        public BezierConnectorLineRenderer LineRenderer;

        public bool IsGrabbing { get; private set; }
        private bool m_hasInitialized = false;
        private ClawMachineClaw[] m_claws;
        private Vector2 m_move;

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(ClawBounds.center, ClawBounds.size);
        }

        protected override void Start()
        {
            m_claws = GetComponentsInChildren<ClawMachineClaw>();
            for (var i = 0; i < PrizeCount; ++i)
            {
                var newPrize = Instantiate(Prizes.Random().gameObject);
                newPrize.transform.SetParent(transform);
                newPrize.transform.localPosition = GetPoint(new Vector3(.5f, .5f, .5f));
            }
            base.Start();
        }

        private Vector3 GetPoint(Vector3 normalizedPoint)
        {
            return ClawBounds.min + new Vector3(ClawBounds.size.x * normalizedPoint.x, ClawBounds.size.y * normalizedPoint.y, ClawBounds.size.z * normalizedPoint.z);
        }

        public override void OnStopActivity(Actor actor)
        {
            m_move = default;
            base.OnStopActivity(actor);
        }

        private void Update()
        {
            {
                // Update movement
                var direction = m_move * MovementSpeed;
                X = Mathf.Clamp01(X - direction.y);
                Z = Mathf.Clamp01(Z + direction.x);

                var offset = direction;
                var angle = new Vector3(Mathf.Atan(offset.x / 1) * Mathf.Rad2Deg, 0, Mathf.Atan(offset.y / 1) * Mathf.Rad2Deg) * JoystickRotationAmount;
                JoyStick.localRotation = Quaternion.Euler(angle);
            }

            foreach (var claw in m_claws)
            {
                claw.TargetOpenAmount = ClawOpen;
            }

            m_buttonTintAmount = Mathf.Clamp01(m_buttonTintAmount - Time.deltaTime);
            ButtonTint.Color = Color.Lerp(Color.white, TintColor, m_buttonTintAmount);
            ButtonTint.Invalidate();

            var targetPosition = GetPoint(new Vector3(X, Y, Z));
            if (targetPosition == Claw.localPosition && m_hasInitialized)
            {
                return;
            }
            m_hasInitialized = true;
            Claw.localPosition = Vector3.Lerp(Claw.localPosition, targetPosition, Time.deltaTime * ChaseSpeed);

            {
                var offset = (Claw.localPosition - ClawOrigin.localPosition).normalized;
                var angle = new Vector3(Mathf.Atan(offset.z / offset.y) * Mathf.Rad2Deg, 0, Mathf.Atan(-offset.x / offset.y) * Mathf.Rad2Deg);
                Claw.localRotation = Quaternion.Euler(angle);
            }
            {
                JoyStick.localRotation = Quaternion.RotateTowards(JoyStick.localRotation, Quaternion.identity, JoystickReturnSpeed * Time.deltaTime);
            }

            ClawOrigin.localPosition = targetPosition.xz().x0z(ClawOrigin.localPosition.y);
            LineRenderer.Invalidate();
        }

        public override IEnumerable<ActorAction> GetActions(Actor context)
        {
            if (!CanUse(context))
                yield break;
            if (Actor == context)
            {
                yield return new ActorAction(eActionKey.MOVE, "Move Claw", gameObject);
                yield return new ActorAction(eActionKey.EQUIP, "Pick With Claw", gameObject);
                yield return new ActorAction(eActionKey.EXIT, "Stop Playing", gameObject);
                yield break;
            }
            else
            {
                yield return new ActorAction(eActionKey.USE, "Start Playing", gameObject);
            }
        }

        public override void ReceiveAction(Actor actor, ActorAction action)
        {
            if (Actor == actor)
            {
                switch (action.Key)
                {
                    case eActionKey.MOVE:
                        var delta = action.VectorContext;
                        Move(actor, delta);
                        return;
                    case eActionKey.EQUIP:
                        if (action.State == eActionState.End)
                        {
                            Fire(actor);
                        }
                        return;
                }
            }

            base.ReceiveAction(actor, action);
        }

        public void Fire(Actor playerActor)
        {
            if (IsGrabbing)
            {
                return;
            }
            if (!playerActor.State.TryAdd(eStateKey.Credits, -PlayCost, DisplayName))
            {
                return;
            }
            StartCoroutine(GrabItem());
        }

        IEnumerator GrabItem()
        {
            IsGrabbing = true;
            m_buttonTintAmount = 1;
            while (Y > 0)
            {
                Y = Mathf.Clamp01(Y - AutoMoveSpeed * Time.deltaTime);
                yield return null;
            }
            foreach (var c in m_claws)
            {
                c.CheckCollision = true;
            }
            while (ClawOpen < 1)
            {
                ClawOpen = Mathf.Clamp01(ClawOpen + ClawSpeed * Time.deltaTime);
                yield return null;
            }
            while (Y < 1)
            {
                Y = Mathf.Clamp01(Y + AutoMoveSpeed * Time.deltaTime);
                yield return null;
            }
            while (X < 1 || Z > 0)
            {
                X = Mathf.Clamp01(X + AutoMoveSpeed * Time.deltaTime);
                Z = Mathf.Clamp01(Z - AutoMoveSpeed * Time.deltaTime);
                yield return null;
            }
            foreach (var c in m_claws)
            {
                c.CheckCollision = false;
            }
            while (ClawOpen > 0)
            {
                ClawOpen = Mathf.Clamp01(ClawOpen - ClawSpeed * Time.deltaTime);
                yield return null;
            }
            IsGrabbing = false;
        }

        public void Move(Actor actor, Vector2 direction)
        {
            if (IsGrabbing)
            {
                m_move = Vector2.zero;
                return;
            }
            m_move = direction;
        }
    }
}