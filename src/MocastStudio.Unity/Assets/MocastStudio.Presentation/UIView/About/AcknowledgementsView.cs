using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TMP_Text;

namespace MocastStudio.Presentation.UIView.About
{
    public sealed class AcknowledgementsView : MonoBehaviour
    {
        [SerializeField] Text _text;
        [SerializeField] Button _close;
        [SerializeField] TextAsset _license;
        [SerializeField] TextAsset _thirdPartyNotices;

        public IObservable<Unit> OnClose => _close.OnClickAsObservable();

        void Awake()
        {
            _text.text = Environment.NewLine + _license.text
                        + Environment.NewLine + _thirdPartyNotices.text;
        }
    }
}
