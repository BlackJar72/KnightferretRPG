using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(AtmosphereProfile))]
    public class AtmosphereProfileEditor : Editor
    {


        AtmosphereProfile profile;
        VisualElement root;
        CozyWeather WeatherSphere => CozyWeather.instance;

        public VisualElement ContentContainer => root.Q<VisualElement>("content-container");
        public Label ContentLabel => root.Q<Label>("content-label");
        public VisualElement TabGroup => root.Q<VisualElement>("tabs");
        public VisualElement TabContent => root.Q<VisualElement>("tab-content");


        void OnEnable()
        {

            profile = (AtmosphereProfile)target;

        }

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Profiles/UXML/atmosphere-profile-editor.uxml"
            );

            asset.CloneTree(root);

            SelectTab(0);

            TabGroup.Q<Button>("lighting").RegisterCallback((ClickEvent evt) => { SelectTab(0); });
            TabGroup.Q<Button>("lighting").Add(new Image() { image = (Texture2D)Resources.Load("Icons/Lighting") });
            TabGroup.Q<Button>("fog").RegisterCallback((ClickEvent evt) => { SelectTab(1); });
            TabGroup.Q<Button>("fog").Add(new Image() { image = (Texture2D)Resources.Load("Icons/Fog") });
            TabGroup.Q<Button>("clouds").RegisterCallback((ClickEvent evt) => { SelectTab(2); });
            TabGroup.Q<Button>("clouds").Add(new Image() { image = (Texture2D)Resources.Load("Icons/Clouds") });
            TabGroup.Q<Button>("celestials").RegisterCallback((ClickEvent evt) => { SelectTab(3); });
            TabGroup.Q<Button>("celestials").Add(new Image() { image = (Texture2D)Resources.Load("Icons/Celestials") });

            return root;
        }

        public void SelectTab(int tabIndex)
        {
            DeselectAllTabs();
            switch (tabIndex)
            {
                case 0:
                    TabGroup.Q<Button>("lighting").AddToClassList("selected");
                    RenderLightingSettings();
                    break;
                case 1:
                    TabGroup.Q<Button>("fog").AddToClassList("selected");
                    RenderFogSettings();
                    break;
                case 2:
                    TabGroup.Q<Button>("clouds").AddToClassList("selected");
                    RenderCloudsSettings();
                    break;
                case 3:
                    TabGroup.Q<Button>("celestials").AddToClassList("selected");
                    RenderCelestialsSettings();
                    break;
                default:
                    TabGroup.Q<Button>("lighting").AddToClassList("selected");
                    RenderLightingSettings();
                    break;
            }

        }

        public void DeselectAllTabs()
        {
            TabGroup.Query<Button>().ForEach(x => x.RemoveFromClassList("selected"));
        }


        public void RenderLightingSettings()
        {

            ContentContainer.Clear();
            ContentLabel.text = "Atmosphere & Lighting";

            Label skydomeSettings = new Label("Skydome Settings");
            skydomeSettings.AddToClassList("h2");
            skydomeSettings.AddToClassList("mb-md");
            ContentContainer.Add(skydomeSettings);

            VariablePropertyField skyZenithColor = new VariablePropertyField(serializedObject.FindProperty("skyZenithColor"), true);
            ContentContainer.Add(skyZenithColor);

            VariablePropertyField skyHorizonColor = new VariablePropertyField(serializedObject.FindProperty("skyHorizonColor"), true);
            ContentContainer.Add(skyHorizonColor);

            VariablePropertyField gradientExponent = new VariablePropertyField(serializedObject.FindProperty("gradientExponent"), 0, 1);
            gradientExponent.AddToClassList("mb-md");
            ContentContainer.Add(gradientExponent);

            Label lightingSettings = new Label("Lighting Settings");
            lightingSettings.AddToClassList("h2");
            lightingSettings.AddToClassList("mb-md");
            ContentContainer.Add(lightingSettings);

            VariablePropertyField sunlightColor = new VariablePropertyField(serializedObject.FindProperty("sunlightColor"), true);
            ContentContainer.Add(sunlightColor);


            PropertyField sunShadowsField = new PropertyField();
            sunShadowsField.BindProperty(serializedObject.FindProperty("sunlightShadows"));
            sunShadowsField.label = "Sun Shadow Type";
            sunShadowsField.AddToClassList("unity-base-field__aligned");
            ContentContainer.Add(sunShadowsField);

            VariablePropertyField moonlightColor = new VariablePropertyField(serializedObject.FindProperty("moonlightColor"), true);
            ContentContainer.Add(moonlightColor);

            PropertyField moonShadowsField = new PropertyField();
            moonShadowsField.BindProperty(serializedObject.FindProperty("moonlightShadows"));
            moonShadowsField.label = "Moon Shadow Type";
            moonShadowsField.AddToClassList("unity-base-field__aligned");
            ContentContainer.Add(moonShadowsField);

            VariablePropertyField ambientLightHorizonColor = new VariablePropertyField(serializedObject.FindProperty("ambientLightHorizonColor"), true);
            ContentContainer.Add(ambientLightHorizonColor);

            VariablePropertyField ambientLightZenithColor = new VariablePropertyField(serializedObject.FindProperty("ambientLightZenithColor"), true);
            ContentContainer.Add(ambientLightZenithColor);

            VariablePropertyField ambientLightMultiplier = new VariablePropertyField(serializedObject.FindProperty("ambientLightMultiplier"), 0, 2);
            ambientLightMultiplier.AddToClassList("mb-md");
            ContentContainer.Add(ambientLightMultiplier);
        }

        public void RenderFogSettings()
        {
            ContentContainer.Clear();
            ContentLabel.text = "Fog";

            Label baseFog = new Label("Fog Generation");
            baseFog.AddToClassList("h2");
            baseFog.AddToClassList("mb-md");
            ContentContainer.Add(baseFog);

            VariablePropertyField fogColor1Field = new VariablePropertyField(serializedObject.FindProperty("fogColor1"), true);
            ContentContainer.Add(fogColor1Field);

            VariablePropertyField fogColor2Field = new VariablePropertyField(serializedObject.FindProperty("fogColor2"), true);
            ContentContainer.Add(fogColor2Field);

            VariablePropertyField fogColor3Field = new VariablePropertyField(serializedObject.FindProperty("fogColor3"), true);
            ContentContainer.Add(fogColor3Field);

            VariablePropertyField fogColor4Field = new VariablePropertyField(serializedObject.FindProperty("fogColor4"), true);
            ContentContainer.Add(fogColor4Field);

            VariablePropertyField fogColor5Field = new VariablePropertyField(serializedObject.FindProperty("fogColor5"), true);
            fogColor5Field.AddToClassList("mb-md");
            ContentContainer.Add(fogColor5Field);

            VariablePropertyField fogStart1Field = new VariablePropertyField(serializedObject.FindProperty("fogStart1"), 0, 50);
            ContentContainer.Add(fogStart1Field);
            VariablePropertyField fogStart2Field = new VariablePropertyField(serializedObject.FindProperty("fogStart2"), 0, 50);
            ContentContainer.Add(fogStart2Field);
            VariablePropertyField fogStart3Field = new VariablePropertyField(serializedObject.FindProperty("fogStart3"), 0, 50);
            ContentContainer.Add(fogStart3Field);
            VariablePropertyField fogStart4Field = new VariablePropertyField(serializedObject.FindProperty("fogStart4"), 0, 50);
            fogStart4Field.AddToClassList("mb-md");
            ContentContainer.Add(fogStart4Field);


            VariablePropertyField fogHeight = new VariablePropertyField(serializedObject.FindProperty("fogHeight"), 0, 2);
            ContentContainer.Add(fogHeight);
            VariablePropertyField fogDensityMultiplier = new VariablePropertyField(serializedObject.FindProperty("fogDensityMultiplier"), 0, 5);
            ContentContainer.Add(fogDensityMultiplier);

            VariablePropertyField fogSmoothnessField = new VariablePropertyField(serializedObject.FindProperty("fogSmoothness"), 0, 1);
            fogSmoothnessField.AddToClassList("mb-md");
            ContentContainer.Add(fogSmoothnessField);

            Label flaresSection = new Label("Fog Light Flares");
            flaresSection.AddToClassList("h2");
            flaresSection.AddToClassList("mb-md");
            ContentContainer.Add(flaresSection);


            VariablePropertyField fogFlareColorField = new VariablePropertyField(serializedObject.FindProperty("fogFlareColor"), true);
            ContentContainer.Add(fogFlareColorField);

            VariablePropertyField fogMoonFlareColorField = new VariablePropertyField(serializedObject.FindProperty("fogMoonFlareColor"), true);
            ContentContainer.Add(fogMoonFlareColorField);

            VariablePropertyField fogLightFlareIntensity = new VariablePropertyField(serializedObject.FindProperty("fogLightFlareIntensity"), 0, 2);
            ContentContainer.Add(fogLightFlareIntensity);

            VariablePropertyField fogLightFlareFalloff = new VariablePropertyField(serializedObject.FindProperty("fogLightFlareFalloff"), 0, 40);
            ContentContainer.Add(fogLightFlareFalloff);

            VariablePropertyField fogLightFlareSquish = new VariablePropertyField(serializedObject.FindProperty("fogLightFlareSquish"), 0, 10);
            fogLightFlareSquish.AddToClassList("mb-md");
            ContentContainer.Add(fogLightFlareSquish);


            Label variationSection = new Label("Fog Variation");
            variationSection.AddToClassList("h2");
            variationSection.AddToClassList("mb-md");
            ContentContainer.Add(variationSection);

            PropertyField fogVariationDirection = new PropertyField();
            fogVariationDirection.BindProperty(serializedObject.FindProperty("fogVariationDirection"));
            ContentContainer.Add(fogVariationDirection);

            VariablePropertyField fogVariationScaleField = new VariablePropertyField(serializedObject.FindProperty("fogVariationScale"), 0, 30);
            ContentContainer.Add(fogVariationScaleField);

            VariablePropertyField fogVariationAmountField = new VariablePropertyField(serializedObject.FindProperty("fogVariationAmount"), 0, 1);
            ContentContainer.Add(fogVariationAmountField);

            VariablePropertyField fogVariationDistanceField = new VariablePropertyField(serializedObject.FindProperty("fogVariationDistance"), 0, 200);
            fogVariationDistanceField.AddToClassList("mb-md");
            ContentContainer.Add(fogVariationDistanceField);

            VisualElement heightFogContainer = new VisualElement();
            SetVisible(heightFogContainer, WeatherSphere.fogStyle == CozyWeather.FogStyle.heightFog);

            Label heightFog = new Label("Height Fog");
            heightFog.AddToClassList("h2");
            heightFog.AddToClassList("mb-md");
            heightFogContainer.Add(heightFog);

            VariablePropertyField heightFogColorField = new VariablePropertyField(serializedObject.FindProperty("heightFogColor"), true);
            heightFogContainer.Add(heightFogColorField);

            VariablePropertyField heightFogIntensityField = new VariablePropertyField(serializedObject.FindProperty("heightFogIntensity"), 0, 1);
            heightFogContainer.Add(heightFogIntensityField);

            VariablePropertyField fogBaseField = new VariablePropertyField(serializedObject.FindProperty("fogBase"), false);
            heightFogContainer.Add(fogBaseField);

            VariablePropertyField heightFogTransitionField = new VariablePropertyField(serializedObject.FindProperty("heightFogTransition"), 0, 500);
            heightFogContainer.Add(heightFogTransitionField);

            VariablePropertyField heightFogDistanceField = new VariablePropertyField(serializedObject.FindProperty("heightFogDistance"), 0, 5000);
            heightFogDistanceField.AddToClassList("mb-md");
            heightFogContainer.Add(heightFogDistanceField);

            VariablePropertyField heightFogVariationScaleField = new VariablePropertyField(serializedObject.FindProperty("heightFogVariationScale"), 100, 1000);
            heightFogContainer.Add(heightFogVariationScaleField);

            VariablePropertyField heightFogVariationAmountField = new VariablePropertyField(serializedObject.FindProperty("heightFogVariationAmount"), 0, 50);
            heightFogContainer.Add(heightFogVariationAmountField);
            ContentContainer.Add(heightFogContainer);

            Label layersSection = new Label("Fog Layers");
            layersSection.AddToClassList("h2");
            layersSection.AddToClassList("mb-md");
            ContentContainer.Add(layersSection);


            VariablePropertyField skyFogAmount = new VariablePropertyField(serializedObject.FindProperty("skyFogAmount"), 0, 1);
            ContentContainer.Add(skyFogAmount);
            VariablePropertyField cloudsFogAmount = new VariablePropertyField(serializedObject.FindProperty("cloudsFogAmount"), 0, 1);
            ContentContainer.Add(cloudsFogAmount);
            VariablePropertyField cloudsFogLightAmount = new VariablePropertyField(serializedObject.FindProperty("cloudsFogLightAmount"), 0, 1);
            ContentContainer.Add(cloudsFogLightAmount);

        }

        public void RenderCloudsSettings()
        {
            ContentContainer.Clear();
            ContentLabel.text = "Clouds";

            Label cloudColorLabel = new Label("Color Settings");
            cloudColorLabel.AddToClassList("h2");
            cloudColorLabel.AddToClassList("mb-md");
            ContentContainer.Add(cloudColorLabel);

            VariablePropertyField cloudColorField = new VariablePropertyField(serializedObject.FindProperty("cloudColor"), true);
            ContentContainer.Add(cloudColorField);

            VariablePropertyField highAltitudeCloudColorField = new VariablePropertyField(serializedObject.FindProperty("highAltitudeCloudColor"), true);
            highAltitudeCloudColorField.AddToClassList("mb-md");
            ContentContainer.Add(highAltitudeCloudColorField);

            VariablePropertyField cloudHighlightColorField = new VariablePropertyField(serializedObject.FindProperty("cloudHighlightColor"), true);
            cloudHighlightColorField.Label.text = "Sun Highlight Color";
            SetVisible(cloudHighlightColorField, WeatherSphere.cloudStyle != CozyWeather.CloudStyle.luxury && WeatherSphere.cloudStyle != CozyWeather.CloudStyle.singleTexture);
            ContentContainer.Add(cloudHighlightColorField);

            VariablePropertyField cloudSunHighlightFalloffField = new VariablePropertyField(serializedObject.FindProperty("cloudSunHighlightFalloff"), 0, 50);
            cloudSunHighlightFalloffField.Label.text = "Sun Highlight Falloff";
            SetVisible(cloudSunHighlightFalloffField, WeatherSphere.cloudStyle != CozyWeather.CloudStyle.luxury && WeatherSphere.cloudStyle != CozyWeather.CloudStyle.singleTexture);
            ContentContainer.Add(cloudSunHighlightFalloffField);

            VariablePropertyField cloudMoonColorField = new VariablePropertyField(serializedObject.FindProperty("cloudMoonColor"), true);
            cloudMoonColorField.Label.text = "Moon Highlight Color";
            SetVisible(cloudMoonColorField, WeatherSphere.cloudStyle != CozyWeather.CloudStyle.luxury &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.singleTexture &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.ghibliMobile &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.cozyMobile);
            ContentContainer.Add(cloudMoonColorField);

            VariablePropertyField cloudMoonHighlightFalloffField = new VariablePropertyField(serializedObject.FindProperty("cloudMoonHighlightFalloff"), 0, 50);
            cloudMoonHighlightFalloffField.Label.text = "Moon Highlight Falloff";
            SetVisible(cloudMoonHighlightFalloffField, WeatherSphere.cloudStyle != CozyWeather.CloudStyle.luxury &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.singleTexture &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.ghibliMobile &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.cozyMobile);
            cloudMoonHighlightFalloffField.AddToClassList("mb-md");
            ContentContainer.Add(cloudMoonHighlightFalloffField);

            Label generationLabel = new Label("Generation Settings");
            generationLabel.AddToClassList("h2");
            generationLabel.AddToClassList("mb-md");
            ContentContainer.Add(generationLabel);

            VariablePropertyField clippingThresholdField = new VariablePropertyField(serializedObject.FindProperty("clippingThreshold"), 0, 1);
            ContentContainer.Add(clippingThresholdField);

            VariablePropertyField cloudWindSpeedField = new VariablePropertyField(serializedObject.FindProperty("cloudWindSpeed"), 0, 10);
            ContentContainer.Add(cloudWindSpeedField);

            VariablePropertyField cloudMainScaleField = new VariablePropertyField(serializedObject.FindProperty("cloudMainScale"), 2, 60);
            ContentContainer.Add(cloudMainScaleField);

            VariablePropertyField cloudDetailScaleField = new VariablePropertyField(serializedObject.FindProperty("cloudDetailScale"), 0.2f, 10);
            ContentContainer.Add(cloudDetailScaleField);

            VariablePropertyField cloudDetailAmountField = new VariablePropertyField(serializedObject.FindProperty("cloudDetailAmount"), 0, 30);
            ContentContainer.Add(cloudDetailAmountField);

            VariablePropertyField acScaleField = new VariablePropertyField(serializedObject.FindProperty("acScale"), 0.1f, 3);
            SetVisible(acScaleField, WeatherSphere.cloudStyle != CozyWeather.CloudStyle.cozyMobile &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.singleTexture &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.ghibliMobile &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.ghibliDesktop);
            acScaleField.Label.text = "Altocumulus Scale";
            ContentContainer.Add(acScaleField);

            VariablePropertyField cirroMoveSpeedField = new VariablePropertyField(serializedObject.FindProperty("cirroMoveSpeed"), 0, 3);
            cirroMoveSpeedField.Label.text = "Cirrostratus Movement";
            SetVisible(cirroMoveSpeedField, WeatherSphere.cloudStyle != CozyWeather.CloudStyle.cozyMobile &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.singleTexture &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.ghibliMobile &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.ghibliDesktop);
            ContentContainer.Add(cirroMoveSpeedField);

            VariablePropertyField cirrusMoveSpeedField = new VariablePropertyField(serializedObject.FindProperty("cirrusMoveSpeed"), 0, 3);
            cirroMoveSpeedField.Label.text = "Cirrus Movement";
            SetVisible(cirrusMoveSpeedField, WeatherSphere.cloudStyle != CozyWeather.CloudStyle.cozyMobile &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.singleTexture &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.ghibliMobile &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.ghibliDesktop);
            ContentContainer.Add(cirrusMoveSpeedField);

            VariablePropertyField chemtrailsMoveSpeedField = new VariablePropertyField(serializedObject.FindProperty("chemtrailsMoveSpeed"), 0, 3);
            cirroMoveSpeedField.Label.text = "Chemtrails Movement";
            SetVisible(chemtrailsMoveSpeedField, WeatherSphere.cloudStyle != CozyWeather.CloudStyle.cozyMobile &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.singleTexture &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.ghibliMobile &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.ghibliDesktop);
            ContentContainer.Add(chemtrailsMoveSpeedField);

            VariablePropertyField cloudTextureColorField = new VariablePropertyField(serializedObject.FindProperty("cloudTextureColor"), true);
            SetVisible(cloudTextureColorField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.paintedSkies ||
                                                      WeatherSphere.cloudStyle == CozyWeather.CloudStyle.singleTexture);
            ContentContainer.Add(cloudTextureColorField);

            VariablePropertyField cloudCohesionField = new VariablePropertyField(serializedObject.FindProperty("cloudCohesion"), 0, 10);
            SetVisible(cloudCohesionField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.ghibliDesktop ||
                                                      WeatherSphere.cloudStyle == CozyWeather.CloudStyle.ghibliMobile);
            ContentContainer.Add(cloudCohesionField);

            VariablePropertyField spherizeField = new VariablePropertyField(serializedObject.FindProperty("spherize"), 0, 1);
            SetVisible(spherizeField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.ghibliDesktop ||
                                                      WeatherSphere.cloudStyle == CozyWeather.CloudStyle.ghibliMobile);
            ContentContainer.Add(spherizeField);

            VariablePropertyField shadowDistanceField = new VariablePropertyField(serializedObject.FindProperty("shadowDistance"), 0, 10);
            SetVisible(shadowDistanceField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.ghibliDesktop ||
                                                      WeatherSphere.cloudStyle == CozyWeather.CloudStyle.ghibliMobile);
            ContentContainer.Add(shadowDistanceField);

            VariablePropertyField cloudThicknessField = new VariablePropertyField(serializedObject.FindProperty("cloudThickness"), 0, 4);
            SetVisible(cloudThicknessField, WeatherSphere.cloudStyle != CozyWeather.CloudStyle.cozyDesktop &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.cozyMobile &&
                                                      WeatherSphere.cloudStyle != CozyWeather.CloudStyle.singleTexture);
            ContentContainer.Add(cloudThicknessField);

            VariablePropertyField textureAmountField = new VariablePropertyField(serializedObject.FindProperty("textureAmount"), 0, 3);
            SetVisible(shadowDistanceField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.paintedSkies);
            ContentContainer.Add(textureAmountField);
            textureAmountField.AddToClassList("mb-md");

            VisualElement currentSettings = new VisualElement();
            SetVisible(currentSettings, !WeatherSphere.weatherModule);

            
            Label texturesLabel = new Label("Cloud Textures");
            texturesLabel.AddToClassList("h2");
            texturesLabel.AddToClassList("mb-md");
            SetVisible(texturesLabel, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.cozyDesktop || 
                                      WeatherSphere.cloudStyle == CozyWeather.CloudStyle.paintedSkies || 
                                      WeatherSphere.cloudStyle == CozyWeather.CloudStyle.singleTexture);
            ContentContainer.Add(texturesLabel);
            
            PropertyField cloudTextureField = new PropertyField();
            cloudTextureField.BindProperty(serializedObject.FindProperty("cloudTexture"));
            SetVisible(cloudTextureField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.paintedSkies ||
                                                      WeatherSphere.cloudStyle == CozyWeather.CloudStyle.singleTexture);
            ContentContainer.Add(cloudTextureField);
            
            PropertyField texturePanDirectionField = new PropertyField();
            texturePanDirectionField.BindProperty(serializedObject.FindProperty("texturePanDirection"));
            SetVisible(texturePanDirectionField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.singleTexture);
            ContentContainer.Add(texturePanDirectionField);
            
            PropertyField chemtrailsTextureField = new PropertyField();
            chemtrailsTextureField.BindProperty(serializedObject.FindProperty("chemtrailsTexture"));
            SetVisible(chemtrailsTextureField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.paintedSkies ||
                                                      WeatherSphere.cloudStyle == CozyWeather.CloudStyle.cozyDesktop);
            ContentContainer.Add(chemtrailsTextureField);
            
            PropertyField cirrusCloudTextureField = new PropertyField();
            cirrusCloudTextureField.BindProperty(serializedObject.FindProperty("cirrusCloudTexture"));
            SetVisible(cirrusCloudTextureField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.paintedSkies ||
                                                      WeatherSphere.cloudStyle == CozyWeather.CloudStyle.cozyDesktop);
            ContentContainer.Add(cirrusCloudTextureField);
            
            PropertyField cirrostratusCloudTextureField = new PropertyField();
            cirrostratusCloudTextureField.BindProperty(serializedObject.FindProperty("cirrostratusCloudTexture"));
            SetVisible(cirrostratusCloudTextureField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.paintedSkies ||
                                                      WeatherSphere.cloudStyle == CozyWeather.CloudStyle.cozyDesktop);
            ContentContainer.Add(cirrostratusCloudTextureField);
            
            PropertyField altocumulusCloudTextureField = new PropertyField();
            altocumulusCloudTextureField.BindProperty(serializedObject.FindProperty("altocumulusCloudTexture"));
            SetVisible(altocumulusCloudTextureField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.paintedSkies ||
                                                      WeatherSphere.cloudStyle == CozyWeather.CloudStyle.cozyDesktop);
            ContentContainer.Add(altocumulusCloudTextureField);

            Label luxuryTexturesLabel = new Label("Luxury Clouds Textures");
            luxuryTexturesLabel.AddToClassList("h2");
            luxuryTexturesLabel.AddToClassList("mb-md");
            SetVisible(luxuryTexturesLabel, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.luxury);
            ContentContainer.Add(luxuryTexturesLabel);

            PropertyField partlyCloudyLuxuryCloudsField = new PropertyField();
            partlyCloudyLuxuryCloudsField.BindProperty(serializedObject.FindProperty("partlyCloudyLuxuryClouds"));
            SetVisible(partlyCloudyLuxuryCloudsField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.luxury);
            ContentContainer.Add(partlyCloudyLuxuryCloudsField);

            PropertyField mostlyCloudyLuxuryCloudsField = new PropertyField();
            mostlyCloudyLuxuryCloudsField.BindProperty(serializedObject.FindProperty("mostlyCloudyLuxuryClouds"));
            SetVisible(mostlyCloudyLuxuryCloudsField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.luxury);
            ContentContainer.Add(mostlyCloudyLuxuryCloudsField);

            PropertyField overcastLuxuryCloudsField = new PropertyField();
            overcastLuxuryCloudsField.BindProperty(serializedObject.FindProperty("overcastLuxuryClouds"));
            SetVisible(overcastLuxuryCloudsField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.luxury);
            ContentContainer.Add(overcastLuxuryCloudsField);

            PropertyField lowBorderLuxuryCloudsField = new PropertyField();
            lowBorderLuxuryCloudsField.BindProperty(serializedObject.FindProperty("lowBorderLuxuryClouds"));
            SetVisible(lowBorderLuxuryCloudsField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.luxury);
            ContentContainer.Add(lowBorderLuxuryCloudsField);

            PropertyField highBorderLuxuryCloudsField = new PropertyField();
            highBorderLuxuryCloudsField.BindProperty(serializedObject.FindProperty("highBorderLuxuryClouds"));
            SetVisible(highBorderLuxuryCloudsField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.luxury);
            ContentContainer.Add(highBorderLuxuryCloudsField);

            PropertyField lowNimbusLuxuryCloudsField = new PropertyField();
            lowNimbusLuxuryCloudsField.BindProperty(serializedObject.FindProperty("lowNimbusLuxuryClouds"));
            SetVisible(lowNimbusLuxuryCloudsField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.luxury);
            ContentContainer.Add(lowNimbusLuxuryCloudsField);

            PropertyField midNimbusLuxuryCloudsField = new PropertyField();
            midNimbusLuxuryCloudsField.BindProperty(serializedObject.FindProperty("midNimbusLuxuryClouds"));
            SetVisible(midNimbusLuxuryCloudsField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.luxury);
            ContentContainer.Add(midNimbusLuxuryCloudsField);

            PropertyField highNimbusLuxuryCloudsField = new PropertyField();
            highNimbusLuxuryCloudsField.BindProperty(serializedObject.FindProperty("highNimbusLuxuryClouds"));
            SetVisible(highNimbusLuxuryCloudsField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.luxury);
            ContentContainer.Add(highNimbusLuxuryCloudsField);

            PropertyField luxuryVariationField = new PropertyField();
            luxuryVariationField.BindProperty(serializedObject.FindProperty("luxuryVariation"));
            SetVisible(luxuryVariationField, WeatherSphere.cloudStyle == CozyWeather.CloudStyle.luxury);
            ContentContainer.Add(luxuryVariationField);

            Label currentCloudSettingsLabel = new Label("Current Settings");
            currentCloudSettingsLabel.AddToClassList("h2");
            currentCloudSettingsLabel.AddToClassList("mb-md");
            currentSettings.Add(currentCloudSettingsLabel);

            Slider cumulusField = new Slider("Cumulus Clouds")
            {
                value = WeatherSphere.cumulus,
                lowValue = 0,
                highValue = 1,
                showInputField = true
            };
            cumulusField.AddToClassList("unity-base-field__aligned");
            cumulusField.RegisterCallback((ChangeEvent<float> evt) =>
            {
                WeatherSphere.cumulus = evt.newValue;
            });
            currentSettings.Add(cumulusField);

            Slider altocumulusField = new Slider("Altocumulus Clouds")
            {
                value = WeatherSphere.altocumulus,
                lowValue = 0,
                highValue = 2,
                showInputField = true
            };
            altocumulusField.AddToClassList("unity-base-field__aligned");
            altocumulusField.RegisterCallback((ChangeEvent<float> evt) =>
            {
                WeatherSphere.altocumulus = evt.newValue;
            });
            currentSettings.Add(altocumulusField);

            Slider chemtrailsField = new Slider("Chemtrails")
            {
                value = WeatherSphere.chemtrails,
                lowValue = 0,
                highValue = 2,
                showInputField = true
            };
            chemtrailsField.AddToClassList("unity-base-field__aligned");
            chemtrailsField.RegisterCallback((ChangeEvent<float> evt) =>
            {
                WeatherSphere.chemtrails = evt.newValue;
            });
            currentSettings.Add(chemtrailsField);

            Slider cirrusField = new Slider("Cirrus Clouds")
            {
                value = WeatherSphere.cirrus,
                lowValue = 0,
                highValue = 2,
                showInputField = true
            };
            cirrusField.AddToClassList("unity-base-field__aligned");
            cirrusField.RegisterCallback((ChangeEvent<float> evt) =>
            {
                WeatherSphere.cirrus = evt.newValue;
            });
            currentSettings.Add(cirrusField);

            Slider nimbusField = new Slider("Nimbus Clouds")
            {
                value = WeatherSphere.nimbus,
                lowValue = 0,
                highValue = 2,
                showInputField = true
            };
            nimbusField.AddToClassList("unity-base-field__aligned");
            nimbusField.RegisterCallback((ChangeEvent<float> evt) =>
            {
                WeatherSphere.nimbus = evt.newValue;
            });
            currentSettings.Add(nimbusField);

            Slider borderField = new Slider("Border Clouds")
            {
                value = WeatherSphere.borderHeight,
                lowValue = 0,
                highValue = 2,
                showInputField = true
            };
            borderField.AddToClassList("unity-base-field__aligned");
            borderField.RegisterCallback((ChangeEvent<float> evt) =>
            {
                WeatherSphere.borderHeight = evt.newValue;
            });
            currentSettings.Add(borderField);

            ContentContainer.Add(currentSettings);

        }

        public void RenderCelestialsSettings()
        {
            ContentContainer.Clear();
            ContentLabel.text = "Celestials and VFX";

            Label sunSettingsLabel = new Label("Sun Settings");
            sunSettingsLabel.AddToClassList("h2");
            sunSettingsLabel.AddToClassList("mb-md");
            ContentContainer.Add(sunSettingsLabel);

            VariablePropertyField sunColorField = new VariablePropertyField(serializedObject.FindProperty("sunColor"), true);
            ContentContainer.Add(sunColorField);

            VariablePropertyField sunSizeField = new VariablePropertyField(serializedObject.FindProperty("sunSize"), 0, 5);
            ContentContainer.Add(sunSizeField);

            VariablePropertyField sunDirectionField = new VariablePropertyField(serializedObject.FindProperty("sunDirection"), 0, 360);
            ContentContainer.Add(sunDirectionField);

            VariablePropertyField sunPitchField = new VariablePropertyField(serializedObject.FindProperty("sunPitch"), -90, 90);
            ContentContainer.Add(sunPitchField);

            VariablePropertyField sunFalloffField = new VariablePropertyField(serializedObject.FindProperty("sunFalloff"), 0, 1);
            sunFalloffField.Label.text = "Sun Halo Falloff";
            ContentContainer.Add(sunFalloffField);

            VariablePropertyField sunFlareColorField = new VariablePropertyField(serializedObject.FindProperty("sunFlareColor"), true);
            sunFlareColorField.Label.text = "Sun Halo Color";
            ContentContainer.Add(sunFlareColorField);

#if COZY_URP || COZY_HDRP
            PropertyField sunFlare = new PropertyField();
            sunFlare.BindProperty(serializedObject.FindProperty("sunFlare"));
            sunFlare.AddToClassList("mb-md");
            sunFlare.AddToClassList("pl-4");
            ContentContainer.Add(sunFlare);
#endif

            Label moonSettingsLabel = new Label("Moon Settings");
            moonSettingsLabel.AddToClassList("h2");
            moonSettingsLabel.AddToClassList("mb-md");
            ContentContainer.Add(moonSettingsLabel);

            VariablePropertyField moonColorField = new VariablePropertyField(serializedObject.FindProperty("moonColor"), true);
            ContentContainer.Add(moonColorField);

            VariablePropertyField moonFalloffField = new VariablePropertyField(serializedObject.FindProperty("moonFalloff"), 0, 1);
            ContentContainer.Add(moonFalloffField);

            VariablePropertyField moonFlareColorField = new VariablePropertyField(serializedObject.FindProperty("moonFlareColor"), true);
            ContentContainer.Add(moonFlareColorField);

#if COZY_URP || COZY_HDRP
            PropertyField moonFlare = new PropertyField();
            moonFlare.BindProperty(serializedObject.FindProperty("moonFlare"));
            moonFlare.AddToClassList("pl-4");
            moonFlare.AddToClassList("mb-md");
            ContentContainer.Add(moonFlare);
#endif


            Label celestialSettingsLabel = new Label("Sky Effects");
            celestialSettingsLabel.AddToClassList("h2");
            celestialSettingsLabel.AddToClassList("mb-md");
            ContentContainer.Add(celestialSettingsLabel);

            VariablePropertyField starColor = new VariablePropertyField(serializedObject.FindProperty("starColor"), true);
            ContentContainer.Add(starColor);

            PropertyField starMap = new PropertyField();
            starMap.BindProperty(serializedObject.FindProperty("starDomeTexture"));
            starMap.AddToClassList("mb-md");
            ContentContainer.Add(starMap);

            VariablePropertyField constellationIntensity = new VariablePropertyField(serializedObject.FindProperty("constellationIntensity"), 0, 1);
            ContentContainer.Add(constellationIntensity);

            PropertyField constellationDomeTexture = new PropertyField();
            constellationDomeTexture.BindProperty(serializedObject.FindProperty("constellationDomeTexture"));
            constellationDomeTexture.AddToClassList("mb-md");
            ContentContainer.Add(constellationDomeTexture);


            VariablePropertyField galaxyIntensity = new VariablePropertyField(serializedObject.FindProperty("galaxyIntensity"), 0, 1);
            ContentContainer.Add(galaxyIntensity);

            VariablePropertyField galaxy1ColorField = new VariablePropertyField(serializedObject.FindProperty("galaxy1Color"), true);
            ContentContainer.Add(galaxy1ColorField);

            VariablePropertyField galaxy2ColorField = new VariablePropertyField(serializedObject.FindProperty("galaxy2Color"), true);
            ContentContainer.Add(galaxy2ColorField);

            VariablePropertyField galaxy3ColorField = new VariablePropertyField(serializedObject.FindProperty("galaxy3Color"), true);
            ContentContainer.Add(galaxy3ColorField);

            PropertyField galaxyMap = new PropertyField();
            galaxyMap.BindProperty(serializedObject.FindProperty("galaxyDomeTexture"));
            ContentContainer.Add(galaxyMap);


            // PropertyField galaxyStarMap = new PropertyField();
            // galaxyStarMap.BindProperty(serializedObject.FindProperty("galaxyStarMap"));
            // ContentContainer.Add(galaxyStarMap);

            PropertyField galaxyVariationMap = new PropertyField();
            galaxyVariationMap.BindProperty(serializedObject.FindProperty("galaxyVariationMap"));
            galaxyVariationMap.AddToClassList("mb-md");
            ContentContainer.Add(galaxyVariationMap);

            VariablePropertyField lightScatteringColorField = new VariablePropertyField(serializedObject.FindProperty("lightScatteringColor"), true);
            ContentContainer.Add(lightScatteringColorField);

            PropertyField lightScatteringMap = new PropertyField();
            lightScatteringMap.BindProperty(serializedObject.FindProperty("lightScatteringMap"));
            ContentContainer.Add(lightScatteringMap);

            VariablePropertyField lightScatteringPosition = new VariablePropertyField(serializedObject.FindProperty("lightScatteringPosition"), -1, 1);
            ContentContainer.Add(lightScatteringPosition);

            VariablePropertyField lightScatteringHeight = new VariablePropertyField(serializedObject.FindProperty("lightScatteringHeight"), 0, 1);
            lightScatteringHeight.AddToClassList("mb-md");
            ContentContainer.Add(lightScatteringHeight);

            PropertyField useRainbow = new PropertyField();
            useRainbow.BindProperty(serializedObject.FindProperty("useRainbow"));
            ContentContainer.Add(useRainbow);

            PropertyField rainbowTexture = new PropertyField();
            rainbowTexture.BindProperty(serializedObject.FindProperty("rainbowTexture"));
            ContentContainer.Add(rainbowTexture);

            VariablePropertyField rainbowPositionField = new VariablePropertyField(serializedObject.FindProperty("rainbowPosition"), 0, 100);
            ContentContainer.Add(rainbowPositionField);

            VariablePropertyField rainbowWidthField = new VariablePropertyField(serializedObject.FindProperty("rainbowWidth"), 0, 50);
            ContentContainer.Add(rainbowWidthField);



        }

        public void SetVisible(VisualElement element, bool condition)
        {
            element.style.display = condition ? DisplayStyle.Flex : DisplayStyle.None;
        }

    }
}