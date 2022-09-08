using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManage : MonoBehaviour
{
    public static DataManage instance;
    
    public List<int> UpperEX = new List<int>();

    public List<int> UnderEX = new List<int>();

    public List<int> WalkEX = new List<int>();

    public List<int> LegupEX = new List<int>();

    public List<int> BireEX = new List<int>();

    public List<int> MuscleEX = new List<int>();

    

    void Awake()
    {
        instance = this;
    }
}
