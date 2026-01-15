using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
#if COZY_URP
using UnityEngine.Rendering.Universal;
#endif
using UnityEngine.UIElements;

namespace DistantLands.Cozy.EditorScripts
{
    public class CozySetupWizard : EditorWindow
    {
        private VisualElement RenderPipelinePackagesElement => rootVisualElement.Q<VisualElement>("render-pipeline-packages");
        private VisualElement ProjectSetupElement => rootVisualElement.Q<VisualElement>("project-setup");
        private Button DocumentationElement => rootVisualElement.Q<Button>("documentation");
        private Button FaqsElement => rootVisualElement.Q<Button>("faqs");
        private Button SupportElement => rootVisualElement.Q<Button>("support");
        public CozyWeatherEditor editorWindow;

        public static void ShowSetupWizard(CozyWeatherEditor _editorWindow)
        {
            CozySetupWizard wnd = GetWindow<CozySetupWizard>();
            wnd.editorWindow = _editorWindow;
            wnd.titleContent = new GUIContent("Project Setup Wizard");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Window/UXML/cozy-setup-wizard.uxml"
            );
            asset.CloneTree(root);

            RefreshSetupUI();
            RefreshPipelineUI();

            DocumentationElement.RegisterCallback((ClickEvent evt) =>
            {
                CozySceneTools.OpenDocs();
            });
            SupportElement.RegisterCallback((ClickEvent evt) =>
            {
                CozySceneTools.OpenDiscord();
            });
            FaqsElement.RegisterCallback((ClickEvent evt) =>
            {
                CozySceneTools.OpenFAQs();
            });

        }

        void RefreshPipelineUI()
        {
            RenderPipelinePackagesElement.Clear();

            if (EditorPrefs.GetString("CZY_RP", "") == "")
            {
                HelpBox help = new HelpBox("Please import a render pipeline support package", HelpBoxMessageType.Warning);
                RenderPipelinePackagesElement.Add(help);
            }

            SetupCheck urpPackage = new SetupCheck("Universal Render Pipeline", "Import", true,
            () =>
            {
                return EditorPrefs.GetString("CZY_RP", "") == "URP";
            },
            () =>
            {
                string relativePath = AssetInformation.INTEGRATION_PATH + "Import for URP.unitypackage";
                string absolutePath = Path.GetFullPath(relativePath);
                Application.OpenURL(absolutePath);
                Debug.Log($"Successfully installed COZY v{AssetInformation.INSTALLED_VERSION} for the Universal Render Pipeline.");
                EditorPrefs.SetString("CZY_SemVersion", AssetInformation.INSTALLED_VERSION.ToString());
                EditorPrefs.SetString("CZY_RP", "URP");
                RefreshPipelineUI();
            }
            );
            RenderPipelinePackagesElement.Add(urpPackage);

            SetupCheck birpPackage = new SetupCheck("Built-in Render Pipeline", "Import", true,
            () =>
            {
                return EditorPrefs.GetString("CZY_RP", "") == "BiRP";
            },
            () =>
            {
                string relativePath = AssetInformation.INTEGRATION_PATH + "Import for BiRP.unitypackage";
                string absolutePath = Path.GetFullPath(relativePath);
                Application.OpenURL(absolutePath);
                Debug.Log($"Successfully installed COZY v{AssetInformation.INSTALLED_VERSION} for the Built-in Render Pipeline.");
                EditorPrefs.SetString("CZY_SemVersion", AssetInformation.INSTALLED_VERSION.ToString());
                EditorPrefs.SetString("CZY_RP", "BiRP");
                RefreshPipelineUI();
            }
            );
            RenderPipelinePackagesElement.Add(birpPackage);

            SetupCheck hdrpPackage = new SetupCheck("High Definition Render Pipeline", "Import", true,
            () =>
            {
                return EditorPrefs.GetString("CZY_RP", "") == "HDRP";
            },
            () =>
            {
                string relativePath = AssetInformation.INTEGRATION_PATH + "Import for HDRP.unitypackage";
                string absolutePath = Path.GetFullPath(relativePath);
                Application.OpenURL(absolutePath);
                Debug.Log($"Successfully installed COZY v{AssetInformation.INSTALLED_VERSION} for the High Definition Render Pipeline.");
                EditorPrefs.SetString("CZY_SemVersion", AssetInformation.INSTALLED_VERSION.ToString());
                EditorPrefs.SetString("CZY_RP", "HDRP");
                RefreshPipelineUI();
            }
            );
            RenderPipelinePackagesElement.Add(hdrpPackage);

            CozyWeatherEditor.instance.UpdateStatusIcon();

        }

