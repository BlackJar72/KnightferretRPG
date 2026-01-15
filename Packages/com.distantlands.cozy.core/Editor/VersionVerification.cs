using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System;
using UnityEditor.PackageManager.UI;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;


namespace DistantLands.Cozy.EditorScripts
{
    [InitializeOnLoad]
    public class VersionVerification
    {


        private static HttpClient httpClient = new HttpClient();

        private static bool _updateCheckPerformed
        {
            get
            {
                return SessionState.GetBool("CZY_UpdateChecked", false);
            }
            set
            {
                SessionState.SetBool("CZY_UpdateChecked", value);
            }
        }

        static VersionVerification()
        {
            // Retrieving cached SessionState/PlayerPrefs values is not allowed from an instance field initializer
            EditorApplication.update += CheckForUpdatesAfterEditorUpdate;
        }

        private static async void CheckForUpdatesAfterEditorUpdate()
        {
            EditorApplication.update -= CheckForUpdatesAfterEditorUpdate;

            await CheckForUpdates((success, currentVersion, latestVersion) =>
            {
                if (success)
                {
                    AssetInformation.INSTALLED_VERSION = currentVersion;
                    AssetInformation.UAS_VERSION = latestVersion;
                }
            });
        }

        private static async Task CheckForUpdates(Action<bool, Version, Version> OnUpdatesChecked)
        {
            var latestVersionResult = await GetLatestCozyVersion();
            if (latestVersionResult == null)
            {
                OnUpdatesChecked?.Invoke(false, null, null);
                return;
            }


            Version currentVersion = null;
            Version latestVersion = null;

            try
            {
                currentVersion = new Version(UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages().FirstOrDefault(x => x.name == "com.distantlands.cozy.core").version);
                latestVersion = latestVersionResult;
            }
            catch
            {
                OnUpdatesChecked?.Invoke(false, null, null);
            }

            OnUpdatesChecked?.Invoke(true, currentVersion, latestVersion);
        }

        public class ASV
        {
            public string version;
            public string name;
            public string category;
            public int id;
            public string publisher;
        }

        public static async Task<Version> GetLatestCozyVersion()
        {
            try
            {
                var url = AssetInformation.VERSION_URL;
                var result = await httpClient.GetAsync(url);

                result.EnsureSuccessStatusCode();

                var resultStr = await result.Content.ReadAsStringAsync();
                ASV version = JsonUtility.FromJson<ASV>(resultStr);

                string[] numbers = version.version.Split(".");
                int major = int.TryParse(numbers[0], out major) ? major : 0;
                int minor = int.TryParse(numbers[1], out minor) ? minor : 0;
                int patch = int.TryParse(numbers[2], out patch) ? patch : 0;

                return new Version(major, minor, patch);
            }
            catch
            {
                return null;
            }
        }


    }

}