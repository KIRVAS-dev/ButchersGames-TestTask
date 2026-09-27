using System;

namespace UI.FloatingText
{
    public interface IFloatingTextView
    {
        event Action GainSeriesEnded;
        event Action LossSeriesEnded;

        void ShowGain(int total);
        void ShowLoss(int total);
    }
}
