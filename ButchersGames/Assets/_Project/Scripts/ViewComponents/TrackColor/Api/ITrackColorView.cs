namespace ViewComponents.TrackColor
{
    internal interface ITrackColorView
    {
        int ColorCount { get; }

        void SetColor(int index);
    }
}
