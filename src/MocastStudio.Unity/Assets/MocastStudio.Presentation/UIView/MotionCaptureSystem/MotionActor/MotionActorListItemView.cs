using System;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TMP_Text;

namespace MocastStudio.Presentation.UIView.MotionActor
{
    public sealed class MotionActorListItemView : MonoBehaviour
    {
        [SerializeField] Text _idText;
        [SerializeField] Text _nameText;
        [SerializeField] Toggle _rootBoneOffsetToggle;

        private int _id;

        public Observable<(int ActorId, bool RootBoneOffsetEnabled)> OnRootBoneOffsetToggleValueChanged =>
            _rootBoneOffsetToggle.OnValueChangedAsObservable()
                .Select(isOn => (ActorId: _id, RootBoneOffsetEnabled: isOn));

        public void SetId(int id)
        {
            _id = id;
            _idText.text = $"ID: {id}";
        }

        public void SetValues(string name)
        {
            _nameText.text = $"Name: {name}";
        }
    }
}
