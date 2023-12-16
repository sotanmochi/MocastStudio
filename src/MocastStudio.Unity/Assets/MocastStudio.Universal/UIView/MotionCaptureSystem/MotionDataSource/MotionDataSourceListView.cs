using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MocastStudio.Universal.UIView.MotionDataSource
{
    public sealed class MotionDataSourceListView : MonoBehaviour
    {
        [SerializeField] MotionDataSourceListItemView _nodePrefab;
        [SerializeField] VerticalLayoutGroup _contentsRoot;

        private readonly Dictionary<int, MotionDataSourceListItemView> _listItemViews = new();
        private readonly Subject<int> _connectionNotifier = new();
        private readonly Subject<int> _disconnectionNotifier = new();

        public IObservable<int> OnConnectionRequested => _connectionNotifier;
        public IObservable<int> OnDisconnectionRequested => _disconnectionNotifier;

        void OnDestroy() => RemoveAll();

        public void UpdateView(int dataSourceId, string dataSourceType, string address, int port, int streamingDataId)
        {
            if (_listItemViews.TryGetValue(dataSourceId, out var view))
            {
                view.SetType(dataSourceType);
                view.SetValues(address, port, streamingDataId);
            }
            else
            {
                Add(dataSourceId, dataSourceType, address, port, streamingDataId);
            }
        }

        public void UpdateStatusColor(int dataSourceId, Color color)
        {
            if (_listItemViews.TryGetValue(dataSourceId, out var view))
            {
                view.SetStausColor(color);
            }
        }

        public void Add(int dataSourceId, string dataSourceType, string address, int port, int streamingDataId)
        {
            var itemView = Instantiate(_nodePrefab) as MotionDataSourceListItemView;
            itemView.transform.SetParent(_contentsRoot.transform, false);

            itemView.SetId(dataSourceId);
            itemView.SetType(dataSourceType);
            itemView.SetValues(address, port, streamingDataId);

            itemView.OnConnectionRequested
                .TakeUntilDestroy(this)
                .Subscribe(id => _connectionNotifier.OnNext(id));

            itemView.OnDisconnectionRequested
                .TakeUntilDestroy(this)
                .Subscribe(id => _disconnectionNotifier.OnNext(id));

            _listItemViews.Add(dataSourceId, itemView);
        }

        public void Remove(int dataSourceId)
        {
            if (_listItemViews.Remove(dataSourceId, out var itemView))
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
    }
}
