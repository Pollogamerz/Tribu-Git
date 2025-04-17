using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColorMinigameManager : MonoBehaviour
{
    public static ColorMinigameManager Instance;

    [Header("Fase del minijuego")]
    public Color32 _currentDrawToColor;
    public Color32 _currentColorChoose;
    public Fase_Color _currentFaseColor = Fase_Color.Primer_Color;
    private bool _isFaseCompleted = false;
    private GameObject _currentDrawing;
    private int _desafioIndex = 0;

    [Header("Configuracion de colores")]
    [SerializeField] private GameObject _cubetasPrefab;
    [SerializeField] private CubetaPropiedades[] _cubetaPropiedades;
    [SerializeField] private Transform[] _cubetasSpawners;
    private readonly List<GameObject> _colores = new List<GameObject>();

    [Header("Configuracion de Dibujos")]
    [SerializeField] private GameObject _dibujoPrefab;
    [SerializeField] private List<CubetaPropiedades> _dibujos;
    [SerializeField] private Transform[] _dibujoSpawner;

    [Header("Audio")]
    [SerializeField] private AudioClip _correctAnswerSFX;
    [SerializeField] private AudioClip _incorrectAnswerSFX;

    [Header("Pista")]
    [SerializeField] private GameObject _hintPanel;
    [SerializeField] private TMP_Text _drawingHint;

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
        if (_isFaseCompleted)
        {
            FaseSelection();
        }
    }

    public void CheckColors()
    {
        if (CompareColors(_currentColorChoose, _currentDrawToColor))
        {
            CorrectColor();
        }
        else
        {
            WrongColor();
        }
    }

    private void CorrectColor()
    {
        if(_currentFaseColor != Fase_Color.Desafio_Color || _desafioIndex > 3)
        {
            AudioManager.instance.PlaySFX(_correctAnswerSFX);
            _currentFaseColor++;
            _isFaseCompleted = true;
            Destroy(_currentDrawing);
        }
        else
        {
            AudioManager.instance.PlaySFX(_correctAnswerSFX);
            Destroy(_currentDrawing);
            _desafioIndex++;
            SpawnDrawing(_dibujos[Random.Range(0, 2)], _dibujoSpawner[1]);
        }

        if (_currentFaseColor == Fase_Color.Fiesta_Color) {
            AudioManager.instance.PlaySFX(_correctAnswerSFX);
            FiestaColorFunction();
        }

    }

    private void WrongColor()
    {
        AudioManager.instance.PlaySFX(_incorrectAnswerSFX);
        Debug.Log("Color equivocado");
    }

    private void SetUpColors()
    {
        int i = 0;
        foreach (var data in _cubetaPropiedades)
        {
            var Bucket = Instantiate(_cubetasPrefab, _cubetasSpawners[i]);
            Bucket.GetComponent<Cubeta>().Initialize(data);
            _colores.Add(Bucket);
            i++;
        }
    }

    private void SpawnDrawing(CubetaPropiedades data, Transform position)
    {
        var dibujo = Instantiate(_dibujoPrefab, position);
        dibujo.GetComponent<Dibujo>().Initialize(data);
        _currentDrawing = dibujo;
        _drawingHint.text = data.Descripcion;
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

    public static bool CompareColors(Color32 a, Color32 b)
    {
        return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
    }

    private void FiestaColorFunction()
    {
        _hintPanel.SetActive(false);
        for (int i = 0; i < _cubetaPropiedades.Length; i++)
        {
            _colores[i].SetActive(false);
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
