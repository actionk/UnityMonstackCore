using Plugins.UnityMonstackCore.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace Plugins.Shared.UnityMonstackCore.Extensions
{
    public class RandomExtensionTest : MonoBehaviour
    {
        public GameObject prefab;
        public int count;
        public int seed;
        public float radius;

        [Button]
        public void Clean()
        {
            transform.DestroyChildren(true);
        }

        [Button]
        public void Random()
        {
            var random = new Random(seed);
            for (var i = 0; i < count; i++)
            {
                var randomValue = random.Next2DPointInRadius(radius);
                var created = Instantiate(prefab, randomValue.ToFloat3(), Quaternion.identity, transform);
            }
        }
    }
}