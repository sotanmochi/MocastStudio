using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TMP_Text;

namespace MocastStudio.Universal.UIView.About
{
    public sealed class AcknowledgementsView : MonoBehaviour
    {
        [SerializeField] Text _text;
        [SerializeField] Button _close;

        public IObservable<Unit> OnClose => _close.OnClickAsObservable();

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}
