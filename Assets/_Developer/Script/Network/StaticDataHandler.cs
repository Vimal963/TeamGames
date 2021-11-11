using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamGame
{

    /// <summary>
    /// here we define all static data menbers like API,URL,server Event name etc
    /// </summary>

    public static class StaticDataHandler
    {
        #region Network Data

        public const string URL_SERVER = "http://3.13.122.9:6363/";   //this is used for Live game
        public const string URL_API = "http://3.13.122.9:6363/";      //this is used for Live game

        //public const string URL_SERVER = "http://3.13.122.9:1212/"; //TESTING (Local)
        //public const string URL_API = "http://3.13.122.9:1212/";   //TESTING (Local)




        public static string URL_LOGIN = URL_API + "login";
        public static string URL_SIGNUP = URL_API + "register";
        public static string URL_TODAY_LEADERBOARD = URL_API + "leaderboardToday";
        public static string URL_ALLTIME_LEADERBOARD = URL_API + "leaderboard";
        public static string URL_USERUPDATE = URL_API + "userUpdate";
        public static string URL_CHAT = URL_API + "chat";
        public static string URL_STACK = URL_API + "admin/stackList";

        public const string URL_SOCKET = "socket.io/"; // Don't change this value

        public const string URL_LOGIN_SOCKET = "/login";
        public const string URL_MAIN_SOCKET = "/AllInOne";
        public const string URL_THOUSAND = "/Thousand";
        public const string URL_BLINK = "/Blink";

        public const string LISTEN_DEALERID = "delarId";
        public const string LISTEN_JOIN_OR_CREATE_ROOM = "playerList";
        public const string LISTEN_PLAYER_LEFT = "playerLeft";
        public const string LISTEN_PLAYER_WAITING_LIST = "playerLeft";
        public const string LISTEN_TOTALSTACKS = "totalStacks";
        public const string LISTEN_SEATOUT = "seatOut";
        public const string LISTEN_PLAYER_READY_FOR_BET = "startBet";
        public const string LISTEN_PLAYER_TURN = "playerTurn";
        public const string LISTEN_PLAYER_TURN_TIMER = "beatTimer";
        public const string LISTEN_PLAYER_ACTION = "playerAction";
        public const string LISTEN_ALL_PLAYER_BET_DONE = "complateBeat";
        public const string LISTEN_GAME_START_TIMER = "startGameTimer";
        public const string LISTEN_PLAYER_WAITING_FOR_NEXTROUND = "waitingForNextRound";
        public const string LISTEN_PLAYER_WAITING_FOR_OTHERPLAYER = "waitingPlayerMessage";
        public const string LISTEN_PLAYER_READY_FOR_GAME = "startGame";
        public const string LISTEN_WINNER_PLAYER_DATA = "winnerPlayer";
        public const string LISTEN_CHANGE_POKERGAME_TYPE = "changeGameType";
        public const string LISTEN_SITOUTACTION = "seatOutPlayerAction";


        public const string EMIT_JOIN_OR_CREATE_ROOM = "createJoinRoom";
        public const string EMIT_PLAYER_LEFT = "disconnectManually";
        public const string EMIT_PLAYER_DISCONNECT = "disconnect";
        public const string EMIT_PLAYER_PLAYER_BET = "playerBeat";
        public const string EMIT_START_GAME_COUNT_DOWN = "startGameCountDown";
        public const string EMIT_WINNER_PLAYER_DATA = "winnerPlayerData";
        public const string EMIT_CHANGE_POKERGAME = "systemDisconnect";


        // ====> "STOP THE CLOCK" listen Emit event
        public const string LISTEN_THOUSAND_COUNTDOWN = "countDown";
        public const string LISTEN_THOUSAND_REFLECT_TIMER = "reflectTimer";
        public const string LISTEN_THOUSAND_MY_SCORE = "yourScore";

        public const string EMIT_ThousandScore = "myScore";


        // ====> "BLINK" listen Emit event
        public const string LISTEN_BLINK_COUNTDOWN = "countDown";
        public const string LISTEN_BLINK_REFLECT_TIMER = "reflectTimer";
        public const string LISTEN_BLINK_MY_SCORE = "yourScore";

        public const string EMIT_BlinkScore = "myScore";

        #endregion


        private static bool isConnect;
        public static bool IsConnectedToSocket
        {
            get => isConnect;
            set => isConnect = value;
        }

        public static bool IsLoggedIn
        {
            get => PlayerPrefs.GetInt("LoggedIn", 0) != 0;
            set => PlayerPrefs.SetInt("LoggedIn", value ? 1 : 0);
        }

        public static string MyPlayerID
        {
            get => PlayerPrefs.GetString("PlayerID", "");
            set => PlayerPrefs.SetString("PlayerID", value);
        }

        public static string MyUserName
        {
            get => PlayerPrefs.GetString("UserName", "user");
            set => PlayerPrefs.SetString("UserName", value);
        }

        public static string MyPlayerName
        {
            get => PlayerPrefs.GetString("PlayerName", "user");
            set => PlayerPrefs.SetString("PlayerName", value);
        }

        public static int MyProfilePic
        {
            get => PlayerPrefs.GetInt("Profile", 0);
            set => PlayerPrefs.SetInt("Profile", value);
        }

        public static float MyChips
        {
            get => PlayerPrefs.GetFloat("Chips", 0);
            set => PlayerPrefs.SetFloat("Chips", value);
        }

        public static int Language
        {
            get => PlayerPrefs.GetInt("Language", 0);
            set => PlayerPrefs.SetInt("Language", value);
        }

        public static bool Sound
        {
            get => PlayerPrefs.GetInt("Sound", 1) != 0;
            set => PlayerPrefs.SetInt("Sound", value ? 1 : 0);
        }

        public static string totalStacksForRound;
        public static bool inRound = false, sitOutForNextRound = false;

        public static string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
            + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
            + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        public static bool ValidateEmail(string email)
        {
            if (email != null)
                return Regex.IsMatch(email, MatchEmailPattern);
            else
                return false;
        }

        public static string FormatTime(float timeInSeconds)
        {
            var ts = TimeSpan.FromSeconds(timeInSeconds);
            return string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
        }

        public static JSONArray ConvertToJsonArray(List<string> list)
        {
            JSONArray friendsListJSONArray = new JSONArray();

            foreach (var item in list)
            {
                Debug.Log("Facebook friends id---->" + item);
                friendsListJSONArray.Add(item);
            }

            return friendsListJSONArray;
        }

        public static string GetStackval(string str)
        {
            if (str.Contains("$"))
                return str.Replace("$", "");
            else if (str.Contains("C"))
                return str.Replace("C", "");
            else
                return str;
        }

        public static float GetTwoDecimalValue(float value)
        {
            if (value == 0) return 0;
            return float.Parse(value.ToString("0.00"));
        }

        public static float GetTwoDecimalValue(string value)
        {
            if (value == null) return 0;
            return float.Parse(float.Parse(value).ToString("0.00"));
        }

        public static string GetTwoDecimalString(float value)
        {
            if (value == 0) return "0.00";
            return value.ToString("0.00");
        }

        public static string GetTwoDecimalString(string value)
        {
            if (value == null) return "0.00";
            return float.Parse(value).ToString("0.00");
        }

        public static bool IsInternetAccessUnavailable()
        {
            return Application.internetReachability == NetworkReachability.NotReachable;
        }

        public static void LoadHomeScene()
        {
            SceneManager.LoadScene("HomeScene");
        }

        public static void LoadBetScene()
        {
            SceneManager.LoadScene("BetScene");
        }

        public static void ResetSomeData()
        {
            inRound = false;
            sitOutForNextRound = false;
        }

    }
}