using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutKoma : MonoBehaviour
{
    // 指定された駒を指定された座標に移動させる


    private static PutKoma mInstance;
    public static PutKoma Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject go = new GameObject("PutKoma");
                mInstance = go.AddComponent<PutKoma>();
            }
            return mInstance;
        }
    }

    public GameObject CreateKoma(GameObject koma, int yoko, int tate, double rYoko, double aidaYoko, double rTate, double aidaTate,string name,bool enemyFlag)
    {
        GameObject go 
            = Instantiate(
                            koma,
                            ReturnMasuZahyou(koma.transform.localPosition, yoko, tate,  rYoko,  aidaYoko,  rTate,  aidaTate),
                            koma.transform.localRotation
                            );
        go.name = name;
        if (enemyFlag)
        {
            Vector3 vec3 = go.transform.localEulerAngles;
            vec3.z = 180f;
            go.transform.localEulerAngles = vec3;
        }

        return go;
    }

    public GameObject DriveKoma(GameObject koma, int yoko, int tate, double rYoko, double aidaYoko, double rTate, double aidaTate)
    {
        Vector3 vec3 = ReturnMasuZahyou(koma.transform.localPosition, yoko, tate, rYoko, aidaYoko, rTate, aidaTate);
        koma.transform.localPosition = vec3;
        return koma;
    }

    public GameObject DriveFreeKoma(GameObject koma, float yokoF, float tateF,bool bFlag)
    {
        Vector3 vec3 = new Vector3(tateF, koma.transform.localPosition.y, yokoF );
        koma.transform.localPosition = vec3;

        if (!bFlag)
        {
            if (koma.transform.localEulerAngles.y > 180f)
            {
                // 逆位置の場合
                Vector3 vecA3 = koma.transform.localEulerAngles;
                vecA3.y = 90f;
                koma.transform.localEulerAngles = vecA3;
            }
            else
            {
                // 正位置の場合
                Vector3 vecA3 = koma.transform.localEulerAngles;
                vecA3.y = 270f;
                koma.transform.localEulerAngles = vecA3;
            }
        }
        return koma;
    }

    public Vector3 ReturnMasuZahyou(Vector3 vec3, int yoko, int tate, double rYoko, double aidaYoko, double rTate,double aidaTate)
    {
        vec3.z = (float)((-1) * rYoko + (aidaYoko / 2) + (aidaYoko) * (yoko - 1));
        vec3.x = (float)((-1) * rTate + (aidaTate / 2) + (aidaTate) * (tate - 1));

        return vec3;
    }
}
