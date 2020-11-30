using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARIni : MonoBehaviour
{
    // メイン
    public string komaTag = "Koma";
    public string banTag = "Ban";
    public string komadaiTag = "Komadai";

    [SerializeField, Tooltip("将棋盤")] GameObject Ban;
    [SerializeField, Tooltip("持ち駒を置く台")] GameObject Komadai;

    static readonly double rTate = 0.819;
    static readonly double rYoko = 0.747;
    static readonly double aidaTate = rTate * 2 / 9;
    static readonly double aidaYoko = rYoko * 2 / 9;
    HoldKomaInfo holdKoma;
    bool bangaiFlag = false;
    bool firstTouched = false;

    #region 駒の定義
    [SerializeField, Tooltip("歩兵")] GameObject hu;
    [SerializeField, Tooltip("香車")] GameObject kyousya;
    [SerializeField, Tooltip("桂馬")] GameObject keima;
    [SerializeField, Tooltip("銀将")] GameObject gin;
    [SerializeField, Tooltip("金将")] GameObject kin;
    [SerializeField, Tooltip("角行")] GameObject kaku;
    [SerializeField, Tooltip("飛車")] GameObject hisya;
    [SerializeField, Tooltip("王将")] GameObject ou;
    [SerializeField, Tooltip("玉将")] GameObject gyoku;
    KomaManager manager;

    GameObject go;

    public AudioClip komaUchi;
    public AudioClip komaTori;
    AudioSource audioSource;

    #endregion

    private ARRaycastManager raycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private ARPlaneManager planeManager;

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
        audioSource = GetComponent<AudioSource>();

        // uGUIを非表示
        HiddenUGUI();

        manager = KomaManager.Instance;
        manager.Init();
        holdKoma = new HoldKomaInfo();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Vector2 touchPosition = Input.GetTouch(0).position;
            if (raycastManager.Raycast(touchPosition, hits, TrackableType.Planes) && !(firstTouched))
            {
                // Raycastの衝突情報は距離によってソートされるため、0番目が最も近い場所でヒットした情報となります
                var hitPose = hits[0].pose;
                //spawnedObject.transform.position = hitPose.position;
                //spawnedObject = Instantiate(tohu, hitPose.position, Quaternion.identity);

                // タッチした位置を基準にオブジェクトを配置
                #region 駒の配置
                // yoko,tate        
                //味方
                MoveKomaIni(ou, 5, 9, "ou_1", false ,hitPose.position);
                MoveKomaIni(hisya, 8, 8, "hi_1", false, hitPose.position);
                MoveKomaIni(kaku, 2, 8, "ka_1", false, hitPose.position);
                MoveKomaIni(kin, 4, 9, "ki_1", false, hitPose.position);
                MoveKomaIni(kin, 6, 9, "ki_2", false, hitPose.position);
                MoveKomaIni(gin, 3, 9, "gi_1", false, hitPose.position);
                MoveKomaIni(gin, 7, 9, "gi_2", false, hitPose.position);
                MoveKomaIni(keima, 2, 9, "ke_1", false, hitPose.position);
                MoveKomaIni(keima, 8, 9, "ke_2", false, hitPose.position);
                MoveKomaIni(kyousya, 1, 9, "ky_1", false, hitPose.position);
                MoveKomaIni(kyousya, 9, 9, "ky_2", false, hitPose.position);
                for (int i = 1; i <= 9; i++)
                {
                    MoveKomaIni(hu, i, 7, "hu_" + i, false, hitPose.position);
                }

                //敵
                MoveKomaIni(ou, 5, 1, "ou_11", true, hitPose.position);
                MoveKomaIni(hisya, 2, 2, "hi_11", true, hitPose.position);
                MoveKomaIni(kaku, 8, 2, "ka_11", true, hitPose.position);
                MoveKomaIni(kin, 4, 1, "ki_11", true, hitPose.position);
                MoveKomaIni(kin, 6, 1, "ki_12", true, hitPose.position);
                MoveKomaIni(gin, 3, 1, "gi_11", true, hitPose.position);
                MoveKomaIni(gin, 7, 1, "gi_12", true, hitPose.position);
                MoveKomaIni(keima, 2, 1, "ke_11", true, hitPose.position);
                MoveKomaIni(keima, 8, 1, "ke_12", true, hitPose.position);
                MoveKomaIni(kyousya, 1, 1, "ky_11", true, hitPose.position);
                MoveKomaIni(kyousya, 9, 1, "ky_12", true, hitPose.position);
                for (int i = 1; i <= 9; i++)
                {
                    MoveKomaIni(hu, i, 3, "hu_1" + i, true, hitPose.position);
                }
                #endregion

                Ban = Instantiate(Ban, Ban.transform.localPosition+hitPose.position, Ban.transform.localRotation);
                Ban.name = "Ban";
                GameObject Komadai1 = Instantiate(Komadai, new Vector3(0.65f, 0f, 1.3f)+ hitPose.position, Komadai.transform.localRotation);
                Komadai1.name = "Komadai1";
                GameObject Komadai2 = Instantiate(Komadai, new Vector3(-0.65f, 0f, -1.3f)+ hitPose.position, Komadai.transform.localRotation);
                Komadai2.name = "Komadai2";

                firstTouched = true;
                planeManager.requestedDetectionMode = PlaneDetectionMode.None;
                SetAllPlanesActive(false);

            }
        }
    }


    void MoveKomaIni(GameObject koma, int yoko, int tate, string name, bool enemyFlag, Vector3 vec3)
    {
        PutKoma putKomaInst = PutKoma.Instance;
        GameObject go = putKomaInst.CreateKoma(koma, yoko, tate, rYoko, aidaYoko, rTate, aidaTate, name, enemyFlag,vec3);
        manager.SetKomaDetail(yoko, tate, go.name);

    }
    void DisplayUGUI()
    {
        // 成、不成を判定するためのGUI表示
        GameObject cvs = GameObject.Find("Canvas");
        Transform target = cvs.transform.Find("NariPanel");
        target.gameObject.SetActive(true);
    }

    void HiddenUGUI()
    {
        // 成、不成を判定するためのGUI表示
        GameObject cvs = GameObject.Find("Canvas");
        Transform target = cvs.transform.Find("NariPanel");
        target.gameObject.SetActive(false);
    }

    void SetAllPlanesActive(bool value)
    {
        foreach (var plane in planeManager.trackables)
            plane.gameObject.SetActive(value);
    }
}

