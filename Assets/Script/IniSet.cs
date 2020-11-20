using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class IniSet : MonoBehaviour
{
    // メイン
    public string komaTag = "Koma";
    public string banTag = "Ban";
    public string komadaiTag = "Komadai";

    public GameObject Ban;
    public GameObject Komadai;

    static readonly double rTate = 0.819;
    static readonly double rYoko = 0.747;
    static readonly double aidaTate = rTate * 2 / 9;
    static readonly double aidaYoko = rYoko * 2 / 9;
    HoldKomaInfo holdKoma;
    bool bangaiFlag = false;

    #region 駒の定義
    public GameObject hu;
    public GameObject kyousya;
    public GameObject keima;
    public GameObject gin;
    public GameObject kin;
    public GameObject kaku;
    public GameObject hisya;
    public GameObject ou;
    public GameObject gyoku;
    KomaManager manager;

    GameObject go;

    public AudioClip komaUchi;
    public AudioClip komaTori;
    AudioSource audioSource;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        manager = KomaManager.Instance;
        manager.Init();
        holdKoma = new HoldKomaInfo();

        #region 駒の配置
        // yoko,tate        

        //味方
        MoveKomaIni(ou, 5, 9, "ou_1", false);
        MoveKomaIni(hisya, 8, 8, "hi_1", false);
        MoveKomaIni(kaku, 2, 8, "ka_1", false);
        MoveKomaIni(kin, 4, 9, "ki_1", false);
        MoveKomaIni(kin, 6, 9, "ki_2", false);
        MoveKomaIni(gin, 3, 9, "gi_1", false);
        MoveKomaIni(gin, 7, 9, "gi_2", false);
        MoveKomaIni(keima, 2, 9, "ke_1", false);
        MoveKomaIni(keima, 8, 9, "ke_2", false);
        MoveKomaIni(kyousya, 1, 9, "ky_1", false);
        MoveKomaIni(kyousya, 9, 9, "ky_2", false);
        for (int i = 1; i <= 9; i++)
        {
            MoveKomaIni(hu, i, 7, "hu_" + i, false);
        }

        //敵
        MoveKomaIni(ou, 5, 1, "ou_11", true);
        MoveKomaIni(hisya, 2, 2, "hi_11", true);
        MoveKomaIni(kaku, 8, 2, "ka_11", true);
        MoveKomaIni(kin, 4, 1, "ki_11", true);
        MoveKomaIni(kin, 6, 1, "ki_12", true);
        MoveKomaIni(gin, 3, 1, "gi_11", true);
        MoveKomaIni(gin, 7, 1, "gi_12", true);
        MoveKomaIni(keima, 2, 1, "ke_11", true);
        MoveKomaIni(keima, 8, 1, "ke_12", true);
        MoveKomaIni(kyousya, 1, 1, "ky_11", true);
        MoveKomaIni(kyousya, 9, 1, "ky_12", true);
        for (int i = 1; i <= 9; i++)
        {
            MoveKomaIni(hu, i, 3, "hu_1" + i, true);
        }
        #endregion


        Ban = Instantiate(Ban, Ban.transform.localPosition, Ban.transform.localRotation);
        Ban.name = "Ban";
        GameObject Komadai1 = Instantiate(Komadai, new Vector3(0.65f, 0f, 1.3f), Komadai.transform.localRotation);
        Komadai1.name = "Komadai1";
        GameObject Komadai2 = Instantiate(Komadai, new Vector3(-0.65f, 0f, -1.3f), Komadai.transform.localRotation);
        Komadai2.name = "Komadai2";

        audioSource = GetComponent<AudioSource>();

        // uGUIを非表示
        HiddenUGUI();

        // 場所を操作
        /*
        敵左上 (x, z)=(-0.819, -0.747)
        ----------------------------
        |香|桂|銀|金|玉|金|銀|桂|香|
        ----------------------------
        |　|飛|　|　|　|　|　|角|　|
        ----------------------------
        |歩|歩|歩|歩|歩|歩|歩|歩|歩|
        ----------------------------
        |　|　|　|　|　|　|　|　|　|　　　　
        ----------------------------
        |　|　|　|　|　|　|　|　|　|　　　　　　　　
        ----------------------------
        |　|　|　|　|　|　|　|　|　|　　　　　　
        ----------------------------
        |歩|歩|歩|歩|歩|歩|歩|歩|歩|
        ----------------------------
        |　|角|　|　|　|　|　|飛|　|　
        ----------------------------
        |香|桂|銀|金|王|金|銀|桂|香|
        ----------------------------
                                    味方右下 (x, z)=(0.819, 0.747)
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // GUIに対するタッチであれば無視
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject()) return;
            //    EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

            Ray ray = new Ray();
            RaycastHit hit = new RaycastHit();
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //マウスクリックした場所からRayを飛ばし、オブジェクトがあればtrue 
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
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
                            MoveKomaFree(holdKomaObj, hit.point.z, hit.point.x, bangaiFlag);
                            //成なら表に戻す
                            if (holdKomaObj.transform.localEulerAngles.x < 180f)
                            {
                                //裏向きなら
                                ReverseKoma(holdKomaObj);
                            }
                                holdKomaObj.GetComponent<KomaControl>().ClickKomaAction();
                            holdKoma.Init();
                            bangaiFlag = false;
                        }
                    }
                }
                else
                {
                    // 駒台以外を触った場合
                    JudgeIchi judgeIchi = JudgeIchi.Instance;
                    int yoko = judgeIchi.JudgeMasuYoko(hit.point.z, rYoko, aidaYoko);
                    int tate = judgeIchi.JudgeMasuTate(hit.point.x, rTate, aidaTate);


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
                                    go.GetComponent<KomaControl>().ClickKomaAction();
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
                                        hit.collider.gameObject.GetComponent<KomaControl>().ClickKomaAction();
                                        holdKoma = new HoldKomaInfo();
                                    }
                                    else
                                    {
                                        // それ以外

                                        // マス上の駒を浮かせる。                            
                                        go.GetComponent<KomaControl>().ClickKomaAction();
                                        // 選択中の駒を置く
                                        GameObject holdKomaObj = holdKoma.hKoma;
                                        MoveKoma(holdKomaObj, yoko, tate);
                                        // なり判定★
                                        CheckNariMasu(holdKoma, tate);
                                        holdKomaObj.GetComponent<KomaControl>().ClickKomaAction();
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
                                    MoveKoma(holdKomaObj, yoko, tate);
                                    //なり判定★
                                    Debug.Log(yoko.ToString() + "," + tate.ToString() + "-" + holdKomaObj.transform.localEulerAngles.x + "," + holdKomaObj.transform.localEulerAngles.y + "," + holdKomaObj.transform.localEulerAngles.z);
                                    CheckNariMasu(holdKoma, tate);
                                    holdKomaObj.GetComponent<KomaControl>().ClickKomaAction();

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
                                go.GetComponent<KomaControl>().ClickKomaAction();
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



    void MoveKomaIni(GameObject koma, int yoko, int tate, string name, bool enemyFlag)
    {
        PutKoma putKomaInst = PutKoma.Instance;
        GameObject go = putKomaInst.CreateKoma(koma, yoko, tate, rYoko, aidaYoko, rTate, aidaTate, name, enemyFlag);
        manager.SetKomaDetail(yoko, tate, go.name);

    }

    GameObject MoveKoma(GameObject koma, int yoko, int tate)
    {
        PutKoma putKomaInst = PutKoma.Instance;
        audioSource.PlayOneShot(komaUchi);
        //DisplayUGUI();

        return putKomaInst.DriveKoma(koma, yoko, tate, rYoko, aidaYoko, rTate, aidaTate);
    }

    GameObject MoveKomaFree(GameObject koma, float yokoF, float tateF, bool bFlag)
    {
        PutKoma putKomaInst = PutKoma.Instance;
        audioSource.PlayOneShot(komaUchi);
        return putKomaInst.DriveFreeKoma(koma, yokoF, tateF, bFlag);
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
}

// 入る場合のみ成実装→出る場合の実装を次回実装
