﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Round : MonoBehaviour {

    private class UnitToSpawn
    {
        public GameObject unit;
        public Vector2 position;
        public uint timeBeforSpawn;
        public int faction;
        public Element element;
        public int chemin;
        public int etape;
    }
    private List<UnitToSpawn> unitsToSpawn = new List<UnitToSpawn>();

    private GameObject unitsContainer;

    private uint time;
    public uint nbRound = 0;
    public uint nbFramesAvantDebut;
    private uint compteurFrames;
    private Text zoneTexte;

    // Use this for initialization
    void Start() {
        unitsContainer = new GameObject("Units");
        unitsContainer.transform.parent = transform;
        compteurFrames = 0;
        zoneTexte = GameObject.FindGameObjectWithTag("Description").GetComponent<Text>();
    }

    void startRound() {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            AI artificialIntelligence = player.GetComponent<AI>();
            if (artificialIntelligence != null)
            {
                artificialIntelligence.beginRound();
            }
        }
        System.Threading.Thread.Sleep(500);
        GameObject[] barracks = GameObject.FindGameObjectsWithTag("barrack");
        foreach (GameObject barrack in barracks)
        {
            Baraquement barrackScript = barrack.GetComponent<Baraquement>();
            for(uint index = 0; index < barrackScript.nbUnitToSpawnPerRound; ++index)
            {
                UnitToSpawn unitToSpawnToAdd = new UnitToSpawn();
                unitToSpawnToAdd.unit = barrackScript.unitToSpawn;
                unitToSpawnToAdd.position = barrack.transform.position;
                unitToSpawnToAdd.faction = barrackScript.camp;
                unitToSpawnToAdd.element = barrackScript.element;
                unitToSpawnToAdd.chemin = barrackScript.chemin;
                unitToSpawnToAdd.etape = barrackScript.etape;
                unitToSpawnToAdd.timeBeforSpawn = index * barrackScript.spawnInterval;
                unitsToSpawn.Add(unitToSpawnToAdd);
            }
        }
        ++nbRound;
        time = 0;
	}
	
	void FixedUpdate()
    {
        if (!Pause.isPaused)
        {
            if (compteurFrames < nbFramesAvantDebut)
            {
                compteurFrames++;
                zoneTexte.text = "Temps avant le début de la partie : "+(Mathf.Ceil((nbFramesAvantDebut - compteurFrames) / 60.0f).ToString());
            }
            else
            {
                unitsToSpawn.RemoveAll(unitToSpawn => tryToSpawnUnit(unitToSpawn));
                ++time;
                if (isEnd())
                {
                    startRound();
                    foreach (var player in GameObject.FindGameObjectsWithTag("Player")) player.GetComponent<Joueur>().argent += 25;
                }
            }
        }
    }

    bool tryToSpawnUnit(UnitToSpawn unitToTryToSpawn)
    {
        if(unitToTryToSpawn.timeBeforSpawn <= time)
        {
            GameObject newUnit = Instantiate(unitToTryToSpawn.unit);
            newUnit.transform.position = unitToTryToSpawn.position;
            newUnit.GetComponent<Soldat>().camp = unitToTryToSpawn.faction;
            newUnit.transform.parent = unitsContainer.transform;
            newUnit.GetComponent<Soldat>().element = unitToTryToSpawn.element;
            newUnit.GetComponent<Soldat>().chemin = unitToTryToSpawn.chemin;
            newUnit.GetComponent<Soldat>().etape = unitToTryToSpawn.etape;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isEnd()
    {
        return (unitsContainer.GetComponentsInChildren<Soldat>().Length == 0 && unitsToSpawn.Count == 0);
    }
}
