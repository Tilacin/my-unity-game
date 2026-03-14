using UnityEngine;

public class Skins : MonoBehaviour
{
    [SerializeField] private SkinData[] _skinsData;

    public int Length => _skinsData.Length;

    public Material GetMaterial(int index)
    {
        if (_skinsData == null || _skinsData.Length == 0) return null;
        if (index < 0 || index >= _skinsData.Length) index = 0;
        return _skinsData[index].material;
    }

    // Для магазина потом
    public SkinData GetSkinData(int index)
    {
        if (index < 0 || index >= _skinsData.Length) return null;
        return _skinsData[index];
    }
}
