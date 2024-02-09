using System.Threading;
using TMPro;
using UnityEngine;

namespace R3
{
    public static partial class TextMeshProComponentExtensions
    {
        internal static CancellationToken GetDestroyCancellationToken(this MonoBehaviour value)
        {
            // UNITY_2022_2_OR_NEWER has MonoBehavior.destroyCancellationToken
#if UNITY_2022_2_OR_NEWER
            return value.destroyCancellationToken;
#else
            return CancellationToken.None;;
#endif
        }
        
        /// <summary>Observe onValueChanged with current `value` on subscribe.</summary>
        public static Observable<int> OnValueChangedAsObservable(this TMP_Dropdown dropdown)
        {
            return Observable.Create<int, TMP_Dropdown>(dropdown, static (observer, d) =>
            {
                observer.OnNext(d.value);
                return d.onValueChanged.AsObservable(d.GetDestroyCancellationToken()).Subscribe(observer);
            });
        }
    }
}
