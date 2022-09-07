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
        //drawer.DrawPoseResult(poseResult);

        if (landmarkResult != null && landmarkResult.score > 0.2f)
        {
            //drawer.DrawCropMatrix(pose.CropMatrix);
            //drawer.DrawLandmarkResult(landmarkResult, visibilityThreshold, canvas.planeDistance);

            // if (options.landmark.useWorldLandmarks)
            // {
            //     drawer.DrawWorldLandmarks(landmarkResult, visibilityThreshold);
            // }
            check_move_1(landmarkResult);
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

    private void can_check_1(Vector4[] marks)
    {
        float l_gap, r_gap, gap;

        gap = (marks[12].x - marks[11].x)/2;
        if(marks[14].x > marks[12].x - gap && marks[14].x < marks[12].x - gap && marks[16].x > marks[12].x-gap && marks[16].x < marks[12].x + gap
        && marks[13].x > marks[11].x - gap && marks[13].x < marks[11].x - gap && marks[15].x > marks[11].x-gap && marks[15].x < marks[11].x + gap)
        {
            l_gap = (marks[11].y - marks[13].y)*2/3;
            r_gap = (marks[12].y - marks[14].y)*2/3;
            if(l_gap > 0 && r_gap > 0)
            {
                if(marks[13].y < marks[15].y - l_gap && marks[14].y < marks[16].y - r_gap)
                {
                    can_state = true;
                    Debug.Log("조건 성공 활성화");
                    Gamemanage.GetComponent<GameManage>().Start_timer(6 , 0.0f);
                }
            }
        } 
    }
    private void check_move_1(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;

        if(can_state)
        {
            if(!GameManage.is_time)
            {
                stop_check();
            }
            float r_gap, l_gap;
            bool r_state = false, l_state = false;

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
        }
    }

    
    private void check_move_2(PoseLandmarkDetect.Result result)
    {
        landmarks = result.viewportLandmarks;

        if(can_state)
        {
            if(!GameManage.is_time)
            {
                stop_check();
            }
            float r_gap, l_gap;
            bool r_state = false, l_state = false;

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
    private void stop_check()
    {
        can_state = false;
    }
}
