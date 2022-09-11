﻿using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WebCamInput))]
public sealed class BlazePoseSample : MonoBehaviour
{
    [SerializeField]
    public BlazePose.Options options = default;

    public GameObject timeManage;


    [SerializeField]
    private RectTransform containerView = null;
   

    [SerializeField]
    private bool runBackground;     


    public BlazePose pose;
    public PoseDetect.Result poseResult;
    public PoseLandmarkDetect.Result landmarkResult;
    private readonly Vector4[] viewportLandmarks;
    private BlazePoseDrawer drawer;
    
    private Vector4[] landmarks;

    private int Ex_index;


    private bool r_state;
    private bool l_state;
    private UniTask<bool> task;
    private CancellationToken cancellationToken;

    public List<Text> upper_score = new List<Text>();

    public List<Text> under_score = new List<Text>();

    public List<Text> walk_score_total = new List<Text>();

    public List<Text> legup_score_one = new List<Text>();

    public List<Text> bire_score = new List<Text>();

    public List<Text> muscle_score_hip = new List<Text>();
    
    public List<GameObject> Ex_Objcet = new List<GameObject>();

    private void Start()
    {
        
        pose = new BlazePose(options);

        drawer = new BlazePoseDrawer(Camera.main, gameObject.layer, containerView);

        cancellationToken = this.GetCancellationTokenOnDestroy();

        GetComponent<WebCamInput>().OnTextureUpdate.AddListener(OnTextureUpdate);

        Ex_index = 0;

        TimeManage.can_state = false;
    }

    public void Restart_check()
    {
        if(TimeManage.Now_EX_State%2 == 0)
        {            
            TimeManage.Now_EX_State--;
        }
        timeManage.GetComponent<TimeManage>().lnit_timer();
    }

    private void OnTextureUpdate(Texture texture)
    {
        if (runBackground)
        {
            if (task.Status.IsCompleted())
            {
                task = InvokeAsync(texture);
            }
        }
        else
        {
            Invoke(texture);
        }
    }

    private float Calcul_angle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        List<float> x = new List<float>();
        List<float> y = new List<float>();

        x.Add((float)p1.x);
        x.Add((float)p2.x);
        x.Add((float)p3.x);
        y.Add((float)p1.y);
        y.Add((float)p2.y);
        y.Add((float)p3.y);

        Vector3 V1 = new Vector3();
        Vector3 V2 = new Vector3();

        V1.z = V2.z = 0;
        V1.x = x[0] - x[1];
        V1.y = y[0] - y[1];
        V1.x = V1.x / (float)Math.Sqrt(V1.x * V1.x + V1.y * V1.y);
        V1.y = V1.y / (float)Math.Sqrt(V1.x * V1.x + V1.y * V1.y);
        V2.x = x[2] - x[1];
        V2.y = y[2] - y[1];
        V2.x = V2.x / (float)Math.Sqrt(V2.x * V2.x + V2.y * V2.y);
        V2.y = V2.y / (float)Math.Sqrt(V2.x * V2.x + V2.y * V2.y);

        float seta = Mathf.Acos(Vector3.Dot(V1, V2));

        

