using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManage : MonoBehaviour
{
    [SerializeField]
    public static int l_count;
    [SerializeField]
    public static int r_count;
    [SerializeField]
    public static bool can_check;
    [SerializeField]
    public static bool l_now_state;
    public static bool r_now_state;

    public static int Now_EX_State;

    public static bool can_state;

    public static bool is_time;

    public Text timeText;

    public static float minute;
    public static float second;

    
    

    // Start is called before the first frame update
    void Start()
    {
        can_check = true;
        can_state = false;
    }

    // Update is called once per fram

    void Update()
    {
        time_check();
    }

    public void part_1()
    {

    }

    private void time_check()
    {
        if(is_time)
        {
            second -= Time.deltaTime;
            if(second < 0)
            {
                minute -= 1;
                second = 60;
            }

            timeText.text = minute.ToString() + ":" + ((int)second).ToString();
            if(minute < 0)
            {
                Stop_timer();
            }
        }

    }
    public void Start_timer(int start_min, float start_sec)
    {
        Debug.Log("타이머 시작!");
        is_time = true;
        minute = start_min;
        second = start_sec;
    }
    public void Stop_timer()
    {
        Debug.Log("시간 끝납!");
        minute = 0.0f;
        second = 0.0f;
        is_time = false;
        timeText.text = minute.ToString() + ":" + ((int)second).ToString();
    }

    public void Pause_timer()
    {
        is_time = false;
        Debug.Log("타이머 일시정지!");
        timeText.text = minute.ToString() + ":" + ((int)second).ToString();
    }

    public void Restart_timer()
    {
        is_time = true;
        Debug.Log("타이머 재개");   
    }

    public void Pasue_sec(float sec)
    {
        float time = 0;

        Pause_timer();

        while(time < sec)
        {
            //time += Time.deltatime;
        }

        Restart_timer();
    }
}
