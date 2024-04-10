using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.pattern
{
    public class PoolingPattern<T> where T : MonoBehaviour //where it is a game object
    {
        private GameObject prefab;
        private Queue<T> queue;

        public PoolingPattern(GameObject prefab)
        {
            queue = new Queue<T>();
            this.prefab = prefab;
        }

        public void Init(int numberOfItems)
        {
            for (int i = 0; i < numberOfItems; i++)
            {
                Add();
            }
        }

        public void InitWithParent(int numberOfItems, Transform parent)
        {
            for (int i = 0; i < numberOfItems; i++)
            {
                Add(parent);
            }
        }

        public void Add()
        {
            GameObject initObject = GameObject.Instantiate(prefab);
            initObject.SetActive(false);
            queue.Enqueue(initObject.GetComponent<T>());
        }

        public void Add(Transform parent)
        {
            GameObject initObject = GameObject.Instantiate(prefab, parent);
            initObject.SetActive(false);
            queue.Enqueue(initObject.GetComponent<T>());

        }

        public T Get()
        {
            if (queue.Count == 0)
            {
                Add();
            }
            var initObject = queue.Dequeue();
            initObject.gameObject.SetActive(true);
            return initObject;
        }

        public void Retrieve(T initObject)
        {
            initObject.gameObject.SetActive(false);
            queue.Enqueue(initObject);
        }

    }
}