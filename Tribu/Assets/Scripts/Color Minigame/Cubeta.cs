using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cubeta : MonoBehaviour
{
    public Color32 _colorCubeta;

    public void Initialize(CubetaPropiedades data)
    {
        GetComponent<SpriteRenderer>().color = data.Color;
        GetComponent<SpriteRenderer>().sprite = data.Sprite;
        _colorCubeta = data.Color;
    }

}
