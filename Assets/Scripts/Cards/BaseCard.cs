using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard",menuName = "Cards/NewCard",order = 1)]
public class BaseCard : ScriptableObject
{
    public Sprite Icon;
    public string Name;
    public CardTypeEnum Type = CardTypeEnum.Common;
    public int Cost;
    [Multiline] public string Description;

    public List<Properties> Properties;

    public BaseCard()
    {
        Properties = new List<Properties>();
        for (int i = 0; i < (int)Type; i++)
        {
            Properties.Add(new Properties());
        }
    }
}

[Serializable]
public struct Properties
{
    public float Damage;
    public float HitPoint;
    public float Speed;
    public float LifeTime;
    public float Time;
    public float Radius;

    public void Reset()
    {
        Damage = 0f;
        HitPoint = 0f;
        Speed = 0f;
        LifeTime = 0f;
        Time = 0f;
        Radius = 0f;
    }
}
