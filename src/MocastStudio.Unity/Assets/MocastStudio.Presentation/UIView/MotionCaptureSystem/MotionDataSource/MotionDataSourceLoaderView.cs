using System;
using System.Collections.Generic;
using MocapSignalTransmission.Infrastructure.Constants;
using MocapSignalTransmission.MotionDataSource;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Dropdown = TMPro.TMP_Dropdown;
using InputField = TMPro.TMP_InputField;

namespace MocastStudio.Presentation.UIView.MotionDataSource
{
    public sealed class MotionDataSourceLoaderView : MonoBehaviour
    {
        public static readonly string DropdownMessage = "Select System Type";

        [SerializeField] Dropdown _dataSourceTypeDropdown;
        [SerializeField] InputField _streamingDataId;
        [SerializeField] InputField _serverAddress;
        [SerializeField] InputField _port;
        [SerializeField] Button _addButton;

        private readonly Subject<MotionDataSourceSettings> _dataSourceAdditionNotifier = new();

        private IReadOnlyList<MotionDataSourceType> _dataSourceTypes;
        private MotionDataSourceType _currentDataSourceType;

        public IObservable<MotionDataSourceSettings> OnDataSourceAdditionRequested => _dataSourceAdditionNotifier;

        void Awake() => Initialize();

        private void Initialize()
        {
            _addButton.OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => NotifyDataSourceAddtionRequest());
    
            _dataSourceTypeDropdown.OnValueChangedAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(selectedIndex => 
                {
                    if (selectedIndex < 1)
                    {
                        _currentDataSourceType = MotionDataSourceType.Unknown;
                    }
                    else
                    {
                        var index = selectedIndex - 1;
                        _currentDataSourceType = _dataSourceTypes[index];
                    }
                });
        }

        public void UpdateDataSourceTypeDropdown(IReadOnlyList<MotionDataSourceType> dropdownItems)
        {
            _dataSourceTypes = dropdownItems;

            _dataSourceTypeDropdown.ClearOptions();
            _dataSourceTypeDropdown.RefreshShownValue();
            _dataSourceTypeDropdown.options.Add(new Dropdown.OptionData { text = DropdownMessage });

            foreach (var item in dropdownItems)
            {
                _dataSourceTypeDropdown.options.Add(new Dropdown.OptionData { text = $"{item}" });
            }

            _dataSourceTypeDropdown.RefreshShownValue();
        }

        private void NotifyDataSourceAddtionRequest()
        {
            if (Int32.TryParse(_port.text, out var port) && Int32.TryParse(_streamingDataId.text, out var streamDataId))
            {
                _dataSourceAdditionNotifier.OnNext(new MotionDataSourceSettings
                (
                    dataSourceType: (int)_currentDataSourceType,
                    streamingDataId: streamDataId,
                    serverAddress: _serverAddress.text,
                    port: port
                ));
            }
            else
            {
                Debug.LogError($"[{nameof(MotionDataSourceLoaderView)}] Parse error.");
            }
        }
    }
}
