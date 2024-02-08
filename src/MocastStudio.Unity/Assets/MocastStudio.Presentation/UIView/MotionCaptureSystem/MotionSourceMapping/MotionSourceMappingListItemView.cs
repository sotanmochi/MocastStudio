using System;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TMP_Text;

namespace MocastStudio.Presentation.UIView.MotionSourceMapping
{
    public sealed class MotionSourceMappingListItemView : MonoBehaviour
    {
        [SerializeField] Text _actorIdText;
        [SerializeField] Text _dataSourceIdText;
        [SerializeField] Button _removeButton;

        private MotionActorDataSourcePair _value;

        public Observable<MotionActorDataSourcePair> OnRemovalRequested =>
            _removeButton.OnClickAsObservable().Select(_ => _value);
        
        public void SetValue(MotionActorDataSourcePair value)
        {
            _value = value;
            _actorIdText.text = $"Actor ID: {value.ActorId}";
            _dataSourceIdText.text = $"Data Source ID: {value.DataSourceId}";
        }
    }
}
