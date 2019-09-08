#if UNITY_EDITOR && !UNITY_WEBPLAYER
using System;
using System.IO;
using UnityEditor;

namespace UnityEngine.Purchasing {
    public class UnityPurchasingEditor
    {
        private const string ModePath = "Assets/Plugins/UnityPurchasing/resources/BillingMode.txt";

        [MenuItem("Window/UnityPurchasing/Android/Target Amazon")]
        private static void TargetAmazon()
        {
            Enable(AndroidStore.AmazonAppStore);
        }

        [MenuItem("Window/UnityPurchasing/Android/Target Google Play")]
        private static void TargetGooglePlay()
        {
            Enable(AndroidStore.GooglePlay);
        }

        private static void Enable(params AndroidStore[] enabled)
        {
            foreach (AndroidStore p in Enum.GetValues(typeof (AndroidStore)))
            {
                string path = string.Format("Assets/Plugins/UnityPurchasing/Bin/Android/{0}.aar", p);
                var importer = (PluginImporter) PluginImporter.GetAtPath(path);
                if (null != importer)
                {
                    var enable = Array.Exists(enabled, x => x == p);
                    importer.SetCompatibleWithPlatform(BuildTarget.Android, enable);
                }
            }

            SetAndroidMode(enabled);
        }

        private static void SetAndroidMode(params AndroidStore[] enabled)
        {
            var platform = AndroidStore.NotSpecified;
            if (enabled.Length == 1)
                platform = enabled[0];

            Directory.CreateDirectory(Path.GetDirectoryName(ModePath));
            File.WriteAllText(ModePath, platform.ToString());
            AssetDatabase.ImportAsset(ModePath);
        }
    }
}
#endif
