using System.IO;
using UnityEditor;
using UnityEngine;

namespace MuteMarker.Scripts.Editor
{
    [CustomEditor(typeof(MuteMarkerSetup))]
    public partial class MuteMarkerEditor : UnityEditor.Editor
    {
        private string rootPath;
        private string outPath;
        
        private int toolbar;
        
        public bool ambidextrous;
        public bool righthanded;
        public bool overloadedParams;

        private SerializedProperty _avatar;
        private SerializedProperty _lifetime;
        private SerializedProperty _size;
        private SerializedProperty _emissionPower;

        private void OnEnable()
        {
            MuteMarkerSetup script = (MuteMarkerSetup) target;
            
            if (PrefabUtility.IsAnyPrefabInstanceRoot(script.gameObject))
                PrefabUtility.UnpackPrefabInstance(script.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);

            string path = AssetDatabase.GetAssetPath(script.root);
            if (!path.Equals(""))
                rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(script.root));

            _avatar = serializedObject.FindProperty(nameof(script.avatar));
            _lifetime = serializedObject.FindProperty(nameof(script.lifetime));
            _size = serializedObject.FindProperty(nameof(script.size));
            _emissionPower = serializedObject.FindProperty(nameof(script.emissionPower));
        }

        public override void OnInspectorGUI()
        {
            MuteMarkerSetup script = (MuteMarkerSetup) target;

            string[] text = new string[] {"Mute Marker Installer"};
            toolbar = GUILayout.Toolbar(toolbar, text);

            Inspector();
            ParameterInspector();

            bool error = Check();
            
            EditorGUI.BeginDisabledGroup(error);
            if (GUILayout.Button("Apply/Update Marker"))
            {
                outPath = rootPath + "/a.out/" + script.avatar.name;
                
                CreateFolders(outPath);
                
                RemovePrefab();
                RemoveAnimator();
                RemoveMenu();
                RemoveParameter();
                RemoveMarkers();
                
                SetPrefab();
                CreateMaterial();
                CreateAnimator();
                UpdateDescriptor();
                CreateMenu();
                CreateParameter();
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Remove Marker"))
            {
                RemovePrefab();
                RemoveAnimator();
                RemoveMenu();
                RemoveParameter();
                RemoveMarkers();
            }
        }
    }
}