        void RefreshSetupUI()
        {

            ProjectSetupElement.Clear();

            SetupCheck linearColorspace = new SetupCheck("Linear Colorspace", "Resolve",
            () =>
            {
                return PlayerSettings.colorSpace == ColorSpace.Linear;
            },
            () =>
            {
                PlayerSettings.colorSpace = ColorSpace.Linear;
                RefreshSetupUI();
            }
            );
            ProjectSetupElement.Add(linearColorspace);

#if COZY_URP
            SetupCheck depthTexture = new SetupCheck("Depth Texture", "Resolve",
            () =>
            {
                for (int i = 0; i < GraphicsSettings.allConfiguredRenderPipelines.Length; i++)
                {
                    if (GraphicsSettings.allConfiguredRenderPipelines[i].GetType() != typeof(UniversalRenderPipelineAsset)) continue;

                    UniversalRenderPipelineAsset pipeline = (UniversalRenderPipelineAsset)GraphicsSettings.allConfiguredRenderPipelines[i];

                    if (pipeline.supportsCameraDepthTexture == false)
                    {
                        return false;
                    }
                }
                return true;
            },
            () =>
            {
                for (int i = 0; i < GraphicsSettings.allConfiguredRenderPipelines.Length; i++)
                {
                    if (GraphicsSettings.allConfiguredRenderPipelines[i].GetType() != typeof(UniversalRenderPipelineAsset)) continue;
                    UniversalRenderPipelineAsset pipeline = (UniversalRenderPipelineAsset)GraphicsSettings.allConfiguredRenderPipelines[i];
                    pipeline.supportsCameraDepthTexture = true;
                    EditorUtility.SetDirty(pipeline);
                }
                RefreshSetupUI();
            }
            );
            ProjectSetupElement.Add(depthTexture);

            SetupCheck opaqueTexture = new SetupCheck("Opaque Texture", "Resolve",
            () =>
            {
                for (int i = 0; i < GraphicsSettings.allConfiguredRenderPipelines.Length; i++)
                {
                    if (GraphicsSettings.allConfiguredRenderPipelines[i].GetType() != typeof(UniversalRenderPipelineAsset)) continue;

                    UniversalRenderPipelineAsset pipeline = (UniversalRenderPipelineAsset)GraphicsSettings.allConfiguredRenderPipelines[i];

                    if (pipeline.supportsCameraOpaqueTexture == false)
                    {
                        return false;
                    }
                }

                return true;
            },
            () =>
            {
                for (int i = 0; i < GraphicsSettings.allConfiguredRenderPipelines.Length; i++)
                {
                    if (GraphicsSettings.allConfiguredRenderPipelines[i].GetType() != typeof(UniversalRenderPipelineAsset)) continue;
                    UniversalRenderPipelineAsset pipeline = (UniversalRenderPipelineAsset)GraphicsSettings.allConfiguredRenderPipelines[i];
                    pipeline.supportsCameraOpaqueTexture = true;
                    EditorUtility.SetDirty(pipeline);
                }
                RefreshSetupUI();
            }
            );
            ProjectSetupElement.Add(opaqueTexture);

            SetupCheck opaqueDownsampling = new SetupCheck("Opaque Downsampling", "Resolve",
            () =>
            {
                for (int i = 0; i < GraphicsSettings.allConfiguredRenderPipelines.Length; i++)
                {
                    if (GraphicsSettings.allConfiguredRenderPipelines[i].GetType() != typeof(UniversalRenderPipelineAsset)) continue;

                    UniversalRenderPipelineAsset pipeline = (UniversalRenderPipelineAsset)GraphicsSettings.allConfiguredRenderPipelines[i];

                    if (!(pipeline.opaqueDownsampling == Downsampling.None))
                    {
                        return false;
                    }
                }

                return true;
            },
            () =>
            {
                for (int i = 0; i < GraphicsSettings.allConfiguredRenderPipelines.Length; i++)
                {
                    // Loop through all configured render pipelines
                    if (GraphicsSettings.allConfiguredRenderPipelines[i].GetType() != typeof(UniversalRenderPipelineAsset))
                        continue;

                    // Cast the pipeline to the correct type
                    UniversalRenderPipelineAsset pipeline = (UniversalRenderPipelineAsset)GraphicsSettings.allConfiguredRenderPipelines[i];

                    // Get the type of the UniversalRenderPipelineAsset class
                    Type t = typeof(UniversalRenderPipelineAsset);

                    // Get the private field using reflection and BindingFlags
                    FieldInfo fieldInfo = t.GetField("m_OpaqueDownsampling", BindingFlags.NonPublic | BindingFlags.Instance);

                    // Check if the field was found
                    if (fieldInfo != null)
                    {
                        // Set the value of the private field
                        fieldInfo.SetValue(pipeline, Downsampling.None);
                    }
                    else
                    {
                        Debug.LogError("Field 'm_OpaqueDownsampling' not found!");
                    }
                    EditorUtility.SetDirty(pipeline);
                }
                RefreshSetupUI();
            }
            );
            ProjectSetupElement.Add(opaqueDownsampling);

            SetupCheck hdrDisabled = new SetupCheck("HDR Enabled", "Resolve",
            () =>
            {
                for (int i = 0; i < GraphicsSettings.allConfiguredRenderPipelines.Length; i++)
                {
                    if (GraphicsSettings.allConfiguredRenderPipelines[i].GetType() != typeof(UniversalRenderPipelineAsset)) continue;

                    UniversalRenderPipelineAsset pipeline = (UniversalRenderPipelineAsset)GraphicsSettings.allConfiguredRenderPipelines[i];

                    if (!(pipeline.supportsHDR == true))
                    {
                        return false;
                    }
                }

                return true;
            },
            () =>
            {
                for (int i = 0; i < GraphicsSettings.allConfiguredRenderPipelines.Length; i++)
                {
                    if (GraphicsSettings.allConfiguredRenderPipelines[i].GetType() != typeof(UniversalRenderPipelineAsset)) continue;
                    UniversalRenderPipelineAsset pipeline = (UniversalRenderPipelineAsset)GraphicsSettings.allConfiguredRenderPipelines[i];
                    pipeline.supportsHDR = true;
                    EditorUtility.SetDirty(pipeline);
                }

                RefreshSetupUI();
            }
            );
            ProjectSetupElement.Add(hdrDisabled);
#endif

            CozyWeatherEditor.instance.UpdateStatusIcon();

        }


        public static bool CheckStatus
        {
            get
            {
                if (EditorPrefs.GetString("CZY_RP", "") == "")
                    return false;
                if (PlayerSettings.colorSpace != ColorSpace.Linear)
                    return false;
#if COZY_URP
                for (int i = 0; i < GraphicsSettings.allConfiguredRenderPipelines.Length; i++)
                {
                    if (GraphicsSettings.allConfiguredRenderPipelines[i].GetType() != typeof(UniversalRenderPipelineAsset)) continue;

                    UniversalRenderPipelineAsset pipeline = (UniversalRenderPipelineAsset)GraphicsSettings.allConfiguredRenderPipelines[i];

                    if (pipeline.supportsCameraDepthTexture == false)
                    {
                        return false;
                    }
                    if (pipeline.supportsCameraOpaqueTexture == false)
                    {
                        return false;
                    }
                    if (!(pipeline.opaqueDownsampling == Downsampling.None))
                    {
                        return false;
                    }
                    if (!(pipeline.supportsHDR == true))
                    {
                        return false;
                    }
                }
#endif

                return true;
            }
        }

    }
}