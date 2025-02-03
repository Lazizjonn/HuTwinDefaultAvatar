// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction;
using UnityEngine;

namespace Oculus.Movement.Tracking
{
    /// <summary>
    /// Face class that allows applying correctives and/or
    /// modifiers.
    /// </summary>
    public class CorrectivesFace : OVRCustomFace
    {
        /// <summary>
        /// If true, the correctives driver will apply correctives.
        /// </summary>
        public bool CorrectivesEnabled { get; set; }

        /// <summary>
        /// Last updated expression weights.
        /// </summary>
        public float[] ExpressionWeights { get; private set; }

        /// <summary>
        /// Allows one to freeze current values obtained from facial expressions component.
        /// </summary>
        public bool FreezeExpressionWeights { get; set; }

        /// <summary>
        /// Forces the jaw to drop to accomodate the tongue if it is being stuck out.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether or not to force the jaw open to accomodate an extended tongue")]
        protected bool _forceJawDropForTongue = true;

        /// <inheritdoc cref="_forceJawDropForTongue" />
        public bool ForceJawDropForTongue
        {
            get { return _forceJawDropForTongue; }
            set { _forceJawDropForTongue = value; }
        }

        [SerializeField]
        [Tooltip("Minimum value of TongueOut to force JawDrop open")]
        private float _tongueOutThreshold = 0.25f;

        [SerializeField]
        [Tooltip("Minimum value of JawDrop to force when the user's tongue is out")]
        private float _minJawDrop = 0.5f;

        /// <summary>
        /// Optional blendshape modifier component.
        /// </summary>
        [SerializeField]
        [Optional]
        [Tooltip(CorrectivesFaceTooltips.BlendshapeModifier)]
        protected BlendshapeModifier _blendshapeModifier;

        /// <inheritdoc cref="_blendshapeModifier"/>
        public BlendshapeModifier BlendshapeModifier
        {
            get { return _blendshapeModifier; }
            set { _blendshapeModifier = value; }
        }

        /// <summary>
        /// The json file containing the in-betweens and combinations data.
        /// </summary>
        [SerializeField]
        [Optional]
        [Tooltip(CorrectivesFaceTooltips.CombinationShapesTextAsset)]
        protected TextAsset _combinationShapesTextAsset;

        /// <inheritdoc cref="_combinationShapesTextAsset"/>
        public TextAsset CombinationShapesTextAsset
        {
            get { return _combinationShapesTextAsset; }
            set { _combinationShapesTextAsset = value; }
        }

        /// <summary>
        /// Allows modifying retargeting type field.
        /// </summary>
        public RetargetingType RetargetingTypeField
        {
            get => RetargetingValue;
            set => RetargetingValue = value;
        }

        /// <summary>
        /// Allows modifying duplicates field.
        /// </summary>
        public bool AllowDuplicateMappingField
        {
            get => AllowDuplicateMapping;
            set => AllowDuplicateMapping = value;
        }

        /// <summary>
        /// Cached mesh blendshape values.
        /// </summary>
        protected float[] _cachedBlendshapeValues;

        /// <summary>
        /// The correctives module.
        /// </summary>
        protected CorrectivesModule _correctivesModule;

        /// <summary>
        /// Initializes weights and correctives module.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            InitializeExpressionWeights();

            CorrectivesEnabled = true;
            if (_combinationShapesTextAsset != null)
            {
                _correctivesModule = new CorrectivesModule(_combinationShapesTextAsset);
            }

            /*faceExpressions = GetComponent<OVRFaceExpressions>();

            if (faceExpressions == null)
            {
                Debug.LogError("TTT, OVRFaceExpressions component not found on this GameObject, Awake()");
                enabled = false;
                return;
            }
            else
            {
                Debug.LogError("TTT, OVRFaceExpressions component found on this GameObject, Awake()");
            }*/
        }

        /// <summary>
        /// Returns OVRFaceExpressions value for the blend shape index provided.
        /// </summary>
        /// <param name="blendShapeIndex">Blend shape index.</param>
        /// <returns>OVRFaceExpression value.</returns>
        public OVRFaceExpressions.FaceExpression GetFaceExpressionForIndex(int blendShapeIndex)
        {
            return GetFaceExpression(blendShapeIndex);
        }

