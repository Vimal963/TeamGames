using UnityEngine;

namespace Assets.SimpleLocalization
{

    public enum LOCALIZATION_FONT_ENGLISH
    {
        ENG_REGULAR, ENG_SEMIBOLD, ENG_EXTRABOLD, ENG_MULTI_BOLD
    }

    public enum LOCALIZATION_FONT_CHINESE
    {
        CHINESE_SIMPLYFIED
    }

    public class Example : MonoBehaviour
    {
        public Font mEnglishTextREGULAR, mEnglishTextSEMIBOLD, mEnglishTextExtraBold, mEnglishTextMultiBold, mChineseSimplyfied;
        public static Example INSTANCE;


        public void Awake()
        {

            if (INSTANCE == null)
            {
                INSTANCE = this;
                DontDestroyOnLoad(INSTANCE);
            }
            else
            {
                Destroy(gameObject);
            }


            LocalizationManager.Read();

        }

        private void Start()
        {
            if (TeamGame.ImportantDataMembers.Instance.currentLanguage == LANGUAGE.Chinese)
            {
                LocalizationManager.Language = "Chinese";
            }
            else
            {
                LocalizationManager.Language = "English";
            }
        }

        public void SetLocalization(string localization)
        {
            LocalizationManager.Language = localization;
        }

    }
}