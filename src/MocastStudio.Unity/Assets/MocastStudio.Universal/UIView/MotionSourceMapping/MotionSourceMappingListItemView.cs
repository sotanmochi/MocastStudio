using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TMP_Text;

namespace MocastStudio.Universal.UIView.MotionSourceMapping
{
    public sealed class MotionSourceMappingListItemView : MonoBehaviour
    {
        [SerializeField] Text _actorIdText;
        [SerializeField] Text _dataSourceIdText;
        [SerializeField] Button _removeButton;

        private MotionActorDataSourcePair _value;

        public IObservable<MotionActorDataSourcePair> OnRemovalRequested =>
            _removeButton.OnClickAsObservable().Select(_ => _value).TakeUntilDestroy(this);

        public void SetValue(MotionActorDataSourcePair value)
        {
            _value = value;
            _actorIdText.text = $"Actor ID: {value.ActorId}";
            _dataSourceIdText.text = $"Data Source ID: {value.DataSourceId}";
        }
    }
}
