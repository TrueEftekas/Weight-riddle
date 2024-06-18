using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanCollisionScript : MonoBehaviour
{
    [SerializeField]
    List<int> Coins = new List<int>();

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Coin")
        {
            Coins.Add(int.Parse(other.name.Replace("Coin", "")));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Coin")
        {
            Coins.Remove(int.Parse(other.name.Replace("Coin", "")));
        }
    }

    public List<int> GetCoins()
    {
        return Coins;
    }

    public void ClearCoins()
    {
        Coins.Clear();
    }
}
