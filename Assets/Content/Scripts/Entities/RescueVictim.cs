using UnityEngine;
using System.Collections.Generic;

public class RescueVictim : MonoBehaviour
{
    public float lifetime;
    public string alertText;
    public string rescuedText;
    public SpriteRenderer victimRepresentation;
    public SpriteRenderer minimapIcon;
    public List<AudioClip> voices;
}
