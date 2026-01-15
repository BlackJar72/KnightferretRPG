//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula
//  Code derived from https://github.com/slipster216/ShaderPackager with permission from Jason Booth

using System.Collections.Generic;
using UnityEngine;
#if UnityEditor
using UnityEditor;
#endif

namespace DistantLands.Cozy.ShaderUtility
{
    public class CozyShaderPackage : ScriptableObject
    {
        public enum SRPTarget
        {
            BIRP,
            URP,
            HDRP
        }
        public enum UnityVersion
        {
            Min = 0,
            Unity2021_2 = 20212,
            Unity2021_3 = 20213,
            Unity2022_1 = 20221,
            Unity2022_2 = 20222,
            Unity2022_3 = 20223,
            Unity2023_2 = 20232,
            Unity2023_3 = 20233,
            Max = 30000
        }
        [System.Serializable]
        public class Entry
        {
            public SRPTarget srpTarget = SRPTarget.BIRP;
            public UnityVersion min = UnityVersion.Min;
            public UnityVersion max = UnityVersion.Max;
            public Shader shader;
            [HideInInspector] public string shaderSource;
        }

        public List<Entry> entries = new List<Entry>();

        public void PackageShaderVariants()
        {
            foreach (var e in entries)
            {
                if (!e.shader)
                {
                    break;
                }
                if (e.shader != null)
                {
#if UnityEditor
                    var path = AssetDatabase.GetAssetPath(e.shader);
                    e.shaderSource = System.IO.File.ReadAllText(path);
#endif
                }
            }
        }
        public static SRPTarget GetCurrentSRP()
        {
#if COZY_URP
            return SRPTarget.URP;
#elif COZY_HDRP
            return SRPTarget.HDRP;
#else
            return SRPTarget.BIRP;
#endif
        }
        public string GetShaderSource()
        {
            UnityVersion curVersion = UnityVersion.Min;

#if UNITY_2021_2_OR_NEWER
            curVersion = UnityVersion.Unity2021_2;
#endif
#if UNITY_2021_3_OR_NEWER
            curVersion = UnityVersion.Unity2021_3;
#endif
#if UNITY_2022_1_OR_NEWER
            curVersion = UnityVersion.Unity2022_1;
#endif
#if UNITY_2022_2_OR_NEWER
            curVersion = UnityVersion.Unity2022_2;
#endif
#if UNITY_2022_3_OR_NEWER
      curVersion = UnityVersion.Unity2022_3;
#endif

            SRPTarget target = GetCurrentSRP();

            string source = null;
            foreach (var entry in entries)
            {
                if (target != entry.srpTarget)
                    continue;
                if (curVersion >= entry.min && curVersion <= entry.max)
                {
                    if (source != null)
                    {
                        Debug.LogWarning("Found multiple possible entries for unity version of shader");
                    }
                    source = entry.shaderSource;
                }
            }
            return source;
        }

    }
}