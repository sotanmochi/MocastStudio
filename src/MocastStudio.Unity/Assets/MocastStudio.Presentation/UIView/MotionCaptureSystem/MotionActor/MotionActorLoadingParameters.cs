namespace MocastStudio.Presentation.UIView.MotionActor
{
    public sealed class MotionActorLoadingParameters
    {
        public string ResourcePath { get; }

        public MotionActorLoadingParameters(string resourcePath)
        {
            ResourcePath = resourcePath;
        }
    }
}
