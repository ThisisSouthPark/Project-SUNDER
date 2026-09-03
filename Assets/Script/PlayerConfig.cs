using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        jsontest jtest = new jsontest(); //하나의 문자열을 JSON으로 변환
        string jsondata = JsonConvert.SerializeObject(jtest);
        Debug.Log(jsondata);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public class jsontest  //JSON 파일에 넣을 변수들
    {
        public int i;
        public float flo;
        public double d;
        public string str;
        public bool bo;
        public int[] iarr;
        public IntVector2 iVector;

        public jsontest()
        {
            i = 99;
            flo = 2.5f;
            d = 2.3;
            str = "Hello!";
            bo = true;
            iarr = new int[] { 1, 2, 3 };

            iVector = new IntVector2(1, 2);
        }

        public void print()
        {
            Debug.Log("int = " + i);
            Debug.Log("floatloat = " +  flo);
            Debug.Log("string = " +  str);
            Debug.Log("bool = " +  bo);

            for (int idx = 0; idx < iarr.Length; idx++)
            {  //iarr 배열의 수를 다 출력할떄까지 실행
                Debug.Log(string.Format("iarr [{0}] = {1} ", idx, iarr[idx]));
            }
        }

        public class IntVector2  //플레이어가 직접 조종하면 바뀌어야하는 변수, 움직임
        {
            public int x;
            public int y;

            public IntVector2(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
    }
}
