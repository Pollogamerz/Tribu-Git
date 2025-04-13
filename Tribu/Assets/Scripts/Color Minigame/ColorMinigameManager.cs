using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorMinigameManager : MonoBehaviour
{
    public static ColorMinigameManager Instance;

    [Header("Configuracion de colores")]
    [SerializeField] private GameObject _cubetasPrefab;
    [SerializeField] private CubetaPropiedades[] _cubetaPropiedades;
    [SerializeField] private Transform[] _cubetasSpawners;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        SetUpColors();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetUpColors()
    {
        int i = 0;
        foreach (var data in _cubetaPropiedades)
        {
            var Bucket = Instantiate(_cubetasPrefab, _cubetasSpawners[i]);
            Bucket.GetComponent<Cubeta>().Initialize(data);
            i++;
        }
    }
}

public enum Fase_Color
{
    Primer_Color,
    Segundo_Color,
    Tercer_Color,
    Desafio_Color,
    Fiesta_Color
}
