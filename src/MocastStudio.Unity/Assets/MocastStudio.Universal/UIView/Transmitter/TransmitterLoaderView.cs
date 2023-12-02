using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Dropdown = TMPro.TMP_Dropdown;
using InputField = TMPro.TMP_InputField;

namespace MocastStudio.Universal.UIView.Transmitter
{
    public sealed class TransmitterLoaderView : MonoBehaviour
    {
        public static readonly string DropdownMessage = "Select Streaming Type";

        [SerializeField] Dropdown _dropdown;
        [SerializeField] InputField _actorIdListInputField;
        [SerializeField] InputField _serverAddress;
        [SerializeField] InputField _port;
        [SerializeField] Button _addButton;

        private readonly Subject<TransmitterSettingsRequest> _additionNotifier = new();
        private readonly List<int> _actorIdList = new();

        private IReadOnlyList<TransmitterType> _transmitterTypes;
        private TransmitterType _selectedTransmitterType;

        public IObservable<TransmitterSettingsRequest> OnAdditionRequested => _additionNotifier;

        void Awake() => Initialize();

        private void Initialize()
        {
            _addButton.OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => NotifyAdditionRequest());

            _dropdown.OnValueChangedAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(selectedIndex =>
                {
                    if (selectedIndex < 1)
                    {
                        _selectedTransmitterType = TransmitterType.Unknown;
                    }
                    else
                    {
                        var index = selectedIndex - 1;
                        _selectedTransmitterType = _transmitterTypes[index];
                    }
                });
        }

        public void UpdateTransmitterTypeDropdown(IReadOnlyList<TransmitterType> dropdownItems)
        {
            _transmitterTypes = dropdownItems;

            _dropdown.ClearOptions();
            _dropdown.RefreshShownValue();
            _dropdown.options.Add(new Dropdown.OptionData { text = DropdownMessage });

            foreach (var item in dropdownItems)
            {
                _dropdown.options.Add(new Dropdown.OptionData { text = $"{item}" });
            }

            _dropdown.RefreshShownValue();
        }

        private void NotifyAdditionRequest()
        {
            if (Int32.TryParse(_port.text, out var port))
            {
                _actorIdList.Clear();

                foreach (var str in _actorIdListInputField.text.Split(','))
                {
                    if (Int32.TryParse(str.Trim(), out var value))
                    {
                        _actorIdList.Add(value);
                    }
                }

                _additionNotifier.OnNext(new TransmitterSettingsRequest(_selectedTransmitterType,_actorIdList.AsReadOnly(), _serverAddress.text, port));
            }
            else
            {
                Debug.LogError($"[{nameof(TransmitterLoaderView)}] Parse error of port number");
            }
        }
    }
}
