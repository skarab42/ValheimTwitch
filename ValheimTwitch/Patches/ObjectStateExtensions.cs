using System;
using UnityEngine;

namespace ValheimTwitch.Patches
{
    public static class ObjectStateExtensions
    {
        public static IStateListener GetStateListener(this GameObject obj)
        {
            return obj.GetComponent<ObjectStateListener>() ?? obj.AddComponent<ObjectStateListener>();
        }

        public interface IStateListener
        {
            event Action Enabled;
            event Action Disabled;
        }

        class ObjectStateListener : MonoBehaviour, IStateListener
        {
            public event Action Enabled;
            public event Action Disabled;

            void Awake()
            {
                hideFlags = HideFlags.DontSaveInBuild | HideFlags.HideInInspector;
            }

            void OnEnable()
            {
                TryInvoke(Enabled);
            }

            void OnDisable()
            {
                TryInvoke(Disabled);
            }

            void TryInvoke(Action action)
            {
                action?.Invoke();
            }
        }
    }
}
