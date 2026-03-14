
using UnityEngine;

public class SetSkin : MonoBehaviour
{
    [System.Serializable]
    public struct SkinPart
    {
        public SkinnedMeshRenderer renderer;    // что меняем
        public Material material;               // на какой материал (опционально)
    }

    [SerializeField] private SkinPart[] _skinParts;

    public void Set(Material material)
    {
        foreach (var part in _skinParts)
        {
            if (part.renderer != null)
                part.renderer.material = material;
        }
    }

    // Если захочешь менять конкретную часть отдельно
    public void SetPart(int index, Material material)
    {
        if (index >= 0 && index < _skinParts.Length && _skinParts[index].renderer != null)
            _skinParts[index].renderer.material = material;
    }
}
