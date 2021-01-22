using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        private void Awake()
        {
            // Ensure single instance
            if (Instance == null)
            {
                Instance = this as T;
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual void Initialize() { }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}