using System;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TMP_Text;

namespace MocastStudio.Presentation.UIView.MotionDataSource
{
    public sealed class MotionDataSourceListItemView : MonoBehaviour
    {
        [SerializeField] Image _statusImage;
        [SerializeField] Text _idText;
        [SerializeField] Text _dataSourceTypeText;
        [SerializeField] Text _streamingDataIdText;
        [SerializeField] Text _serverAddressText;
        [SerializeField] Text _portText;
        [SerializeField] Toggle _connectionToggle;

        private int _dataSourceId = -1;

        public Observable<int> OnConnectionRequested =>
            _connectionToggle.OnValueChangedAsObservable()
                .Where(isOn => isOn)
                .Select(_ => _dataSourceId);

        public Observable<int> OnDisconnectionRequested =>
            _connectionToggle.OnValueChangedAsObservable()
                .Where(isOn => !isOn)
                .Select(_ => _dataSourceId);
        
        public void SetId(int dataSourceId)
        {
            _dataSourceId = dataSourceId;
            _idText.text = $"ID: {dataSourceId}";
        }

        public void SetType(string dataSourceType)
        {
            _dataSourceTypeText.text = $"Type: {dataSourceType}";
        }

        public void SetValues(string address, int port, int streamingDataId)
        {
            _serverAddressText.text = $"Address: {address}";
            _portText.text = $"Port: {port}";
            _streamingDataIdText.text = $"Streaming Data ID: {streamingDataId}";
        }

        public void SetStausColor(Color color)
        {
            _statusImage.color = color;
        }
    }
}
