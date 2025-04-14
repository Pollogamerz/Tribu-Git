using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColorMinigameManager : MonoBehaviour
{
    public static ColorMinigameManager Instance;

    [Header("Fase del minijuego")]
    [SerializeField] private GameObject _currentColorChoose;
    [SerializeField] private GameObject _currentDrawToColor;
    public Fase_Color _currentFaseColor = Fase_Color.Primer_Color;
    public bool _isFaseCompleted = false;

    [Header("Configuracion de colores")]
    [SerializeField] private GameObject _cubetasPrefab;
    [SerializeField] private CubetaPropiedades[] _cubetaPropiedades;
    [SerializeField] private Transform[] _cubetasSpawners;

    [Header("Configuracion de Dibujos")]
    [SerializeField] private GameObject _dibujoPrefab;
    [SerializeField] private List<CubetaPropiedades> _dibujos;
    [SerializeField] private Transform[] _dibujoSpawner;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        SetUpColors();
        FaseSelection();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _currentFaseColor++;
            _isFaseCompleted = true;
        }

        if (_isFaseCompleted)
        {
            FaseSelection();
        }
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

    private void SpawnDrawing(CubetaPropiedades data, Transform position)
    {
        var dibujo = Instantiate(_dibujoPrefab, position);
        dibujo.GetComponent<Dibujo>().Initialize(data);
        _currentDrawToColor = dibujo;
    }

    private void FaseSelection()
    {
        switch (_currentFaseColor)
        {
            case Fase_Color.Primer_Color:
                SpawnDrawing(_dibujos[0], _dibujoSpawner[1]);
                _isFaseCompleted = false;
                break;
            case Fase_Color.Segundo_Color:
                SpawnDrawing(_dibujos[1], _dibujoSpawner[1]);
                _isFaseCompleted = false;
                break;
            case Fase_Color.Tercer_Color:
                SpawnDrawing(_dibujos[2], _dibujoSpawner[1]);
                _isFaseCompleted = false;
                break;
            case Fase_Color.Desafio_Color:
                SpawnDrawing(_dibujos[Random.Range(0, 2)], _dibujoSpawner[1]);
                _isFaseCompleted = false;
                break;
            case Fase_Color.Fiesta_Color:
                SpawnDrawing(_dibujos[0], _dibujoSpawner[0]);
                SpawnDrawing(_dibujos[1], _dibujoSpawner[1]);
                SpawnDrawing(_dibujos[2], _dibujoSpawner[2]);
                _isFaseCompleted = false;
                break;
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
