using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Assets.SimpleLocalization
{

    public class LocalizedText : MonoBehaviour
    {
        public string LocalizationKey;
        [SerializeField] bool isTMP = false;

        [SerializeField] private LOCALIZATION_FONT_ENGLISH _FONT_ENG;
        [SerializeField] private LOCALIZATION_FONT_CHINESE _FONT_CHINESE;

        public void Start()
        {
            Localize();
            LocalizationManager.LocalizationChanged += Localize;
        }

        public void OnDestroy()
        {
            LocalizationManager.LocalizationChanged -= Localize;
        }

        private void Localize()
        {

            if (isTMP)
                GetComponent<TextMeshProUGUI>().text = LocalizationManager.Localize(LocalizationKey);
            else
            {
                if (TeamGame.ImportantDataMembers.Instance.currentLanguage == LANGUAGE.Chinese)
                {
                    switch (_FONT_CHINESE)
                    {
                        case LOCALIZATION_FONT_CHINESE.CHINESE_SIMPLYFIED:
                            GetComponent<Text>().font = Example.INSTANCE.mChineseSimplyfied;
                            break;
                    }
                }
                else
                {
                    switch (_FONT_ENG)
                    {
                        case LOCALIZATION_FONT_ENGLISH.ENG_SEMIBOLD:
                            GetComponent<Text>().font = Example.INSTANCE.mEnglishTextSEMIBOLD;
                            break;

                        case LOCALIZATION_FONT_ENGLISH.ENG_REGULAR:
                            GetComponent<Text>().font = Example.INSTANCE.mEnglishTextREGULAR;
                            break;

                        case LOCALIZATION_FONT_ENGLISH.ENG_EXTRABOLD:
                            GetComponent<Text>().font = Example.INSTANCE.mEnglishTextExtraBold;
                            break;

                        case LOCALIZATION_FONT_ENGLISH.ENG_MULTI_BOLD:
                            GetComponent<Text>().font = Example.INSTANCE.mEnglishTextMultiBold;
                            break;

                        default:
                            break;
                    }
                }
                GetComponent<Text>().text = LocalizationManager.Localize(LocalizationKey);
            }
        }

    }
}