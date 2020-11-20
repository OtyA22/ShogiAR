using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeIchi : MonoBehaviour
{
    //打ち込みのマス判定 

    private static JudgeIchi mInstance;
    public static JudgeIchi Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject go = new GameObject("JudgeIchi");
                mInstance = go.AddComponent<JudgeIchi>();
            }
            return mInstance;
        }
    }

            //縦マス
            public int JudgeMasuTate(float posX, double rTate, double aidaTate)
    {
        int X = 0;

        for (int num = 1; num < 10; num++)
        {
            if (posX >= (-1)*rTate && posX <= (-1)*rTate + num*aidaTate)
            {
                X = num;
                break;
            }
        }

        return X;
    }

    //横マス
    public int JudgeMasuYoko(float posZ, double rYoko, double aidaYoko)
    {
        int Z = 0;

        for (int num = 1; num < 10; num++)
        {
            if (posZ >= (-1) * rYoko && posZ <= (-1) * rYoko + num * aidaYoko)
            {
                Z = num;
                break;
            }
        }

        return Z;
    }
}
