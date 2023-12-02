using System;
using TMPro;

namespace UniRx
{
    public static partial class TextMeshProComponentExtensions
    {
        /// <summary>Observe onValueChanged with current `value` on subscribe.</summary>
        public static IObservable<int> OnValueChangedAsObservable(this TMP_Dropdown dropdown)
        {
            return Observable.CreateWithState<int, TMP_Dropdown>(dropdown, (d, observer) =>
            {
                observer.OnNext(d.value);
                return d.onValueChanged.AsObservable().Subscribe(observer);
            });
        }
    }
}
