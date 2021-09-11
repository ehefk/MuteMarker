using System;
using System.Collections.Generic;
using UnityEditor;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace MuteMarker.Scripts.Editor
{
    public static class MuteMarkerMenuTool
    {
        public static VRCExpressionsMenu.Control MenuFindControl(VRCExpressionsMenu source, string name)
        {
            if (!source)
                return null;

            foreach (VRCExpressionsMenu.Control item in source.controls)
            {
                if (item.name.Equals(name))
                    return item;
            }

            return null;
        }

        public static void MenuCopyControl(VRCExpressionsMenu target, VRCExpressionsMenu source,
            string name)
        {
            VRCExpressionsMenu.Control control = MenuFindControl(source, name);

            MenuCopyControl(target, control);
        }

        public static VRCExpressionsMenu.Control MenuCopyControl(VRCExpressionsMenu target,
            VRCExpressionsMenu.Control control)
        {
            VRCExpressionsMenu.Control newControl = new VRCExpressionsMenu.Control()
            {
                icon = control.icon,
                labels = control.labels,
                name = control.name,
                parameter = control.parameter,
                style = control.style,
                subMenu = control.subMenu,
                subParameters = control.subParameters,
                type = control.type,
                value = control.value
            };
            target.controls.Add(newControl);
            EditorUtility.SetDirty(target);

            return newControl;
        }

        public static void AddParameter(VRCExpressionParameters parameters, string name,
            VRCExpressionParameters.ValueType valueType, float defaultValue, bool saved)
        {
            VRCExpressionParameters.Parameter targetParam = parameters.FindParameter(name);

            if (targetParam == null)
            {
                int length = parameters.parameters.Length;
                
                Array.Resize(ref parameters.parameters, length + 1);
                parameters.parameters[length] = new VRCExpressionParameters.Parameter();
                targetParam = parameters.parameters[length];
            }

            targetParam.name = name;
            targetParam.valueType = valueType;
            targetParam.defaultValue = defaultValue;
            targetParam.saved = saved;
            
            EditorUtility.SetDirty(parameters);
        }

        public static void RemoveParameter(VRCExpressionParameters target, string name)
        {
            if (!target)
                return;

            var parameter = target.FindParameter(name);
            if (parameter == null)
                return;

            List<VRCExpressionParameters.Parameter> list =
                new List<VRCExpressionParameters.Parameter>(target.parameters);
            list.Remove(parameter);
            target.parameters = list.ToArray();

            EditorUtility.SetDirty(target);
        }

        public static int EmptyParamCount(VRCExpressionParameters param)
        {
            int count = 0;
            foreach (VRCExpressionParameters.Parameter item in param.parameters)
            {
                if (item.name.Equals("")) count++;
            }

            return count;
        }

        public static int MarkerParamCount(VRCExpressionParameters param)
        {
            int count = 0;
            
            if (param.FindParameter("MarkerGesture") != null)
                count = 8;
            if (param.FindParameter("MarkerLeft") != null)
                count++;
            if (param.FindParameter("MarkerRight") != null)
                count++;

            return count;
        }
    }
}