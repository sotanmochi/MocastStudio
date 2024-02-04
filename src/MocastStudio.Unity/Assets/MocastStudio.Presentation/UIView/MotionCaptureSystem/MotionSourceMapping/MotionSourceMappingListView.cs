using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Dropdown = TMPro.TMP_Dropdown;

namespace MocastStudio.Presentation.UIView.MotionSourceMapping
{
    public sealed class MotionSourceMappingListView : MonoBehaviour
    {
        public static readonly string MotionActorDropdownMessage = "Select Motion Actor ID";
        public static readonly string DataSourceDropdownMessage = "Select Data Source ID";

        [SerializeField] MotionSourceMappingListItemView _itemViewPrefab;
        [SerializeField] Dropdown _motionActorDropdown;
        [SerializeField] Dropdown _dataSourceDropdown;
        [SerializeField] Button _addButton;
        [SerializeField] VerticalLayoutGroup _contentsRoot;

        private readonly Dictionary<MotionActorDataSourcePair, MotionSourceMappingListItemView> _listItemViews = new();
        private readonly Subject<MotionActorDataSourcePair> _additionNotifier = new();
        private readonly Subject<MotionActorDataSourcePair> _removalNotifier = new();

        private IReadOnlyList<int> _motionActorIds;
        private IReadOnlyList<int> _dataSourceIds;
        private int _currentMotionActorId;
        private int _currentDataSourceId;

        public Observable<MotionActorDataSourcePair> OnAdditionRequested => _additionNotifier;
        public Observable<MotionActorDataSourcePair> OnRemovalRequested => _removalNotifier;

        void Awake() => Initialize();

        void OnDestroy() => RemoveAll();

        private void Initialize()
        {
            _addButton.OnClickAsObservable()
                .Subscribe(_ => 
                {
                    _additionNotifier.OnNext(new MotionActorDataSourcePair(_currentMotionActorId, _currentDataSourceId));
                })
                .AddTo(this);

            _motionActorDropdown.OnValueChangedAsObservable()
                .Subscribe(selectedIndex => 
                {
                    if (selectedIndex < 1)
                    {
                        _currentMotionActorId = -1;
                    }
                    else
                    {
                        var index = selectedIndex - 1;
                        _currentMotionActorId = _motionActorIds[index];
                    }
                })
                .AddTo(this);

            _dataSourceDropdown.OnValueChangedAsObservable()
                .Subscribe(selectedIndex => 
                {
                    if (selectedIndex < 1)
                    {
                        _currentDataSourceId = -1;
                    }
                    else
                    {
                        var index = selectedIndex - 1;
                        _currentDataSourceId = _dataSourceIds[index];
                    }
                })
                .AddTo(this);
        }

        public void UpdatMotionActorDropdown(IReadOnlyList<int> dropdownItems)
        {
            _motionActorIds = dropdownItems;

            _motionActorDropdown.ClearOptions();
            _motionActorDropdown.RefreshShownValue();
            _motionActorDropdown.options.Add(new Dropdown.OptionData { text = MotionActorDropdownMessage });
            
            foreach (var item in dropdownItems)
            {
                _motionActorDropdown.options.Add(new Dropdown.OptionData { text = $"{item}" });
            }
            
            _motionActorDropdown.RefreshShownValue();
        }

        public void UpdateDataSourceDropdown(IReadOnlyList<int> dropdownItems)
        {
            _dataSourceIds = dropdownItems;

            _dataSourceDropdown.ClearOptions();
            _dataSourceDropdown.RefreshShownValue();
            _dataSourceDropdown.options.Add(new Dropdown.OptionData { text = DataSourceDropdownMessage });

            foreach (var item in dropdownItems)
            {
                _dataSourceDropdown.options.Add(new Dropdown.OptionData { text = $"{item}" });
            }

            _dataSourceDropdown.RefreshShownValue();
        }

        public void UpdateItemView(MotionActorDataSourcePair data)
        {
            if (_listItemViews.TryGetValue(data, out var view))
            {
                view.SetValue(data);
            }
            else
            {
                AddItemView(data);
            }
        }

        public void RemoveItemView(MotionActorDataSourcePair data)
        {
            if (_listItemViews.Remove(data, out var itemView))
            {
                Destroy(itemView.gameObject);
            }
        }

        public void RemoveAll()
        {
            foreach (var itemView in _listItemViews.Values)
            {
                Destroy(itemView.gameObject);
            }
            _listItemViews.Clear();
        }

        private void AddItemView(MotionActorDataSourcePair data)
        {
            var itemView = Instantiate(_itemViewPrefab) as MotionSourceMappingListItemView;
            itemView.transform.SetParent(_contentsRoot.transform, false);

            itemView.SetValue(data);

            itemView.OnRemovalRequested
                .Subscribe(value => _removalNotifier.OnNext(value))
                .AddTo(this);

            _listItemViews.Add(data, itemView);
        }
    }
}
