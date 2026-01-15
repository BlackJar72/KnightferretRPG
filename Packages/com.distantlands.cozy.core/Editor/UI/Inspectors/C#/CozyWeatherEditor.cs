using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditor.UIElements;
using System;
using UnityEditor.Experimental.GraphView;
using DistantLands.Cozy.Data;
using UnityEditor.Search;
using System.Linq;


namespace DistantLands.Cozy.EditorScripts
{

    [CustomEditor(typeof(CozyWeather))]
    public class CozyWeatherEditor : Editor
    {

        CozyWeather weatherSphere;

        public List<CozyModuleEditor> moduleEditors = new List<CozyModuleEditor>();
        public VisualElement root;
        VisualElement HomePage => root.Query<VisualElement>("home-page");
        VisualElement ModulePage => root.Query<VisualElement>("modular-page");
        VisualElement WidgetContainer => root.Query<VisualElement>("widget-container");
        VisualElement AddModuleButton => root.Query<VisualElement>("add-module");
        VisualElement SettingsButton => root.Query<VisualElement>("settings");
        VisualElement SceneButton => root.Query<VisualElement>("scene-tools");
        VisualElement StylesButton => root.Query<VisualElement>("styles");


        VisualElement Banner => root.Query<VisualElement>("banner-container");
        VisualElement BannerImage => Banner.Query<VisualElement>("background-image");
        VisualElement BannerIcon => Banner.Query<VisualElement>("logo");
        Label BannerTitle => Banner.Query<Label>("title");
        Label BannerSubtitle => Banner.Query<Label>("desc");
        VisualElement StatusReport => Banner.Query<VisualElement>("status-report");


        VisualElement FXBlockZone => root.Query<VisualElement>("fx-block-zone");
        VisualElement FogCullingZone => root.Query<VisualElement>("fog-culling-zone");
        VisualElement GlobalBiome => root.Query<VisualElement>("global-biome");
        VisualElement LocalBiome => root.Query<VisualElement>("local-biome");

        ToolbarSearchField searchbar => root.Q<ToolbarSearchField>();
        VisualElement SearchResults => root.Query<VisualElement>("search-results");

        Toggle UpdateInEditMode => root.Query<Toggle>("update-in-edit-mode");
        Toggle FogInScene => root.Query<Toggle>("fog");
        Toggle CenterWeather => root.Query<Toggle>("follow-camera");
        Toggle Gizmos => root.Query<Toggle>("gizmos");
        Toggle Tooltips => root.Query<Toggle>("tooltips");
        Toggle Graphs => root.Query<Toggle>("graphs");

        VisualElement SkyStyleRoot => root.Query<VisualElement>("sky-style-holder");
        VisualElement CloudStyleRoot => root.Query<VisualElement>("cloud-style-holder");
        VisualElement FogStyleRoot => root.Query<VisualElement>("fog-style-holder");
        List<Type> mods;

        public static CozyWeatherEditor instance;



        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            if (!target)
                return;

            weatherSphere = (CozyWeather)target;
            ResetModuleEditors();
            instance = this;
            CozyWeather.refreshModules += UpdateWidgets;
            Tools.hidden = true;

        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            CozyWeather.refreshModules -= UpdateWidgets;
            Tools.hidden = false;
        }

        public void ResetModuleEditors()
        {
            moduleEditors.Clear();

            for (int i = 0; i < weatherSphere.modules.Count; i++)
            {
                CozyModuleEditor moduleEditor = (CozyModuleEditor)CreateEditor(weatherSphere.modules[i]);
                if (moduleEditor == null)
                    continue;
                moduleEditor.weatherEditor = this;
                moduleEditors.Add(moduleEditor);
            }

            moduleEditors.RemoveAll(x => x == null);
        }

        public override VisualElement CreateInspectorGUI()
        {

            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Inspectors/UXML/cozy-inspector.uxml"
            );

            asset.CloneTree(root);
            UpdateBanner("COZY", "Stylized Weather 3", Resources.Load<Texture2D>("Promo/Distant Lands Watermark"), Resources.Load<Texture2D>("Banners/Cozy Weather"));


            searchbar.RegisterCallback((ChangeEvent<string> evt) =>
            {
                if (evt.newValue == "")
                {
                    ClearSearch();
                }
                else
                {
                    Search(evt.newValue);
                }
            });
            AddModuleButton.Add(new Image() { image = EditorGUIUtility.IconContent("Toolbar Plus More").image, tooltip = "Add Module" });
            AddModuleButton.RegisterCallback<ClickEvent>(AddModule);
            SettingsButton.Add(new Image() { image = EditorGUIUtility.IconContent("_Popup").image, tooltip = "Settings" });
            SettingsButton.RegisterCallback((ClickEvent evt) => { OpenSettings(); });
            SceneButton.Add(new Image() { image = EditorGUIUtility.IconContent("d_SceneViewTools@2x").image, tooltip = "Scene Tools" });
            SceneButton.RegisterCallback((ClickEvent evt) => { OpenSceneTools(); });
            StylesButton.Add(new Image() { image = EditorGUIUtility.IconContent("d_Grid.PaintTool@2x").image, tooltip = "Styles" });
            StylesButton.RegisterCallback((ClickEvent evt) => { OpenStyles(); });



