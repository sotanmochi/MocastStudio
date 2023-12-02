using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TransmitterStatusType = MocapSignalTransmission.Transmitter.TransmitterStatusType;

namespace MocastStudio.Universal.UIView.Transmitter
{
    public sealed class TransmitterListView : MonoBehaviour
    {
        [SerializeField] TransmitterListItemView _itemViewPrefab;
        [SerializeField] VerticalLayoutGroup _contentsRoot;

        private readonly Subject<int> _onConnectionNotifier = new();
        private readonly Subject<int> _onDisconnectionNotifier = new();
        private readonly Dictionary<int, TransmitterListItemView> _listItemViews = new();

        public IObservable<int> OnConnectionRequested => _onConnectionNotifier;
        public IObservable<int> OnDisconnectionRequested => _onDisconnectionNotifier;

        public void UpdateStatus(int transmitterId, TransmitterStatusType statusType)
        {
            if (_listItemViews.TryGetValue(transmitterId, out var itemView))
            {
                itemView.SetStatus(statusType);
            }
        }

        public void UpdateActorIdList(int transmitterId, IReadOnlyCollection<int> actorIds)
        {
            if (_listItemViews.TryGetValue(transmitterId, out var itemView))
            {
                itemView.SetActorId(actorIds);
            }
        }

        public void UpdateItemView(int transmitterId, TransmitterSettingsRequest settings)
        {
            if (_listItemViews.TryGetValue(transmitterId, out var itemView))
            {
                itemView.SetValues(settings.TransmitterType, settings.ServerAddress, settings.Port);
            }
            else
            {
                AddItemView(transmitterId, settings);
            }
        }

        public void RemoveItemView(int transmitterId)
        {
            if (_listItemViews.Remove(transmitterId, out var itemView))
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

        private void AddItemView(int transmitterId, TransmitterSettingsRequest settings)
        {
            var itemView = Instantiate(_itemViewPrefab) as TransmitterListItemView;
            itemView.transform.SetParent(_contentsRoot.transform, false);

            itemView.SetId(transmitterId);
            itemView.SetActorId(settings.ActorIds);
            itemView.SetValues(settings.TransmitterType, settings.ServerAddress, settings.Port);

            itemView.OnConnectionRequested
                .TakeUntilDestroy(this)
                .Subscribe(id => _onConnectionNotifier.OnNext(id));

            itemView.OnDisconnectionRequested
                .TakeUntilDestroy(this)
                .Subscribe(id => _onDisconnectionNotifier.OnNext(id));

            _listItemViews.Add(transmitterId, itemView);
        }
    }
}
