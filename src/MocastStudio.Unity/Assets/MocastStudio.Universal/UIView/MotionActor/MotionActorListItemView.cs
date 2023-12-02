using System;
using UnityEngine;
using Text = TMPro.TMP_Text;

namespace MocastStudio.Universal.UIView.MotionActor
{
    public sealed class MotionActorListItemView : MonoBehaviour
    {
        [SerializeField] Text _idText;
        [SerializeField] Text _nameText;

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
