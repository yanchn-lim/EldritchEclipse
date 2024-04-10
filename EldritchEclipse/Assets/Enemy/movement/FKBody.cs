using Assets.Enemy.Manager;
using System.Collections;
using UnityEngine;

namespace Movement
{
    public class FKBody : MonoBehaviour
    {
        private MovementManager manager;
        private Transform parent;
        
        public void Init(MovementManager manager)
        {
            
            this.manager = manager;
        }

        private void Update()
        {
            Ray ray = new Ray(transform.position , Vector3.down);
            if (Physics.Raycast(ray, out var hit, 4f, ~LayerMaskManager.EnemyLayerMask))
            {
                //check distance 
                Debug.DrawLine(transform.position , hit.point , Color.red);
                var distance = Vector3.Distance(hit.point, transform.position);
                if (distance > manager.PreferredHeight || 
                    distance < manager.PreferredHeight )
                {
                    distance = manager.PreferredHeight - distance;
                    transform.position += new Vector3(0, distance * Time.deltaTime * manager.FkDamping, 0);
                }
                
            }

        }

    }
}