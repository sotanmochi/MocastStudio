namespace MocastStudio.Presentation.UIView
{
    public readonly struct UIViewStatus
    {
        public readonly UIViewType ViewType;
        public readonly UIViewStatusType StatusType;

        public UIViewStatus(UIViewType viewType, UIViewStatusType statusType)
        {
            ViewType = viewType;
            StatusType = statusType;
        }
    }
}
