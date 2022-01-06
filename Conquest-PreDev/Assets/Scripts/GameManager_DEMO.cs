using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_DEMO : MonoBehaviour
{
    public static GameObject[] players;
    Transform spawns;
    public int preptime;
    public int maxtime;
    public float matchtime = 0f;
    bool started = false;
    // Start is called before the first frame update

    public enum UnitState
    {
        Defending,
        Attacking,
        Following
    }

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

    }
    private void Awake()
    {
        matchtime -= preptime;
    }

    public static int CountPlayers()
    {
        int slot = 1;

        GameObject[] s = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in s)
        {
            var pm = player.GetComponent<PlayerManager>();
            pm.playerNumber = slot;
            slot++;
        }

        int playerCount = s.Length;

        return playerCount;
    }

    void StartGame()
    {
        started = true;
        Debug.Log("The Game Has Started!!");

        //eventually have the 2d players array represent each team but for now hard coede


    }

    public static Vector3 GetPlayerPos(int pidx)
    {
        Vector3 pos = players[pidx].transform.position;
        return pos;
    }

    private void Update()
    {
        matchtime += Time.deltaTime;
        if (matchtime > 0 && !started)
        {
            StartGame();
        }
    }

}
