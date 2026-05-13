using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Animation;
using UnityEngine;

[CreateAssetMenu(fileName = "MenuGameData", menuName = "MenuGameData")]
public class GameDataMenu : ScriptableObject
{
    [Header("Персонажи в меню")]

    public List<CharactersData> Characters = new List<CharactersData>();

    [Serializable]
    public class CharactersData
    {
        public int id = 0;
        public string CharacterName = "dabydaby";
        public GameObject Character;
        public GameObject Weapon;
        public GameObject Outfit;
        public int bravery = 20;
        public int sapience = 20;
        public int excerpt = 20;
        public int justice = 20;
        public int lvl = 1;

    }

    [Serializable]
    public class AnomalyData
    {
        public int id = 0;
        public string towerName = "Base";
        public GameObject Anomaly;
        public int clepot = 0;
        public string danger = "Zaian";
        public int lvl = 1;

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
        public List<AnomalyData> Anomaly = new List<AnomalyData>();
        public int extensions = 1;//расширения депортамента

    }

}