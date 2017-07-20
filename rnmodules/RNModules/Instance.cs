using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RNModules
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class Instance : MonoBehaviour
    {
        private static bool loadedInScene;
        private string version = "";

        internal void Awake()
        {
            // Allow loading the background in the laoding screen
            Application.runInBackground = true;

            // Ensure that only one copy of the service is run per scene change.
            if (loadedInScene)
            {
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                Debug.Log("Multiple copies of current version. Using the first copy. Version: " +
                    currentAssembly.GetName().Version);
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);

            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            version = v.Major + "." + v.Minor + "." + v.Build;

            loadedInScene = true;
        }
    }
}