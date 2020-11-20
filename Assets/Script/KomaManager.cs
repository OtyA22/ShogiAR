using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 * 駒管理
 * KomaMnager manager = KomaManager.Instance;
 * 
 */
public class KomaManager : MonoBehaviour
{
    private static KomaManager mInstance;
    // 9x9マスの駒の管理
    Dictionary<string, KomaDetail> komas = new Dictionary<string, KomaDetail>();
    private KomaManager()
    {}
    public static KomaManager Instance
    {
        get
        {
            if(mInstance == null)
            {
                GameObject go = new GameObject("KomaManager");
                mInstance = go.AddComponent<KomaManager>();
            }
            return mInstance;
        }
    }

    // Start is called before the first frame update
    public void Init()
    {
        for(int x=1; x<=9; x++)
        {
            for(int y = 1; y <= 9; y++)
            {
                // [x-y]というキーの器に初期化情報を入れていく。
                komas[x + "-" + y] = new KomaDetail();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {        
    }

    public void SetKomaDetail(int x, int y, string objName)
    {
        // objNameを_で分割して、names配列に代入する
        string[] names = objName.Split(new char[] { '_' });
        string name = names[0];
        KomaDetail detail = new KomaDetail();
        detail.x = x;
        detail.y = y;
        detail.komaName = name;
        detail.objName = objName;
        if(int.Parse(names[1]) >= 10)
        {
            // 敵向きの場合
            detail.enemyFlag = true;
        }
        else
        {
            detail.enemyFlag = false;
        }
        komas[x + "-" + y] = detail;
    }

    public KomaDetail GetKomaDetail(int x, int y)
    {
        return komas[x + "-" + y];
    }
    public void DeleteKomaDetail(int x, int y)
    {
        komas[x + "-" + y] = new KomaDetail();
    }
}