        return seta*180/3.141592f;
    }

    
    private void Update()
    {
        if (landmarkResult != null && landmarkResult.score > 0.2f)
        {
            switch(TimeManage.Now_EX_State)
            {
                case 1:
                    check_move_1(landmarkResult);
                    break;
                case 2:
                    check_move_2(landmarkResult);
                    break;
                case 3:
                    check_move_3(landmarkResult);
                    break;
                case 4:
                    check_move_4(landmarkResult);
                    break;
                case 5:
                    check_move_5(landmarkResult);
                    break;
                case 6:
                    check_move_6(landmarkResult);
                    break;
                case 7:
                    check_move_7(landmarkResult);
                    break;
                case 8:
                    check_move_8(landmarkResult);
                    break;
                case 9:
                    check_move_9(landmarkResult);
                    break;
                case 10:
                    check_move_10(landmarkResult);
                    break;
                case 11:
                    check_move_11(landmarkResult);
                    break;
                case 12:
                    check_move_12(landmarkResult);
                    break;
            }
            
        }
    }

    private void Invoke(Texture texture)
    {
        landmarkResult = pose.Invoke(texture);
        poseResult = pose.PoseResult;
        if (pose.LandmarkInputTexture != null)
        {
            //debugView.texture = pose.LandmarkInputTexture;
        }
        if (landmarkResult != null && landmarkResult.SegmentationTexture != null)
        {
            //segmentationView.texture = landmarkResult.SegmentationTexture;
        }
    }

    private async UniTask<bool> InvokeAsync(Texture texture)
    {
        landmarkResult = await pose.InvokeAsync(texture, cancellationToken);
        poseResult = pose.PoseResult;
        if (pose.LandmarkInputTexture != null)
        {
            //debugView.texture = pose.LandmarkInputTexture;
        }
        if (landmarkResult != null && landmarkResult.SegmentationTexture != null)
        {
            //segmentationView.texture = landmarkResult.SegmentationTexture;
        }
        return landmarkResult != null;
    }

    // 운동 시간 종료시 함수 실행
    public void stop_check() 
    {
        TimeManage.can_state = false;
        TimeManage.Now_EX_State = 0;
    }

    // 첫번째 운동 스타트 체크
    private void can_check_1(Vector4[] marks)
    {
        float seta_r = Calcul_angle(marks[24],marks[12],marks[14]);
        float seta_l = Calcul_angle(marks[23],marks[11],marks[13]);

        if(seta_l > 60 && seta_r > 60)
        {  
            r_state = false;
            l_state = false;
            TimeManage.can_state = true;
            Debug.Log("조건 성공 활성화");
            timeManage.GetComponent<TimeManage>().Start_timer(6 , 0.0f);
        } 
    }

    private void Set_ScoreBoard(int n)
    {
        if(n == 1)
        {
            for(int i = 0; i < 4; i++)
            {
                upper_score[i].text = DataManage.instance.UpperEX[i].ToString();
            }
        }
        else if(n == 3)
        {
            for(int i = 0; i < 3; i++)
            {
                under_score[i].text = DataManage.instance.UnderEX[i].ToString();
            }
        }
    }
    // 첫번째 운동 움직임 체크
    private void check_move_1(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;

        if(TimeManage.can_state)
        {
            if(!TimeManage.is_time)
            {
                TimeManage.can_state = false;
                timeManage.GetComponent<TimeManage>().Pause_timer();
            }

            if(TimeManage.minute < 3)
            {
                TimeManage.Now_EX_State++;
                TimeManage.can_state = false;
                TimeManage.minute = 3;
                TimeManage.second = 0.0f;
                timeManage.GetComponent<TimeManage>().Pause_timer();
            }

            float seta_r = Calcul_angle(landmarks[12], landmarks[14], landmarks[16]);
            float seta_l = Calcul_angle(landmarks[11], landmarks[13], landmarks[15]);

            Debug.Log(seta_r);
            if(seta_r > 100)
            {
                if(r_state == true)
                {
                    r_state = false;
                    Debug.Log("오른손 다운");
                }
            }
            else if(seta_r < 35)
            {
                if(r_state == false)
                {
                    r_state = true;
                    DataManage.instance.UpperEX[1]++;
                    upper_score[1].text = DataManage.instance.UpperEX[1].ToString();
                    Debug.Log("오른손 업");
                }
            }

            if(seta_l > 100)
            {
                if(l_state == true)
                {
                    l_state = false;
                    Debug.Log("왼손 다운");
                }
            }
            else if(seta_l < 35)
            {
                if(l_state == false)
                {
                    l_state = true;
                    DataManage.instance.UpperEX[0]++;
                    upper_score[0].text = DataManage.instance.UpperEX[0].ToString();
                    Debug.Log("왼손 업");
                }
            }
        }
        else
        {
            can_check_1(landmarks);

        }
    }

    // 두번째 운동 스타트 체크
    private void can_check_2(Vector4[] marks)
    {
        float seta_r = Calcul_angle(marks[24],marks[12],marks[14]);
        float seta_l = Calcul_angle(marks[23],marks[11],marks[13]);

        if(seta_l > 60 && seta_r > 60)
        {
            r_state = false;
            l_state = false;
            TimeManage.can_state = true;
            Debug.Log("조건 성공 활성화");
            timeManage.GetComponent<TimeManage>().Restart_timer();
        } 
    }
    
    // 두번째 운동 움직임 체크
    private void check_move_2(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;

        if(TimeManage.can_state)
        {
            if(!TimeManage.is_time)
            {
                stop_check();
            }

            Vector3 r_arm = landmarks[14];
            Vector3 r_hand = landmarks[16];
            Vector3 r_sholder = landmarks[12];

            Vector3 l_arm = landmarks[13];
            Vector3 l_hand = landmarks[15];
            Vector3 l_sholder = landmarks[11];

            float r_gap = r_arm.y - r_sholder.y;
            float l_gap = l_arm.y - l_sholder.y;

            if(r_state)
            {
                if(r_arm.y < r_sholder.y*2 - landmarks[10].y)
                {
                    Debug.Log("팔 내림");
                    r_state = false;
                }
            }
            else
            {
                if(landmarks[10].y < r_arm.y)
                {
                    if(r_hand.y> r_arm.y + r_gap*2/3)
                    {
                        r_state = true;
                        Debug.Log("팔 들어올림");
                    }
                }
            }
            
            if(l_state)
            {
                if(l_arm.y < l_sholder.y*2 - landmarks[9].y)
                {
                    Debug.Log("팔 내림");
                    l_state = false;
                }
            }
            else
            {
                if(landmarks[9].y < l_arm.y)
                {
                    if(l_hand.y> l_arm.y + l_gap*2/3)
                    {
                        l_state = true;
                        Debug.Log("팔 들어올림");
                        DataManage.instance.UpperEX[2]++;
                    }
                }
            }
        }
        else
        {
            can_check_2(landmarks);
        }
    }
    
    
    private void can_check_3(Vector4[] marks)
    {
        float gap = (marks[12].y - marks[24].y)/2;
        if(marks[24].y < marks[26].y + gap  && marks[23].y < marks[25].y + gap)
        {
            float seta_r = Calcul_angle(marks[24],marks[12],marks[14]);
            float seta_l = Calcul_angle(marks[23],marks[11],marks[13]);
            Debug.Log("다리 앉음");
            if(seta_l > 60 && seta_r > 60)
            {
                r_state = false;
                TimeManage.can_state = true;
                Debug.Log("조건3 성공 활성화!");
                timeManage.GetComponent<TimeManage>().Start_timer(2 , 0.0f);    
            }
        }
    }
    private void check_move_3(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;
        
        if(TimeManage.can_state)
        {
            if(TimeManage.minute == 0)
            {
                r_state = true;
                TimeManage.Now_EX_State++;
                TimeManage.minute = 1;
                TimeManage.second = 0;
                timeManage.GetComponent<TimeManage>().Pasue_sec(20.0f);
            }

            float gap = (landmarks[12].y - landmarks[24].y)/2;

            if(r_state)
            {
                if(landmarks[24].y < landmarks[26].y + gap && landmarks[24].y > landmarks[26].y - gap)
                {
                    r_state = false;
                    Debug.Log("앉음!");
                }
                
            }
            else
            {
                if(landmarks[24].y > landmarks[26].y + gap*4/3)
                {
                    r_state = true;
                    Debug.Log("일어남 카운트!");
                    DataManage.instance.UnderEX[0]++;
                    DataManage.instance.UnderEX[1]++;
                    under_score[0].text = DataManage.instance.UnderEX[0].ToString();
                    under_score[1].text = DataManage.instance.UnderEX[1].ToString();
                }
            }
        }
        else
        {
            can_check_3(landmarks);
        }
    }


    private void check_move_4(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;

        if(TimeManage.is_time)
        {       
            float seta = Calcul_angle(landmarks[24], landmarks[26], landmarks[28]);

            if(r_state)
            {
                if(seta < 100)
                {
                    r_state = false;
                    Debug.Log("앉음!");
                }
                
            }
            else
            {
                if(seta > 140)
                {
                    r_state = false;
                    Debug.Log("일어남 카운트!");
                    DataManage.instance.UnderEX[0]++;
                    DataManage.instance.UnderEX[2]++;
                    under_score[0].text = DataManage.instance.UnderEX[0].ToString();
                    under_score[2].text = DataManage.instance.UnderEX[2].ToString();
                }
            }   
        }
        else
        {
            if(TimeManage.minute == 0)
            {
                stop_check();
            }
        }
    }

    private void can_check_5(Vector4[] marks)
    {
        float seta_r = Calcul_angle(marks[24],marks[12],marks[14]);
        float seta_l = Calcul_angle(marks[23],marks[11],marks[13]);

        if(seta_l > 60 && seta_r > 60)
        {
            r_state = false;
            l_state = false;
            TimeManage.can_state = true;
            Debug.Log("조건 성공 활성화");
            timeManage.GetComponent<TimeManage>().Start_timer(4 , 0.0f);
        } 
    }
    private void check_move_5(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;

        if(TimeManage.can_state)
        {
            if(TimeManage.minute < 2)
            {
                TimeManage.minute = 2;
                TimeManage.second = 0.0f;
                TimeManage.Now_EX_State++;
                TimeManage.can_state = false;
                timeManage.GetComponent<TimeManage>().Pause_timer();
            }

            float gap = (landmarks[25].y - landmarks[27].y)/3;

            if(r_state)
            {
                if(landmarks[26].y + gap < landmarks[25].y)
                {
                    r_state = false;
                    Debug.Log("왼발 들어올림");
                }
            }   
            else
            {
                if(landmarks[25].y + gap < landmarks[26].y)
                {
                    r_state = true;
                    DataManage.instance.WalkEX[0]++;
                    DataManage.instance.WalkEX[1]++;
                    walk_score_total[0].text = DataManage.instance.WalkEX[0].ToString();
                    walk_score_total[1].text = DataManage.instance.WalkEX[1].ToString();
                    Debug.Log("오른발 들어올림");
                }
            }         
        }   
        else
        {
            can_check_5(landmarks);
        }
    }

    private void can_check_6(Vector4[] marks)
    {
        if(marks[14].y > marks[10].y)
        {
            TimeManage.can_state = true;
            timeManage.GetComponent<TimeManage>().Restart_timer();
            r_state = false;
            l_state = false;
            Debug.Log("조건 활성화!");
        }
    }
    private void check_move_6(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;

        if(TimeManage.can_state)
        {
            if(TimeManage.is_time == false)
            {
                stop_check();
            }
            
            if(r_state)
            {

            }
            else
            {

            }
            
        }
        else
        {
            can_check_6(landmarks);
        }
    }

    private void can_check_7(Vector4[] marks)
    {
        if(marks[14].y > marks[10].y)
        {
            TimeManage.can_state = true;
            timeManage.GetComponent<TimeManage>().Start_timer(8,0.0f);
            r_state = false;
            l_state = false;
            Debug.Log("조건 활성화!");
            TimeManage.can_check = false;
        }
    }
    private void check_move_7(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;

        if(TimeManage.can_state)
        {
            if(TimeManage.minute < 4)
            {
                TimeManage.Now_EX_State++;
                r_state = false;
            }
            
            if(TimeManage.can_check)
            {
                float seta_l = Calcul_angle(landmarks[23], landmarks[25], landmarks[27]);
                
                if(l_state)
                {
                    if(seta_l < 90)
                    {
                        l_state = false;
                        Debug.Log("왼발 내림");
                        TimeManage.can_check = false;
                        DataManage.instance.LegupEX[0]++;
                        legup_score_one[0].text = DataManage.instance.LegupEX[0].ToString();
                    }
                }
                else
                {
                    if(seta_l > 105)
                    {
                        l_state = true;
                        Debug.Log("왼발 올림");
                    }
                }
            }
            else
            {
                float seta_r = Calcul_angle(landmarks[24], landmarks[26], landmarks[28]);

                Debug.Log(seta_r);
                if(r_state)
                {
                    if(seta_r < 90)
                    {
                        r_state = false;
                        Debug.Log("오른발 내림");
                        TimeManage.can_check = true;
                    }
                }
                else
                {
                    if(seta_r > 105)
                    {
                        r_state = true;
                        Debug.Log("오른발 올림");
                    }
                }
            }
        }
        else
        {
            can_check_7(landmarks);
        }
    }

    private void check_move_8(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;

        float seta_l = Calcul_angle(landmarks[23], landmarks[25], landmarks[27]);
        float seta_r = Calcul_angle(landmarks[24], landmarks[26], landmarks[28]);

        if(r_state)
        {
            if(seta_l < 90 && seta_r < 90)
            {
                Debug.Log("다리 내림");
                r_state = false;
                DataManage.instance.LegupEX[1]++;
                legup_score_one[1].text = DataManage.instance.LegupEX[1].ToString();
            }
        }
        else
        {
            if(seta_l > 105 && seta_r > 105)
            {
                Debug.Log("다리 올림");
                r_state = true;
            }
        }
    }

    private void check_move_9(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;

        if(TimeManage.can_state)
        {

        }
        else
        {
            
        }
    }
    private void check_move_10(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;

        if(TimeManage.can_state)
        {

        }
        else
        {
        }
    }
    private void check_move_11(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;

        if(TimeManage.can_state)
        {

        }
        else
        {
            
        }
    }private void check_move_12(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;

        if(TimeManage.can_state)
        {

        }
        else
        {
            
        }
    }
    public void Part1_Start()
    {
        TimeManage.Now_EX_State = 1;
        TimeManage.can_state = false;
        Ex_Objcet[Ex_index].SetActive(false);
        Ex_Objcet[0].SetActive(true);
        Ex_index = 0;
        Set_ScoreBoard(TimeManage.Now_EX_State);
        timeManage.GetComponent<TimeManage>().lnit_timer();
        
    }
    public void Part2_Start()
    {
        TimeManage.Now_EX_State = 3;
        TimeManage.can_state = false;
        Ex_Objcet[Ex_index].SetActive(false);
        Ex_Objcet[1].SetActive(true);
        Ex_index = 1;
        Set_ScoreBoard(TimeManage.Now_EX_State);
        timeManage.GetComponent<TimeManage>().lnit_timer();
    }
    public void Part3_Start()
    {
        TimeManage.Now_EX_State = 5;
        TimeManage.can_state = false;
        Ex_Objcet[Ex_index].SetActive(false);
        Ex_Objcet[2].SetActive(true);
        Ex_index = 2;
        Set_ScoreBoard(TimeManage.Now_EX_State);
        timeManage.GetComponent<TimeManage>().lnit_timer();
    }
    public void Part4_Start()
    {
        TimeManage.Now_EX_State = 7;
        TimeManage.can_state = false;
        Ex_Objcet[Ex_index].SetActive(false);
        Ex_Objcet[3].SetActive(true);
        Ex_index = 3;
        Set_ScoreBoard(TimeManage.Now_EX_State);
        timeManage.GetComponent<TimeManage>().lnit_timer();
    }
    public void Part5_Start()
    {
        TimeManage.Now_EX_State = 9;
        TimeManage.can_state = false;
        Ex_Objcet[Ex_index].SetActive(false);
        Ex_Objcet[4].SetActive(true);
        Ex_index = 4;
        Set_ScoreBoard(TimeManage.Now_EX_State);
        timeManage.GetComponent<TimeManage>().lnit_timer();
    }
    public void Part6_Start()
    {
        TimeManage.Now_EX_State = 11;
        TimeManage.can_state = false;
        Ex_Objcet[Ex_index].SetActive(false);
        Ex_Objcet[5].SetActive(true);
        Ex_index = 5;
        Set_ScoreBoard(TimeManage.Now_EX_State);
        timeManage.GetComponent<TimeManage>().lnit_timer();
    }
}