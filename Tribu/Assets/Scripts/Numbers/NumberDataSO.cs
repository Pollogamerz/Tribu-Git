using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNumberData", menuName = "Numbers/Number Data")]
public class NumberDataSO : ScriptableObject
{
    public int number;
    public string description;
}
