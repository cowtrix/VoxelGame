using UnityEngine;

namespace Generation
{
    public class DynamicGeneratorScaleNode : DynamicGeneratorNode
    {
        public bool Uniform;
        public Vector3 MinScale = Vector3.one;
        public Vector3 MaxScale = Vector3.one;

        protected override void GenerateInternal()
        {
            Random.InitState(Seed);
            if (Uniform)
            {
                var scale = Random.Range(MinScale.x, MaxScale.x);
                transform.localScale = new Vector3(
                    scale,
                    scale,
                    scale);
            }
            else
            {
                transform.localScale = new Vector3(
                    Random.Range(MinScale.x, MaxScale.x),
                    Random.Range(MinScale.y, MaxScale.y),
                    Random.Range(MinScale.z, MaxScale.z));
            }
        }
    }
}