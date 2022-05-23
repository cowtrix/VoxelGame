using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul.Utilities;

namespace Interaction.Activities.Dotc
{
    public class DotcGameCreep : DotcGameEntity
    {
        public float MoveSpeed = 1;
        public float PushOutDistance = 0.01234568f * 2;
        public float AttackSpeed = 1;
        public int AttackDamage = 1;
        public LineRenderer Laser;
        public Queue<Transform> CurrentPath { get; set; }

        public override string DisplayName => throw new System.NotImplementedException();

        public void WakeUp() => StartCoroutine(AIBehaviour());

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Vector3.zero, PushOutDistance);
        }

        IEnumerator AIBehaviour()
        {
            while (true)
            {
                var overlappingEntities = LocalEntities
                    .Where(e => e && e is DotcGameCreep && Vector3.Distance(e.transform.position, transform.position) < PushOutDistance)
                    .ToList();
                if (overlappingEntities.Any())
                {
                    var avgPos = overlappingEntities.First().transform.position;
                    foreach (var overlappingEntity in overlappingEntities.Skip(1))
                    {
                        avgPos += overlappingEntity.transform.position;
                    }
                    avgPos /= overlappingEntities.Count();

                    var pushOutDir = transform.worldToLocalMatrix.MultiplyVector((transform.position - avgPos).normalized.Flatten());
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, transform.localPosition + pushOutDir, MoveSpeed * Time.deltaTime);
                    yield return null;
                }

                var closestEnemy = GetClosestEnemy();
                if (closestEnemy != null)
                {
                    Laser.SetPosition(1, Laser.transform.worldToLocalMatrix.MultiplyPoint3x4(closestEnemy.transform.position));
                    yield return new WaitForSeconds(AttackSpeed / 2f);
                    Laser.SetPosition(1, Vector3.zero);
                    closestEnemy.TakeDamage(AttackDamage);
                    yield return new WaitForSeconds(AttackSpeed / 2f);
                }
                else
                {
                    if (CurrentPath == null || !CurrentPath.Any())
                    {
                        gameObject.SafeDestroy();
                        yield break;
                    }
                    if (Vector3.Distance(transform.localPosition, CurrentPath.Peek().localPosition) < AttackRadius * .75f)
                    {
                        CurrentPath.Dequeue();
                    }
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, CurrentPath.Peek().localPosition, MoveSpeed * Time.deltaTime);
                }
                yield return null;
            }
        }
    }
}