using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;

namespace Interaction.Activities.Dotc
{
    public class DotcGameTower : DotcGameEntity
    {
        public float AttackSpeed = 1;
        public int AttackDamage = 1;
        public LineRenderer Laser;

        protected override void Start()
        {
            base.Start();
            StartCoroutine(AIBehaviour());
        }

        IEnumerator AIBehaviour()
        {
            while (true)
            {
                var closestEnemy = GetClosestEnemy();
                if (closestEnemy != null)
                {
                    Laser.SetPosition(1, Laser.transform.worldToLocalMatrix.MultiplyPoint3x4(closestEnemy.transform.position));
                    yield return new WaitForSeconds(AttackSpeed / 2f);
                    Laser.SetPosition(1, Vector3.zero);
                    closestEnemy.TakeDamage(AttackDamage);
                    yield return new WaitForSeconds(AttackSpeed / 2f);
                }
                yield return null;
            }
        }
    }
}