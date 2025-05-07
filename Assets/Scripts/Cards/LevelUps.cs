using System.Collections.Generic;
using UnityEngine;
using System;

public enum CardTypeEnum
{
    Common = 8,
    Epic = 5,
    Legendary = 3
}

[CreateAssetMenu(fileName = "NewLevelUp",menuName = "Cards/NewLevelUp",order = 1)]
public class LevelUps : ScriptableObject
{
    public string Name;
    public CardTypeEnum Type;
    public List<LevelCost> Costs = new List<LevelCost>();

    public void Init()
    {
        Name = name;
        SetType();

        for (int i = 0; i < (int)Type; i++)
        {
            Costs.Add(new LevelCost());
        }
    }

    private void SetType()
    {
        var typeNames = Enum.GetNames(typeof(CardTypeEnum));
        var typeValues = Enum.GetValues(typeof(CardTypeEnum));

        for (int i = 0; i < typeNames.Length; i++)
        {
            if(Name == typeNames[i])
            {
                Type = (CardTypeEnum)typeValues.GetValue(i);
            }
        }
    }

}

[Serializable]
public class LevelCost
{
    public int CardsCount;
    public int GoldCount;
}
