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

    
    void Set_Data()
    {
        for(int i = 0; i < 4; i++)
        {
            instance.UpperEX.Add(0);
        }
        for(int i = 0; i < 3;i++)
        {
            instance.UnderEX.Add(0);
            instance.WalkEX.Add(0);
            instance.BireEX.Add(0);
        }
        for(int i = 0; i < 2;i++)
        {
            instance.LegupEX.Add(0);
            instance.MuscleEX.Add(0);
        }
    }
    void Awake()
    {
        instance = this;

        Set_Data();        
    }
}
