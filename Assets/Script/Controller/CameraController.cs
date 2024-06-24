using UnityEngine;
using Singleton; //MonoSingleton
using DG.Tweening; //DOTween

public class CameraController : MonoSingleton<CameraController>
{
    public Vector3 defalutPosition = new Vector3(0f, 1f, -10f);

    [Header("Move&Zoom")]
    public float moveBox = 4.5f;
    public float minZoom = 3f;
    public float maxZoom = 12f;
    public float curBox = 0;
    public const float defaultZoom = 8.5f;

    [Header("Vibrate")]
    Tween cameraTwn;
    Vector3 startPosition;
    float wheel;

    #region mono func

    public void Start()
    {
        ResetCamera();
    }

    public void Update()
    {
        //컴퓨터 내부엔 양클 불가라서 임시로 적용
        wheel = Input.GetAxis("Mouse ScrollWheel");

        if (wheel != 0f)
            Zoom(wheel);
    }

    #endregion

    #region move zoom func

    [NaughtyAttributes.Button]
    /// <summary>
    /// 위치 및 사이즈 초기화
    /// </summary>
    public void ResetCamera() 
    {
        transform.position = defalutPosition;
        Camera.main.orthographicSize = defaultZoom;
        curBox = (1-(defaultZoom - minZoom) / (maxZoom - minZoom)) * moveBox;
    }

    /// <summary>
    /// 카메라 이동
    /// </summary>
    /// <param name="delta"></param>
    public void MoveToCamera(Vector2 delta)
    {
        transform.position = transform.position + (Vector3)(delta * moveBox);

        BoxInCamera();
    }

    /// <summary>
    /// 카메라 줌 인&아웃
    /// </summary>
    /// <param name="delta"></param>
    public void Zoom(float delta)
    {
        curBox = Camera.main.orthographicSize + delta;

        if (curBox < minZoom)
            curBox = minZoom;
        else if (curBox > maxZoom)
            curBox = maxZoom;

        Camera.main.orthographicSize = curBox;

        //0~1f사이의 현재 줌 사이즈 * 최대 이동 위치
        curBox = (1-(curBox - minZoom) / (maxZoom - minZoom)) * moveBox;

        BoxInCamera();
    }

    /// <summary>
    /// 사이즈 대비 밖으로 안나가게 하기
    /// </summary>
    void BoxInCamera() 
    {
        Vector3 position = transform.position;

        if (position.x > curBox)
            position.x = curBox;
        else if (position.x < -curBox)
            position.x = -curBox;

        if (position.y > curBox)
            position.y = curBox;
        else if (position.y < -curBox)
            position.y = -curBox;

        transform.position = position;
    }

    #endregion

    #region tween func

    //임시 테스트용 진동
    [NaughtyAttributes.Button]
    public void TestVib()
    {
        switch (Random.Range(0, 3)) 
        { 
            case 0:
                Debug.Log("Default Vibration");
                Vibration(Random.Range(4, 8), Random.Range(0.1f, 0.6f), Random.Range(0.2f, 0.6f));
                break;

            case 1:
                Debug.Log("Vertical Vibration");
                VerticalVibration(Random.Range(4, 8), Random.Range(0.1f, 0.3f), Random.Range(0.3f, 0.6f), Random.Range(0.2f, 0.6f));
                break;

            case 2:
                Debug.Log("Horizontal Vibration");
                HorizontalVibration(Random.Range(4, 8), Random.Range(0.1f, 0.3f), Random.Range(0.3f, 0.6f), Random.Range(0.2f, 0.6f));
                break;
        }
    }

    /// <summary>
    /// 카메라 진동
    /// </summary>
    /// <param name="bounce">움직임 횟수</param>
    /// <param name="force">세기</param>
    /// <param name="playtime">진행시간</param>
    /// <param name="isfirst">반복함수 여부</param>
    public void Vibration(int bounce = 5, float force = 0.2f, float playtime = 0.5f, bool isfirst = true)
    {
        //진행 도중의 움직임은 제거
        if (cameraTwn != null)
            cameraTwn.Kill();

        //최대 사이즈 반경으로 위치 잡기
        Vector3 position = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        position = position.normalized * force;
        position.z = defalutPosition.z;

        //처음이면 위치를 저장하고 마지막이면 처음 위치로 되돌린다.
        if (isfirst)
            startPosition = transform.position;
        else if (bounce == 1 && !isfirst)
            position = startPosition;

        cameraTwn = DOTween.To(
            () => transform.position, //시작
            pos => transform.position = pos, //중간 설정
            position, playtime / bounce) //도착 및 시간
            .OnComplete(() =>
            {
                if (bounce > 1)
                    Vibration(bounce - 1, force, playtime - (playtime / bounce), false);
            });
    }

    /// <summary>
    /// 카메라 수직 진동
    /// </summary>
    /// <param name="bounce">움직임 횟수</param>
    /// <param name="minforce">최소세기</param>
    /// <param name="minforce">최대세기</param>
    /// <param name="playtime">진행시간</param>
    /// <param name="isfirst">반복함수 여부</param>
    public void VerticalVibration(int bounce = 5, float minforce = 0.2f, float maxforce = 0.4f, float playtime = 0.5f, bool isfirst = true)
    {
        //진행 도중의 움직임은 제거
        if (cameraTwn != null)
            cameraTwn.Kill();

        //최대 사이즈 반경으로 위치 잡기
        Vector3 position = (transform.position.y < 0) ? Vector3.up : Vector3.down;
        position = position.normalized * Random.Range(minforce, maxforce);
        position.z = defalutPosition.z;

        //처음이면 위치를 저장하고 마지막이면 처음 위치로 되돌린다.
        if (isfirst)
            startPosition = transform.position;
        else if (bounce == 1 && !isfirst)
            position = startPosition;

        cameraTwn = DOTween.To(
            () => transform.position, //시작
            pos => transform.position = pos, //중간 설정
            position, playtime / bounce) //도착 및 시간
            .OnComplete(() =>
            {
                if (bounce > 1)
                    VerticalVibration(bounce - 1, minforce, maxforce, playtime - (playtime / bounce), false);
            });
    }
    
    /// <summary>
    /// 카메라 수평 진동
    /// </summary>
    /// <param name="bounce">움직임 횟수</param>
    /// <param name="minforce">최소세기</param>
    /// <param name="minforce">최대세기</param>
    /// <param name="playtime">진행시간</param>
    /// <param name="isfirst">반복함수 여부</param>
    public void HorizontalVibration(int bounce = 5, float minforce = 0.2f, float maxforce = 0.4f, float playtime = 0.5f, bool isfirst = true)
    {
        //진행 도중의 움직임은 제거
        if (cameraTwn != null)
            cameraTwn.Kill();

        //최대 사이즈 반경으로 위치 잡기
        Vector3 position = (transform.position.x < 0) ? Vector3.right : Vector3.left;
        position = position.normalized * Random.Range(minforce, maxforce);
        position.z = defalutPosition.z;

        //처음이면 위치를 저장하고 마지막이면 처음 위치로 되돌린다.
        if (isfirst)
            startPosition = transform.position;
        else if (bounce == 1 && !isfirst)
            position = startPosition;

        cameraTwn = DOTween.To(
            () => transform.position, //시작
            pos => transform.position = pos, //중간 설정
            position, playtime / bounce) //도착 및 시간
            .OnComplete(() =>
            {
                if (bounce > 1)
                    HorizontalVibration(bounce - 1, minforce, maxforce, playtime - (playtime / bounce), false);
            });
    }

    #endregion
}
