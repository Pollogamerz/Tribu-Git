using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dibujo : MonoBehaviour
{
    public Color32 _colorDeseado;

    public void Initialize(CubetaPropiedades data)
    {
        GetComponent<SpriteRenderer>().color = data.Color;
        GetComponent<SpriteRenderer>().sprite = data.Sprite;
        _colorDeseado = data.Color;
    }
}
