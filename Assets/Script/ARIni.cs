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

    Vector3 touchVec;
  

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
    [SerializeField] private Camera arCamera;
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
    }

    void Start()
    {
        holdKoma = new HoldKomaInfo();
    }

        void Update()
    {
        if (Input.touchCount > 0)
        {
            // GUIに対するタッチであれば無視
            if (
                //EventSystem.current.IsPointerOverGameObject()) return;
                EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

            Vector2 touchPosition = Input.GetTouch(0).position;
            RaycastHit hit;
            if (raycastManager.Raycast(touchPosition, hits, TrackableType.Planes) && !(firstTouched))
            {
                // 最初に検出平面にタッチしたとき

                // Raycastの衝突情報は距離によってソートされるため、0番目が最も近い場所でヒットした情報となります
                var hitPose = hits[0].pose;
                touchVec = hitPose.position;

                // タッチした位置を基準にオブジェクトを配置
                #region 駒の配置
                // yoko,tate        
                //味方
                MoveKomaIni(ou, 5, 9, "ou_1", false , touchVec);
                MoveKomaIni(hisya, 8, 8, "hi_1", false, touchVec);
                MoveKomaIni(kaku, 2, 8, "ka_1", false, touchVec);
                MoveKomaIni(kin, 4, 9, "ki_1", false, touchVec);
                MoveKomaIni(kin, 6, 9, "ki_2", false, touchVec);
                MoveKomaIni(gin, 3, 9, "gi_1", false, touchVec);
                MoveKomaIni(gin, 7, 9, "gi_2", false, touchVec);
                MoveKomaIni(keima, 2, 9, "ke_1", false, touchVec);
                MoveKomaIni(keima, 8, 9, "ke_2", false, touchVec);
                MoveKomaIni(kyousya, 1, 9, "ky_1", false, touchVec);
                MoveKomaIni(kyousya, 9, 9, "ky_2", false, touchVec);
                for (int i = 1; i <= 9; i++)
                {
                    MoveKomaIni(hu, i, 7, "hu_" + i, false, touchVec);
                }

                //敵
                MoveKomaIni(ou, 5, 1, "ou_11", true, touchVec);
                MoveKomaIni(hisya, 2, 2, "hi_11", true, touchVec);
                MoveKomaIni(kaku, 8, 2, "ka_11", true, touchVec);
                MoveKomaIni(kin, 4, 1, "ki_11", true, touchVec);
                MoveKomaIni(kin, 6, 1, "ki_12", true, touchVec);
                MoveKomaIni(gin, 3, 1, "gi_11", true, touchVec);
                MoveKomaIni(gin, 7, 1, "gi_12", true, touchVec);
                MoveKomaIni(keima, 2, 1, "ke_11", true, touchVec);
                MoveKomaIni(keima, 8, 1, "ke_12", true, touchVec);
                MoveKomaIni(kyousya, 1, 1, "ky_11", true, touchVec);
                MoveKomaIni(kyousya, 9, 1, "ky_12", true, touchVec);
                for (int i = 1; i <= 9; i++)
                {
                    MoveKomaIni(hu, i, 3, "hu_1" + i, true, touchVec);
                }
                #endregion

                Ban = Instantiate(Ban, Ban.transform.localPosition+ touchVec, Ban.transform.localRotation);
                Ban.name = "Ban";
                GameObject Komadai1 = Instantiate(Komadai, new Vector3(0.65f, 0f, 1.3f)+ touchVec, Komadai.transform.localRotation);
                Komadai1.name = "Komadai1";
                GameObject Komadai2 = Instantiate(Komadai, new Vector3(-0.65f, 0f, -1.3f)+ touchVec, Komadai.transform.localRotation);
                Komadai2.name = "Komadai2";

                firstTouched = true;
                planeManager.requestedDetectionMode = PlaneDetectionMode.None;
                SetAllPlanesActive(false);

            }
            else if( Physics.Raycast(arCamera.ScreenPointToRay(touchPosition),out hit)  && firstTouched && Input.GetTouch(0).phase == TouchPhase.Began)
            {

                if (hit.collider.gameObject.CompareTag(komadaiTag))
                {
                    // 駒台に当たった場合
                    if (holdKoma.hKoma != null)
                    {
                        // 駒が選択済みだった場合
                        if (holdKoma.x == -1 || holdKoma.y == -1)
                        {
                            // 駒がとられた直後 or 駒台上で再選択した場合
                            GameObject holdKomaObj = holdKoma.hKoma;
                            MoveKomaFree(holdKomaObj, hit.point.z, hit.point.x, bangaiFlag, touchVec);
                            //成なら表に戻す
                            if (holdKomaObj.transform.localEulerAngles.x < 180f)
                            {
                                //裏向きなら
                                ReverseKoma(holdKomaObj);
                            }
                            holdKomaObj.GetComponent<KomaControl>().ClickKomaAction(touchVec);
                            holdKoma.Init();
                            bangaiFlag = false;
                        }
                    }
                }
                else
                {
                    // 駒台以外を触った場合
                    JudgeIchi judgeIchi = JudgeIchi.Instance;
                    int yoko = judgeIchi.JudgeMasuYoko(hit.point.z - touchVec.z, rYoko, aidaYoko); //ok
                    int tate = judgeIchi.JudgeMasuTate(hit.point.x - touchVec.x, rTate, aidaTate); //ok


                    if (yoko != 0 && tate != 0)
                    {
                        // 枠の内側であれば
                        if (hit.collider.gameObject.CompareTag(banTag) || hit.collider.gameObject.CompareTag(komaTag))
                        {
                            // タッチした対象が駒か盤の場合

                            go = GameObject.Find(manager.GetKomaDetail(yoko, tate).objName);
                            if (holdKoma.hKoma == null)
                            {
                                // 選択済みの駒がない場合
                                if (go != null)
                                {
                                    // 新たに駒を選択した場合                     
                                    go.GetComponent<KomaControl>().ClickKomaAction(touchVec);
                                    holdKoma.hKoma = go;
                                    holdKoma.x = yoko;
                                    holdKoma.y = tate;

                                    audioSource.PlayOneShot(komaTori);
                                }

                            }
                            else
                            {
                                // 選択済みの駒がある場合

                                if (go != null)
                                {
                                    //マスの上に駒があった場合

                                    if (go.name.Equals(holdKoma.hKoma.name))
                                    {
                                        // 選択中の駒を再選択した場合
                                        hit.collider.gameObject.GetComponent<KomaControl>().ClickKomaAction(touchVec);
                                        holdKoma = new HoldKomaInfo();
                                    }
                                    else
                                    {
                                        // それ以外

                                        // マス上の駒を浮かせる。                            
                                        go.GetComponent<KomaControl>().ClickKomaAction(touchVec);
                                        // 選択中の駒を置く
                                        GameObject holdKomaObj = holdKoma.hKoma;
                                        MoveKoma(holdKomaObj, yoko, tate, touchVec);
                                        // なり判定★
                                        if (checkNariKoma(holdKomaObj)) CheckNariMasu(holdKoma, tate);
                                        holdKomaObj.GetComponent<KomaControl>().ClickKomaAction(touchVec);
                                        if (!bangaiFlag)
                                        {
                                            manager.DeleteKomaDetail(holdKoma.x, holdKoma.y);
                                        }
                                        manager.SetKomaDetail(yoko, tate, holdKomaObj.name);
                                        bangaiFlag = false;
                                        // 浮かせた駒を保持する（元あった場所は選択中だった駒があるので、後に置く際に消す必要はない）
                                        holdKoma.hKoma = go.gameObject;
                                        holdKoma.x = -1;
                                        holdKoma.y = -1;
                                    }
                                }
                                else
                                {
                                    // マスの上に駒がなかった場合

                                    GameObject holdKomaObj = holdKoma.hKoma;
                                    MoveKoma(holdKomaObj, yoko, tate, touchVec);
                                    //なり判定★
                                    if(checkNariKoma(holdKomaObj)) CheckNariMasu(holdKoma, tate);
                                    holdKomaObj.GetComponent<KomaControl>().ClickKomaAction(touchVec);

                                    if (holdKoma.x != -1 && holdKoma.y != -1)
                                    {
                                        manager.DeleteKomaDetail(holdKoma.x, holdKoma.y);
                                    }
                                    manager.SetKomaDetail(yoko, tate, holdKomaObj.name);

                                    bangaiFlag = false;
                                    holdKoma.Init();
                                }
                            }
                        }
                    }
                    else
                    {
                        // マスの外であった場合
                        if (hit.collider.gameObject.CompareTag(komaTag))
                        {
                            // マス外（駒台の上）に置かれた駒の場合

                            if (!bangaiFlag)
                            {
                                // 盤外の駒を選択していない場合
                                go = hit.collider.gameObject;
                                go.GetComponent<KomaControl>().ClickKomaAction(touchVec);
                                holdKoma.hKoma = go;
                                holdKoma.x = -1;
                                holdKoma.y = -1;
                                bangaiFlag = true;
                                audioSource.PlayOneShot(komaTori);
                            }
                        }
                    }
                }
            }
        }
    }


    void MoveKomaIni(GameObject koma, int yoko, int tate, string name, bool enemyFlag, Vector3 vec3)
    {
        PutKoma putKomaInst = PutKoma.Instance;
        GameObject go = putKomaInst.CreateKoma(koma, yoko, tate, rYoko, aidaYoko, rTate, aidaTate, name, enemyFlag,vec3);
        manager.SetKomaDetail(yoko, tate, go.name);

    }

    GameObject MoveKoma(GameObject koma, int yoko, int tate, Vector3 vec3)
    {
        PutKoma putKomaInst = PutKoma.Instance;
        audioSource.PlayOneShot(komaUchi);
        //DisplayUGUI();

        return putKomaInst.DriveKoma(koma, yoko, tate, rYoko, aidaYoko, rTate, aidaTate, vec3);
    }

    GameObject MoveKomaFree(GameObject koma, float yokoF, float tateF, bool bFlag, Vector3 vec3)
    {
        PutKoma putKomaInst = PutKoma.Instance;
        audioSource.PlayOneShot(komaUchi);
        return putKomaInst.DriveFreeKoma(koma, yokoF, tateF, bFlag, vec3);
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

    //なりの判定
    void CheckNariMasu(HoldKomaInfo komaInfo, int tate)
    {
        GameObject koma = komaInfo.hKoma;
        // 敵陣に入っていた場合の動き
        if (koma.transform.localEulerAngles.x > 180f)
        {
            // 表向きなら

            if (koma.transform.localEulerAngles.y < 180f)
            {
                // 正位置なら
                if (komaInfo.y >= 1 && komaInfo.y <= 3)
                {
                    //ReverseKoma(koma);
                    UIControl.holdKoma = koma;
                    DisplayUGUI();
                    return;
                }
            }
            else
            {
                //逆位置なら
                if (komaInfo.y >= 7 && komaInfo.y <= 9)
                {
                    //ReverseKoma(koma);
                    UIControl.holdKoma = koma;
                    DisplayUGUI();
                    return;
                }
            }

            // 敵陣に入った時の成判定
            // 表向きなら

            if (koma.transform.localEulerAngles.y < 180f)
            {
                // 正位置なら
                if (tate >= 1 && tate <= 3)
                {
                    //ReverseKoma(koma);
                    UIControl.holdKoma = koma;
                    DisplayUGUI();
                }
            }
            else
            {
                //逆位置なら
                if (tate >= 7 && tate <= 9)
                {
                    //ReverseKoma(koma);
                    UIControl.holdKoma = koma;
                    DisplayUGUI();
                }
            }
        }
    }

    public void ReverseKoma(GameObject koma)
    {
        if (koma.transform.localEulerAngles.x > 180f)
        {
            // 表表示の場合
            if (koma.transform.localEulerAngles.y < 180f)
            {
                // 正位置なら
                koma.transform.localEulerAngles = new Vector3(90f, 180f, -90f);
            }
            else
            {
                // 逆位置なら
                koma.transform.localEulerAngles = new Vector3(90f, 0f, -90f);
            }
        }
        else
        {
            // 裏表示の場合
            if (koma.transform.localEulerAngles.y > 90f)
            {
                // 正位置なら
                koma.transform.localEulerAngles = new Vector3(270f, 90f, 0f);
            }
            else
            {
                // 逆位置なら
                koma.transform.localEulerAngles = new Vector3(270f, 270f, 0f);
            }
        }
    }

    void SetAllPlanesActive(bool value)
    {
        foreach (var plane in planeManager.trackables)
            plane.gameObject.SetActive(value);
    }

    bool checkNariKoma(GameObject komaObj)
    {
        string komaName = komaObj.name;
        if (komaName.IndexOf("ki") != -1) return false;
        if (komaName.IndexOf("ou") != -1) return false;

        return true;
    }
}

