using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerScript : MonoBehaviour
{
    MasterScript masterScript;
    [SerializeField]
    bool Weight;
    // Start is called before the first frame update
    void Awake()
    {
        masterScript = FindObjectOfType<MasterScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Coin")
        {
            masterScript.EvaluateAnswer(int.Parse(other.name.Replace("Coin", "")), Weight);
        }
    }
}