        /// <summary>
        /// Initialize the expression weights array.
        /// </summary>
        protected void InitializeExpressionWeights()
        {
            ExpressionWeights = new float[(int)OVRFaceExpressions.FaceExpression.Max];
            for (int i = 0; i < (int)OVRFaceExpressions.FaceExpression.Max; i++)
            {
                ExpressionWeights[i] = 0.0f;
            }
        }

        /// <summary>
        /// Activates a single face expression and sets the other to zero.
        /// </summary>
        /// <param name="faceExpression">Face expression to activate.</param>
        public void ActivateFaceExpression(OVRFaceExpressions.FaceExpression faceExpression)
        {
            if (ExpressionWeights == null ||
                ExpressionWeights.Length != (int)OVRFaceExpressions.FaceExpression.Max)
            {
                InitializeExpressionWeights();
            }
            InitializeCachedValues();

            int numBlendshapes = SkinnedMesh.sharedMesh.blendShapeCount;
            for (int blendShapeIndex = 0; blendShapeIndex < numBlendshapes; ++blendShapeIndex)
            {
                OVRFaceExpressions.FaceExpression blendShapeToFaceExpression = GetFaceExpression(blendShapeIndex);
                if (blendShapeToFaceExpression >= OVRFaceExpressions.FaceExpression.Max ||
                    blendShapeToFaceExpression < 0)
                {
                    continue;
                }
                ExpressionWeights[(int)blendShapeToFaceExpression] = (blendShapeToFaceExpression == faceExpression) ?
                    1.0f : 0.0f;
            }

            InitializeCachedValues();
            UpdateCachedMeshValues();
            if (_correctivesModule != null && CorrectivesEnabled)
            {
                _correctivesModule.ApplyCorrectives(_cachedBlendshapeValues);
            }

            UpdateSkinnedMesh();
        }

        /// <summary>
        /// Applies correctives to values before updating the skinned mesh.
        /// </summary>
        protected override void Update()
        {
            /*if (ExpressionWeights == null ||
                ExpressionWeights.Length != (int)OVRFaceExpressions.FaceExpression.Max)
            {
                InitializeExpressionWeights();
            }

            if (_faceExpressions.enabled &&
                _faceExpressions.FaceTrackingEnabled &&
                _faceExpressions.ValidExpressions)
            {
                UpdateExpressionWeights();
                InitializeCachedValues();
                UpdateCachedMeshValues();
                if (_correctivesModule != null && CorrectivesEnabled)
                {
                    _correctivesModule.ApplyCorrectives(_cachedBlendshapeValues);
                }
                UpdateSkinnedMesh();
            }*/
        }

        /// <summary>
        /// Update the expression weight values from face tracking data.
        /// </summary>
        protected void UpdateExpressionWeights()
        {
            if (FreezeExpressionWeights)
            {
                return;
            }

            int numBlendshapes = SkinnedMesh.sharedMesh.blendShapeCount;
            for (int blendShapeIndex = 0; blendShapeIndex < numBlendshapes; ++blendShapeIndex)
            {
                OVRFaceExpressions.FaceExpression blendShapeToFaceExpression = GetFaceExpression(blendShapeIndex);
                if (blendShapeToFaceExpression >= OVRFaceExpressions.FaceExpression.Max ||
                    blendShapeToFaceExpression < 0)
                {
                    continue;
                }
                ExpressionWeights[(int)blendShapeToFaceExpression] = _faceExpressions[blendShapeToFaceExpression];
            }
        }

        /// <summary>
        /// Initialize cached blendshape values.
        /// </summary>
        protected void InitializeCachedValues()
        {
            int numBlendshapes = SkinnedMesh.sharedMesh.blendShapeCount;
            if (_cachedBlendshapeValues == null)
            {
                _cachedBlendshapeValues = new float[numBlendshapes];
                for (int i = 0; i < numBlendshapes; i++)
                {
                    _cachedBlendshapeValues[i] = 0.0f;
                }
            }
        }

