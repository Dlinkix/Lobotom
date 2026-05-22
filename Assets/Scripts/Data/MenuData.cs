using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "MenuGameData", menuName = "MenuGameData")]
public class GameDataMenu : ScriptableObject
{
    [Header("Персонажи в меню")]

    public List<CharactersData> Characters = new List<CharactersData>();


    [Header("Данные дня")]
    public int savedCurrentDay = 1;
    public string savedDayState = "NewDay";
    public void ForceSave()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    [Serializable]
    public class CharactersData
    {
        public int id = 0;
        public string startRoomID;
        public string CharacterName = "dabydaby";
        public GameObject Character;
        public GameObject Weapon;
        public GameObject Outfit;
        public int bravery = 20;
        public int sapience = 20;
        public int excerpt = 20;
        public int justice = 20;
        public int lvl = 1;

        [Header("Уровни (вычисляются автоматически)")]
        public int braveryLevel;
        public int sapienceLevel;
        public int excerptLevel;
        public int justiceLevel;

        public void CalculateLevels()
        {
            braveryLevel = GetLevelFromPoints(bravery);
            sapienceLevel = GetLevelFromPoints(sapience);
            excerptLevel = GetLevelFromPoints(excerpt);
            justiceLevel = GetLevelFromPoints(justice);
        }

        private int GetLevelFromPoints(int points)
        {
            if (points >= 80) return 5;
            if (points >= 60) return 4;
            if (points >= 40) return 3;
            if (points >= 24) return 2;
            return 1;
        }
    }

    [Serializable]
    public class Abnormality
    {
        [Header("Основное")]
        public string abnormalityName;
        public int id;
        public string startRoomID;
        public GameObject prefab;
        public float width;
        public float height;
        public Sprite icon;
        public string Discription;
        public Sprite imageAbnormaly;
        public int lvl = 1;
        public int peBoxReward = 5;
        public string danger = "Zayin";

        [Header("Экипировка")]
        public EGOWeapon egoWeapon = new EGOWeapon();
        public EGOSuit egoSuit = new EGOSuit();
        public EGOGift egoGift = new EGOGift();

        [Header("Типы работ")]
        public WorkType instinctWork = new WorkType();
        public WorkType insightWork = new WorkType();
        public WorkType attachmentWork = new WorkType();
        public WorkType repressionWork = new WorkType();

        [Header("Побег")]
        public bool canEscape = true;
        public float qliphothCounter = 2;
    }

    [System.Serializable]
    public class WorkType
    {
        public string name;
        [Header("Шанс успеха для каждого уровня")]
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

    [Header("Департамент")]

    public List<DepartmentData> Department = new List<DepartmentData>();

    //public List

    [Serializable]
    public class DepartmentData
    {
        public int id = 0;
        public string depatmentName = "FirstDepart";
        public List<CharactersData> Characters = new List<CharactersData>();
        public List<Abnormality> Anomaly = new List<Abnormality>();
        public int extensions = 1;//расширения депортамента

    }

}