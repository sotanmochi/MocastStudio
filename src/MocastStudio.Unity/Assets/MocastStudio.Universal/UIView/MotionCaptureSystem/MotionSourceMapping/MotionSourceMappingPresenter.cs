using System;
using System.Linq;
using MessagePipe;
using MocapSignalTransmission.MotionActor;
using MocapSignalTransmission.MotionDataSource;
using UniRx;
using VContainer.Unity;

namespace MocastStudio.Universal.UIView.MotionSourceMapping
{
    public sealed class MotionSourceMappingPresenter : IInitializable, IDisposable
    {
        readonly MotionActorService _motionActorService;
        readonly MotionActorServiceContext _motionActorContext;
        readonly MotionDataSourceServiceContext _motionDataSourceContext;
        readonly MotionSourceMappingListView _motionSourceMappingListView;
        readonly CompositeDisposable _compositeDisposable = new();

        public MotionSourceMappingPresenter(
            MotionActorService motionActorService,
            MotionActorServiceContext motionActorContext,
            MotionDataSourceServiceContext motionDataSourceContext,
            MotionSourceMappingListView motionSourceMappingListView)
        {
            _motionActorService = motionActorService;
            _motionActorContext = motionActorContext;
            _motionDataSourceContext = motionDataSourceContext;
            _motionSourceMappingListView = motionSourceMappingListView;
        }

        void IDisposable.Dispose()
        {
            _compositeDisposable.Dispose();
        }

        void IInitializable.Initialize()
        {
            UnityEngine.Debug.Log($"<color=lime>[{nameof(MotionSourceMappingPresenter)}] Initialize</color>");

            _motionActorContext.OnMotionActorAdded
                .Subscribe(_ =>
                {
                    var motionActorIds = _motionActorContext.MotionActors
                        .Select(motionActor => motionActor.ActorId)
                        .ToList();

                    _motionSourceMappingListView.UpdatMotionActorDropdown(motionActorIds);
                })
                .AddTo(_compositeDisposable);

            _motionDataSourceContext.OnDataSourceAdded
                .Subscribe(_ =>
                {
                    var dataSourceIds = _motionDataSourceContext.DataSourceSettingsList
                        .Select(dataSource => dataSource.DataSourceId)
                        .ToList();

                    _motionSourceMappingListView.UpdateDataSourceDropdown(dataSourceIds);
                })
                .AddTo(_compositeDisposable);

            _motionActorContext.OnDataSourceMappingUpdated
                .Subscribe(motionActorDataSourcePairs =>
                {
                    foreach (var pair in motionActorDataSourcePairs)
                    {
                        _motionSourceMappingListView.UpdateItemView(new MotionActorDataSourcePair(pair.ActorId, pair.DataSourceId));
                    }                   
                })
                .AddTo(_compositeDisposable);

            _motionSourceMappingListView.OnAdditionRequested
                .Subscribe(value =>
                {
                    if (value.ActorId < 0 || value.DataSourceId < 0)
                    {
                        UnityEngine.Debug.LogError($"[{nameof(MotionSourceMappingPresenter)}] Invalid DataSourceId: {value.DataSourceId}, ActorId: {value.ActorId}");
                        return;
                    }
                    _motionActorService.TryAddDataSourceMapping(value.ActorId, value.DataSourceId);
                })
                .AddTo(_compositeDisposable);

            _motionSourceMappingListView.OnRemovalRequested
                .Subscribe(value =>
                {
                    _motionActorService.RemoveDataSourceMapping(value.ActorId, value.DataSourceId);
                    _motionSourceMappingListView.RemoveItemView(value);
                })
                .AddTo(_compositeDisposable);
        }
    }
}
