﻿/*********************************/
/*         LIQUID VOLUME         */
/*       Created by Kronnect     */
/*********************************/

// Enable floating point render textures
#define LIQUID_VOLUME_FP_RENDER_TEXTURES

// Displays sliced liquid volume
//#define DEBUG_SLICE

using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LiquidVolumeFX {

    public delegate void PropertiesChangedEvent(LiquidVolume lv);

    public enum TOPOLOGY {
        Sphere = 0,
        Cylinder = 1,
        Cube = 2,
        Irregular = 10
    }

    public enum DETAIL {
        Simple = 0,
        SimpleNoFlask = 1,
        Default = 10,
        DefaultNoFlask = 11
    }

    public enum LEVEL_COMPENSATION {
        None = 0,
        Fast = 10,
        Accurate = 20
    }


    public static class DetailExtensions {

        public static bool allowsRefraction(this DETAIL detail) {
            return detail != DETAIL.DefaultNoFlask;
        }

        public static bool usesFlask(this DETAIL detail) {
            return detail == DETAIL.Simple || detail == DETAIL.Default;
        }

    }


    [ExecuteInEditMode]
    [HelpURL("https://kronnect.com/support")]
    [AddComponentMenu("Effects/Liquid Volume")]
    [DisallowMultipleComponent]
    public partial class LiquidVolume : MonoBehaviour {

        public static bool FORCE_GLES_COMPATIBILITY = false;

        public event PropertiesChangedEvent onPropertiesChanged;


        [SerializeField]
        TOPOLOGY _topology = TOPOLOGY.Sphere;

        public TOPOLOGY topology {
            get { return _topology; }
            set {
                if (_topology != value) {
                    _topology = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        DETAIL _detail = DETAIL.Default;

        public DETAIL detail {
            get { return _detail; }
            set {
                if (_detail != value) {
                    _detail = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        float _level = 0.5f;

        public float level {
            get { return _level; }
            set {
                if (_level != value) {
                    _level = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        [Range(0, 1)]
        float _levelMultiplier = 1f;

        public float levelMultiplier {
            get { return _levelMultiplier; }
            set {
                if (_levelMultiplier != value) {
                    _levelMultiplier = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Tooltip("Uses directional light color")]
        bool _useLightColor;

        public bool useLightColor {
            get { return _useLightColor; }
            set {
                if (_useLightColor != value) {
                    _useLightColor = value;
                    UpdateMaterialProperties();
                }
            }
        }



        [SerializeField]
        [Tooltip("Uses directional light direction for day/night cycle")]
        bool _useLightDirection;

        public bool useLightDirection {
            get { return _useLightDirection; }
            set {
                if (_useLightDirection != value) {
                    _useLightDirection = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Light _directionalLight;

        public Light directionalLight {
            get { return _directionalLight; }
            set {
                if (_directionalLight != value) {
                    _directionalLight = value;
                    UpdateMaterialProperties();
                }
            }
        }



        [SerializeField]
        [ColorUsage(true)]
        Color _liquidColor1 = new Color(0, 1, 0, 0.1f);

        public Color liquidColor1 {
            get { return _liquidColor1; }
            set {
                if (_liquidColor1 != value) {
                    _liquidColor1 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0.1f, 4.85f)]
        float _liquidScale1 = 1f;

        public float liquidScale1 {
            get { return _liquidScale1; }
            set {
                if (_liquidScale1 != value) {
                    _liquidScale1 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [ColorUsage(true)]
        Color _liquidColor2 = new Color(1, 0, 0, 0.3f);

        public Color liquidColor2 {
            get { return _liquidColor2; }
            set {
                if (_liquidColor2 != value) {
                    _liquidColor2 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(2f, 4.85f)]
        float _liquidScale2 = 5f;

        public float liquidScale2 {
            get { return _liquidScale2; }
            set {
                if (_liquidScale2 != value) {
                    _liquidScale2 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        float _alpha = 1f;

        public float alpha {
            get { return _alpha; }
            set {
                if (_alpha != Mathf.Clamp01(value)) {
                    _alpha = Mathf.Clamp01(value);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [ColorUsage(true)]
        Color _emissionColor = new Color(0, 0, 0);

        public Color emissionColor {
            get { return _emissionColor; }
            set {
                if (_emissionColor != value) {
                    _emissionColor = value;
                    UpdateMaterialProperties();
                }
            }
        }
        [SerializeField]
        bool _ditherShadows = true;

        public bool ditherShadows {
            get { return _ditherShadows; }
            set {
                if (_ditherShadows != value) {
                    _ditherShadows = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        float _murkiness = 1.0f;

        public float murkiness {
            get { return _murkiness; }
            set {
                if (_murkiness != value) {
                    _murkiness = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1f)]
        float _turbulence1 = 0.5f;

        public float turbulence1 {
            get { return _turbulence1; }
            set {
                if (_turbulence1 != value) {
                    _turbulence1 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1f)]
        float _turbulence2 = 0.2f;

        public float turbulence2 {
            get { return _turbulence2; }
            set {
                if (_turbulence2 != value) {
                    _turbulence2 = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        float _frecuency = 1f;

        public float frecuency {
            get { return _frecuency; }
            set {
                if (_frecuency != value) {
                    _frecuency = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0f, 2f)]
        float _speed = 1f;

        public float speed {
            get { return _speed; }
            set {
                if (_speed != value) {
                    _speed = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 5f)]
        float _sparklingIntensity = 0.1f;

        public float sparklingIntensity {
            get { return _sparklingIntensity; }
            set {
                if (_sparklingIntensity != value) {
                    _sparklingIntensity = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        float _sparklingAmount = 0.2f;

        public float sparklingAmount {
            get { return _sparklingAmount; }
            set {
                if (_sparklingAmount != value) {
                    _sparklingAmount = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 10)]
        float _deepObscurance = 2.0f;

        public float deepObscurance {
            get { return _deepObscurance; }
            set {
                if (_deepObscurance != value) {
                    _deepObscurance = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [ColorUsage(true)]
        Color _foamColor = new Color(1, 1, 1, 0.65f);

        public Color foamColor {
            get { return _foamColor; }
            set {
                if (_foamColor != value) {
                    _foamColor = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0.01f, 1f)]
        float _foamScale = 0.2f;

        public float foamScale {
            get { return _foamScale; }
            set {
                if (_foamScale != value) {
                    _foamScale = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 0.1f)]
        float _foamThickness = 0.04f;

        public float foamThickness {
            get { return _foamThickness; }
            set {
                if (_foamThickness != value) {
                    _foamThickness = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(-1, 1)]
        float _foamDensity = 0.5f;

        public float foamDensity {
            get { return _foamDensity; }
            set {
                if (_foamDensity != value) {
                    _foamDensity = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(4, 100)]
        float _foamWeight = 10f;

        public float foamWeight {
            get { return _foamWeight; }
            set {
                if (_foamWeight != value) {
                    _foamWeight = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        float _foamTurbulence = 1f;

        public float foamTurbulence {
            get { return _foamTurbulence; }
            set {
                if (_foamTurbulence != value) {
                    _foamTurbulence = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool _foamVisibleFromBottom = true;

        public bool foamVisibleFromBottom {
            get { return _foamVisibleFromBottom; }
            set {
                if (_foamVisibleFromBottom != value) {
                    _foamVisibleFromBottom = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool _smokeEnabled = true;

        public bool smokeEnabled {
            get { return _smokeEnabled; }
            set {
                if (_smokeEnabled != value) {
                    _smokeEnabled = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [ColorUsage(true)]
        [SerializeField]
        Color _smokeColor = new Color(0.7f, 0.7f, 0.7f, 0.25f);

        public Color smokeColor {
            get { return _smokeColor; }
            set {
                if (_smokeColor != value) {
                    _smokeColor = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0.01f, 1f)]
        float _smokeScale = 0.25f;

        public float smokeScale {
            get { return _smokeScale; }
            set {
                if (_smokeScale != value) {
                    _smokeScale = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 10f)]
        float _smokeBaseObscurance = 2.0f;

        public float smokeBaseObscurance {
            get { return _smokeBaseObscurance; }
            set {
                if (_smokeBaseObscurance != value) {
                    _smokeBaseObscurance = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        [Range(0, 10f)]
        float _smokeHeightAtten;

        public float smokeHeightAtten {
            get { return _smokeHeightAtten; }
            set {
                if (_smokeHeightAtten != value) {
                    _smokeHeightAtten = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        [Range(0, 20f)]
        float _smokeSpeed = 5.0f;

        public float smokeSpeed {
            get { return _smokeSpeed; }
            set {
                if (_smokeSpeed != value) {
                    _smokeSpeed = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool _fixMesh;

        public bool fixMesh {
            get { return _fixMesh; }
            set {
                if (_fixMesh != value) {
                    _fixMesh = value;
                    UpdateMaterialProperties();
                }
            }
        }

        public Mesh originalMesh;
        public Vector3 originalPivotOffset;

        [SerializeField]
        Vector3 _pivotOffset;

        public Vector3 pivotOffset {
            get { return _pivotOffset; }
            set {
                if (_pivotOffset != value) {
                    _pivotOffset = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool _limitVerticalRange;
        public bool limitVerticalRange {
            get { return _limitVerticalRange; }
            set {
                if (_limitVerticalRange != value) {
                    _limitVerticalRange = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        [Range(0, 1.5f)]
        float _upperLimit = 1.5f;

        public float upperLimit {
            get { return _upperLimit; }
            set {
                if (_upperLimit != value) {
                    _upperLimit = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(-1.5f, 1.5f)]
        float _lowerLimit = -1.5f;

        public float lowerLimit {
            get { return _lowerLimit; }
            set {
                if (_lowerLimit != value) {
                    _lowerLimit = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        int _subMeshIndex = -1;

        public int subMeshIndex {
            get { return _subMeshIndex; }
            set {
                if (_subMeshIndex != value) {
                    _subMeshIndex = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        Material _flaskMaterial;

        public Material flaskMaterial {
            get { return _flaskMaterial; }
            set {
                if (_flaskMaterial != value) {
                    _flaskMaterial = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        float _flaskThickness = 0.03f;

        public float flaskThickness {
            get { return _flaskThickness; }
            set {
                if (_flaskThickness != value) {
                    _flaskThickness = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        float _glossinessInternal = 0.3f;

        public float glossinessInternal {
            get { return _glossinessInternal; }
            set {
                if (_glossinessInternal != value) {
                    _glossinessInternal = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool _scatteringEnabled = false;

        public bool scatteringEnabled {
            get { return _scatteringEnabled; }
            set {
                if (_scatteringEnabled != value) {
                    _scatteringEnabled = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(1, 16)]
        int _scatteringPower = 5;

        public int scatteringPower {
            get { return _scatteringPower; }
            set {
                if (_scatteringPower != value) {
                    _scatteringPower = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 10)]
        float _scatteringAmount = 0.3f;

        public float scatteringAmount {
            get { return _scatteringAmount; }
            set {
                if (_scatteringAmount != value) {
                    _scatteringAmount = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool _refractionBlur = true;

        public bool refractionBlur {
            get { return _refractionBlur; }
            set {
                if (_refractionBlur != value) {
                    _refractionBlur = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1)]
        float _blurIntensity = 0.75f;

        public float blurIntensity {
            get { return _blurIntensity; }
            set {
                if (_blurIntensity != Mathf.Clamp01(value)) {
                    _blurIntensity = Mathf.Clamp01(value);
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        int _liquidRaySteps = 10;

        public int liquidRaySteps {
            get { return _liquidRaySteps; }
            set {
                if (_liquidRaySteps != value) {
                    _liquidRaySteps = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        int _foamRaySteps = 7;

        public int foamRaySteps {
            get { return _foamRaySteps; }
            set {
                if (_foamRaySteps != value) {
                    _foamRaySteps = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        int _smokeRaySteps = 5;

        public int smokeRaySteps {
            get { return _smokeRaySteps; }
            set {
                if (_smokeRaySteps != value) {
                    _smokeRaySteps = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Texture2D _bumpMap;

        public Texture2D bumpMap {
            get { return _bumpMap; }
            set {
                if (_bumpMap != value) {
                    _bumpMap = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 1f)]
        float _bumpStrength = 1f;

        public float bumpStrength {
            get { return _bumpStrength; }
            set {
                if (_bumpStrength != value) {
                    _bumpStrength = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 10f)]
        float _bumpDistortionScale = 1f;

        public float bumpDistortionScale {
            get { return _bumpDistortionScale; }
            set {
                if (_bumpDistortionScale != value) {
                    _bumpDistortionScale = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Vector2 _bumpDistortionOffset;

        public Vector2 bumpDistortionOffset {
            get { return _bumpDistortionOffset; }
            set {
                if (_bumpDistortionOffset != value) {
                    _bumpDistortionOffset = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Texture2D _distortionMap;

        public Texture2D distortionMap {
            get { return _distortionMap; }
            set {
                if (_distortionMap != value) {
                    _distortionMap = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        Texture2D _texture;

        public Texture2D texture {
            get { return _texture; }
            set {
                if (_texture != value) {
                    _texture = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Vector2
            _textureScale = Vector2.one;

        public Vector2 textureScale {
            get { return _textureScale; }
            set {
                if (_textureScale != value) {
                    _textureScale = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Vector2
            _textureOffset;

        public Vector2 textureOffset {
            get { return _textureOffset; }
            set {
                if (_textureOffset != value) {
                    _textureOffset = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0, 10f)]
        float
            _distortionAmount = 1f;

        public float distortionAmount {
            get { return _distortionAmount; }
            set {
                if (_distortionAmount != value) {
                    _distortionAmount = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool _depthAware;

        public bool depthAware {
            get { return _depthAware; }
            set {
                if (_depthAware != value) {
                    _depthAware = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        float _depthAwareOffset;

        public float depthAwareOffset {
            get { return _depthAwareOffset; }
            set {
                if (_depthAwareOffset != value) {
                    _depthAwareOffset = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool _irregularDepthDebug;

        public bool irregularDepthDebug {
            get { return _irregularDepthDebug; }
            set {
                if (_irregularDepthDebug != value) {
                    _irregularDepthDebug = value;
                    UpdateMaterialProperties();
                }
            }
        }



        [SerializeField]
        bool _depthAwareCustomPass;

        public bool depthAwareCustomPass {
            get { return _depthAwareCustomPass; }
            set {
                if (_depthAwareCustomPass != value) {
                    _depthAwareCustomPass = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        bool _depthAwareCustomPassDebug;

        public bool depthAwareCustomPassDebug {
            get { return _depthAwareCustomPassDebug; }
            set {
                if (_depthAwareCustomPassDebug != value) {
                    _depthAwareCustomPassDebug = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        [Range(0, 5f)]
        float _doubleSidedBias;

        public float doubleSidedBias {
            get { return _doubleSidedBias; }
            set {
                if (_doubleSidedBias != value) {
                    _doubleSidedBias = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        float _backDepthBias;

        public float backDepthBias {
            get { return _backDepthBias; }
            set {
                if (value < 0) value = 0;
                if (_backDepthBias != value) {
                    _backDepthBias = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        LEVEL_COMPENSATION _rotationLevelCompensation = LEVEL_COMPENSATION.None;

        public LEVEL_COMPENSATION rotationLevelCompensation {
            get { return _rotationLevelCompensation; }
            set {
                if (_rotationLevelCompensation != value) {
                    _rotationLevelCompensation = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool _ignoreGravity;

        public bool ignoreGravity {
            get { return _ignoreGravity; }
            set {
                if (_ignoreGravity != value) {
                    _ignoreGravity = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        bool _reactToForces;

        public bool reactToForces {
            get { return _reactToForces; }
            set {
                if (_reactToForces != value) {
                    _reactToForces = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        Vector3 _extentsScale = Vector3.one;

        public Vector3 extentsScale {
            get { return _extentsScale; }
            set {
                if (_extentsScale != value) {
                    _extentsScale = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(1, 3)]
        int _noiseVariation = 1;

        public int noiseVariation {
            get { return _noiseVariation; }
            set {
                if (_noiseVariation != value) {
                    _noiseVariation = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        bool _allowViewFromInside = false;

        public bool allowViewFromInside {
            get { return _allowViewFromInside; }
            set {
                if (_allowViewFromInside != value) {
                    _allowViewFromInside = value;
                    lastDistanceToCam = -1;
                    CheckInsideOut();
                }
            }
        }


        [SerializeField]
        bool
            _debugSpillPoint = false;

        public bool debugSpillPoint {
            get { return _debugSpillPoint; }
            set {
                if (_debugSpillPoint != value) {
                    _debugSpillPoint = value;
                }
            }
        }

        [SerializeField]
        int
            _renderQueue = 3001;

        public int renderQueue {
            get { return _renderQueue; }
            set {
                if (_renderQueue != value) {
                    _renderQueue = value;
                    UpdateMaterialProperties();
                }
            }
        }


        [SerializeField]
        Cubemap _reflectionTexture;

        public Cubemap reflectionTexture {
            get { return _reflectionTexture; }
            set {
                if (_reflectionTexture != value) {
                    _reflectionTexture = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0.1f, 5f)]
        float _physicsMass = 1f;

        public float physicsMass {
            get { return _physicsMass; }
            set {
                if (_physicsMass != value) {
                    _physicsMass = value;
                    UpdateMaterialProperties();
                }
            }
        }

        [SerializeField]
        [Range(0.0f, 0.2f)]
        float _physicsAngularDamp = 0.02f;

        public float physicsAngularDamp {
            get { return _physicsAngularDamp; }
            set {
                if (_physicsAngularDamp != value) {
                    _physicsAngularDamp = value;
                    UpdateMaterialProperties();
                }
            }
        }
   
#if LIQUID_VOLUME_FP_RENDER_TEXTURES
        public static bool useFPRenderTextures => true;
#else
        public static bool useFPRenderTextures => false;
#endif

        // ---- INTERNAL CODE ----
        const int SHADER_KEYWORD_DEPTH_AWARE_INDEX = 0;
        const int SHADER_KEYWORD_DEPTH_AWARE_CUSTOM_PASS_INDEX = 1;
        const int SHADER_KEYWORD_IGNORE_GRAVITY_INDEX = 2;
        const int SHADER_KEYWORD_NON_AABB_INDEX = 3;
        const int SHADER_KEYWORD_TOPOLOGY_INDEX = 4;
        const int SHADER_KEYWORD_REFRACTION_INDEX = 5;

        const string SHADER_KEYWORD_DEPTH_AWARE = "LIQUID_VOLUME_DEPTH_AWARE";
        const string SHADER_KEYWORD_DEPTH_AWARE_CUSTOM_PASS = "LIQUID_VOLUME_DEPTH_AWARE_PASS";
        const string SHADER_KEYWORD_NON_AABB = "LIQUID_VOLUME_NON_AABB";
        const string SHADER_KEYWORD_IGNORE_GRAVITY = "LIQUID_VOLUME_IGNORE_GRAVITY";
        const string SHADER_KEYWORD_SPHERE = "LIQUID_VOLUME_SPHERE";
        const string SHADER_KEYWORD_CUBE = "LIQUID_VOLUME_CUBE";
        const string SHADER_KEYWORD_CYLINDER = "LIQUID_VOLUME_CYLINDER";
        const string SHADER_KEYWORD_IRREGULAR = "LIQUID_VOLUME_IRREGULAR";
        const string SHADER_KEYWORD_FP_RENDER_TEXTURE = "LIQUID_VOLUME_FP_RENDER_TEXTURES";
        const string SHADER_KEYWORD_USE_REFRACTION = "LIQUID_VOLUME_USE_REFRACTION";

        const string SPILL_POINT_GIZMO = "SpillPointGizmo";

        [NonSerialized]
        public Material liqMat;
        Material liqMatSimple, liqMatDefaultNoFlask;
        Mesh mesh;
        [NonSerialized]
        public Renderer mr;
        readonly static List<Material> mrSharedMaterials = new List<Material>();
        Vector3 lastPosition, lastScale;
        Quaternion lastRotation;
        string[] shaderKeywords;
        bool camInside;
        float lastDistanceToCam;
        DETAIL currentDetail;
        Vector4 turb, shaderTurb;
        float turbulenceSpeed, murkinessSpeed;
        float liquidLevelPos;
        bool shouldUpdateMaterialProperties;
        int currentNoiseVariation;
        float levelMultipled;

        Texture2D noise3DUnwrapped;
        Texture3D[] noise3DTex;
        Color[][] colors3D;

        // Mesh info
        Vector3[] verticesUnsorted, verticesSorted;
        static Vector3[] rotatedVertices;
        int[] verticesIndices;
        float volumeRef, lastLevelVolumeRef;

        // Physics
        Vector3 inertia, lastAvgVelocity;
        float angularVelocity, angularInertia;
        float turbulenceDueForces;
        Quaternion liquidRot;

        float prevThickness;

        // Spill point debug
        GameObject spillPointGizmo;
        static string[] defaultContainerNames = {
            "GLASS",
            "CONTAINER",
            "BOTTLE",
            "POTION",
            "FLASK",
            "LIQUID"
        };

        // Point light effects
        Color[] pointLightColorBuffer;
        Vector4[] pointLightPositionBuffer;
        int lastPointLightCount;


        #region Gameloop events


        void OnEnable() {
            if (!gameObject.activeInHierarchy)
                return;
            levelMultipled = _level * _levelMultiplier;
            turb.z = 1f;
            turbulenceDueForces = 0f;
            turbulenceSpeed = 1f;
            liquidRot = transform.rotation;
            currentDetail = _detail;
            currentNoiseVariation = -1;
            lastPosition = transform.position;
            lastRotation = transform.rotation;
            lastScale = transform.localScale;
            prevThickness = _flaskThickness;
            if (_depthAwareCustomPass && transform.parent == null) {
                _depthAwareCustomPass = false;
            }
            UpdateMaterialPropertiesNow();
            if (!Application.isPlaying) {
                shouldUpdateMaterialProperties = true;
            }
        }

        void Reset() {
            // Try to assign propert topology based on mesh
            if (mesh == null)
                return;

            if (mesh.vertexCount == 24) {
                topology = TOPOLOGY.Cube;
            } else {
                Renderer renderer = GetComponent<Renderer>();
                if (renderer == null) {
                    if (mesh.bounds.extents.y > mesh.bounds.extents.x) {
                        topology = TOPOLOGY.Cylinder;
                    }
                } else if (renderer.bounds.extents.y > renderer.bounds.extents.x) {
                    topology = TOPOLOGY.Cylinder;
                    if (!Application.isPlaying) {
                        if (transform.rotation.eulerAngles != Vector3.zero && (mesh.bounds.extents.y <= mesh.bounds.extents.x || mesh.bounds.extents.y <= mesh.bounds.extents.z)) {
                            Debug.LogWarning("Intrinsic model rotation detected. Consider using the Bake Transform and/or Center Pivot options in Advanced section.");
                        }
                    }

                }
            }
        }


        void OnDestroy() {

            RestoreOriginalMesh();

            liqMat = null;
            if (liqMatSimple != null) {
                DestroyImmediate(liqMatSimple);
                liqMatSimple = null;
            }
            if (liqMatDefaultNoFlask != null) {
                DestroyImmediate(liqMatDefaultNoFlask);
                liqMatDefaultNoFlask = null;
            }

            if (noise3DTex != null) {
                for (int k = 0; k < noise3DTex.Length; k++) {
                    Texture3D tex = noise3DTex[k];
                    if (tex != null && tex.name.Contains("Clone")) {
                        DestroyImmediate(tex);
                        noise3DTex[k] = null;
                    }
                }
            }

            LiquidVolumeDepthPrePassRenderFeature.RemoveLiquidFromBackRenderers(this);
            LiquidVolumeDepthPrePassRenderFeature.RemoveLiquidFromFrontRenderers(this);

        }


        void RenderObject() {
            var act = gameObject.activeInHierarchy && enabled;

            if (shouldUpdateMaterialProperties || !Application.isPlaying) {
                shouldUpdateMaterialProperties = false;
                UpdateMaterialPropertiesNow();
            }

            if (act && _allowViewFromInside) {
                CheckInsideOut();
            }

            UpdateAnimations();

            if (!act || _topology != TOPOLOGY.Irregular) {
                LiquidVolumeDepthPrePassRenderFeature.RemoveLiquidFromBackRenderers(this);
            } else if (_topology == TOPOLOGY.Irregular) {
                LiquidVolumeDepthPrePassRenderFeature.AddLiquidToBackRenderers(this);
            }

            Transform parent = transform.parent;
            if (parent != null) {
                Renderer parentRenderer = GetComponentInParent<Renderer>();
                if (!act || !_depthAwareCustomPass) {
                    LiquidVolumeDepthPrePassRenderFeature.RemoveLiquidFromFrontRenderers(this);
                } else if (_depthAwareCustomPass) {
                    LiquidVolumeDepthPrePassRenderFeature.AddLiquidToFrontRenderers(this);
                }
            }
            if (_debugSpillPoint) {
                UpdateSpillPointGizmo();
            }

        }

        public void OnWillRenderObject() {
            RenderObject();
        }

        void FixedUpdate() {
            turbulenceSpeed += Time.deltaTime * 3f * _speed;
            liqMat.SetFloat(ShaderParams.TurbulenceSpeed, turbulenceSpeed * 4f);
            murkinessSpeed += Time.deltaTime * 0.05f * (shaderTurb.x + shaderTurb.y);
            liqMat.SetFloat(ShaderParams.MurkinessSpeed, murkinessSpeed);
        }


        void OnDidApplyAnimationProperties() {  // support for animating property based fields
            shouldUpdateMaterialProperties = true;
        }

        #endregion

        #region Internal stuff

        struct MeshCache {
            public Vector3[] verticesSorted;
            public Vector3[] verticesUnsorted;
            public int[] indices;
        }

        static readonly Dictionary<Mesh, MeshCache> meshCache = new Dictionary<Mesh, MeshCache>();

        public void ClearMeshCache() {
            meshCache.Clear();
        }

        void ReadVertices() {
            if (mesh == null)
                return;

            if (!meshCache.TryGetValue(mesh, out MeshCache meshData)) {
                if (!mesh.isReadable) {
                    Debug.LogError("Mesh " + mesh.name + " is not readable. Please select your mesh and enable the Read/Write Enabled option.");
                }

                verticesUnsorted = mesh.vertices;
                verticesIndices = mesh.triangles;

                int vertexCount = verticesUnsorted.Length;

                if (verticesSorted == null || verticesSorted.Length != vertexCount) {
                    verticesSorted = new Vector3[vertexCount];
                }
                Array.Copy(verticesUnsorted, verticesSorted, vertexCount);
                Array.Sort(verticesSorted, vertexComparer);

                meshData.verticesUnsorted = verticesUnsorted;
                meshData.indices = verticesIndices;
                meshData.verticesSorted = verticesSorted;

                if (meshCache.Count > 64) {
                    // Clear cache to avoid memory overrun
                    ClearMeshCache();
                }
                meshCache[mesh] = meshData;
            } else {
                verticesUnsorted = meshData.verticesUnsorted;
                verticesIndices = meshData.indices;
                verticesSorted = meshData.verticesSorted;
            }
        }

        int vertexComparer(Vector3 v0, Vector3 v1) {
            if (v1.y < v0.y) return -1;
            if (v1.y > v0.y) return 1;
            return 0;
        }


        void UpdateAnimations() {
            // Check proper scale
            switch (topology) {
                case TOPOLOGY.Sphere:
                    if (transform.localScale.y != transform.localScale.x || transform.localScale.z != transform.localScale.x)
                        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.x, transform.localScale.x);
                    break;
                case TOPOLOGY.Cylinder:
                    if (transform.localScale.z != transform.localScale.x)
                        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.x);
                    break;
            }


            if (liqMat != null) {
                Vector3 turbDir = Vector3.right;
                Quaternion rot = transform.rotation;
                if (_reactToForces) {
                    Quaternion instantRot = transform.rotation;
                    float dt = Time.deltaTime;
                    if (Application.isPlaying && dt > 0) {
                        Vector3 instantVelocity = (transform.position - lastPosition) / dt;

                        Vector3 instantAccel = (instantVelocity - lastAvgVelocity);
                        lastAvgVelocity = instantVelocity;
                        inertia += instantVelocity;

                        float accelMag = instantAccel.magnitude;
                        float force = Mathf.Max(accelMag / _physicsMass - _physicsAngularDamp * 150 * dt, 0f);
                        angularInertia += force;
                        angularVelocity += angularInertia;
                        if (angularVelocity > 0) {
                            angularInertia -= Mathf.Abs(angularVelocity) * dt * _physicsMass;
                        } else if (angularVelocity < 0) {
                            angularInertia += Mathf.Abs(angularVelocity) * dt * _physicsMass;
                        }
                        float damp = 1f - _physicsAngularDamp;
                        angularInertia *= damp;
                        inertia *= damp;

                        float mag = Mathf.Clamp(angularVelocity, -90f, 90f);
                        float inertiaMag = inertia.magnitude;
                        if (inertiaMag > 0) {
                            turbDir = inertia / inertiaMag;
                        }
                        Vector3 axis = Vector3.Cross(turbDir, Vector3.down);
                        instantRot = Quaternion.AngleAxis(mag, axis);

                        float cinematic = Mathf.Abs(angularInertia) + Mathf.Abs(angularVelocity);
                        turbulenceDueForces = Mathf.Min(0.5f / _physicsMass, turbulenceDueForces + cinematic / 1000f);
                        turbulenceDueForces *= damp;
                    } else {
                        turbulenceDueForces = 0;
                    }

                    if (_topology == TOPOLOGY.Sphere) {
                        liquidRot = Quaternion.Lerp(liquidRot, instantRot, 0.1f);
                        rot = liquidRot;
                    }
                } else if (turbulenceDueForces > 0) {
                    turbulenceDueForces *= 0.1f;
                }
                Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one);
                liqMat.SetMatrix(ShaderParams.RotationMatrix, m.inverse);
                if (_topology != TOPOLOGY.Sphere) {
                    float tx = turbDir.x;
                    turbDir.x += (turbDir.z - turbDir.x) * 0.25f;
                    turbDir.z += (tx - turbDir.z) * 0.25f;
                }
                turb.z = turbDir.x;
                turb.w = turbDir.z;
            }

            bool hasRotated = transform.rotation != lastRotation;
            if (_reactToForces || hasRotated || transform.position != lastPosition || transform.localScale != lastScale) {
                UpdateLevels(hasRotated);
            }
        }

        public void UpdateMaterialProperties() {
            if (Application.isPlaying) {
                shouldUpdateMaterialProperties = true;
            } else {
                UpdateMaterialPropertiesNow();
            }

        }

        void UpdateMaterialPropertiesNow() {
            if (!gameObject.activeInHierarchy)
                return;

            switch (_detail) {
                case DETAIL.Simple:
                case DETAIL.SimpleNoFlask:
                    if (liqMatSimple == null) {
                        liqMatSimple = Instantiate(Resources.Load<Material>("Materials/LiquidVolumeSimple")) as Material;
                    }
                    liqMat = liqMatSimple;
                    break;
                default:
                    if (liqMatDefaultNoFlask == null) {
                        liqMatDefaultNoFlask = Instantiate(Resources.Load<Material>("Materials/LiquidVolumeDefaultNoFlask")) as Material;
                    }
                    liqMat = liqMatDefaultNoFlask;
                    break;
            }

            if (_flaskMaterial == null) {
                _flaskMaterial = Instantiate(Resources.Load<Material>("Materials/Flask"));
            }

            if (liqMat == null)
                return;

            CheckMeshDisplacement();

            if (currentDetail != _detail) {
                currentDetail = _detail;
            }

            UpdateLevels();
            if (mr == null)
                return;

            // Try to compute submesh index heuristically if this is the first time the liquid has been added to a multi-material mesh
            mr.GetSharedMaterials(mrSharedMaterials);
            int sharedMaterialsCount = mrSharedMaterials.Count;
            if (_subMeshIndex < 0) {
                for (int w = 0; w < defaultContainerNames.Length; w++) {
                    if (_subMeshIndex >= 0)
                        break;
                    for (int k = 0; k < sharedMaterialsCount; k++) {
                        if (mrSharedMaterials[k] != null && mrSharedMaterials[k] != _flaskMaterial && mrSharedMaterials[k].name.ToUpper().Contains(defaultContainerNames[w])) {
                            _subMeshIndex = k;
                            break;
                        }
                    }
                }
            }
            if (_subMeshIndex < 0)
                _subMeshIndex = 0;

            if (sharedMaterialsCount > 1 && _subMeshIndex >= 0 && _subMeshIndex < sharedMaterialsCount) {
                mrSharedMaterials[_subMeshIndex] = liqMat;
            } else {
                mrSharedMaterials.Clear();
                mrSharedMaterials.Add(liqMat);
            }

            if (_flaskMaterial != null) {
                bool shouldUseFlaskMaterial = _detail.usesFlask();
                if (shouldUseFlaskMaterial && !mrSharedMaterials.Contains(_flaskMaterial)) {
                    // empty slot?
                    for (int k = 0; k < mrSharedMaterials.Count; k++) {
                        if (mrSharedMaterials[k] == null) {
                            mrSharedMaterials[k] = _flaskMaterial;
                            shouldUseFlaskMaterial = false;
                        }
                    }
                    if (shouldUseFlaskMaterial) {
                        mrSharedMaterials.Add(_flaskMaterial);
                    }
                } else if (!shouldUseFlaskMaterial && mrSharedMaterials.Contains(_flaskMaterial)) {
                    mrSharedMaterials.Remove(_flaskMaterial);
                }
                //_flaskMaterial.renderQueue = _renderQueue + 1;
                _flaskMaterial.SetFloat(ShaderParams.QueueOffset, _renderQueue - 3000);
                _flaskMaterial.SetFloat(ShaderParams.PreserveSpecular, 0);
            }
            mr.sharedMaterials = mrSharedMaterials.ToArray();

            liqMat.SetColor(ShaderParams.Color1, ApplyGlobalAlpha(_liquidColor1));
            liqMat.SetColor(ShaderParams.Color2, ApplyGlobalAlpha(_liquidColor2));
            liqMat.SetColor(ShaderParams.EmissionColor, _emissionColor);

            if (_useLightColor && _directionalLight != null) {
                Color lightColor = _directionalLight.color;
                liqMat.SetColor(ShaderParams.LightColor, lightColor);
            } else {
                liqMat.SetColor(ShaderParams.LightColor, Color.white);
            }
            if (_useLightDirection && _directionalLight != null) {
                liqMat.SetVector(ShaderParams.LightDir, -_directionalLight.transform.forward);
            } else {
                liqMat.SetVector(ShaderParams.LightDir, Vector3.up);
            }

            int scatteringPower = _scatteringPower;
            float scatteringAmount = _scatteringAmount;
            if (!_scatteringEnabled) {
                scatteringPower = 0;
                scatteringAmount = 0;
            }
            liqMat.SetVector(ShaderParams.GlossinessInt, new Vector4((1f - _glossinessInternal) * 96f + 1f, Mathf.Pow(2, scatteringPower), scatteringAmount, _glossinessInternal));
            liqMat.SetFloat(ShaderParams.DoubleSidedBias, _doubleSidedBias);
            liqMat.SetFloat(ShaderParams.BackDepthBias, -_backDepthBias);

            liqMat.SetFloat(ShaderParams.Muddy, _murkiness);
            liqMat.SetFloat(ShaderParams.Alpha, _alpha);

            float alphaCombined = _alpha * Mathf.Clamp01((_liquidColor1.a + _liquidColor2.a) * 4f);
            if (_ditherShadows) {
                liqMat.SetFloat(ShaderParams.AlphaCombined, alphaCombined);
            } else {
                liqMat.SetFloat(ShaderParams.AlphaCombined, alphaCombined > 0 ? 1000f : 0f);
            }

            liqMat.SetFloat(ShaderParams.SparklingIntensity, _sparklingIntensity * 250.0f);
            liqMat.SetFloat(ShaderParams.SparklingThreshold, 1.0f - _sparklingAmount);
            liqMat.SetFloat(ShaderParams.DepthAtten, _deepObscurance);
            Color smokeColor = ApplyGlobalAlpha(_smokeColor);
            int smokeRaySteps = _smokeRaySteps;
            if (!_smokeEnabled) {
                smokeColor.a = 0;
                smokeRaySteps = 1;
            }
            liqMat.SetColor(ShaderParams.SmokeColor, smokeColor);
            liqMat.SetFloat(ShaderParams.SmokeAtten, _smokeBaseObscurance);
            liqMat.SetFloat(ShaderParams.SmokeHeightAtten, _smokeHeightAtten);
            liqMat.SetFloat(ShaderParams.SmokeSpeed, _smokeSpeed);
            liqMat.SetFloat(ShaderParams.SmokeRaySteps, smokeRaySteps);

            liqMat.SetFloat(ShaderParams.LiquidRaySteps, _liquidRaySteps);

            liqMat.SetColor(ShaderParams.FoamColor, ApplyGlobalAlpha(_foamColor));
            liqMat.SetFloat(ShaderParams.FoamRaySteps, _foamThickness > 0 ? _foamRaySteps : 1);
            liqMat.SetFloat(ShaderParams.FoamDensity, _foamThickness > 0 ? _foamDensity : -1f);
            liqMat.SetFloat(ShaderParams.FoamWeight, _foamWeight);
            liqMat.SetFloat(ShaderParams.FoamBottom, _foamVisibleFromBottom ? 1f : 0f);
            liqMat.SetFloat(ShaderParams.FoamTurbulence, _foamTurbulence);


            if (_noiseVariation != currentNoiseVariation) {
                currentNoiseVariation = _noiseVariation;
                if (noise3DTex == null || noise3DTex.Length != 4) {
                    noise3DTex = new Texture3D[4];
                }
                if (noise3DTex[currentNoiseVariation] == null) {
                    noise3DTex[currentNoiseVariation] = Resources.Load<Texture3D>("Textures/Noise3D" + currentNoiseVariation.ToString());
                }
                Texture3D tex3d = noise3DTex[currentNoiseVariation];
                if (tex3d != null) {
                    liqMat.SetTexture(ShaderParams.NoiseTex, tex3d);
                }
            }

            liqMat.renderQueue = _renderQueue;

            UpdateInsideOut();

            if (_topology == TOPOLOGY.Irregular) {
                if (prevThickness != _flaskThickness) {
                    prevThickness = _flaskThickness;
                }
            }

            onPropertiesChanged?.Invoke(this);

        }

        Color ApplyGlobalAlpha(Color originalColor) {
            return new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.a * _alpha);
        }


        void GetRenderer() {
            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf != null) {
                mesh = mf.sharedMesh;
                mr = GetComponent<MeshRenderer>();
            } else {
                SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
                if (smr != null) {
                    mesh = smr.sharedMesh;
                    mr = smr;
                }
            }
        }

        void UpdateLevels(bool updateShaderKeywords = true) {

            _level = Mathf.Clamp01(_level);
            levelMultipled = _level * _levelMultiplier;

            if (liqMat == null)
                return;

            if (mesh == null) {
                GetRenderer();
                ReadVertices();
            } else if (mr == null) {
                GetRenderer();
            }
            if (mesh == null || mr == null) {
                return;
            }

            Vector4 size = new Vector4(mesh.bounds.extents.x * 2f * transform.lossyScale.x, mesh.bounds.extents.y * 2f * transform.lossyScale.y, mesh.bounds.extents.z * 2f * transform.lossyScale.z, 0);
            size.x *= _extentsScale.x;
            size.y *= _extentsScale.y;
            size.z *= _extentsScale.z;
            float maxWidth = Mathf.Max(size.x, size.z);

            Vector3 extents = _ignoreGravity ? new Vector3(size.x * 0.5f, size.y * 0.5f, size.z * 0.5f) : mr.bounds.extents;
            extents *= (1f - _flaskThickness);
            extents.x *= _extentsScale.x;
            extents.y *= _extentsScale.y;
            extents.z *= _extentsScale.z;

            // Compensate levelpos with upperlimit
            float rotationAdjustment;
            if (_upperLimit < 1f && !_ignoreGravity) {
                float y1 = transform.TransformPoint(Vector3.up * extents.y).y;
                float y0 = transform.TransformPoint(Vector3.up * (extents.y * _upperLimit)).y;
                rotationAdjustment = Mathf.Max(y0 - y1, 0);
            } else {
                rotationAdjustment = 0;
            }

#if DEBUG_SLICE
            float approxVolume = 0;
#endif


            // Compensate rotation in cylindrical shapes where mesh height is on another scale than width
            float thisLevel = levelMultipled;
            if (_rotationLevelCompensation != LEVEL_COMPENSATION.None && !_ignoreGravity && thisLevel > 0) {

                MeshVolumeCalcFunction volumeFunction;
                int exitIterations;

                if (_rotationLevelCompensation == LEVEL_COMPENSATION.Fast) {
                    volumeFunction = GetMeshVolumeUnderLevelFast;
                    exitIterations = 8;
                } else {
                    volumeFunction = GetMeshVolumeUnderLevel;
                    exitIterations = 10;
                }

                if (lastLevelVolumeRef != thisLevel) {
                    lastLevelVolumeRef = thisLevel;

                    if (_topology == TOPOLOGY.Cylinder) {
                        float r = size.x * 0.5f;
                        float h = size.y * thisLevel;
                        volumeRef = Mathf.PI * r * r * h; // perfect cylinder volume (optimization for performance reasons, ideally should go through normal mesh volume computation - the else part)
                    } else {
                        Quaternion q = transform.rotation;
                        transform.rotation = Quaternion.identity;

                        float tempExtentY = _ignoreGravity ? size.y * 0.5f : mr.bounds.extents.y;
                        tempExtentY *= (1f - _flaskThickness);
                        tempExtentY *= _extentsScale.y;

                        RotateVertices();
                        volumeRef = volumeFunction(thisLevel, tempExtentY);
                        transform.rotation = q;
                    }
                }
                RotateVertices();

                float nearestLevel = thisLevel;
                float minVolumeDiff = float.MaxValue;

                float maxLevel = Mathf.Clamp01(thisLevel + 0.5f);
                float minLevel = Mathf.Clamp01(thisLevel - 0.5f);
                for (int i = 0; i < 12; i++) {
                    thisLevel = (minLevel + maxLevel) * 0.5f;
                    float volume = volumeFunction(thisLevel, extents.y);
                    float volumeDiff = Mathf.Abs(volumeRef - volume);
                    if (volumeDiff < minVolumeDiff) {
#if DEBUG_SLICE
                            approxVolume = volume;
#endif
                        minVolumeDiff = volumeDiff;
                        nearestLevel = thisLevel;
                    }
                    if (volume < volumeRef) {
                        minLevel = thisLevel;
                    } else {
                        if (i >= exitIterations)
                            break;
                        maxLevel = thisLevel;
                    }
                }

                thisLevel = nearestLevel * _levelMultiplier;

            } else {
                if (levelMultipled <= 0)
                    thisLevel = -0.001f; // ensure it's below the flask thickness
            }

            liquidLevelPos = mr.bounds.center.y - extents.y;
            liquidLevelPos += extents.y * 2f * thisLevel + rotationAdjustment;

#if DEBUG_SLICE
            Debug.Log("Ref: " + volumeRef + " Vol: " + approxVolume + " Level: " + thisLevel);
#endif

            liqMat.SetFloat(ShaderParams.LevelPos, liquidLevelPos);
            float upperLimit = mesh.bounds.extents.y * _extentsScale.y * _upperLimit;
            liqMat.SetFloat(ShaderParams.UpperLimit, _limitVerticalRange ? upperLimit : float.MaxValue);
            float lowerLimit = mesh.bounds.extents.y * _extentsScale.y * _lowerLimit;
            liqMat.SetFloat(ShaderParams.LowerLimit, _limitVerticalRange ? lowerLimit : float.MinValue);
            float visibleLevel = (levelMultipled <= 0 || levelMultipled >= 1f) ? 0f : 1f;
            UpdateTurbulence();
            float foamPos = mr.bounds.center.y - extents.y + (rotationAdjustment + extents.y * 2.0f * (thisLevel + _foamThickness)) * visibleLevel;
            liqMat.SetFloat(ShaderParams.FoamMaxPos, foamPos);
            Vector4 thickness = new Vector4(1.0f - _flaskThickness, (1.0f - _flaskThickness * maxWidth / size.z), (1.0f - _flaskThickness * maxWidth / size.z), 0);
            liqMat.SetVector(ShaderParams.FlaskThickness, thickness);
            size.w = size.x * 0.5f * thickness.x;
            size.x = Vector3.Distance(mr.bounds.max, mr.bounds.min);
            liqMat.SetVector(ShaderParams.Size, size);
            float scaleFactor = size.y * 0.5f * (1.0f - _flaskThickness * maxWidth / size.y);
            liqMat.SetVector(ShaderParams.Scale, new Vector4(_smokeScale / scaleFactor, _foamScale / scaleFactor, _liquidScale1 / scaleFactor, _liquidScale2 / scaleFactor));
            liqMat.SetVector(ShaderParams.Center, transform.position);

            if (shaderKeywords == null || shaderKeywords.Length != 6) {
                shaderKeywords = new string[6];
            }
            for (int k = 0; k < shaderKeywords.Length; k++) {
                shaderKeywords[k] = null;
            }

            if (_depthAware) {
                shaderKeywords[SHADER_KEYWORD_DEPTH_AWARE_INDEX] = SHADER_KEYWORD_DEPTH_AWARE;
                liqMat.SetFloat(ShaderParams.DepthAwareOffset, _depthAwareOffset);
            }

            if (_depthAwareCustomPass) {
                shaderKeywords[SHADER_KEYWORD_DEPTH_AWARE_CUSTOM_PASS_INDEX] = SHADER_KEYWORD_DEPTH_AWARE_CUSTOM_PASS;
            }

            if (_reactToForces && _topology == TOPOLOGY.Sphere) {
                shaderKeywords[SHADER_KEYWORD_IGNORE_GRAVITY_INDEX] = SHADER_KEYWORD_IGNORE_GRAVITY;
            } else if (_ignoreGravity) {
                shaderKeywords[SHADER_KEYWORD_IGNORE_GRAVITY_INDEX] = SHADER_KEYWORD_IGNORE_GRAVITY;
            } else if (transform.rotation.eulerAngles != Vector3.zero) {
                shaderKeywords[SHADER_KEYWORD_NON_AABB_INDEX] = SHADER_KEYWORD_NON_AABB;
            }
            switch (_topology) {
                case TOPOLOGY.Sphere:
                    shaderKeywords[SHADER_KEYWORD_TOPOLOGY_INDEX] = SHADER_KEYWORD_SPHERE;
                    break;
                case TOPOLOGY.Cube:
                    shaderKeywords[SHADER_KEYWORD_TOPOLOGY_INDEX] = SHADER_KEYWORD_CUBE;
                    break;
                case TOPOLOGY.Cylinder:
                    shaderKeywords[SHADER_KEYWORD_TOPOLOGY_INDEX] = SHADER_KEYWORD_CYLINDER;
                    break;
                default:
                    shaderKeywords[SHADER_KEYWORD_TOPOLOGY_INDEX] = SHADER_KEYWORD_IRREGULAR;
                    break;
            }
            if (_refractionBlur && _detail.allowsRefraction()) {
                liqMat.SetFloat(ShaderParams.FlaskBlurIntensity, _blurIntensity * (_refractionBlur ? 1f : 0f));
                shaderKeywords[SHADER_KEYWORD_REFRACTION_INDEX] = SHADER_KEYWORD_USE_REFRACTION;
            }
            if (updateShaderKeywords) {
                liqMat.shaderKeywords = shaderKeywords;
            }

            lastPosition = transform.position;
            lastScale = transform.localScale;
            lastRotation = transform.rotation;
        }


        void RotateVertices() {
            int vertexCount = verticesUnsorted.Length;
            if (rotatedVertices == null || rotatedVertices.Length != vertexCount) {
                rotatedVertices = new Vector3[vertexCount];
            }
            for (int k = 0; k < vertexCount; k++) {
                rotatedVertices[k] = transform.TransformPoint(verticesUnsorted[k]);
            }
        }


        float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 zeroPoint) {
            // zero point can be useful to avoid floating point issues if mesh is at very distant positions but in general this can be omitted
            p1.x -= zeroPoint.x; p1.y -= zeroPoint.y; p1.z -= zeroPoint.z;
            p2.x -= zeroPoint.x; p2.y -= zeroPoint.y; p2.z -= zeroPoint.z;
            p3.x -= zeroPoint.x; p3.y -= zeroPoint.y; p3.z -= zeroPoint.z;
            float v321 = p3.x * p2.y * p1.z;
            float v231 = p2.x * p3.y * p1.z;
            float v312 = p3.x * p1.y * p2.z;
            float v132 = p1.x * p3.y * p2.z;
            float v213 = p2.x * p1.y * p3.z;
            float v123 = p1.x * p2.y * p3.z;
            float e = (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
            return e;
        }


        delegate float MeshVolumeCalcFunction(float level01, float yExtent);

        public float GetMeshVolumeUnderLevelFast(float level01, float yExtent) {

            float level = mr.bounds.center.y - yExtent;
            level += yExtent * 2f * level01;
            return GetMeshVolumeUnderLevelWSFast(level);
        }

        public float GetMeshVolumeWSFast() {
            return GetMeshVolumeUnderLevelWSFast(float.MaxValue);
        }

        public float GetMeshVolumeUnderLevelWSFast(float level) {

            Vector3 zeroPoint = mr.bounds.center;
            float vol = 0;
            for (int k = 0; k < verticesIndices.Length; k += 3) {
                Vector3 p1 = rotatedVertices[verticesIndices[k]];
                Vector3 p2 = rotatedVertices[verticesIndices[k + 1]];
                Vector3 p3 = rotatedVertices[verticesIndices[k + 2]];
                if (p1.y > level) p1.y = level;
                if (p2.y > level) p2.y = level;
                if (p3.y > level) p3.y = level;
                vol += SignedVolumeOfTriangle(p1, p2, p3, zeroPoint);
            }
            return Mathf.Abs(vol);
        }

        Vector3 ClampVertexToSlicePlane(Vector3 p, Vector3 q, float level) {
            Vector3 qp = (q - p).normalized;
            float h = p.y - level;
            return p + qp * h / -qp.y;
        }


#if DEBUG_SLICE
        GameObject o;
#endif
        readonly List<Vector3> verts = new List<Vector3>();
        readonly List<Vector3> cutPoints = new List<Vector3>();
        Vector3 cutPlaneCenter;

        /// Approximates the mesh volume under a fill level expressed in 0-1 range
        /// </summary>
        /// <param name="level01"></param>
        /// <param name="zeroPoint"></param>
        /// <param name="yExtent"></param>
        /// <returns></returns>
        public float GetMeshVolumeUnderLevel(float level01, float yExtent) {

            float level = mr.bounds.center.y - yExtent;
            level += yExtent * 2f * level01;
            return GetMeshVolumeUnderLevelWS(level);

        }


        public float GetMeshVolumeWS() {
            return GetMeshVolumeUnderLevelWS(float.MaxValue);
        }

        public float GetMeshVolumeUnderLevelWS(float level) {

            Vector3 zeroPoint = mr.bounds.center;
            cutPlaneCenter = Vector3.zero;
            cutPoints.Clear();
            verts.Clear();

            // Slice mesh
            int indicesCount = verticesIndices.Length;
            for (int k = 0; k < indicesCount; k += 3) {
                Vector3 p1 = rotatedVertices[verticesIndices[k]];
                Vector3 p2 = rotatedVertices[verticesIndices[k + 1]];
                Vector3 p3 = rotatedVertices[verticesIndices[k + 2]];

                if (p1.y > level && p2.y > level && p3.y > level) continue;

                if (p1.y < level && p2.y > level && p3.y > level) {
                    p2 = ClampVertexToSlicePlane(p2, p1, level);
                    p3 = ClampVertexToSlicePlane(p3, p1, level);
                    cutPoints.Add(p2);
                    cutPoints.Add(p3);
                    cutPlaneCenter += p2;
                    cutPlaneCenter += p3;
                } else if (p2.y < level && p1.y > level && p3.y > level) {
                    p1 = ClampVertexToSlicePlane(p1, p2, level);
                    p3 = ClampVertexToSlicePlane(p3, p2, level);
                    cutPoints.Add(p1);
                    cutPoints.Add(p3);
                    cutPlaneCenter += p1;
                    cutPlaneCenter += p3;
                } else if (p3.y < level && p1.y > level && p2.y > level) {
                    p1 = ClampVertexToSlicePlane(p1, p3, level);
                    p2 = ClampVertexToSlicePlane(p2, p3, level);
                    cutPoints.Add(p1);
                    cutPoints.Add(p2);
                    cutPlaneCenter += p1;
                    cutPlaneCenter += p2;
                } else if (p1.y > level && p2.y < level && p3.y < level) {
                    Vector3 p12 = ClampVertexToSlicePlane(p1, p2, level);
                    Vector3 p13 = ClampVertexToSlicePlane(p1, p3, level);
                    verts.Add(p12);
                    verts.Add(p2);
                    verts.Add(p3);
                    verts.Add(p13);
                    verts.Add(p12);
                    verts.Add(p3);
                    cutPoints.Add(p12);
                    cutPoints.Add(p13);
                    cutPlaneCenter += p12;
                    cutPlaneCenter += p13;
                    continue;
                } else if (p2.y > level && p1.y < level && p3.y < level) {
                    Vector3 p21 = ClampVertexToSlicePlane(p2, p1, level);
                    Vector3 p23 = ClampVertexToSlicePlane(p2, p3, level);
                    verts.Add(p1);
                    verts.Add(p21);
                    verts.Add(p3);
                    verts.Add(p21);
                    verts.Add(p23);
                    verts.Add(p3);
                    cutPoints.Add(p21);
                    cutPoints.Add(p23);
                    cutPlaneCenter += p21;
                    cutPlaneCenter += p23;
                    continue;
                } else if (p3.y > level && p1.y < level && p2.y < level) {
                    Vector3 p31 = ClampVertexToSlicePlane(p3, p1, level);
                    Vector3 p32 = ClampVertexToSlicePlane(p3, p2, level);
                    verts.Add(p31);
                    verts.Add(p1);
                    verts.Add(p2);
                    verts.Add(p32);
                    verts.Add(p31);
                    verts.Add(p2);
                    cutPoints.Add(p31);
                    cutPoints.Add(p32);
                    cutPlaneCenter += p31;
                    cutPlaneCenter += p32;
                    continue;
                }
                verts.Add(p1);
                verts.Add(p2);
                verts.Add(p3);
            }

            // close sliced mesh
            int cutPointsCount = cutPoints.Count;
            if (cutPoints.Count >= 3) {
                cutPlaneCenter /= cutPointsCount;
                cutPoints.Sort(PolygonSortOnPlane);

                for (int k = 0; k < cutPointsCount; k++) {
                    Vector3 p1 = cutPoints[k];
                    Vector3 p2;
                    if (k == cutPointsCount - 1) {
                        p2 = cutPoints[0];
                    } else {
                        p2 = cutPoints[k + 1];
                    }
                    verts.Add(cutPlaneCenter);
                    verts.Add(p1);
                    verts.Add(p2);
                }
            }

            // compute mesh volume
            int vertCount = verts.Count;
            float vol = 0;
            for (int k = 0; k < vertCount; k += 3) {
                vol += SignedVolumeOfTriangle(verts[k], verts[k + 1], verts[k + 2], zeroPoint);
            }

#if DEBUG_SLICE
            if (o == null) {
                o = new GameObject("Sliced", typeof(MeshFilter), typeof(MeshRenderer));
            }

            Mesh mesh = new Mesh();
            mesh.vertices = verts.ToArray();
            int len = mesh.vertices.Length;
            int[] inds = new int[len];
            for (int k = 0; k < inds.Length; k++) {
                inds[k] = k;
            }
            mesh.triangles = inds;
            MeshFilter mf = o.GetComponent<MeshFilter>();
            mf.mesh = mesh;
#endif

            return Mathf.Abs(vol);
        }

        int PolygonSortOnPlane(Vector3 p1, Vector3 p2) {
            float r1 = Mathf.Atan2(p1.x - cutPlaneCenter.x, p1.z - cutPlaneCenter.z);
            float r2 = Mathf.Atan2(p2.x - cutPlaneCenter.x, p2.z - cutPlaneCenter.z);
            if (r1 < r2) return -1;
            if (r1 > r2) return 1;
            return 0;
        }

        void UpdateTurbulence() {
            if (liqMat == null)
                return;
            float visibleLevel = levelMultipled > 0 ? 1f : 0f; // (_level<=0 || _level>=1f) ? 0.1f: 1f;	// commented out to allow animation even level is 0 or full
            float isInsideContainer = (camInside && _allowViewFromInside) ? 0f : 1f;
            turb.x = _turbulence1 * visibleLevel * isInsideContainer;
            turb.y = Mathf.Max(_turbulence2, turbulenceDueForces) * visibleLevel * isInsideContainer;
            shaderTurb = turb;
            shaderTurb.z *= 3.1415927f * _frecuency * 4f;
            shaderTurb.w *= 3.1415927f * _frecuency * 4f;
            liqMat.SetVector(ShaderParams.Turbulence, shaderTurb);
        }

        void CheckInsideOut() {
            Camera cam = Camera.current;
            if (cam == null || mr == null) {
                if (!_allowViewFromInside)
                    UpdateInsideOut();
                return;
            }

            Vector3 currentCamPos = cam.transform.position + cam.transform.forward * cam.nearClipPlane;
            float currentDistanceToCam = (currentCamPos - transform.position).sqrMagnitude;
            if (currentDistanceToCam == lastDistanceToCam)
                return;
            lastDistanceToCam = currentDistanceToCam;

            // Check if position is inside container
            bool nowInside = false;
            switch (_topology) {
                case TOPOLOGY.Cube:
                    nowInside = PointInAABB(currentCamPos);
                    break;
                case TOPOLOGY.Cylinder:
                    nowInside = PointInCylinder(currentCamPos);
                    break;
                default:
                    float diam = mesh.bounds.extents.x * 2f;
                    nowInside = (currentCamPos - transform.position).sqrMagnitude < (diam * diam);
                    break;
            }

            if (nowInside != camInside) {
                camInside = nowInside;
                UpdateInsideOut();
            }
        }


        bool PointInAABB(Vector3 point) {
            point = transform.InverseTransformPoint(point);
            Vector3 ext = mesh.bounds.extents;
            if (point.x < ext.x && point.x > -ext.x &&
                point.y < ext.y && point.y > -ext.y &&
                point.z < ext.z && point.z > -ext.z) {
                return true;
            } else {
                return false;
            }
        }

        bool PointInCylinder(Vector3 point) {
            point = transform.InverseTransformPoint(point);
            Vector3 ext = mesh.bounds.extents;
            if (point.x < ext.x && point.x > -ext.x &&
                point.y < ext.y && point.y > -ext.y &&
                point.z < ext.z && point.z > -ext.z) {

                point.y = 0;
                Vector3 currentPos = transform.position;
                currentPos.y = 0;
                return (point - currentPos).sqrMagnitude < ext.x * ext.x;
            }
            return false;
        }


        void UpdateInsideOut() {
            if (liqMat == null)
                return;
            if (_allowViewFromInside && camInside) {
                liqMat.SetInt(ShaderParams.CullMode, (int)CullMode.Front);
                liqMat.SetInt(ShaderParams.ZTestMode, (int)CompareFunction.Always);
                if (_flaskMaterial != null) {
                    _flaskMaterial.SetInt(ShaderParams.CullMode, (int)CullMode.Front);
                    _flaskMaterial.SetInt(ShaderParams.ZTestMode, (int)CompareFunction.Always);
                }
            } else {
                liqMat.SetInt(ShaderParams.CullMode, (int)CullMode.Back);
                liqMat.SetInt(ShaderParams.ZTestMode, (int)CompareFunction.LessEqual);
                if (_flaskMaterial != null) {
                    _flaskMaterial.SetInt(ShaderParams.CullMode, (int)CullMode.Back);
                    _flaskMaterial.SetInt(ShaderParams.ZTestMode, (int)CompareFunction.LessEqual);
                }
            }
            UpdateTurbulence();
        }

        #endregion


        #region Public API

        /// <summary>
        /// Returns the vertical position in world space coordinates of the liquid surface
        /// </summary>
        /// <value>The get liquid surface Y position.</value>
        public float liquidSurfaceYPosition {
            get {
                return liquidLevelPos;
            }
        }

        /// <summary>
        /// Computes approximate point where liquid starts pouring over the flask when it's rotated
        /// </summary>
        /// <returns><c>true</c>, if spill point is detected, <c>false</c> otherwise.</returns>
        /// <param name="spillPosition">Returned spill position in world space coordinates.</param>
        /// <param name="apertureStart">A value that determines where the aperture of the flask starts (0-1 where 0 is flask center and 1 is the very top).</param>
        public bool GetSpillPoint(out Vector3 spillPosition, float apertureStart = 1f) {
            float spillAmount;
            return GetSpillPoint(out spillPosition, out spillAmount, apertureStart);
        }



        /// <summary>
        /// Computes approximate point where liquid starts pouring over the flask when it's rotated
        /// </summary>
        /// <returns><c>true</c>, if spill point is detected, <c>false</c> otherwise.</returns>
        /// <param name="spillPosition">Returned spill position in world space coordinates.</param>
        /// <param name="spillAmount">A returned value that represent the difference in liquid levels after the spilt. This value depends on the rotationCompensation parameter.</param>
        /// <param name="apertureStart">A value that determines where the aperture of the flask starts (0-1 where 0 is flask center and 1 is the very top).</param>
        /// <param name="rotationCompensation">If set to None (default), the spillAmount value is an approximation of the differences of levels in 0-1 range. Fast and Accurate modes will approximate the difference in volume. Use GetMeshVolumeWS or GetMeshVolumeWSFast to get the total volume.</param>
        public bool GetSpillPoint(out Vector3 spillPosition, out float spillAmount, float apertureStart = 1f, LEVEL_COMPENSATION rotationCompensation = LEVEL_COMPENSATION.None) {
            spillPosition = Vector3.zero;
            spillAmount = 0;
            if (mesh == null || verticesSorted == null || levelMultipled <= 0)
                return false;

            float maxy = float.MinValue;
            for (int k = 0; k < verticesSorted.Length; k++) {
                Vector3 vertex = verticesSorted[k];
                if (vertex.y > maxy) {
                    maxy = vertex.y;
                }
            }
            float clampy = maxy * apertureStart * 0.99f;
            Vector3 vt = transform.position;
            bool crossed = false;
            float miny = float.MaxValue;
            for (int k = 0; k < verticesSorted.Length; k++) {
                Vector3 vertex = verticesSorted[k];
                if (vertex.y < clampy)
                    break;
                vertex = transform.TransformPoint(vertex);
                if (vertex.y < liquidLevelPos && vertex.y < miny) {
                    miny = vertex.y;
                    vt = vertex;
                    crossed = true;
                }
            }
            if (!crossed)
                return false;

            spillPosition = vt;

            switch (rotationCompensation) {
                case LEVEL_COMPENSATION.Accurate:
                    spillAmount = GetMeshVolumeUnderLevelWS(liquidLevelPos) - GetMeshVolumeUnderLevelWS(vt.y);
                    break;

                case LEVEL_COMPENSATION.Fast:
                    spillAmount = GetMeshVolumeUnderLevelWSFast(liquidLevelPos) - GetMeshVolumeUnderLevelWSFast(vt.y);
                    break;

                default:
                    spillAmount = (liquidLevelPos - vt.y) / (mr.bounds.extents.y * 2f);
                    break;
            }
            return true;
        }


        void UpdateSpillPointGizmo() {
            if (!_debugSpillPoint) {
                if (spillPointGizmo != null) {
                    DestroyImmediate(spillPointGizmo.gameObject);
                    spillPointGizmo = null;
                }
                return;
            }

            if (spillPointGizmo == null) {
                Transform t = transform.Find(SPILL_POINT_GIZMO);
                if (t != null) {
                    DestroyImmediate(t.gameObject);
                }
                spillPointGizmo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                spillPointGizmo.name = SPILL_POINT_GIZMO;
                spillPointGizmo.transform.SetParent(transform, true);
                Collider collider = spillPointGizmo.GetComponent<Collider>();
                if (collider != null)
                    DestroyImmediate(collider);
                MeshRenderer mr = spillPointGizmo.GetComponent<MeshRenderer>();
                if (mr != null) {
                    mr.sharedMaterial = Instantiate(mr.sharedMaterial); // to avoid Editor (non playing) warning
                    mr.sharedMaterial.hideFlags = HideFlags.DontSave;
                    mr.sharedMaterial.color = Color.yellow;
                }
            }

            Vector3 spillPoint;
            if (GetSpillPoint(out spillPoint, 1f)) {
                spillPointGizmo.transform.position = spillPoint;
                if (mesh != null) {
                    Vector3 size = mesh.bounds.extents * 0.2f;
                    float s = size.x > size.y ? size.x : size.z;
                    s = s > size.z ? s : size.z;
                    spillPointGizmo.transform.localScale = new Vector3(s, s, s);
                } else {
                    spillPointGizmo.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                }
                spillPointGizmo.SetActive(true);
            } else {
                spillPointGizmo.SetActive(false);
            }

        }

        /// <summary>
        /// Applies current transform rotation and scale to the vertices and resets the transform rotation and scale to default values.
        /// This operation makes the game object transform point upright as normal game objects and is required for Liquid Volume to work on imported models that comes with a rotation
        /// </summary>
        public void BakeRotation() {

            if (transform.localRotation == transform.rotation) {
                // nothing to do!
                return;
            }
            MeshFilter mf = GetComponent<MeshFilter>();
            Mesh mesh = mf.sharedMesh;
            if (mesh == null) return;
            mesh = Instantiate(mesh);
            Vector3[] vertices = mesh.vertices;
            Vector3 scale = transform.localScale;
            Vector3 localPos = transform.localPosition;

            transform.localScale = Vector3.one;

            Transform parent = transform.parent;
            if (parent != null) {
                transform.SetParent(null, false);
            }

            for (int k = 0; k < vertices.Length; k++) {
                vertices[k] = transform.TransformVector(vertices[k]);
            }
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mf.sharedMesh = mesh;

            // Ensure parent has no different rotation
            if (parent != null) {
                transform.SetParent(parent, false);
                transform.localPosition = localPos;
            }

            transform.localRotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = scale;

            RefreshMeshAndCollider();
        }

        /// <summary>
        /// This operation computes the geometric center of all vertices and displaces them so the pivot is centered in the model
        /// </summary>
        public void CenterPivot() {
            CenterPivot(Vector3.zero);
        }

        [SerializeField]
        Mesh fixedMesh;

        /// <summary>
        /// This operation computes the geometric center of all vertices and displaces them so the pivot is centered in the model
        /// </summary>
        public void CenterPivot(Vector3 offset) {

            MeshFilter mf = GetComponent<MeshFilter>();
            Mesh mesh = mf.sharedMesh;
            if (mesh == null) return;

            mesh = Instantiate(mesh);
            mesh.name = mf.sharedMesh.name; // keep original name to detect if user assigns a different mesh to meshfilter and discard originalMesh reference

            Vector3[] vertices = mesh.vertices;

            Vector3 midPoint = Vector3.zero;
            for (int k = 0; k < vertices.Length; k++) {
                midPoint += vertices[k];
            }
            midPoint /= vertices.Length;
            midPoint += offset;
            for (int k = 0; k < vertices.Length; k++) {
                vertices[k] -= midPoint;
            }
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mf.sharedMesh = mesh;

            fixedMesh = mesh;

            Vector3 localScale = transform.localScale;
            midPoint.x *= localScale.x;
            midPoint.y *= localScale.y;
            midPoint.z *= localScale.z;
            transform.localPosition += midPoint;

            RefreshMeshAndCollider();
        }

        public void RefreshMeshAndCollider() {
            ClearMeshCache();
            MeshCollider mc = GetComponent<MeshCollider>();
            if (mc != null) {
                Mesh oldMesh = mc.sharedMesh;
                mc.sharedMesh = null;
                mc.sharedMesh = oldMesh;
            }
        }

        public void Redraw() {
            UpdateMaterialProperties();
        }


        #endregion

        #region Mesh Displacement

        void CheckMeshDisplacement() {
            MeshFilter meshFilter = GetComponent<MeshFilter>();

            if (meshFilter == null) {
                originalMesh = null;
                return;
            }

            Mesh currentMesh = meshFilter.sharedMesh;
            if (currentMesh == null) {
                if (_fixMesh) {
                    if (fixedMesh != null) {
                        meshFilter.sharedMesh = fixedMesh;
                        return;
                    } else if (originalMesh != null) {
                        meshFilter.sharedMesh = originalMesh;
                    }
                } else {
                    originalMesh = null;
                    return;
                }
                currentMesh = meshFilter.sharedMesh;
            }

            if (!_fixMesh) {
                RestoreOriginalMesh();
                originalMesh = null;
                return;
            }

            // Backup original mesh
            if (originalMesh == null || !(originalMesh.name.Equals(currentMesh.name))) {
                originalMesh = meshFilter.sharedMesh;
            }

            if (currentMesh != originalMesh) {
                RestoreOriginalMesh();
            }

            Vector3 pos = transform.localPosition;
            CenterPivot(_pivotOffset);
            originalPivotOffset = transform.localPosition - pos;
        }

        void RestoreOriginalMesh() {
            fixedMesh = null;
            if (originalMesh == null) return;

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null) return;

            meshFilter.sharedMesh = originalMesh;
            transform.localPosition -= originalPivotOffset;
            RefreshMeshAndCollider();
        }

        #endregion


        public void CopyFrom(LiquidVolume lv) {
            if (lv == null) return;
            this._allowViewFromInside = lv._allowViewFromInside;
            this._alpha = lv._alpha;
            this._backDepthBias = lv._backDepthBias;
            this._blurIntensity = lv._blurIntensity;
            this._bumpDistortionOffset = lv._bumpDistortionOffset;
            this._bumpDistortionScale = lv._bumpDistortionScale;
            this._bumpMap = lv._bumpMap;
            this._bumpStrength = lv._bumpStrength;
            this._debugSpillPoint = lv._debugSpillPoint;
            this._deepObscurance = lv._deepObscurance;
            this._depthAware = lv._depthAware;
            this._depthAwareCustomPass = lv._depthAwareCustomPass;
            this._depthAwareCustomPassDebug = lv._depthAwareCustomPassDebug;
            this._depthAwareOffset = lv._depthAwareOffset;
            this._detail = lv._detail;
            this._distortionAmount = lv._distortionAmount;
            this._distortionMap = lv._distortionMap;
            this._ditherShadows = lv._ditherShadows;
            this._doubleSidedBias = lv._doubleSidedBias;
            this._emissionColor = lv._emissionColor;
            this._extentsScale = lv._extentsScale;
            this._fixMesh = lv._fixMesh;
            this._flaskThickness = lv._flaskThickness;
            this._foamColor = lv._foamColor;
            this._foamDensity = lv._foamDensity;
            this._foamRaySteps = lv._foamRaySteps;
            this._foamScale = lv._foamScale;
            this._foamThickness = lv._foamThickness;
            this._foamTurbulence = lv._foamTurbulence;
            this._foamVisibleFromBottom = lv._foamVisibleFromBottom;
            this._foamWeight = lv._foamWeight;
            this._frecuency = lv._frecuency;
            this._ignoreGravity = lv._ignoreGravity;
            this._irregularDepthDebug = lv._irregularDepthDebug;
            this._level = lv._level;
            this._levelMultiplier = lv._levelMultiplier;
            this._liquidColor1 = lv._liquidColor1;
            this._liquidColor2 = lv._liquidColor2;
            this._liquidRaySteps = lv._liquidRaySteps;
            this._liquidScale1 = lv._liquidScale1;
            this._liquidScale2 = lv._liquidScale2;
            this._lowerLimit = lv._lowerLimit;
            this._murkiness = lv._murkiness;
            this._noiseVariation = lv._noiseVariation;
            this._physicsAngularDamp = lv._physicsAngularDamp;
            this._physicsMass = lv._physicsMass;
            this._pivotOffset = lv._pivotOffset;
            this._reactToForces = lv._reactToForces;
            this._reflectionTexture = lv._reflectionTexture;
            this._refractionBlur = lv._refractionBlur;
            this._renderQueue = lv._renderQueue;
            this._scatteringAmount = lv._scatteringAmount;
            this._scatteringEnabled = lv._scatteringEnabled;
            this._scatteringPower = lv._scatteringPower;
            this._smokeBaseObscurance = lv._smokeBaseObscurance;
            this._smokeColor = lv._smokeColor;
            this._smokeEnabled = lv._smokeEnabled;
            this._smokeHeightAtten = lv._smokeHeightAtten;
            this._smokeRaySteps = lv._smokeRaySteps;
            this._smokeScale = lv._smokeScale;
            this._smokeSpeed = lv._smokeSpeed;
            this._sparklingAmount = lv._sparklingAmount;
            this._sparklingIntensity = lv._sparklingIntensity;
            this._speed = lv._speed;
            this._subMeshIndex = lv._subMeshIndex;
            this._texture = lv._texture;
            this._textureOffset = lv._textureOffset;
            this._textureScale = lv._textureScale;
            this._topology = lv._topology;
            this._turbulence1 = lv._turbulence1;
            this._turbulence2 = lv._turbulence2;
            this._upperLimit = lv._upperLimit;
            shouldUpdateMaterialProperties = true;
        }
    }
}
