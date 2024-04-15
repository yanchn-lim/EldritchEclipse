using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

namespace Enemy
{
    public class EnemySheet : ScriptableObject
    {
        public int GrowthEnergy;
        public int Health;
        public int Speed;
        public int AttackSpeed;
        public int Damage;

        public float PerformanceIndicator;
        public float TotalDamageTaken;
        public float TotalPlayerDamage;
    }

    [CustomEditor(typeof(EnemySheet))]
    public class EnemySheetEditor : Editor
    {
        private SerializeField GrowthEnergy;

        public override void OnInspectorGUI()
        {
            
        }
    }


}