using System;
using TMPro;
using UnityEngine;

namespace MocastStudio.Universal.UIView.MotionActor
{
    public sealed class MotionActorListItemView : MonoBehaviour
    {
        [SerializeField] TMP_Text _idText;
        [SerializeField] TMP_Text _nameText;

        public void SetId(int id)
        {
            _idText.text = $"ID: {id}";
        }

        public void SetValues(string name)
        {
            _nameText.text = $"Name: {name}";
        }
    }
}
