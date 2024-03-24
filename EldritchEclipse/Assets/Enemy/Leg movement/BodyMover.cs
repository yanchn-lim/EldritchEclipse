using System.Collections;
using UnityEngine;

namespace Assets.Enemy
{
    public class BodyMover : MonoBehaviour
    {
        [SerializeField]private bool LeftLegStart;
        public bool leftLegStart { get; private set; }

        private void Start()
        {
            leftLegStart = LeftLegStart;
        }
        public void ToggleLeg()
        {
            leftLegStart = !leftLegStart;
        }
    }
}