        /// <summary>
        /// Update the initialized cached values.
        /// </summary>
        protected void UpdateCachedMeshValues()
        {
            int numBlendshapes = SkinnedMesh.sharedMesh.blendShapeCount;
            for (int blendShapeIndex = 0; blendShapeIndex < numBlendshapes; ++blendShapeIndex)
            {
                OVRFaceExpressions.FaceExpression blendShapeToFaceExpression =
                    GetFaceExpression(blendShapeIndex);
                if (blendShapeToFaceExpression >= OVRFaceExpressions.FaceExpression.Max ||
                    blendShapeToFaceExpression < 0)
                {
                    continue;
                }

                float currentWeight = ExpressionWeights[(int)blendShapeToFaceExpression];
                // Recover true eyes closed values
                if (blendShapeToFaceExpression == OVRFaceExpressions.FaceExpression.EyesClosedL)
                {
                    currentWeight += ExpressionWeights[(int)OVRFaceExpressions.FaceExpression.EyesLookDownL];
                }
                else if (blendShapeToFaceExpression == OVRFaceExpressions.FaceExpression.EyesClosedR)
                {
                    currentWeight += ExpressionWeights[(int)OVRFaceExpressions.FaceExpression.EyesLookDownR];
                }
                else if (ForceJawDropForTongue && blendShapeToFaceExpression == OVRFaceExpressions.FaceExpression.JawDrop)
                {
                    // Fetch this from the underlying expressions in case the renderer being fixed
                    // has JawDrop but not TongueOut.
                    var tongueWeight = ExpressionWeights[(int)OVRFaceExpressions.FaceExpression.TongueOut];
                    if (tongueWeight > _tongueOutThreshold)
                    {
                        currentWeight = Mathf.Max(_minJawDrop, currentWeight);
                    }
                }

                if (_blendshapeModifier != null)
                {
                    currentWeight = _blendshapeModifier.GetModifiedWeight(blendShapeToFaceExpression, currentWeight);
                }
                _cachedBlendshapeValues[blendShapeIndex] = currentWeight * BlendShapeStrengthMultiplier;
            }
        }

        /// <summary>
        /// Update the skinned mesh with the cached blendshape values.
        /// </summary>
        protected void UpdateSkinnedMesh()
        {
            var numBlendshapes = _cachedBlendshapeValues.Length;
            for (int blendShapeIndex = 0; blendShapeIndex < numBlendshapes; ++blendShapeIndex)
            {
                Debug.LogError($"--- UpdateExpressionWeightFromRemote(), _cachedBlendshapeValues[{blendShapeIndex}]: {_cachedBlendshapeValues[blendShapeIndex]} ");
                SkinnedMesh.SetBlendShapeWeight(blendShapeIndex, _cachedBlendshapeValues[blendShapeIndex]);
            }
        }

