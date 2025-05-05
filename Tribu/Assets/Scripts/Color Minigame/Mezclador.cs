using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mezclador : MonoBehaviour
{
    public static Mezclador Instance;

    public string _firstColor = "", _secondColor = "";
    public BaseDeCombinaciones baseDeCombinaciones;

    [SerializeField] private GameObject _prefabCubeta;
    private Dictionary<string, CubetaPropiedades> combinacionesDic;

    private void Awake()
    {
        combinacionesDic = new Dictionary<string, CubetaPropiedades>();

        foreach (var combinacion in baseDeCombinaciones.combinaciones)
        {
            string clave = GenerarClave(combinacion.colorA, combinacion.colorB);
            if (!combinacionesDic.ContainsKey(clave))
            {
                combinacionesDic.Add(clave, combinacion.Propiedades);
            }
        }
    }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }

    private string GenerarClave(string a, string b)
    {
        return string.Compare(a, b) < 0 ? $"{a}_{b}" : $"{b}_{a}";
    }

    private void IntentarInstanciar(string nombre1, string nombre2, Vector3 posicion)
    {
        string clave = GenerarClave(nombre1, nombre2);

        if (combinacionesDic.TryGetValue(clave, out CubetaPropiedades propiedades))
        {
            var cubeta = Instantiate(_prefabCubeta, posicion, Quaternion.identity);
            cubeta.GetComponent<Cubeta>().Initialize(propiedades);
            cubeta.GetComponent<Cubeta>()._isCombined = true;
            Debug.Log($"Instanciado prefab por combinación: {clave}");
            _firstColor = "";
            _secondColor = "";
        }
        else
        {
            Debug.Log("Combinación no válida");
        }
    }

    public void SpawnNewColor()
    {
        IntentarInstanciar(_firstColor,_secondColor,transform.position);
    }
}

[System.Serializable]
public class CombinacionData
{
    public string colorA;
    public string colorB;
    public CubetaPropiedades Propiedades;
}

[CreateAssetMenu(fileName = "BaseDeCombinaciones", menuName = "Color/Base de Combinaciones")]
public class BaseDeCombinaciones : ScriptableObject
{
    public List<CombinacionData> combinaciones;
}