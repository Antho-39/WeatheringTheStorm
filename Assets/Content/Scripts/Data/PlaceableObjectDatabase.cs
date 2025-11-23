using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlaceableObjectDatabase", menuName = "Game/Placeable Database")]
public class PlaceableObjectDatabase : ScriptableObject
{
    public List<PlaceableObjectDefinition> objects;

    public PlaceableObjectDefinition GetById(string id)
    {
        return objects.Find(o => o.id == id);
    }
}