        //=================================================================================================================================//
        public void UpdateExpressionWeightFromRemote(float[] remoteExpressionWeights)
        {
            Debug.LogError("--- UpdateExpressionWeightFromRemote(), 1");

            // Initialize expression weights if null
            if (ExpressionWeights == null)
            {
                InitializeExpressionWeights();
            }

            Debug.LogError("--- UpdateExpressionWeightFromRemote(), 2");

            int numBlendshapes = SkinnedMesh.sharedMesh.blendShapeCount;
            Debug.LogError("--- UpdateExpressionWeightFromRemote(), numBlendshapes length: " + numBlendshapes);

            // Ensure _cachedBlendshapeValues is initialized once
            if (_cachedBlendshapeValues == null)
            {
                _cachedBlendshapeValues = new float[numBlendshapes];
                for (int i = 0; i < numBlendshapes; i++)
                {
                    _cachedBlendshapeValues[i] = 0.0f;
                }
            }

            // Iterate over the blend shapes and update values
            for (int blendShapeIndex = 0; blendShapeIndex < numBlendshapes; ++blendShapeIndex)
            {
                OVRFaceExpressions.FaceExpression blendShapeToFaceExpression = GetFaceExpression(blendShapeIndex);

                // Skip invalid expressions
                if (blendShapeToFaceExpression >= OVRFaceExpressions.FaceExpression.Max || blendShapeToFaceExpression < 0)
                {
                    continue;
                }

                // Update expression weight from remote data
                ExpressionWeights[(int)blendShapeToFaceExpression] = remoteExpressionWeights[(int)blendShapeToFaceExpression];
                Debug.LogError($"--- UpdateExpressionWeightFromRemote(), ExpressionWeights[{(int)blendShapeToFaceExpression}]: {ExpressionWeights[(int)blendShapeToFaceExpression]} ");

                float currentWeight = ExpressionWeights[(int)blendShapeToFaceExpression];

                // Adjust weights based on specific face expressions
                if (blendShapeToFaceExpression == OVRFaceExpressions.FaceExpression.EyesClosedL)
                {
                    currentWeight += ExpressionWeights[(int)OVRFaceExpressions.FaceExpression.EyesLookDownL];
                }
                else if (blendShapeToFaceExpression == OVRFaceExpressions.FaceExpression.EyesClosedR)
                {
                    currentWeight += ExpressionWeights[(int)OVRFaceExpressions.FaceExpression.EyesLookDownR];
                }
                else if (ForceJawDropForTongue && blendShapeToFaceExpression == OVRFaceExpressions.FaceExpression.JawDrop)
                {
                    var tongueWeight = ExpressionWeights[(int)OVRFaceExpressions.FaceExpression.TongueOut];
                    if (tongueWeight > _tongueOutThreshold)
                    {
                        currentWeight = Mathf.Max(_minJawDrop, currentWeight);
                    }
                }

                // Apply any modifier to the current weight
                if (_blendshapeModifier != null)
                {
                    currentWeight = _blendshapeModifier.GetModifiedWeight(blendShapeToFaceExpression, currentWeight);
                }

                // Store the modified weight in the cached values
                _cachedBlendshapeValues[blendShapeIndex] = currentWeight * BlendShapeStrengthMultiplier;

                Debug.LogError($"--- UpdateExpressionWeightFromRemote(), _cachedBlendshapeValues[{blendShapeIndex}]: {_cachedBlendshapeValues[blendShapeIndex]} ");
                SkinnedMesh.SetBlendShapeWeight(blendShapeIndex, _cachedBlendshapeValues[blendShapeIndex]);
            }

            /*// Apply correctives if enabled
            if (_correctivesModule != null && CorrectivesEnabled)
            {
                _correctivesModule.ApplyCorrectives(_cachedBlendshapeValues);
            }

            // Update the SkinnedMesh with the final weights
            for (int blendShapeIndex = 0; blendShapeIndex < numBlendshapes; ++blendShapeIndex)
            {
                Debug.LogError($"--- UpdateExpressionWeightFromRemote(), _cachedBlendshapeValues[{blendShapeIndex}]: {_cachedBlendshapeValues[blendShapeIndex]} ");
                SkinnedMesh.SetBlendShapeWeight(blendShapeIndex, _cachedBlendshapeValues[blendShapeIndex]);
            }*/
        }

        public float[] PrepareRemoteExpressionWeights()
        {
            if (_faceExpressions == null || !_faceExpressions.ValidExpressions)
            {
                Debug.LogWarning("TTT, Invalid or null face expressions provided to PrepareRemoteExpressionWeights.");
                return new float[(int)OVRFaceExpressions.FaceExpression.Max];
            }

            int numExpressions = (int)OVRFaceExpressions.FaceExpression.Max;
            
            float[] remoteExpressionWeights = new float[numExpressions];

            int numBlendshapes = SkinnedMesh.sharedMesh.blendShapeCount;
            for (int blendShapeIndex = 0; blendShapeIndex < numBlendshapes; ++blendShapeIndex)
            {
                OVRFaceExpressions.FaceExpression blendShapeToFaceExpression = GetFaceExpression(blendShapeIndex);
                if (blendShapeToFaceExpression >= OVRFaceExpressions.FaceExpression.Max ||
                    blendShapeToFaceExpression < 0)
                {
                    continue;
                }
                remoteExpressionWeights[(int)blendShapeToFaceExpression] = _faceExpressions[blendShapeToFaceExpression];
            }

            Debug.Log("TTT, Prepared remote expression weights.");
            return remoteExpressionWeights;
        }
    }
}
