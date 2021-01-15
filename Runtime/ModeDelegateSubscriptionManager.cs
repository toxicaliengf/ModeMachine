using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModeMachine
{
    internal class ModeDelegateSubscriptionManager : MonoBehaviour
    {
        const string SINGLETON_PATH = "";
        private static ModeDelegateSubscriptionManager _instance;
        private static object _lock = new object();

        public static ModeDelegateSubscriptionManager Instance
        {
            get
            {
                if (wasDestroyed)
                {
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        if (_instance == null)
                        {
                            {
                                GameObject singleton = new GameObject("ModeDelegateSubscriptionManager");
                                singleton.hideFlags = HideFlags.HideInHierarchy;
                                _instance = singleton.AddComponent<ModeDelegateSubscriptionManager>();
                            }
                        }
                        DontDestroyOnLoad(_instance);
                    }

                    return _instance;
                }
            }
        }

        private static bool wasDestroyed = false;
        public void OnDestroy()
        {
            wasDestroyed = true;
            for(int i = 0; i < modes.Count; i++)
            {
                if (modes[i] != null)
                    modes[i].CleanupDelegates();
            }
        }

        List<Mode> modes = new List<Mode>();

        public void RegisterMode(Mode mode)
        {
            if (!modes.Contains(mode))
            {
                modes.Add(mode);
            }
        }
    }
}
