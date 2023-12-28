using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MocastStudio.Presentation.UIView
{
    public class SystemMenuView : MonoBehaviour
    {
        [SerializeField] Button _about;

        readonly Subject<UIViewType> _itemSelected = new();

        public IObservable<UIViewType> OnItemSelected => _itemSelected;

        void Awake()
        {
            _about.OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => 
                {
                    _itemSelected.OnNext(UIViewType.About);
                });
        }
    }
}
