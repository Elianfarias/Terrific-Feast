using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObjects/PlayerSettings")]
public class PlayerDataSO : ScriptableObject
{
    public float volumeMusic;
    public float volumeSFX;
    public float volumeUI;
}