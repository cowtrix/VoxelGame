using Actors;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Interaction.Activities
{
    public class PinballMachine : Activity
    {
        public string GameName = "Powzer's Pew Pew";
        public override string DisplayName => GameName;

        public int CurrentScore { get; set; }
        public AudioSource MusicSource => GetComponent<AudioSource>();
        public int PlayCost = 1;

        [Header("Game")]
        public Vector3 BallSpawnPosition;
        public Bounds BallCatcher;
        public Vector3 BallGravity = new Vector3(1, -1, 0);
        public float PaddleReturnSpeed = 1;
        public Rigidbody Ball, PaddleRight, PaddleLeft;
        public float PaddleRestRotation = 10;
        public float PaddlePushRotation = 30;
        public float PaddleAngleTolerance = 4;
        public TextMeshPro Text;

        [Header("Audio")]
        public AudioClip[] PaddleSounds;
        public float PaddleVolume = .5f;
        public AudioClip[] Music;
        public Vector2 MusicPlayTime = new Vector2(30, 60);

        private float m_nextMusicPlay;
        private bool m_isPlaying;
        private float m_paddleLRot, m_paddleRRot;
        private List<Rotator> m_rotators;

        protected override void Start()
        {
            m_rotators = new List<Rotator>(GetComponentsInChildren<Rotator>());
            foreach (var r in m_rotators)
            {
                r.enabled = false;
            }
            m_paddleLRot = PaddleRestRotation;
            m_paddleRRot = PaddleRestRotation;
            m_nextMusicPlay = Random.Range(MusicPlayTime.x, MusicPlayTime.y);
            CurrentScore = 0;
            base.Start();
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            Ball.AddForce(transform.localToWorldMatrix.MultiplyVector(BallGravity) * dt);

            if (Quaternion.Angle(PaddleLeft.transform.localRotation, Quaternion.Euler(0, m_paddleLRot, 0)) < PaddleAngleTolerance)
            {
                m_paddleLRot = PaddleRestRotation;
            }
            if (Quaternion.Angle(PaddleRight.transform.localRotation, Quaternion.Euler(0, -m_paddleRRot, 0)) < PaddleAngleTolerance)
            {
                m_paddleRRot = PaddleRestRotation;
            }

            PaddleRight.MoveRotation((Quaternion.Lerp(PaddleRight.rotation, transform.rotation * Quaternion.Euler(0, -m_paddleRRot, 0), PaddleReturnSpeed * dt)));
            PaddleLeft.MoveRotation((Quaternion.Lerp(PaddleLeft.rotation, transform.rotation * Quaternion.Euler(0, m_paddleLRot, 0), PaddleReturnSpeed * dt)));

            if (!BallCatcher.Contains(Ball.transform.localPosition))
            {
                // Lose condition
                m_isPlaying = false;
                foreach (var r in m_rotators)
                {
                    r.enabled = false;
                }
                Ball.velocity = Vector3.zero;
                Ball.angularVelocity = Vector3.zero;
            }

            Text.text = $"{CurrentScore}";
        }

        protected void StartPlaying()
        {
            if (m_isPlaying)
            {
                return;
            }
            m_isPlaying = true;
            foreach (var r in m_rotators)
            {
                r.enabled = true;
            }
            CurrentScore = 0;
            Ball.velocity = Vector3.zero;
            Ball.angularVelocity = Vector3.zero;
            Ball.transform.localPosition = BallSpawnPosition;
            if (!MusicSource.isPlaying)
            {
                MusicSource.PlayOneShot(Music.Random());
            }
        }

        protected override int Tick(float dt)
        {
            m_nextMusicPlay -= dt;
            if (m_nextMusicPlay < 0)
            {
                var clip = Music.Random();
                MusicSource.PlayOneShot(clip);
                m_nextMusicPlay = clip.length + Random.Range(MusicPlayTime.x, MusicPlayTime.y);
            }
            return 1;
        }

        public override IEnumerable<ActorAction> GetActions(Actor context)
        {
            if (context == Actor)
            {
                if (!m_isPlaying)
                {
                    yield return new ActorAction(eActionKey.USE, $"Insert Coin ({PlayCost}¢)", gameObject);
                }
                yield return new ActorAction(eActionKey.MOVE, "Move Paddles", gameObject);
            }
            foreach (var a in base.GetActions(context))
            {
                yield return a;
            }
        }

        public override void ReceiveAction(Actor actor, ActorAction action)
        {
            if (actor == Actor)
            {
                if (action.State == eActionState.Start && action.Key == eActionKey.MOVE)
                {
                    if (action.VectorContext.x > .5f)
                    {
                        m_paddleRRot = -PaddlePushRotation;
                        AudioSource.PlayClipAtPoint(PaddleSounds.Random(), PaddleRight.position, PaddleVolume);
                    }
                    else if (action.VectorContext.x < -.5f)
                    {
                        m_paddleLRot = -PaddlePushRotation;
                        AudioSource.PlayClipAtPoint(PaddleSounds.Random(), PaddleLeft.position, PaddleVolume);
                    }
                    return;
                }
                if (!m_isPlaying && action.Key == eActionKey.USE && action.State == eActionState.End && actor.State.TryAdd(eStateKey.Credits, -PlayCost, DisplayName))
                {
                    StartPlaying();
                }
            }
            base.ReceiveAction(actor, action);
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawLine(Vector3.zero, BallGravity);
            Gizmos.DrawWireCube(BallCatcher.center, BallCatcher.size);
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(BallSpawnPosition, .05f);
        }
    }
}