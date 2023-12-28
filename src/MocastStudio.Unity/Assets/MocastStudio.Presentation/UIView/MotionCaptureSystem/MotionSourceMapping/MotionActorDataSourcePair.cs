namespace MocastStudio.Presentation.UIView.MotionSourceMapping
{
    public readonly struct MotionActorDataSourcePair
    {
        public readonly int ActorId;
        public readonly int DataSourceId;

        public MotionActorDataSourcePair(int actorId, int dataSourceId)
        {
            ActorId = actorId;
            DataSourceId = dataSourceId;
        }
    }
}
