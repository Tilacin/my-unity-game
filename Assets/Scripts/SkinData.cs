using UnityEngine;

[CreateAssetMenu(fileName = "NewSkin", menuName = "Skin")]
public class SkinData : ScriptableObject
{
    public string skinName;
    public int price;
    public Material material;
    public Sprite icon;
    public bool isPurchased;
}
