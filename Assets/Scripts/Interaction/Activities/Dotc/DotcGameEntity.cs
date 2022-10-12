using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul.Utilities;

namespace Interaction.Activities.Dotc
{
    public abstract class DotcGameEntity : Interactable
    {
        public override string DisplayName => $"{HitPoints}hp";

        public DotcGame Game;
        public eFaction Faction;
        public float AttackRadius = 1;
        public int HitPoints = 100;

        protected List<DotcGameEntity> LocalEntities = new List<DotcGameEntity>();

        protected override void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(Vector3.zero, AttackRadius);
            base.OnDrawGizmosSelected();
        }

        protected override int TickOnThread(float dt)
        {
            LocalEntities = Physics.OverlapSphere(transform.position, AttackRadius, Game.LayerMask, QueryTriggerInteraction.Collide)
                .Select(c => c.GetComponent<DotcGameEntity>())
                .Where(c => c && c != this)
                .ToList();
            return 2;
        }

        public void TakeDamage(int attackDamage)
        {
            HitPoints -= attackDamage;
            if(HitPoints <= 0 && this)
            {
                gameObject.SafeDestroy();
            }
        }

        protected DotcGameEntity GetClosestEnemy()
        {
            return LocalEntities.Where(e => e && e.Faction != Faction)
                .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
                .FirstOrDefault();
        }
    }
}