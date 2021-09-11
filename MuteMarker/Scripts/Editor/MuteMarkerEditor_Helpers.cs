using System.IO;
using UnityEditor;
using UnityEngine;

namespace MuteMarker.Scripts.Editor
{
    public partial class MuteMarkerEditor
    {
        private void CreateFolders(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            string parent = Path.GetDirectoryName(path);
            string child = Path.GetFileName(path);
            CreateFolders(parent);
            AssetDatabase.CreateFolder(parent, child);
        }

        private Object CopyAsset(Object source, string path)
        {
            string assetPath = AssetDatabase.GetAssetPath(source);

            return CopyAsset(assetPath, path);
        }
        
        private Object CopyAsset(string source, string path)
        {
            if (AssetDatabase.CopyAsset(source, path))
            {
                return AssetDatabase.LoadAssetAtPath<Object>(path);
            }

            return null;
        }

        private void MakeBackup(Object backupTarget, string folderName)
        {
            string path = AssetDatabase.GetAssetPath(backupTarget);
            string directory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            string backupPath = directory + "/" + folderName + "/" + fileName;

            CreateFolders(Path.GetDirectoryName(backupPath));
            CopyAsset(backupTarget, backupPath);
        }

        void GUILine()
        {
            EditorGUILayout.LabelField("",GUI.skin.horizontalSlider);
        }
    }
}