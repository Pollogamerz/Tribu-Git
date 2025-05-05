using UnityEngine;

[CreateAssetMenu(fileName = "Cubeta de color", menuName = "Cubeta/Color", order = 0)]
public class CubetaPropiedades : ScriptableObject
{
    public string NombreColor;
    public Sprite Sprite;
    public Color32 Color;
    [TextArea(2, 4)] public string Descripcion;
    public bool IsCombined;
}
