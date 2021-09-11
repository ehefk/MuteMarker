using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace MuteMarker.Scripts
{
    public class MuteMarkerSetup : MonoBehaviour
    {
        public Object root;
        
        public VRCAvatarDescriptor avatar;

        public float lifetime = 100.0f;
        public float size = 0.01f;
        public float emissionPower;

        public bool rightHanded = true;
        public bool leftHanded = true;

        public bool applyInPlace;

        public Color chooseColor = new Color(0, 255, 255, 255);
    }
}