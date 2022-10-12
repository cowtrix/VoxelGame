using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interaction.Activities.Dotc
{
    public enum eFaction
    {
        Dire,
        Radiant
    }

    public enum eLane
    {
        Safe,
        Mid,
        Off,
    }

    public class DotcGame : Activity
    {
        public override string DisplayName => "Defense Of The Cubes";
        [ColorUsage(false, true)]
        public Color RadiantColor, DireColor;
        public LayerMask LayerMask;

        [Serializable]
        public class Faction
        {
            public eFaction Name;
            public DotcAncient Ancient;
            public DotcGameTower[] Towers;
        }
        public Faction Radiant, Dire;

        public IEnumerable<Transform> GetPathTo(eFaction faction, Transform target)
        {
            var otherAncient = faction == eFaction.Dire ? Radiant.Ancient : Dire.Ancient;
            return new[] { target, otherAncient.transform };
        }
    }

}