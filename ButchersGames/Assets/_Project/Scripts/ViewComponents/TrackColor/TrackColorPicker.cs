using UnityEngine;

namespace ViewComponents.TrackColor
{
    internal sealed class TrackColorPicker
    {
        private const int SingleColor = 1;

        private int _previousIndex;
        private bool _hasPrevious;

        internal int NextIndex(int colorCount)
        {
            int index = RandomIndex(colorCount);

            _previousIndex = index;
            _hasPrevious = true;

            return index;
        }

        private int RandomIndex(int colorCount)
        {
            bool canAvoidRepeat = _hasPrevious && colorCount > SingleColor;

            if (!canAvoidRepeat)
            {
                return Random.Range(0, colorCount);
            }

            int index = Random.Range(0, colorCount - 1);

            return index >= _previousIndex
                ? index + 1
                : index;
        }
    }
}
