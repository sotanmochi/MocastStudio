using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MocastStudio.Presentation.UIView.About
{
    public sealed class AboutView : MonoBehaviour
    {
        [SerializeField] Button _close;
        [SerializeField] Button _acknowledgements;

        public IObservable<Unit> OnClose => _close.OnClickAsObservable();
        public IObservable<Unit> OnOpenAcknowledgements => _acknowledgements.OnClickAsObservable();
    }
}
