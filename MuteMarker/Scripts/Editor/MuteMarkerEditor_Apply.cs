using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace MuteMarker.Scripts.Editor
{
    public partial class MuteMarkerEditor
    {
        private static readonly int EmissionPower = Shader.PropertyToID("_EmissionPower");

        private void SetPrefab()
        {
            MuteMarkerSetup script = (MuteMarkerSetup) target;
            GameObject markerPrefab;

            if (ambidextrous)
                markerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(rootPath + "/Prefabs/MuteMarker.prefab");
            else if(righthanded)
                markerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(rootPath + "/Prefabs/MuteMarker_RightHanded.prefab");
            else
                markerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(rootPath + "/Prefabs/MuteMarker_LeftHanded.prefab");

            markerPrefab = Instantiate(markerPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
            markerPrefab.name = "MuteMarker";
            markerPrefab.transform.parent = script.avatar.transform;
            markerPrefab.transform.position = Vector3.zero;
            markerPrefab.transform.rotation = Quaternion.Euler(Vector3.zero);

            Animator fxAnimator = script.avatar.GetComponent<Animator>();

            // Place marker objects on player index fingers
            Transform rightIndex = fxAnimator.GetBoneTransform(HumanBodyBones.RightIndexDistal);
            Transform rightMarkerPosition = markerPrefab.transform.Find("RightMarker");
            rightMarkerPosition.position = rightIndex.position;
            rightMarkerPosition.rotation = rightIndex.rotation;
            rightMarkerPosition.SetParent(fxAnimator.GetBoneTransform(HumanBodyBones.RightHand));

            Transform leftIndex = fxAnimator.GetBoneTransform(HumanBodyBones.LeftIndexDistal);
            Transform leftMarkerPosition = markerPrefab.transform.Find("LeftMarker");
            leftMarkerPosition.position = leftIndex.position;
            leftMarkerPosition.rotation = leftIndex.rotation;
            leftMarkerPosition.SetParent(fxAnimator.GetBoneTransform(HumanBodyBones.LeftHand));

            if (righthanded || ambidextrous)
            {
                Transform rightMarkerSystem = markerPrefab.transform.Find("Marker_R");
                Transform inkRight = rightMarkerSystem.transform.Find("MarkerTip");
                ParticleSystem rightMarker = rightMarkerSystem.Find("Ink").GetComponent<ParticleSystem>();
                ParticleSystem.MainModule mainModule;
                mainModule = rightMarker.main;
                mainModule.startLifetime = script.lifetime;
                mainModule.startColor = script.chooseColor;
                inkRight.transform.localScale = new Vector3(script.size, script.size, script.size);
            }

            if (ambidextrous || !righthanded)
            {
                Transform leftMarkerSystem = markerPrefab.transform.Find("Marker_L");
                Transform inkLeft = leftMarkerSystem.transform.Find("MarkerTip");
                ParticleSystem leftMarker = leftMarkerSystem.Find("Ink").GetComponent<ParticleSystem>();
                ParticleSystem.MainModule mainModule;
                mainModule = leftMarker.main;
                mainModule.startLifetime = script.lifetime;
                mainModule.startColor = script.chooseColor;
                inkLeft.transform.localScale = new Vector3(script.size, script.size, script.size);
            }
        }

        private void UpdateDescriptor()
        {
            MuteMarkerSetup script = (MuteMarkerSetup) target;

            script.avatar.customizeAnimationLayers = true;
            script.avatar.customExpressions = true;
            script.avatar.baseAnimationLayers[4].isDefault = false;

            script.avatar.customExpressions = true;
        }

        private void CreateAnimator()
        {
            MuteMarkerSetup script = (MuteMarkerSetup) target;

            var customAnimLayer = script.avatar.baseAnimationLayers[4];

            AnimatorController newFxController = null;
            if (script.applyInPlace)
            {
                if (customAnimLayer.animatorController)
                {
                    MakeBackup(customAnimLayer.animatorController, "backups");

                    newFxController = (AnimatorController) customAnimLayer.animatorController;
                }
            }
            else
            {
                newFxController = (AnimatorController) CopyAsset(customAnimLayer.animatorController,
                    outPath + "/MuteFX.controller");
            }

            if (!newFxController)
                newFxController = AnimatorController.CreateAnimatorControllerAtPath(outPath + "/MuteFX.controller");

            string markerController = rootPath + "/Anim/MuteMarkerFX.controller";

            AnimatorController markerFX = AssetDatabase.LoadAssetAtPath<AnimatorController>(markerController);
            MuteMarkerAnimTool.CopyLayerWithData(newFxController, markerFX, "Trigger Left", true);
            MuteMarkerAnimTool.CopyLayerWithData(newFxController, markerFX, "Trigger Right", true);
            MuteMarkerAnimTool.CopyLayerWithData(newFxController, markerFX, "Marker Left", true);
            MuteMarkerAnimTool.CopyLayerWithData(newFxController, markerFX, "Marker Right", true);
            MuteMarkerAnimTool.CopyLayerWithData(newFxController, markerFX, "Marker Grab Right", true);
            MuteMarkerAnimTool.CopyLayerWithData(newFxController, markerFX, "Marker Grab Left", true);
            MuteMarkerAnimTool.CopyParameters(newFxController, markerFX);

            script.avatar.baseAnimationLayers[4].animatorController = newFxController;
        }

        private void CreateMenu()
        {
            MuteMarkerSetup script = (MuteMarkerSetup) target;

            VRCExpressionsMenu muteMenu = null;
           
            if (script.applyInPlace)
            {
                if (script.avatar.expressionParameters)
                {
                    MakeBackup(script.avatar.expressionsMenu, "backups");
                    muteMenu = script.avatar.expressionsMenu;
                }
            }
            else
            {
                muteMenu = (VRCExpressionsMenu) CopyAsset(script.avatar.expressionsMenu,
                    outPath + "/MuteMenu.asset");
            }
            
            if (!muteMenu)
                muteMenu = (VRCExpressionsMenu) CopyAsset(rootPath + "/Anim/MuteMenu.asset",
                    outPath + "/MuteMenu.asset");

            VRCExpressionsMenu newMenu = (VRCExpressionsMenu)CopyAsset(rootPath + "/Anim/MuteMarkerMenu.asset",
                outPath + "/MuteMarkerMenu.asset");
            
            VRCExpressionsMenu marker =
                AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(outPath + "/MuteMarkerMenu.asset");
            VRCExpressionsMenu.Control markerControl =
                MuteMarkerMenuTool.MenuCopyControl(muteMenu, marker.controls[0]);
            markerControl.subMenu = newMenu;
            
            //// toggleMenu >> MarkerMenu >> On/Off Toggles to be copied onto MuteMarkerMenu
            VRCExpressionsMenu markerMenu =
                AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(rootPath + "/Anim/MarkerMenu.asset");

            if (righthanded || ambidextrous)
                MuteMarkerMenuTool.MenuCopyControl(marker, markerMenu, "Marker (Right)");

            if (ambidextrous || !righthanded)
                MuteMarkerMenuTool.MenuCopyControl(marker, markerMenu, "Marker (Left)");

            script.avatar.expressionsMenu = muteMenu;
        }

        private void CreateParameter()
        {
            MuteMarkerSetup script = (MuteMarkerSetup) target;

            VRCExpressionParameters newParam = null;
            if (script.applyInPlace)
            {
                if (script.avatar.expressionParameters)
                {
                    MakeBackup(script.avatar.expressionParameters, "backups");
                    newParam = script.avatar.expressionParameters;
                }
            }
            else
            {
                newParam = (VRCExpressionParameters) CopyAsset(script.avatar.expressionParameters,
                    outPath + "/MuteParams.asset");
            }

            if (!newParam)
                newParam = (VRCExpressionParameters) CopyAsset(rootPath + "/Anim/MuteMarkerParams.asset",
                    outPath + "/MuteParams.asset");

            if(ambidextrous || righthanded)
                MuteMarkerMenuTool.AddParameter(newParam, "MarkerRight", VRCExpressionParameters.ValueType.Bool, 0, false);
            if(!righthanded || ambidextrous)
                MuteMarkerMenuTool.AddParameter(newParam, "MarkerLeft", VRCExpressionParameters.ValueType.Bool, 0, false);
            MuteMarkerMenuTool.AddParameter(newParam, "MarkerGesture", VRCExpressionParameters.ValueType.Int, 0,
                false);

            script.avatar.expressionParameters = newParam;
        }

        private void CreateMaterial()
        {
            MuteMarkerSetup script = (MuteMarkerSetup) target;
            
            Material ink = new Material(Shader.Find("MuteMarker/Ink"));
            AssetDatabase.CreateAsset(ink, rootPath + "/mats/Ink.mat");
            
            ink.SetFloat(EmissionPower, script.emissionPower);
        }
    }
}