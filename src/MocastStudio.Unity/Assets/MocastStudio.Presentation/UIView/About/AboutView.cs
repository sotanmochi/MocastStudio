using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MocastStudio.Presentation.UIView.About
{
    public sealed class AboutView : MonoBehaviour
    {
        [SerializeField] Button _close;
        [SerializeField] Button _acknowledgements;

        public Observable<Unit> OnClose => _close.OnClickAsObservable();
        public Observable<Unit> OnOpenAcknowledgements => _acknowledgements.OnClickAsObservable();
    }
}
