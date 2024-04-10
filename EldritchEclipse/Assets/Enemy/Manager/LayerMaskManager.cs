using System.Collections;
using UnityEngine;

namespace Assets.Enemy.Manager
{
    public class LayerMaskManager : MonoBehaviour
    {
        public static int EnemyLayerMask { get { return 1 >> 6; } }
    }
}