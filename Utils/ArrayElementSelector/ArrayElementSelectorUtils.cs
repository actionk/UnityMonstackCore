using System;

namespace Plugins.Shared.UnityMonstackCore.Utils.ArrayElementSelector
{
    public static class ArrayElementSelectorUtils
    {
        private static Random DEFAULT_RANDOM = new Random();

        public static T SelectByThreshold<T>(this T[] array, float threshold) where T : IElementWithWeight
        {
            if (array.Length == 0)
                return default;

            T currentLevel = default;

            for (var i = 0; i < array.Length; i++)
            {
                var level = array[i];
                currentLevel = level;
                if (threshold < level.Weight)
                    break;
            }

            return currentLevel;
        }

        public static T SelectRandomlyByWeight<T>(this T[] array) where T : IElementWithWeight
        {
            return SelectRandomlyByWeight(array, DEFAULT_RANDOM);
        }

        public static T SelectRandomlyByWeight<T>(this T[] array, Random random) where T : IElementWithWeight
        {
            if (array.Length == 0)
                return default;

            int summaryWeight = 0;
            foreach (var element in array)
                summaryWeight += element.Weight;

            var threshold = random.Next(0, summaryWeight);

            T currentLevel = default;

            var currentWeight = 0;
            for (var i = 0; i < array.Length; i++)
            {
                var level = array[i];
                currentLevel = level;
                currentWeight += level.Weight;
                if (threshold < currentWeight)
                    break;
            }

            return currentLevel;
        }
    }
}