            UpdateWidgets();

            return root;
        }

        void UpdateBanner(string title, string subtitle, Texture2D icon, Texture2D background)
        {
            BannerTitle.text = title;
            BannerSubtitle.text = subtitle;
            BannerImage.style.backgroundImage = background;
            BannerIcon.style.backgroundImage = icon;
            UpdateStatusIcon();
            StatusReport.RegisterCallback((ClickEvent evt) =>
            {
                CozySetupWizard.ShowSetupWizard(this);
            });
        }

        public void UpdateStatusIcon()
        {
            StatusMessage status = StatusReport.Query<StatusMessage>();
            status.UpdateStatus(CozySetupWizard.CheckStatus);
        }

        void UpdateWidgets()
        {

            if (root == null || WidgetContainer == null)
                return;

            ResetModuleEditors();
            WidgetContainer.Clear();

            ModuleWidgetGroup atmosphereModuleContainer = null;
            ModuleWidgetGroup timeModuleContainer = null;
            ModuleWidgetGroup weatherModuleContainer = null;
            ModuleWidgetGroup manipulationModuleContainer = null;
            ModuleWidgetGroup integrationModuleContainer = null;
            ModuleWidgetGroup utilityModuleContainer = null;
            ModuleWidgetGroup otherModuleContainer = null;

            Button widget;
            int moduleNumber = 0;

            if (moduleEditors.Count == 0)
            {
                Label title = new Label()
                {
                    text = "Welcome!"
                };
                title.AddToClassList("h1");
                Label p = new Label()
                {
                    text = "Thank you for choosing COZY: Stylized Weather. Get started by adding a module using the [ + ] button on the toolbar."
                };
                p.AddToClassList("p");


                WidgetContainer.Add(title);
                WidgetContainer.Add(p);

            }

            foreach (CozyModuleEditor moduleEditor in moduleEditors)
            {
                widget = moduleEditor.DisplayWidget();
                widget.RegisterCallback<ClickEvent, int>(OpenModuleUI, moduleNumber);
                widget.RegisterCallback<ContextClickEvent, CozyModuleEditor>(OpenModuleContextMenu, moduleEditor);

                switch (moduleEditor.Category)
                {
                    case CozyModuleEditor.ModuleCategory.atmosphere:
                        if (atmosphereModuleContainer == null)
                            atmosphereModuleContainer = new ModuleWidgetGroup("Atmosphere");

                        atmosphereModuleContainer.AddWidget(widget);
                        break;
                    case CozyModuleEditor.ModuleCategory.time:
                        if (timeModuleContainer == null)
                            timeModuleContainer = new ModuleWidgetGroup("Time");

                        timeModuleContainer.AddWidget(widget);
                        break;
                    case CozyModuleEditor.ModuleCategory.ecosystem:
                        if (weatherModuleContainer == null)
                            weatherModuleContainer = new ModuleWidgetGroup("Ecosystem");

                        weatherModuleContainer.AddWidget(widget);
                        break;
                    case CozyModuleEditor.ModuleCategory.integration:
                        if (integrationModuleContainer == null)
                            integrationModuleContainer = new ModuleWidgetGroup("Integration");

                        integrationModuleContainer.AddWidget(widget);
                        break;
                    case CozyModuleEditor.ModuleCategory.survival:
                        if (manipulationModuleContainer == null)
                            manipulationModuleContainer = new ModuleWidgetGroup("Survival");

                        manipulationModuleContainer.AddWidget(widget);
                        break;
                    case CozyModuleEditor.ModuleCategory.utility:
                        if (utilityModuleContainer == null)
                            utilityModuleContainer = new ModuleWidgetGroup("Utility");

                        utilityModuleContainer.AddWidget(widget);
                        break;
                    case CozyModuleEditor.ModuleCategory.other:
                        if (otherModuleContainer == null)
                            otherModuleContainer = new ModuleWidgetGroup("Other");

                        otherModuleContainer.AddWidget(widget);
                        break;
                }

                moduleNumber++;
            }

            if (atmosphereModuleContainer != null)
            {
                WidgetContainer.Add(atmosphereModuleContainer);
            }
            if (timeModuleContainer != null)
            {
                WidgetContainer.Add(timeModuleContainer);
            }
            if (weatherModuleContainer != null)
            {
                WidgetContainer.Add(weatherModuleContainer);
            }
            if (manipulationModuleContainer != null)
            {
                WidgetContainer.Add(manipulationModuleContainer);
            }
            if (integrationModuleContainer != null)
            {
                WidgetContainer.Add(integrationModuleContainer);
            }
            if (utilityModuleContainer != null)
            {
                WidgetContainer.Add(utilityModuleContainer);
            }
            if (otherModuleContainer != null)
            {
                WidgetContainer.Add(otherModuleContainer);
            }

        }

        void OpenModuleUI(ClickEvent evt, int moduleNumber)
        {


            ModulePage.Clear();
            CozyModuleEditor moduleEditor = moduleEditors[moduleNumber];
            UpdateBanner(moduleEditor.ModuleTitle, moduleEditor.ModuleSubtitle, moduleEditor.ModuleIcon, moduleEditor.BannerBackground);


            Toolbar toolbar = new Toolbar();
            toolbar.AddToClassList("module-toolbar");

            VisualElement backCallbackContainer = new VisualElement();
            backCallbackContainer.style.flexDirection = FlexDirection.Row;

            backCallbackContainer.RegisterCallback((ClickEvent evt) =>
            {
                SwapPages(true);
                UpdateWidgets();
            });

            Image back = new Image() { image = EditorGUIUtility.IconContent("back").image };
            back.style.marginRight = new StyleLength(new Length(4, LengthUnit.Pixel));
            back.style.marginLeft = new StyleLength(new Length(4, LengthUnit.Pixel));
            Image logo = new Image()
            {
                image = moduleEditor.ModuleIcon,
                name = "icon"
            };
            Label label = new Label()
            {
                text = moduleEditor.ModuleTitle,
                tooltip = moduleEditor.ModuleTooltip,
                name = "title"
            };


            backCallbackContainer.Add(back);
            backCallbackContainer.Add(logo);
            backCallbackContainer.Add(label);
            toolbar.Add(backCallbackContainer);

            VisualElement spacer = new VisualElement();
            spacer.style.flexGrow = 1;
            ToolbarButton documentation = new ToolbarButton(moduleEditors[moduleNumber].OpenDocumentationURL)
            {
                name = "documentation"
            };
            documentation.Add(new Image() { image = EditorGUIUtility.IconContent("_Help").image });
            ToolbarButton contextMenu = new ToolbarButton(moduleEditors[moduleNumber].OpenContextMenu)
            {
                name = "context-menu"
            };
            contextMenu.Add(new Image() { image = EditorGUIUtility.IconContent("_Menu").image });

            toolbar.Add(spacer);
            toolbar.Add(documentation);
            toolbar.Add(contextMenu);

            ModulePage.Add(toolbar);
            ModulePage.Add(moduleEditors[moduleNumber].DisplayUI());

            SwapPages(false);

        }

        public void OpenSettings()
        {

            UpdateBanner("Settings", "COZY 3", Resources.Load<Texture2D>("Promo/Distant Lands Watermark"), Resources.Load<Texture2D>("Banners/Cozy Weather"));


            ModulePage.Clear();

            Toolbar toolbar = new Toolbar();
            toolbar.AddToClassList("module-toolbar");

            VisualElement backCallbackContainer = new VisualElement();
            backCallbackContainer.style.flexDirection = FlexDirection.Row;

            backCallbackContainer.RegisterCallback((ClickEvent evt) =>
            {
                SwapPages(true);
                UpdateWidgets();
            });

            Image back = new Image() { image = EditorGUIUtility.IconContent("back").image };
            back.style.marginRight = new StyleLength(new Length(4, LengthUnit.Pixel));
            back.style.marginLeft = new StyleLength(new Length(4, LengthUnit.Pixel));
            Image logo = new Image()
            {
                image = EditorGUIUtility.IconContent("_Popup").image,
                name = "icon"
            };
            Label label = new Label()
            {
                text = "Settings",
                name = "title"
            };


            backCallbackContainer.Add(back);
            backCallbackContainer.Add(logo);
            backCallbackContainer.Add(label);

            VisualElement spacer = new VisualElement();
            spacer.style.flexGrow = 1;
            ToolbarButton documentation = new ToolbarButton(() => { Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/settings"); })
            {
                name = "documentation"
            };
            documentation.Add(new Image() { image = EditorGUIUtility.IconContent("_Help").image });

            toolbar.Add(backCallbackContainer);
            toolbar.Add(spacer);
            toolbar.Add(documentation);

            ModulePage.Add(toolbar);

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Inspectors/UXML/cozy-inspector-settings.uxml"
            );

            asset.CloneTree(ModulePage);
            ModulePage.Bind(serializedObject);


            ModulePage.Q<PropertyField>("separateSunLightAndTransform").RegisterCallback((ChangeEvent<bool> evt) =>
            {
                ModulePage.Q<VisualElement>("sunLightAndTransformContainer").style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });



            FogInScene.value = CozyWeather.SceneFogRendering;
            FogInScene.RegisterCallback((ChangeEvent<bool> evt) =>
            {
                CozyWeather.SceneFogRendering = evt.newValue;
            });

            UpdateInEditMode.value = !CozyWeather.FreezeUpdateInEditMode;
            UpdateInEditMode.RegisterCallback((ChangeEvent<bool> evt) =>
            {
                CozyWeather.FreezeUpdateInEditMode = !evt.newValue;
            });

            CenterWeather.value = CozyWeather.FollowEditorCamera;
            CenterWeather.RegisterCallback((ChangeEvent<bool> evt) =>
            {
                CozyWeather.FollowEditorCamera = evt.newValue;
            });

            Gizmos.value = CozyWeather.DisplayGizmos;
            Gizmos.RegisterCallback((ChangeEvent<bool> evt) =>
            {
                CozyWeather.DisplayGizmos = evt.newValue;
            });

            Tooltips.value = CozyWeather.Tooltips;
            Tooltips.RegisterCallback((ChangeEvent<bool> evt) =>
            {
                CozyWeather.Tooltips = evt.newValue;

                foreach (Tooltip tooltip in root.Query<Tooltip>().ToList())
                {
                    tooltip.style.display = CozyWeather.Tooltips ? DisplayStyle.Flex : DisplayStyle.None;
                }

            });

            Graphs.value = CozyWeather.Graphs;
            Graphs.RegisterCallback((ChangeEvent<bool> evt) =>
            {
                CozyWeather.Graphs = evt.newValue;

                foreach (Graph graph in root.Query<Graph>().ToList())
                {
                    graph.style.display = CozyWeather.Graphs ? DisplayStyle.Flex : DisplayStyle.None;
                }

            });

            SwapPages(false);
        }

        public void OpenStyles()
        {
            UpdateBanner("Styles", "COZY 3", Resources.Load<Texture2D>("Promo/Distant Lands Watermark"), Resources.Load<Texture2D>("Banners/Cozy Weather"));

            ModulePage.Clear();

            Toolbar toolbar = new Toolbar();
            toolbar.AddToClassList("module-toolbar");

            VisualElement backCallbackContainer = new VisualElement();
            backCallbackContainer.style.flexDirection = FlexDirection.Row;

            backCallbackContainer.RegisterCallback((ClickEvent evt) =>
            {
                SwapPages(true);
                UpdateWidgets();
            });

            Image back = new Image() { image = EditorGUIUtility.IconContent("back").image };
            back.style.marginRight = new StyleLength(new Length(4, LengthUnit.Pixel));
            back.style.marginLeft = new StyleLength(new Length(4, LengthUnit.Pixel));
            Image logo = new Image()
            {
                image = EditorGUIUtility.IconContent("d_Grid.PaintTool@2x").image,
                name = "icon"
            };
            Label label = new Label()
            {
                text = "Styles",
                name = "title"
            };


            backCallbackContainer.Add(back);
            backCallbackContainer.Add(logo);
            backCallbackContainer.Add(label);

            VisualElement spacer = new VisualElement();
            spacer.style.flexGrow = 1;
            ToolbarButton documentation = new ToolbarButton(() => { Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/the-weather-sphere"); })
            {
                name = "documentation"
            };
            documentation.Add(new Image() { image = EditorGUIUtility.IconContent("_Help").image });

            toolbar.Add(backCallbackContainer);
            toolbar.Add(spacer);
            toolbar.Add(documentation);

            ModulePage.Add(toolbar);

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Inspectors/UXML/cozy-inspector-styles.uxml"
            );

            asset.CloneTree(ModulePage);

            SkyStyleRoot.Add(StyleButton("Icons/Styles/sky-desktop", "Desktop", CozyWeather.SkyStyle.desktop, true));
            SkyStyleRoot.Add(StyleButton("Icons/Styles/sky-mobile", "Mobile", CozyWeather.SkyStyle.mobile, false));
            SkyStyleRoot.Add(StyleButton("Icons/Styles/none", "None", CozyWeather.SkyStyle.off, true));

            CloudStyleRoot.Add(StyleButton("Icons/Styles/cozy-desktop", "COZY Desktop", CozyWeather.CloudStyle.cozyDesktop, false));
            CloudStyleRoot.Add(StyleButton("Icons/Styles/cozy-mobile", "COZY Mobile", CozyWeather.CloudStyle.cozyMobile, false));
            CloudStyleRoot.Add(StyleButton("Icons/Styles/ghibli-desktop", "Ghibli Desktop", CozyWeather.CloudStyle.ghibliDesktop, false));
            CloudStyleRoot.Add(StyleButton("Icons/Styles/ghibli-mobile", "Ghibli Mobile", CozyWeather.CloudStyle.ghibliMobile, false));
            CloudStyleRoot.Add(StyleButton("Icons/Styles/cozy-painted", "Painted Skies", CozyWeather.CloudStyle.paintedSkies, false));
            CloudStyleRoot.Add(StyleButton("Icons/Styles/cozy-soft", "Soft", CozyWeather.CloudStyle.soft, false));
            CloudStyleRoot.Add(StyleButton("Icons/Styles/cozy-luxury", "Luxury", CozyWeather.CloudStyle.luxury, false));
            CloudStyleRoot.Add(StyleButton("Icons/Styles/single-texture", "Single Texture", CozyWeather.CloudStyle.singleTexture, false));
            CloudStyleRoot.Add(StyleButton("Icons/Styles/none", "None", CozyWeather.CloudStyle.off, true));

            FogStyleRoot.Add(StyleButton("Icons/Styles/fog-stylized", "Stylized", CozyWeather.FogStyle.stylized, true));
            FogStyleRoot.Add(StyleButton("Icons/Styles/fog-height", "Height Fog", CozyWeather.FogStyle.heightFog, true));
            FogStyleRoot.Add(StyleButton("Icons/Styles/fog-stepped", "Stepped", CozyWeather.FogStyle.steppedFog, true));
            FogStyleRoot.Add(StyleButton("Icons/Styles/fog-unity", "Unity", CozyWeather.FogStyle.unity, true));
            FogStyleRoot.Add(StyleButton("Icons/Styles/none", "None", CozyWeather.FogStyle.off, true));


            SwapPages(false);


        }

        public Button StyleButton(string imgUrl, string title, CozyWeather.SkyStyle style, bool dark)
        {
            Button button = new Button();
            Image image = new Image
            {
                image = (Texture)Resources.Load(imgUrl)
            };
            Label label = new Label(title);
            button.AddToClassList("style-selector-button");
            if (style == weatherSphere.skyStyle)
                button.AddToClassList("active");
            if (dark)
                label.AddToClassList("bright");

            button.Add(image);
            button.Add(label);
            button.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                foreach (Button styleButton in SkyStyleRoot.Query<Button>().ToList())
                    styleButton.RemoveFromClassList("active");

                button.AddToClassList("active");
                weatherSphere.SetStyle(style);
                weatherSphere.ResetQuality();
            });

            return button;
        }
        public Button StyleButton(string imgUrl, string title, CozyWeather.FogStyle style, bool dark)
        {
            Button button = new Button();
            Image image = new Image
            {
                image = (Texture)Resources.Load(imgUrl)
            };
            Label label = new Label(title);
            button.AddToClassList("style-selector-button");
            if (style == weatherSphere.fogStyle)
                button.AddToClassList("active");
            if (dark)
                label.AddToClassList("bright");

            button.Add(image);
            button.Add(label);
            button.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                foreach (Button styleButton in FogStyleRoot.Query<Button>().ToList())
                    styleButton.RemoveFromClassList("active");

                button.AddToClassList("active");
                weatherSphere.SetStyle(style);
                weatherSphere.ResetQuality();
            });

            return button;
        }
        public Button StyleButton(string imgUrl, string title, CozyWeather.CloudStyle style, bool dark)
        {
            Button button = new Button();
            Image image = new Image
            {
                image = (Texture)Resources.Load(imgUrl)
            };
            Label label = new Label(title);
            button.AddToClassList("style-selector-button");
            if (style == weatherSphere.cloudStyle)
                button.AddToClassList("active");
            if (dark)
                label.AddToClassList("bright");

            button.Add(image);
            button.Add(label);
            button.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                foreach (Button styleButton in CloudStyleRoot.Query<Button>().ToList())
                    styleButton.RemoveFromClassList("active");

                button.AddToClassList("active");
                weatherSphere.SetStyle(style);
                weatherSphere.ResetQuality();
            });

            return button;
        }

        public void OpenSceneTools()
        {
            UpdateBanner("Scene Tools", "COZY 3", Resources.Load<Texture2D>("Promo/Distant Lands Watermark"), Resources.Load<Texture2D>("Banners/Cozy Weather"));

            ModulePage.Clear();

            Toolbar toolbar = new Toolbar();
            toolbar.AddToClassList("module-toolbar");

            VisualElement backCallbackContainer = new VisualElement();
            backCallbackContainer.style.flexDirection = FlexDirection.Row;

            backCallbackContainer.RegisterCallback((ClickEvent evt) =>
            {
                SwapPages(true);
                UpdateWidgets();
            });

            Image back = new Image() { image = EditorGUIUtility.IconContent("back").image };
            back.style.marginRight = new StyleLength(new Length(4, LengthUnit.Pixel));
            back.style.marginLeft = new StyleLength(new Length(4, LengthUnit.Pixel));
            Image logo = new Image()
            {
                image = EditorGUIUtility.IconContent("d_SceneViewTools@2x").image,
                name = "icon"
            };
            Label label = new Label()
            {
                text = "Scene Tools",
                name = "title"
            };


            backCallbackContainer.Add(back);
            backCallbackContainer.Add(logo);
            backCallbackContainer.Add(label);

            VisualElement spacer = new VisualElement();
            spacer.style.flexGrow = 1;
            ToolbarButton documentation = new ToolbarButton(() => { Application.OpenURL(""); })
            {
                name = "documentation"
            };
            documentation.Add(new Image() { image = EditorGUIUtility.IconContent("_Help").image });

            toolbar.Add(backCallbackContainer);
            toolbar.Add(spacer);
            toolbar.Add(documentation);

            ModulePage.Add(toolbar);

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Inspectors/UXML/cozy-hub-scene-tools.uxml"
            );

            asset.CloneTree(ModulePage);

            Image globalBiomeIcon = new Image
            {
                image = (Texture)Resources.Load("Icons/Global Biome")
            };
            GlobalBiome.Q<VisualElement>("icon").Add(globalBiomeIcon);
            GlobalBiome.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                Camera view = SceneView.lastActiveSceneView.camera;

                GameObject i = new GameObject();
                i.name = "Cozy Biome";
                i.AddComponent<CozyBiome>();
                i.transform.position = (view.transform.forward * 5) + view.transform.position;

                Undo.RegisterCreatedObjectUndo(i, "Create Cozy Biome");
                Selection.activeGameObject = i;
            });
            Image localBiomeIcon = new Image
            {
                image = (Texture)Resources.Load("Icons/Local Biome")
            };
            LocalBiome.Q<VisualElement>("icon").Add(localBiomeIcon);
            LocalBiome.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                Camera view = SceneView.lastActiveSceneView.camera;

                GameObject i = new GameObject();
                i.name = "Cozy Biome";
                CozyBiome biome = i.AddComponent<CozyBiome>();
                BoxCollider collider = i.AddComponent<BoxCollider>();
                collider.size = Vector3.one * 15;
                biome.trigger = collider;
                biome.trigger.isTrigger = true;
                biome.mode = CozyBiome.BiomeMode.Local;
                i.transform.position = (view.transform.forward * 5) + view.transform.position;

                Undo.RegisterCreatedObjectUndo(i, "Create Cozy Biome");
                Selection.activeGameObject = i;
            });

            Image fxBlockZoneIcon = new Image
            {
                image = (Texture)Resources.Load("Icons/FX Block Zone")
            };
            FXBlockZone.Q<VisualElement>("icon").Add(fxBlockZoneIcon);
            FXBlockZone.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                Camera view = SceneView.lastActiveSceneView.camera;

                GameObject i = new GameObject();
                i.name = "Cozy FX Block Zone";
                i.AddComponent<BoxCollider>().isTrigger = true;
                i.tag = "FX Block Zone";
                i.transform.position = (view.transform.forward * 5) + view.transform.position;

                Undo.RegisterCreatedObjectUndo(i, "Create Cozy FX Block Zone");
                Selection.activeGameObject = i;
            });

            Image fogCullingIcon = new Image
            {
                image = (Texture)Resources.Load("Icons/Fog Culling Zone")
            };
            FogCullingZone.Q<VisualElement>("icon").Add(fogCullingIcon);
            FogCullingZone.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                Camera view = SceneView.lastActiveSceneView.camera;
                SkyStyleRoot.Add(StyleButton("Icons/Styles/sky-desktop", "Desktop", CozyWeather.SkyStyle.desktop, true));
                SkyStyleRoot.Add(StyleButton("Icons/Styles/sky-mobile", "Mobile", CozyWeather.SkyStyle.mobile, false));
                SkyStyleRoot.Add(StyleButton("Icons/Styles/none", "None", CozyWeather.SkyStyle.off, true));

                GameObject i = GameObject.CreatePrimitive(PrimitiveType.Cube);
                i.name = "Cozy Fog Cull Zone";
                i.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Fog Culling Zone");
                i.transform.position = (view.transform.forward * 5) + view.transform.position;
                Undo.RegisterCreatedObjectUndo(i, "Create Cozy Fog Culling Zone");
                Selection.activeGameObject = i;
            });
            SwapPages(false);


        }

        void OpenModuleContextMenu(ContextClickEvent evt, CozyModuleEditor moduleEditor)
        {
            moduleEditor.OpenContextMenu();
        }

        void SwapPages(bool goHome)
        {
            if (goHome)
            {
                ModulePage.style.display = DisplayStyle.None;
                HomePage.style.display = DisplayStyle.Flex;
                UpdateBanner("COZY", "Stylized Weather 3", Resources.Load<Texture2D>("Promo/Distant Lands Watermark"), Resources.Load<Texture2D>("Banners/Cozy Weather"));

            }
            else
            {
                ModulePage.style.display = DisplayStyle.Flex;
                HomePage.style.display = DisplayStyle.None;
            }
        }

        public void RepaintUI()
        {
            Repaint();
        }

        void ResetModuleList()
        {

            mods = EditorUtilities.ResetModuleList();

            if (mods.Contains(typeof(CozyModule)))
                mods.Remove(typeof(CozyModule));

            if (mods.Contains(typeof(ExampleModule)))
                mods.Remove(typeof(ExampleModule));

            if (mods.Contains(typeof(CozyTimeOverride)))
                mods.Remove(typeof(CozyTimeOverride));

            if (mods.Contains(typeof(CozyDateOverride)))
                mods.Remove(typeof(CozyDateOverride));

            if (mods.Contains(typeof(CozyBiomeModuleBase<>)))
                mods.Remove(typeof(CozyBiomeModuleBase<>));

            weatherSphere.modules.RemoveAll(x => x == null);

            foreach (CozyModule a in weatherSphere.modules)
                if (mods.Contains(a.GetType()))
                    mods.Remove(a.GetType());
        }

        void AddModule(ClickEvent evt)
        {
            ResetModuleList();
            ModulesSearchProvider provider = CreateInstance<ModulesSearchProvider>();
            provider.modules = mods;
            provider.weather = weatherSphere;
            provider.cozyWeatherEditor = this;
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);

        }

        void OnSceneGUI()
        {

            if (!CozyWeather.DisplayGizmos)
                return;

            Vector3 sunDir = weatherSphere.sunTransform.forward;
            Vector3 sunNormal = weatherSphere.sunTransform.right;
            Vector3 north = Vector3.Cross(weatherSphere.sunTransform.parent.forward, Vector3.up);
            Vector3 west = weatherSphere.sunTransform.parent.forward;
            Vector3 pos = weatherSphere.transform.position;
            GUIStyle textStyleSec = new GUIStyle();
            textStyleSec.normal.textColor = new Color(0, 1, 0, 0.4f);
            textStyleSec.alignment = TextAnchor.MiddleLeft;
            textStyleSec.contentOffset = new Vector2(23, 0);

            if (weatherSphere.GetModule(out CozySatelliteModule module))
            {
                if (module.satellites.Length > 0)
                {
                    Handles.color = new Color(0, 1, 0, 0.4f);
                    Handles.DrawWireArc(pos + weatherSphere.moonDirection.normalized * 10, weatherSphere.moonDirection, sunNormal, 360, 0.5f, 1);
                    Handles.Label(pos + weatherSphere.moonDirection, $"  Current Moon Phase: {module.GetMoonPhase()}", textStyleSec);
                    SatelliteProfile moon = module.satellites[module.mainMoon];
                    if (moon.orbitRef)
                    {
                        Handles.DrawWireArc(pos, moon.orbitRef.right, Quaternion.AngleAxis(5, moon.orbitRef.right) * weatherSphere.moonDirection, 350, 10, 1);

                        float dec = moon.declination * Mathf.Sin(Mathf.PI * 2 * ((weatherSphere.modifiedDayPercentage + (float)(weatherSphere.timeModule.currentDay + moon.rotationPeriodOffset + weatherSphere.timeModule.DaysPerYear * weatherSphere.timeModule.currentYear) % moon.declinationPeriod) / moon.declinationPeriod));

                        Handles.color = new Color(0, 1, 0, 0.5f);
                        Quaternion moonOffset = Quaternion.AngleAxis(-dec - moon.declination, moon.orbitRef.forward);
                        Quaternion negativeMoonOffset = Quaternion.AngleAxis(-dec + moon.declination, moon.orbitRef.forward);
                        Handles.DrawWireArc(pos, moonOffset * moon.orbitRef.right, moon.orbitRef.forward, 360, 10);
                        Handles.DrawWireArc(pos, negativeMoonOffset * moon.orbitRef.right, moon.orbitRef.forward, 360, 10);
                    }
                }
            }

            Handles.color = new Color(1, 0.5f, 0);
            GUIStyle textStyle = new GUIStyle();
            textStyle.normal.textColor = Handles.color;
            textStyle.alignment = TextAnchor.MiddleLeft;
            textStyle.contentOffset = new Vector2(15, 10);

            GUIStyle compassTextStyle = new GUIStyle();
            compassTextStyle.normal.textColor = Color.white;
            compassTextStyle.fontStyle = FontStyle.Bold;
            compassTextStyle.alignment = TextAnchor.MiddleCenter;


            Handles.DrawWireArc(pos, sunNormal, Quaternion.AngleAxis(5, sunNormal) * -sunDir, 350, 10, 1);
            if (weatherSphere.timeModule)
            {
                weatherSphere.timeModule.GetSunTransitTime(out MeridiemTime sunrise, out MeridiemTime sunset);
                Handles.Label(pos - west, $"  Sunrise: {sunrise.ToString()}", textStyle);
                Handles.Label(pos + west, $"  Sunset: {sunset.ToString()}", textStyle);
            }
            Handles.DrawWireArc(pos - sunDir.normalized * 10, sunDir, sunNormal, 360, 0.5f, 1);

            Handles.color = new Color(1, 1, 1, 0.3f);

            Handles.Label(pos + west, "W", compassTextStyle);
            Handles.Label(pos - west, "E", compassTextStyle);
            Handles.Label(pos + north, "N", compassTextStyle);
            Handles.Label(pos - north, "S", compassTextStyle);

            Quaternion rotation = Quaternion.AngleAxis(7.5f, Vector3.up);
            Handles.DrawWireArc(pos, Vector3.up, rotation * west, 75, 10, 2);
            Handles.DrawWireArc(pos, Vector3.up, rotation * -west, 75, 10, 2);
            Handles.DrawWireArc(pos, Vector3.up, rotation * north, 75, 10, 2);
            Handles.DrawWireArc(pos, Vector3.up, rotation * -north, 75, 10, 2);

        }

        public void Search(string keyword)
        {
            SearchResults.Clear();

            for (int i = 0; i < moduleEditors.Count; i++)
            {
                CozyModule module = weatherSphere.modules[i];
                CozyModuleEditor moduleEditor = moduleEditors[i];

                List<PropertyField> fields = SearchModule(keyword, module);

                if (fields.Count > 0)
                {
                    Label label = new Label(moduleEditor.ModuleTitle);
                    label.AddToClassList("h1");
                    SearchResults.Add(label);

                    foreach (PropertyField field in fields)
                    {
                        SearchResults.Add(field);
                    }
                }
            }




        }

        public List<PropertyField> SearchModule(string keyword, CozyModule module)
        {
            SerializedObject moduleSO = new SerializedObject(module);
            List<PropertyField> properties = new List<PropertyField>();
            moduleSO.Update();

            SerializedProperty iterator = moduleSO.GetIterator();
            iterator.NextVisible(true);
            int i = 0;
            while (true)
            {
                if (iterator == null)
                    break;

                i++;
                if (i > 15)
                    break;

                CozySearchable[] index = CozySearchUtils.GetAttributes<CozySearchable>(iterator, false);

                List<string> keywordsOnField = new List<string>();

                if (index.Length > 0 && index[0] != null)
                {
                    keywordsOnField.Add(iterator.displayName);
                    keywordsOnField.AddRange(index[0].keywords);
                }

                if (string.Join('|', keywordsOnField).Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                {
                    PropertyField field = new PropertyField();
                    field.BindProperty(iterator);
                    properties.Add(field);
                }

                if (index.Length > 0 && index[0].deepSearch)
                    if (iterator.objectReferenceValue != null)
                    {
                        if (iterator.objectReferenceValue.GetType().IsSubclassOf(typeof(CozyProfile)))
                        {
                            properties.AddRange(SearchProfile(keyword, iterator.objectReferenceValue as CozyProfile));
                        }
                    }

                if (iterator.hasChildren)
                {
                    if (!iterator.NextVisible(false))
                        break;
                    continue;
                }

                if (!iterator.NextVisible(true))
                    break;
            }

            moduleSO.ApplyModifiedProperties();

            return properties;

        }

        public List<PropertyField> SearchProfile(string keyword, CozyProfile profile)
        {
            SerializedObject moduleSO = new SerializedObject(profile);
            List<PropertyField> properties = new List<PropertyField>();
            moduleSO.Update();

            SerializedProperty iterator = moduleSO.GetIterator();
            iterator.NextVisible(true);
            int i = 0;
            while (true)
            {
                if (iterator == null)
                    break;

                i++;
                if (i > 50)
                    break;

                CozySearchable[] index = CozySearchUtils.GetAttributes<CozySearchable>(iterator, false);

                List<string> keywordsOnField = new List<string>();

                if (index.Length > 0 && index[0] != null)
                {
                    keywordsOnField.Add(iterator.displayName);
                    keywordsOnField.AddRange(index[0].keywords);
                }

                if (string.Join('|', keywordsOnField).Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                {
                    PropertyField field = new PropertyField();
                    field.BindProperty(iterator);
                    properties.Add(field);
                }

                if (iterator.hasChildren)
                {
                    if (!iterator.NextVisible(false))
                        break;
                    continue;
                }

                if (!iterator.NextVisible(true))
                    break;
            }

            moduleSO.ApplyModifiedProperties();

            return properties;

        }

        public void ClearSearch()
        {
            SearchResults.Clear();
        }


    }
}