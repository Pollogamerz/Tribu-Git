using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cubeta : MonoBehaviour
{
    public string _nombreColor;
    public Color32 _colorCubeta;
    public bool _isCombined = false;

    public void Initialize(CubetaPropiedades data)
    {
        GetComponent<SpriteRenderer>().color = data.Color;
        GetComponent<SpriteRenderer>().sprite = data.Sprite;
        _colorCubeta = data.Color;
        _nombreColor = data.NombreColor;
        _isCombined = data.IsCombined;
    }

}
