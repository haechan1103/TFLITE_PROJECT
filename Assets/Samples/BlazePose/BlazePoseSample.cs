using System.Threading;
using Cysharp.Threading.Tasks;
using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WebCamInput))]
public sealed class BlazePoseSample : MonoBehaviour
{
    [SerializeField]
    public BlazePose.Options options = default;

    public GameObject Gamemanage;


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

    private bool can_state;

    private bool r_state;
    private bool l_state;
    private UniTask<bool> task;
    private CancellationToken cancellationToken;

    private void Start()
    {
        
        pose = new BlazePose(options);

        drawer = new BlazePoseDrawer(Camera.main, gameObject.layer, containerView);

        cancellationToken = this.GetCancellationTokenOnDestroy();

        GetComponent<WebCamInput>().OnTextureUpdate.AddListener(OnTextureUpdate);
    }

    private void OnDestroy()
    {
        GetComponent<WebCamInput>().OnTextureUpdate.RemoveListener(OnTextureUpdate);
        pose?.Dispose();
        drawer?.Dispose();
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

    

    
    private void Update()
    {
        if (landmarkResult != null && landmarkResult.score > 0.2f)
        {
            switch(GameManage.Now_EX_State)
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
    private void stop_check() 
    {
        can_state = false;
    }

    // 첫번째 운동 스타트 체크
    private void can_check_1(Vector4[] marks)
    {
        float l_gap, r_gap, gap;

        gap = (marks[12].x - marks[11].x)/3;
        if(marks[14].x > marks[12].x - gap && marks[14].x < marks[12].x + gap && marks[16].x > marks[12].x-gap && marks[16].x < marks[12].x + gap
        && marks[13].x > marks[11].x - gap && marks[13].x < marks[11].x + gap && marks[15].x > marks[11].x-gap && marks[15].x < marks[11].x + gap)
        {
            Debug.Log("x값 맞음");
            l_gap = (marks[11].y - marks[13].y)*2/3;
            r_gap = (marks[12].y - marks[14].y)*2/3;
            if(l_gap > 0 && r_gap > 0)
            {
                if(marks[15].y < marks[13].y - l_gap && marks[16].y < marks[14].y - r_gap)
                {
                    r_state = false;
                    l_state = false;
                    can_state = true;
                    Debug.Log("조건 성공 활성화");
                    Gamemanage.GetComponent<GameManage>().Start_timer(3 , 5.0f);
                }
            }
        } 
    }

    // 첫번째 운동 움직임 체크
    private void check_move_1(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;

        if(can_state)
        {
            if(!GameManage.is_time)
            {
                can_state = false;
                Gamemanage.GetComponent<GameManage>().Pause_timer();
            }

            if(GameManage.minute < 3)
            {
                GameManage.Now_EX_State++;
                can_state = false;
                GameManage.minute = 3;
                GameManage.second = 0.0f;
                Gamemanage.GetComponent<GameManage>().Pause_timer();
            }

            float r_gap, l_gap;

            landmarks = result.viewportLandmarks;
            
            r_gap = landmarks[12].y - landmarks[14].y;
            l_gap = landmarks[11].y - landmarks[13].y;
            
            Vector3 r_arm = landmarks[14];
            Vector3 r_hand = landmarks[16];
            
            Vector3 l_arm = landmarks[13];
            Vector3 l_hand = landmarks[15];

            if(r_gap > 0)
                if(r_hand.y < r_arm.y - r_gap/3*2)
                {
                    if(r_state == true)
                    {
                        r_state = false;
                        GameManage.r_count++;
                        Debug.Log("오른손 다운");
                    }
                }
                else if(r_hand.y > r_arm.y + r_gap/2)
                {
                    if(r_state == false)
                    {
                        r_state = true;
                        Debug.Log("오른손 업");
                    }
                }

            if(l_gap > 0)
            {
                if(l_hand.y < l_arm.y - l_gap/3*2)
                {
                    if(l_state == true)
                    {
                        l_state = false;
                        GameManage.l_count++;
                        Debug.Log("왼손 다운");
                    }
                }
                else if(l_hand.y > l_arm.y + l_gap/2)
                {
                    if(l_state == false)
                    {
                        l_state = true;
                        Debug.Log("왼손 업");
                    }
                }
            }
        }
        else
        {
            can_check_1(landmarks);
            Debug.Log(landmarks[10].x);
        }
    }

    // 두번째 운동 스타트 체크
    private void can_check_2(Vector4[] marks)
    {
       float l_gap, r_gap, gap;

        gap = (marks[12].x - marks[11].x)/3;
        if(marks[14].x > marks[12].x - gap && marks[14].x < marks[12].x + gap && marks[16].x > marks[12].x-gap && marks[16].x < marks[12].x + gap
        && marks[13].x > marks[11].x - gap && marks[13].x < marks[11].x + gap && marks[15].x > marks[11].x-gap && marks[15].x < marks[11].x + gap)
        {
            Debug.Log("x값 맞음");
            l_gap = (marks[11].y - marks[13].y)*2/3;
            r_gap = (marks[12].y - marks[14].y)*2/3;
            if(l_gap > 0 && r_gap > 0)
            {
                if(marks[15].y < marks[13].y - l_gap && marks[16].y < marks[14].y - r_gap)
                {
                    r_state = false;
                    l_state = false;
                    can_state = true;
                    Debug.Log("조건 성공 활성화");
                    Gamemanage.GetComponent<GameManage>().Restart_timer();
                }
            }
        } 
    }
    
    // 두번째 운동 움직임 체크
    private void check_move_2(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;

        if(can_state)
        {
            if(!GameManage.is_time)
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
                        GameManage.r_count++;
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
                        GameManage.l_count++;
                    }
                }
            }
        }
        else
        {
            can_check_2(landmarks);
        }
    }
    private void check_move_3(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;
    }
    private void check_move_4(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;
    }
    private void check_move_5(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;
    }
    private void check_move_6(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;
    }
    private void check_move_7(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;
    }
    private void check_move_8(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;
    }
}
