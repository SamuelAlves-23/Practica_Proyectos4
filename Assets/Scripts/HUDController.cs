using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public TMP_Text criaturas;
    public TMP_Text intrusos;
    public GameObject player;
    public int criaturasR;
    public int intrusosR;
    public GameObject[] enemies;
    public GameObject[] creatures;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        criaturasR = enemies.Length;
        intrusosR = creatures.Length;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void KillCreature()
    {
        criaturasR--;
        criaturas.text = criaturasR.ToString();
    }

    public void KillIntruder()
    {
        intrusosR--;
        intrusos.text = intrusosR.ToString();
    }
}
