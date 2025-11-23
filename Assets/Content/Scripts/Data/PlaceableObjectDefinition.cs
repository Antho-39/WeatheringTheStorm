using UnityEngine;

[CreateAssetMenu(fileName = "PlaceableObject", menuName = "Game/Placeable Object\"")]
public class PlaceableObjectDefinition : ScriptableObject
{
    public string id;
    public GameObject prefab;
    public int cost;
}
