//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using UnityEngine;

namespace DistantLands.Cozy.EditorScripts
{

    [InitializeOnLoad]
    public class E_AddCozyDefines : Editor
    {

        /// <summary>
        /// Symbols that will be added to the editor
        /// </summary>
        public static readonly string[] Symbols = new string[] {
        "COZY_WEATHER",
        "COZY_3_AND_UP"
    };

        /// <summary>
        /// Add define symbols as soon as Unity gets done compiling.
        /// </summary>
        static E_AddCozyDefines()
        {

            if (PlayerPrefs.GetInt("CZY_AddDefines", 1) == 1)
            {

                string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                List<string> allDefines = definesString.Split(';').ToList();
                allDefines.AddRange(Symbols.Except(allDefines));
        
                if (IsPackageInstalled("com.unity.render-pipelines.universal"))
                {
                    if (!allDefines.Contains("COZY_URP"))
                        allDefines.Add("COZY_URP");
                }
                else if (IsPackageInstalled("com.unity.render-pipelines.high-definition"))
                {
                    if (!allDefines.Contains("COZY_HDRP"))
                        allDefines.Add("COZY_HDRP");
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    EditorUserBuildSettings.selectedBuildTargetGroup,
                    string.Join(";", allDefines.ToArray()));
            }
        }

        public static PackageInfo GetPackage(string packageID, bool throwError)
        {
            SearchRequest request = Client.Search(packageID);
            while (request.Status == StatusCode.InProgress) { }

            if (request.Status == StatusCode.Failure && throwError)
            {
                Debug.LogError("Failed to retrieve package from Package Manager...");
                return null;
            }

            return request.Result[0];
        }

        public static bool IsPackageInstalled(string packageID)
        {
            string manifestPath = Application.dataPath + "/../Packages/manifest.json";

            if (File.Exists(manifestPath))
            {
                string manifestContents = File.ReadAllText(manifestPath);

                return manifestContents.Contains(packageID);
            }
            else
            {
                Debug.LogError("Unable to find the manifest file.");
                return false;
            }
        }
    }
}
