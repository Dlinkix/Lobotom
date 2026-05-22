using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbnormalityDatabase", menuName = "Game Data/Abnormality Database")]
public class AbnormalityDatabase : ScriptableObject
{
    [Header("Все возможные аномалии в игре")]
    public List<AbnormalityData> allAbnormalities = new List<AbnormalityData>();
}
[System.Serializable]
public class AbnormalityData
{
    [Header("Основное")]
    public string abnormalityName;
    public int id;
    public string startRoomID;
    public GameObject prefab;
    public float width;
    public float height;
    public Sprite icon;
    public Sprite imageAbnormaly;
    public int lvl = 1;
    public int peBoxReward = 5;
    public string danger = "Zayin";
    public string Discription;
    public EGOSuit egoSuit = new EGOSuit();
    public EGOWeapon egoWeapon = new EGOWeapon();
    public EGOGift egoGift = new EGOGift();  

    [Header("Типы работ")]
    public WorkTypeData instinctWork = new WorkTypeData();
    public WorkTypeData insightWork = new WorkTypeData();
    public WorkTypeData attachmentWork = new WorkTypeData();
    public WorkTypeData repressionWork = new WorkTypeData();

    [Header("Побег")]
    public bool canEscape = true;
    public float qliphothCounter = 2;
}

[System.Serializable]
public class WorkTypeData
{
    public string name;
    [Range(0f, 1f)] public float level1SuccessChance = 0.8f;
    [Range(0f, 1f)] public float level2SuccessChance = 0.8f;
    [Range(0f, 1f)] public float level3SuccessChance = 0.8f;
    [Range(0f, 1f)] public float level4SuccessChance = 0.8f;
    [Range(0f, 1f)] public float level5SuccessChance = 0.8f;

    public float GetSuccessChance(int level)
    {
        switch (level)
        {
            case 1: return level1SuccessChance;
            case 2: return level2SuccessChance;
            case 3: return level3SuccessChance;
            case 4: return level4SuccessChance;
            case 5: return level5SuccessChance;
            default: return 0.5f;
        }
    }
}
[System.Serializable]
public class EGOWeapon
{
    public string name;
    public int damage;
    public int range;
    public float attackSpeed;
}

[System.Serializable]
public class EGOSuit
{
    public string name;
    public int cost = 24;
    public int defense;
    public int resistanceRad;
    public int resistanceWhite;
    public int resistanceBlack;
    public int resistancePale;
}

[System.Serializable]
public class EGOGift
{
    public string name;
    public string slot;
    public int statBonus;
}
