using UnityEditor;
using UnityEngine;

namespace MuteMarker.Scripts.Editor
{
    public partial class MuteMarkerEditor
    {
        private void Inspector()
        {
            GUIStyle style = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = Color.magenta
                }
            };
            MuteMarkerSetup script = (MuteMarkerSetup)target;

            GUILine();
            EditorStyles.label.wordWrap = true;
            
            EditorGUILayout.LabelField(
                "This sets up the marker for you!  Just Choose the color, ink lifetime (in seconds), and size you prefer - then hit Apply.  The defaults should be sensible for most VRChat users.");

            GUILine();
            EditorGUILayout.LabelField("To make changes, just come back here, adjust what you wish then hit apply again.");

            GUILine();
            EditorGUILayout.LabelField(
                "There may be updates in the future to add radial menus with additional features, but for now it's simple and sweet. Enjoy!");
            
            GUILine();
            GUILayout.Label("Maybe important notes:", style);
            EditorGUILayout.LabelField("This Marker requires 9 or 10 bits in your params file.  By default, it leaves any existing params/menu/animator controllers untouched and builds new ones that are placed in /MuteMarker/a.out/");
            
            GUILine();
            EditorGUILayout.PropertyField(_avatar, new GUIContent("Avatar"));

            GUILine();
            style.normal.textColor = Color.cyan;
            
            GUILayout.Label("Marker Settings", style);

            EditorGUILayout.Slider(_lifetime, 15, 360, new GUIContent("Ink Lifetime"));
            EditorGUILayout.Slider(_size, 0, 1, new GUIContent("Ink Size"));
            EditorGUILayout.Slider(_emissionPower, 0, 10, new GUIContent("Emission Power"));
            
            GUILine();
            script.chooseColor = EditorGUILayout.ColorField(new GUIContent("Ink Color"), script.chooseColor);
            GUILine();
            EditorGUILayout.LabelField("If you're trying to squeeze into a better performance ranking, disabling your non-dominant hand will save a couple material slots:");
            GUILine();
            script.rightHanded = EditorGUILayout.Toggle(new GUIContent("Right Hand Enable"),script.rightHanded);
            script.leftHanded = EditorGUILayout.Toggle(new GUIContent("Left Hand Enable"), script.leftHanded);
            GUILine();
            style.normal.textColor = Color.magenta;
            GUILayout.Label("Advanced:", style);
            EditorGUILayout.LabelField("This setting will apply the updates directly to your already-existing layers and expressions.  'backups' folders will be created where these files currently exist in case you need to revert");
            GUILine();
            script.applyInPlace = EditorGUILayout.Toggle(new GUIContent("In-Place Updates"), script.applyInPlace);
            GUILine();
            serializedObject.ApplyModifiedProperties();
        }
        
        private void ParameterInspector()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.green;
            MuteMarkerSetup script = (MuteMarkerSetup) target;
            
            GUILine();

            if (script.avatar && script.avatar.expressionParameters)
            {
                overloadedParams = false;
                
                int usedParams = script.avatar.expressionParameters.CalcTotalCost();
                int remainingParams = 256 - usedParams;
                int requiredParams = ambidextrous ? 10 : 9;
                int emptyParams = MuteMarkerMenuTool.EmptyParamCount(script.avatar.expressionParameters);
                int markerParamCount = MuteMarkerMenuTool.MarkerParamCount(script.avatar.expressionParameters);

                if (remainingParams < requiredParams)
                {
                    if ((markerParamCount < requiredParams) && (remainingParams == 0))
                    {
                        style.normal.textColor = Color.red;
                        overloadedParams = true;
                    }
                    else if (markerParamCount == 0)
                    {
                        style.normal.textColor = Color.red;
                        overloadedParams = true;
                    }
                }

                GUILayout.Label("Used Parameters: " + usedParams + " / 128", style);
                GUILayout.Label("Parameters Required: " + requiredParams, style);
                GUILayout.Label("Empty Parameters: " + emptyParams, style);

                GUILine();
            }
        }
        
        private bool Check()
        {
            bool result = false;
            MuteMarkerSetup script = (MuteMarkerSetup)target;

            if (!script.root || rootPath == null)
            {
                EditorGUILayout.HelpBox("Root", MessageType.Error);
                result = true;
            }
            
            if (overloadedParams)
            {
                EditorGUILayout.HelpBox("Too many parameters!  Check for empty slots.", MessageType.Error);
                result = true;
            }
            
            if (!script.avatar)
            {
                EditorGUILayout.HelpBox("Avatar is not specified.", MessageType.Error);
                result = true;
            }

            if (script.size <= 0)
            {
                EditorGUILayout.HelpBox("Size must be greater than 0.", MessageType.Error);
                result = true;
            }

            ambidextrous = false;
            righthanded = false;
            
            switch (script.rightHanded)
            {
                case true when script.leftHanded:
                {
                    ambidextrous = true;
                    break;
                }
                case true:
                {
                    righthanded = true;
                    break;
                }
                default:
                {
                    if(script.leftHanded)
                        break;
                    EditorGUILayout.HelpBox("Left or right hand?", MessageType.Error);
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
