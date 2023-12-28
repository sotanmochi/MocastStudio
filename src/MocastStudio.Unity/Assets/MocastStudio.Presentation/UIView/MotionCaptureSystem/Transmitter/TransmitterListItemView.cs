using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TMP_Text;
using TransmitterStatusType = MocapSignalTransmission.Transmitter.TransmitterStatusType;

namespace MocastStudio.Presentation.UIView.Transmitter
{
    public sealed class TransmitterListItemView : MonoBehaviour
    {
        [SerializeField] Text _idText;
        [SerializeField] Text _transmitterTypeText;
        [SerializeField] Text _actorIdText;
        [SerializeField] Text _serverAddressText;
        [SerializeField] Text _portText;
        [SerializeField] Image _statusImage;
        [SerializeField] Toggle _connectionToggle;

        private int _id;

        public IObservable<int> OnConnectionRequested => 
            _connectionToggle.OnValueChangedAsObservable()
                .Where(isOn => isOn)
                .Select(_ => _id)
                .TakeUntilDestroy(this);

        public IObservable<int> OnDisconnectionRequested => 
            _connectionToggle.OnValueChangedAsObservable()
                .Where(isOn => !isOn)
                .Select(_ => _id)
                .TakeUntilDestroy(this);

        public void SetId(int id)
        {
            _id = id;
            _idText.text = $"ID: {id}";
        }

        public void SetActorId(IReadOnlyCollection<int> actorIds)
        {
            if (actorIds != null && actorIds.Count > 0)
            {
                var actorIdList = string.Join(", ", actorIds);
                _actorIdText.text = $"Actor ID: {actorIdList}";
            }
        }

        public void SetValues(TransmitterType transmitterType, string address, int port)
        {
            _transmitterTypeText.text = $"Type: {transmitterType}";
            _serverAddressText.text = $"Address: {address}";
            _portText.text = $"Port: {port}";
        }

        public void SetStatus(TransmitterStatusType statusType)
        {
            _statusImage.color = statusType switch
            {
                TransmitterStatusType.Disconnected => Color.red,
                TransmitterStatusType.Connecting => Color.yellow,
                TransmitterStatusType.Connected => Color.green,
                _ => Color.gray,
            };

            var toggleIsOn = statusType != TransmitterStatusType.Disconnected;
            _connectionToggle.SetIsOnWithoutNotify(toggleIsOn);
        }
    }
}
