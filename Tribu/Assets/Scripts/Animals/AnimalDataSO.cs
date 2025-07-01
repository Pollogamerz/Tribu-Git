using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimalData", menuName = "Animals/Animal Data")]
public class AnimalDataSO : ScriptableObject
{
    public string animalName;
    public AudioClip animalSound;
    public Sprite animalSprite;
    public string animalCharacteristic;
    public string animalDescription;
    public Sprite animalCharacteristicSprite;
    public Sprite animalIncompleteSprite;
    public RuntimeAnimatorController animalIdleAnimator;
}
