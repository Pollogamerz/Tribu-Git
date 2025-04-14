using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dibujo : MonoBehaviour
{
    public void Initialize(CubetaPropiedades data)
    {
        GetComponent<SpriteRenderer>().color = data.Color;
        GetComponent<SpriteRenderer>().sprite = data.Sprite;
        ColorMinigameManager.Instance._currentDrawToColor = data.Color;
    }
}
