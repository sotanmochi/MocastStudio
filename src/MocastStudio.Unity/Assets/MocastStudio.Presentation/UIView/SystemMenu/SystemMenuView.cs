using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MocastStudio.Presentation.UIView
{
    public class SystemMenuView : MonoBehaviour
    {
        [SerializeField] Button _mainCamera;
        [SerializeField] Button _motionCapture;
        [SerializeField] Button _about;

        readonly Subject<UIViewType> _itemSelected = new();

        public Observable<UIViewType> OnItemSelected => _itemSelected;

        void Awake()
        {
            _mainCamera.OnClickAsObservable()
                .Subscribe(_ => 
                {
                    _itemSelected.OnNext(UIViewType.MainCamera);
                })
                .AddTo(this);

            _motionCapture.OnClickAsObservable()
                .Subscribe(_ => 
                {
                    _itemSelected.OnNext(UIViewType.MotionCaptureSystem);
                })
                .AddTo(this);

            _about.OnClickAsObservable()
                .Subscribe(_ => 
                {
                    _itemSelected.OnNext(UIViewType.About);
                })
                .AddTo(this);
        }
    }
}
