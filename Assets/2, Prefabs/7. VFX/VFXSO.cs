
using UnityEngine;

namespace VFX_SO
{
    public enum VfxType
    {
        SSS = 0,
        SS,
        S,
        A,
        B,
        C,
        D,
        E,
        F,
    };
    [CreateAssetMenu(fileName = "Vfx Data", menuName = "ScriptableObjects/Vfx Data", order = int.MaxValue)]
    public class VFXData : ScriptableObject
    {
        public VfxType vfxType = VfxType.A;
        public Color color = Color.white;
        public Vector3 lightSize = Vector3.one;
        public Vector3 ringSize = Vector3.one;
        public Vector3 starSize = Vector3.one;
        public Vector2 starOffset = new Vector2(90, 90);
    }
}