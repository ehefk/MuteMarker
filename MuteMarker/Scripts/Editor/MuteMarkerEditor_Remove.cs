using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace MuteMarker.Scripts.Editor
{
    public partial class MuteMarkerEditor
    {
        private void RemovePrefab()
        {
            MuteMarkerSetup script = (MuteMarkerSetup) target;

            Transform markerPrefab = script.avatar.transform.Find("MuteMarker");
            if (markerPrefab)
            {
                DestroyImmediate(markerPrefab.gameObject);
            }
        }

        private void RemoveAnimator()
        {
            MuteMarkerSetup script = (MuteMarkerSetup) target;
            
            if (script.avatar.customExpressions == false)
                return;

            RuntimeAnimatorController fx = script.avatar.baseAnimationLayers[4].animatorController;
            if (fx)
            {
                AnimatorController animatorController = (AnimatorController)fx;
                MuteMarkerAnimTool.RemoveLayer(animatorController, "Trigger Left");
                MuteMarkerAnimTool.RemoveLayer(animatorController, "Trigger Right");
                MuteMarkerAnimTool.RemoveLayer(animatorController, "Marker Right");
                MuteMarkerAnimTool.RemoveLayer(animatorController, "Marker Left");
                MuteMarkerAnimTool.RemoveLayer(animatorController, "Marker Grab Right");
                MuteMarkerAnimTool.RemoveLayer(animatorController, "Marker Grab Left");

                MuteMarkerAnimTool.RemoveParameter(animatorController, "MarkerLeft");
                MuteMarkerAnimTool.RemoveParameter(animatorController, "MarkerRight");
                MuteMarkerAnimTool.RemoveParameter(animatorController, "MarkerGesture");
            }
        }
        
        private void RemoveMenu()
        {
            MuteMarkerSetup script = (MuteMarkerSetup)target;

            if (!script.avatar.expressionsMenu)
                return;

            VRCExpressionsMenu.Control control = MuteMarkerMenuTool.MenuFindControl(script.avatar.expressionsMenu, "Marker (Right)");
            if (control != null)
                script.avatar.expressionsMenu.controls.Remove(control);
            
            control = MuteMarkerMenuTool.MenuFindControl(script.avatar.expressionsMenu, "Marker (Left)");
            if (control != null)
                script.avatar.expressionsMenu.controls.Remove(control);
            
            control = MuteMarkerMenuTool.MenuFindControl(script.avatar.expressionsMenu, "Gesture Select");
            if (control != null)
                script.avatar.expressionsMenu.controls.Remove(control);

            control = MuteMarkerMenuTool.MenuFindControl(script.avatar.expressionsMenu, "Mute Marker");
            if (control != null)
                script.avatar.expressionsMenu.controls.Remove(control);
            
            EditorUtility.SetDirty(script.avatar.expressionsMenu);
        }
        
        private void RemoveParameter()
        {
            MuteMarkerSetup script = (MuteMarkerSetup)target;

            if (!script.avatar.expressionParameters)
                return;

            MuteMarkerMenuTool.RemoveParameter(script.avatar.expressionParameters, "MarkerLeft");
            MuteMarkerMenuTool.RemoveParameter(script.avatar.expressionParameters, "MarkerRight");
            MuteMarkerMenuTool.RemoveParameter(script.avatar.expressionParameters, "MarkerGesture");
            
            EditorUtility.SetDirty(script.avatar.expressionParameters);
        }

        private void RemoveMarkers()
        {
            MuteMarkerSetup script = (MuteMarkerSetup) target;

            Animator fxAnimator = script.avatar.GetComponent<Animator>();

            Transform rightIndex = fxAnimator.GetBoneTransform(HumanBodyBones.RightHand);
            Transform leftIndex = fxAnimator.GetBoneTransform(HumanBodyBones.LeftHand);

            Transform rightMarker = rightIndex.Find("RightMarker");
            Transform leftMarker = leftIndex.Find("LeftMarker");

            if (rightMarker)
            {
                DestroyImmediate(rightMarker.gameObject);
                DestroyImmediate(leftMarker.gameObject);
            }
        }
    }
}