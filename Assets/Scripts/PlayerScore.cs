using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerScore
{
    public GameObject player;
    public int score = 0;


    public PlayerScore(GameObject player)
    {
        this.player = player;
    }

    // Methode zum Hinzuf�gen von Punkten f�r Spieler 1
    public void addScore(int points)
    {
        score += points;
    }

    // Methode zum Hinzuf�gen von Punkten f�r Spieler 2
    public void reduceScore(int points)
    {
        score -= points;
    }

    public GameObject getPlayer()
    {
        return this.player;
    }

    public int getScore()
    {
        return score;
    }

    public TargetEventChecker getTargetEventChecker()
    {

        return player.GetComponent<TargetEventChecker>();
    }
}