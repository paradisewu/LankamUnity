using UnityEngine;
using System.Collections;

namespace LuaFramework
{
    public class Main : MonoBehaviour
    {
        void Start()
        {
            Screen.SetResolution(1080, 1920, true);

            AppFacade.Instance.StartUp();   //启动游戏
        }
    }
}