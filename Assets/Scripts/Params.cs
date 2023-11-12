using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Params : MonoBehaviour
    {
        public Space SpaceA;
        public Space SpaceB;
        public Space SpaceC;
        public Space SpaceD;
        
        public static int WORKER_SPEED = 10;
        public static float TIME_SCALE = 0.01f;
        public static int FRAME_RATE = 2;
        public static double QUIT_EFF = 0.25;
        public static double WORK_EFF = 0.45;


        public static int SPEED = 6;
        public static bool ANIMATE = true;

        /*public static Dictionary<string, Space> spaceDic
            = new Dictionary<string, Space>()
            {
                { "A", SpaceA },
                {"B",SpaceB},
                {"C",SpaceC},
                {"D",SpaceD}
            };*/
    }
}