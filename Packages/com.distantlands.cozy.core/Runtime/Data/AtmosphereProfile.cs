//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using UnityEngine;
using UnityEngine.Rendering;



namespace DistantLands.Cozy.Data
{

    [System.Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/Atmosphere Profile", order = 361)]
    public class AtmosphereProfile : CozyProfile
    {


        [Tooltip("Sets the color of the zenith (or top) of the skybox at a certain time. Starts and ends at midnight.")]
        [CozyPropertyType(true)]
        [CozySearchable]
        public VariableProperty skyZenithColor;
        [Tooltip("Sets the color of the horizon (or middle) of the skybox at a certain time. Starts and ends at midnight.")]
        [CozyPropertyType(true)]
        [CozySearchable]
        public VariableProperty skyHorizonColor;

        [Tooltip("Sets the main color of the clouds at a certain time. Starts and ends at midnight.")]
        [CozyPropertyType(true)]
        [CozySearchable]
        public VariableProperty cloudColor;
        [Tooltip("Sets the highlight color of the clouds at a certain time. Starts and ends at midnight.")]
        [CozyPropertyType(true)]
        [CozySearchable]
        public VariableProperty cloudHighlightColor;
        [Tooltip("Sets the color of the high altitude clouds at a certain time. Starts and ends at midnight.")]
        [CozyPropertyType(true)]
        [CozySearchable]
        public VariableProperty highAltitudeCloudColor;
        [Tooltip("Sets the color of the sun light source at a certain time. Starts and ends at midnight.")]
        [CozyPropertyType(true)]
        [CozySearchable]
        public VariableProperty sunlightColor;
        public LightShadows sunlightShadows = LightShadows.Soft;
        public LightShadows moonlightShadows = LightShadows.Soft;
        [Tooltip("Sets the color of the moon light source at a certain time. Starts and ends at midnight.")]
        [CozyPropertyType(true)]
        [CozySearchable]
        public VariableProperty moonlightColor;
        [Tooltip("Sets the color of the star particle FX and textures at a certain time. Starts and ends at midnight.")]
        [CozyPropertyType(true)]
        [CozySearchable]        
        public VariableProperty starColor;
        [Tooltip("Sets the color of the zenith (or top) of the ambient scene lighting at a certain time. Starts and ends at midnight.")]
        [CozyPropertyType(true)]
        [CozySearchable]
        public VariableProperty ambientLightHorizonColor;
        [Tooltip("Sets the color of the horizon (or middle) of the ambient scene lighting at a certain time. Starts and ends at midnight.")]
        [CozyPropertyType(true)]
        [CozySearchable]
        public VariableProperty ambientLightZenithColor;
        [Tooltip("Multiplies the ambient light intensity.")]
        [CozyPropertyType(false, 0, 4)]
        [CozySearchable]
        public VariableProperty ambientLightMultiplier;
        [Tooltip("Sets the intensity of the galaxy effects at a certain time. Starts and ends at midnight.")]
        [CozyPropertyType(false, 0, 1)]
        [CozySearchable]
        public VariableProperty galaxyIntensity;


        [CozyPropertyType(true)]
        [CozySearchable]
        [Tooltip("Sets the fog color from 0m away from the camera to fog start 1.")]
        public VariableProperty fogColor1;
        [CozySearchable]
        [CozyPropertyType(true)]
        [Tooltip("Sets the fog color from fog start 1 to fog start 2.")]
        public VariableProperty fogColor2;
        [CozySearchable]
        [Tooltip("Sets the fog color from fog start 2 to fog start 3.")]
        [CozyPropertyType(true)]
        public VariableProperty fogColor3;
        [CozySearchable]
        [Tooltip("Sets the fog color from fog start 3 to fog start 4.")]
        [CozyPropertyType(true)]
        public VariableProperty fogColor4;
        [CozySearchable]
        [Tooltip("Sets the fog color from fog start 4 to fog start 5.")]
        [CozyPropertyType(true)]
        public VariableProperty fogColor5;
        [CozySearchable]
        [CozyPropertyType(true)]
        [Tooltip("Sets the color of the fog flare.")]
        public VariableProperty fogFlareColor;
        [CozySearchable]
        [CozyPropertyType(true)]
        [Tooltip("Sets the color of the moon flare for the fog.")]
        public VariableProperty fogMoonFlareColor;
        [CozySearchable]
        [CozyPropertyType(false, 0, 1)]
        [Tooltip("Sets the smoothness of the fog.")]
        public VariableProperty fogSmoothness;

        [CozySearchable]
        public Vector3 fogVariationDirection;
        [CozyPropertyType(false, 0, 30)]
        [Tooltip("Sets the variation scale of the fog.")]
        [CozySearchable]
        public VariableProperty fogVariationScale;
        [CozySearchable]
        [CozyPropertyType(false, 0, 1)]
        [Tooltip("Sets the variation amount.")]
        public VariableProperty fogVariationAmount;
        [CozySearchable]
        [Tooltip("Sets the variation distance of the fog.")]
        [CozyPropertyType(false, 0, 200)]
        public VariableProperty fogVariationDistance;


        [CozyPropertyType(false, 0, 1)]
        [CozySearchable]
        public VariableProperty heightFogIntensity;

        [CozyPropertyType(false, 100, 1000)]
        [CozySearchable]
        public VariableProperty heightFogVariationScale;

        [CozyPropertyType(false, 0, 50)]
        [CozySearchable]
        public VariableProperty heightFogVariationAmount;

        [CozyPropertyType(false)]
        [CozySearchable]
        public VariableProperty fogBase;

        [CozyPropertyType(false, 0, 500)]
        [CozySearchable]
        public VariableProperty heightFogTransition;

        [CozyPropertyType(false, 0, 5000)]
        [CozySearchable]
        public VariableProperty heightFogDistance;

        [CozyPropertyType(true)]
        [CozySearchable]
        public VariableProperty heightFogColor;




        [CozyPropertyType(false, 0, 1)]
        [CozySearchable]
        [Tooltip("Controls the exponent used to modulate from the horizon color to the zenith color of the sky.")]
        public VariableProperty gradientExponent;
        [CozyPropertyType(false, 0, 5)]
        [CozySearchable]
        [Tooltip("Sets the size of the visual sun in the sky.")]
        public VariableProperty sunSize;
        [Tooltip("Sets the world space direction of the sun in degrees.")]
        [CozyPropertyType(false, 0, 360)]
        [CozySearchable]
        public VariableProperty sunDirection;
        [Tooltip("Sets the roll value of the sun's rotation. Allows the sun to be slightly off from directly overhead at noon.")]
        [CozyPropertyType(false, -90, 90)]
        [CozySearchable]
        public VariableProperty sunPitch;
        [CozySearchable]
        [Tooltip("Sets the color of the visual sun in the sky.")]
        [CozyPropertyType(true)]
        public VariableProperty sunColor;
        [CozySearchable]
        [Tooltip("Sets the color of the visual moon in the sky (only impacts the global shader variable for the stylized moon material).")]
        [CozyPropertyType(true)]
        public VariableProperty moonColor;


        [CozyPropertyType(false, 0, 1)]
        [CozySearchable]
        [Tooltip("Sets the falloff of the halo around the visual sun.")]
        public VariableProperty sunFalloff;
        [CozyPropertyType(true)]
        [CozySearchable]
        [Tooltip("Sets the color of the halo around the visual sun.")]
        public VariableProperty sunFlareColor;
        [CozyPropertyType(false, 0, 1)]
        [CozySearchable]
        [Tooltip("Sets the falloff of the halo around the main moon.")]
        public VariableProperty moonFalloff;
        [CozyPropertyType(true)]
        [CozySearchable]
        [Tooltip("Sets the color of the halo around the main moon.")]
        public VariableProperty moonFlareColor;
        [CozyPropertyType(true)]
        [CozySearchable]
        [Tooltip("Sets the color of the first galaxy algorithm.")]
        public VariableProperty galaxy1Color;
        [CozyPropertyType(true)]
        [CozySearchable]
        [Tooltip("Sets the color of the second galaxy algorithm.")]
        public VariableProperty galaxy2Color;
        [CozyPropertyType(true)]
        [CozySearchable]
        [Tooltip("Sets the color of the third galaxy algorithm.")]
        public VariableProperty galaxy3Color;
        [CozyPropertyType(true)]
        [CozySearchable]
        [Tooltip("Sets the color of the light columns around the horizon.")]
        public VariableProperty lightScatteringColor;
        [CozyPropertyType(false, 0, 1)]
        [CozySearchable]
        [Tooltip("Sets the position of the light columns around the horizon.")]
        public VariableProperty lightScatteringPosition;
        [CozyPropertyType(false, 0, 1)]
        [CozySearchable]
        [Tooltip("Sets the position of the light columns around the horizon.")]
        public VariableProperty lightScatteringHeight;
        [CozyPropertyType(false, 0, 1)]
        [CozySearchable]
        [Tooltip("Sets the brightness of constellation lines in the night sky.")]
        public VariableProperty constellationIntensity;
        [Tooltip("Should COZY use a rainbow?")]
        [CozySearchable]
        public bool useRainbow = true;
        public Texture rainbowTexture;
        [Tooltip("Sets the position of the rainbow in the sky.")]
        [CozyPropertyType(false, 0, 100)]
        [CozySearchable]
        public VariableProperty rainbowPosition;
        [Tooltip("Sets the width of the rainbow in the sky.")]
        [CozySearchable]
        [CozyPropertyType(false, 0, 50)]
        public VariableProperty rainbowWidth;


        [CozyPropertyType(false, 0, 5)]
        [CozySearchable]
        [Tooltip("Multiplies the world space distance before entering the fog algorithm. Use this for simple density changes.")]
        public VariableProperty fogDensityMultiplier;

        [Tooltip("Sets the distance at which the first fog color fades into the second fog color.")]
        [CozySearchable]
        public VariableProperty fogStart1 = new VariableProperty() { floatVal = 5};
        [Tooltip("Sets the distance at which the second fog color fades into the third fog color.")]
        [CozySearchable]
        public VariableProperty fogStart2  = new VariableProperty() { floatVal = 12};
        [Tooltip("Sets the distance at which the third fog color fades into the fourth fog color.")]
        [CozySearchable]
        public VariableProperty fogStart3 = new VariableProperty() { floatVal = 20};
        [Tooltip("Sets the distance at which the fourth fog color fades into the fifth fog color.")]
        [CozySearchable]
        public VariableProperty fogStart4 = new VariableProperty() { floatVal = 35};
        [CozyPropertyType(false, 0, 2)]
        [CozySearchable]
        public VariableProperty fogHeight;
        [CozyPropertyType(false, 0, 2)]
        [CozySearchable]
        public VariableProperty fogLightFlareIntensity;
        [CozyPropertyType(false, 0, 40)]
        [CozySearchable]
        public VariableProperty fogLightFlareFalloff;
        [CozyPropertyType(false, 0, 10)]
        [CozySearchable]
        [Tooltip("Sets the height divisor for the fog flare. High values sit the flare closer to the horizon, small values extend the flare into the sky.")]
        public VariableProperty fogLightFlareSquish;

        [CozyPropertyType(true)]
        [CozySearchable]
        public VariableProperty cloudMoonColor;
        [CozyPropertyType(false, 0, 50)]
        [CozySearchable]
        public VariableProperty cloudSunHighlightFalloff;
        [CozyPropertyType(false, 0, 50)]
        [CozySearchable]
        public VariableProperty cloudMoonHighlightFalloff;
        [CozyPropertyType(false, 0, 10)]
        [CozySearchable]
        public VariableProperty cloudWindSpeed;
        [CozyPropertyType(false, 0, 1)]
        [CozySearchable]
        public VariableProperty clippingThreshold;
        [CozyPropertyType(false, 2, 60)]
        [CozySearchable]
        public VariableProperty cloudMainScale;
        [CozyPropertyType(false, 0.2f, 10)]
        [CozySearchable]
        public VariableProperty cloudDetailScale;
        [CozyPropertyType(false, 0, 30)]
        [CozySearchable]
        public VariableProperty cloudDetailAmount;
        [CozyPropertyType(false, 0.1f, 3)]
        [CozySearchable]
        public VariableProperty acScale;
        [CozyPropertyType(false, 0, 3)]
        [CozySearchable]
        public VariableProperty cirroMoveSpeed;
        [CozyPropertyType(false, 0, 3)]
        [CozySearchable]
        public VariableProperty cirrusMoveSpeed;
        [CozyPropertyType(false, 0, 3)]
        [CozySearchable]
        public VariableProperty chemtrailsMoveSpeed;

//TEXTURES

        [CozySearchable]
        public Texture cloudTexture;
        [CozySearchable]
        public Texture chemtrailsTexture;
        [CozySearchable]
        public Texture cirrusCloudTexture;
        [CozySearchable]
        public Texture cirrostratusCloudTexture;
        [CozySearchable]
        public Texture altocumulusCloudTexture;
        [CozySearchable]
        public Texture starMap;
        [CozySearchable]
        public Texture starDomeTexture;
        [CozySearchable]
        public Texture galaxyMap;
        [CozySearchable]
        public Texture galaxyDomeTexture;
        [CozySearchable]
        public Texture constellationDomeTexture;
        [CozySearchable]
        public Texture galaxyStarMap;
        [CozySearchable]
        public Texture galaxyVariationMap;
        [CozySearchable]
        public Texture lightScatteringMap;

//LUXURY CLOUDS

        [CozySearchable]
        public Texture partlyCloudyLuxuryClouds;
        [CozySearchable]
        public Texture mostlyCloudyLuxuryClouds;
        [CozySearchable]
        public Texture overcastLuxuryClouds;
        [CozySearchable]
        public Texture lowBorderLuxuryClouds;
        [CozySearchable]
        public Texture highBorderLuxuryClouds;
        [CozySearchable]
        public Texture lowNimbusLuxuryClouds;
        [CozySearchable]
        public Texture midNimbusLuxuryClouds;
        [CozySearchable]
        public Texture highNimbusLuxuryClouds;
        [CozySearchable]
        public Texture luxuryVariation;

//ADDITIONAL CLOUD PROPERTIES

        [CozyPropertyType(true)]
        [CozySearchable]
        public VariableProperty cloudTextureColor;
        [CozyPropertyType(false, 0, 10)]
        [CozySearchable]
        public VariableProperty cloudCohesion;
        [CozyPropertyType(false, 0, 1)]
        [CozySearchable]
        public VariableProperty spherize;
        [CozyPropertyType(false, 0, 10)]
        [CozySearchable]
        public VariableProperty shadowDistance;
        [CozyPropertyType(false, 0, 4)]
        [CozySearchable]
        public VariableProperty cloudThickness;
        [CozyPropertyType(false, 0, 3)]
        [CozySearchable]
        public VariableProperty textureAmount;
        [CozySearchable]
        public Vector3 texturePanDirection;

//FLARES

#if COZY_URP || COZY_HDRP
        [System.Serializable]
        public class SRPFlare
        {
            public LensFlareDataSRP flare;
            public float intensity = 1;
            public float scale = 1;
            public AnimationCurve screenAttenuation;
            public bool useOcclusion = true;
            public float occlusionRadius = 0.5f;
            public bool allowOffscreen = true;
        }
        public SRPFlare sunFlare;
        public SRPFlare moonFlare;
#endif

//LAYERS

        [CozyPropertyType(false, 0, 1)]
        [CozySearchable]
        public VariableProperty skyFogAmount;
        [CozyPropertyType(false, 0, 1)]
        [CozySearchable]
        public VariableProperty cloudsFogAmount;
        [CozyPropertyType(false, 0, 1)]
        [CozySearchable]
        public VariableProperty cloudsFogLightAmount;

    }


}