using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MocastStudio.Universal.UIView.MotionActor
{
    public sealed class MotionActorListView : MonoBehaviour
    {
        [SerializeField] MotionActorListItemView _itemViewPrefab;
        [SerializeField] VerticalLayoutGroup _contentsRoot;

        private readonly Subject<(int ActorId, bool RootBoneOffsetEnabled)> _rootBoneOffsetStausNotifier = new();
        private readonly Dictionary<int, MotionActorListItemView> _listItemViews = new();

        public IObservable<(int ActorId, bool RootBoneOffsetEnabled)> RootBoneOffsetStatus => _rootBoneOffsetStausNotifier;

        void OnDestroy() => RemoveAll();

        public void UpdateView(int id, string name)
        {
            if (_listItemViews.TryGetValue(id, out var view))
            {
                view.SetId(id);
                view.SetValues(name);
            }
            else
            {
                Add(id, name);
            }
        }

        public void Add(int id, string name)
        {
            var itemView = Instantiate(_itemViewPrefab) as MotionActorListItemView;
            itemView.transform.SetParent(_contentsRoot.transform, false);

            itemView.SetId(id);
            itemView.SetValues(name);

            itemView.OnRootBoneOffsetToggleValueChanged
                .TakeUntilDestroy(this)
                .Subscribe(status => _rootBoneOffsetStausNotifier.OnNext(status));

            _listItemViews.Add(id, itemView);
        }

        public void Remove(int id)
        {
            if (_listItemViews.Remove(id, out var itemView))
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
