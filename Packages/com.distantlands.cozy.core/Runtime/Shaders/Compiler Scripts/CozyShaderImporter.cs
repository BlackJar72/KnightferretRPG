//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;
using UnityEditor;

namespace DistantLands.Cozy.ShaderUtility
{
    [ScriptedImporter(1, "cozyshader")]
    public class CozyShaderImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {

            string fileContent = File.ReadAllText(ctx.assetPath);
            var package = ObjectFactory.CreateInstance<CozyShaderPackage>();
            bool overwriteFileWithShaderText = false;
            bool isJSON = IsJson(fileContent);

            if (!isJSON)
            {
                Shader shader = ShaderUtil.CreateShaderAsset(ctx, fileContent, false);

                ctx.AddObjectToAsset("MainAsset", shader);
                ctx.SetMainObject(shader);
                return;
            }

            if (!string.IsNullOrEmpty(fileContent))
            {
                EditorJsonUtility.FromJsonOverwrite(fileContent, package);
            }

            if (package.entries == null)
            {
                package.entries = new List<CozyShaderPackage.Entry>();
            }

            package.PackageShaderVariants();


            foreach (var e in package.entries)
            {
                if (e.shader != null)
                {
                    ctx.DependsOnSourceAsset(AssetDatabase.GetAssetPath(e.shader));
                }
                else
                    overwriteFileWithShaderText = true;
            }

            string shaderSrc = package.GetShaderSource();
            if (shaderSrc == null)
            {
                Debug.LogWarning($"{name} has no shader for this SRP provided");
                return;
            }

            if (overwriteFileWithShaderText)
            {
                File.WriteAllText(ctx.assetPath, shaderSrc);
                Shader shader = ShaderUtil.CreateShaderAsset(ctx, shaderSrc, false);

                ctx.AddObjectToAsset("MainAsset", shader);
                ctx.SetMainObject(shader);
                return;
            }
            else
            {
                Shader shader = ShaderUtil.CreateShaderAsset(ctx, shaderSrc, false);

                ctx.AddObjectToAsset("MainAsset", shader);
                ctx.SetMainObject(shader);
            }

        }

        bool IsJson(string content)
        {
            // Try parsing the content as JSON
            try
            {
                // JsonUtility.FromJson will throw an exception if the content is not valid JSON
                JsonUtility.FromJson<object>(content);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

    }
}
#endif