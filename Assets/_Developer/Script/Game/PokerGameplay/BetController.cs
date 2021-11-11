using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace TeamGame
{
    public class BetController : MonoBehaviour
    {
        /// <summary>
        /// this will only use to connect player with socket server
        /// </summary>
        void Start()
        {
            Network_Gatekeeper.Instance.ConnectToServer();
        }

    }
}
