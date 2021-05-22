using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    float touchedSpeed = 0.3f;
    float transitionSpeed = 0.5f;

    [SerializeField] Image continue_btn;

    [SerializeField] Text gameName_txt;
    [SerializeField] Text currentScore_txt;
    [SerializeField] Text bestScore_txt;

    public Image fog;
    void Awake() => instance = this;

    void Start() => bestScore_txt.text = "best: " + GameManager.instance.bestScore.ToString();

    #region MENU
    public void StartButtonDown(RectTransform start_btn)
    {
        start_btn.DOScale(1.15f, touchedSpeed);
        start_btn.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0.5f), touchedSpeed);

        AudioManager.instance.Play("ButtonDown");
    }
    public void StartButtonUp(RectTransform start_btn)
    {
        start_btn.GetComponent<Image>().raycastTarget = false;
        start_btn.DOScale(1, touchedSpeed);
        start_btn.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), transitionSpeed);
        gameName_txt.DOColor(new Color(1, 1, 1, 0), transitionSpeed);
        bestScore_txt.DOColor(new Color(1, 1, 1, 0), transitionSpeed);

        currentScore_txt.DOColor(Color.white, transitionSpeed);

        GameManager.instance.NewPlatform(GameManager.instance.currentPlatform);
        AudioManager.instance.Play("ButtonUp");
    }
    #endregion

    #region Game
    public void AddScore() => currentScore_txt.text = GameManager.instance.currentScore.ToString();
    #endregion

    #region GAMEOVER
    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(0.5f);

        bestScore_txt.enabled = true;
        if (GameManager.instance.newBest)
        {
            bestScore_txt.fontSize = 50;
            bestScore_txt.text = "NEW BEST";
        }
        else bestScore_txt.text = "best: " + GameManager.instance.bestScore;

        continue_btn.raycastTarget = true;
        continue_btn.DOColor(Color.white, transitionSpeed);
        bestScore_txt.DOColor(Color.white, transitionSpeed);
    }

    public void ContinueButtonDown()
    {
        continue_btn.rectTransform.DOScale(1.15f, touchedSpeed);
        continue_btn.DOColor(new Color(1, 1, 1, 0.5f), touchedSpeed);

        AudioManager.instance.Play("ButtonDown");
    }
    public void ContinueButtonUp()
    {
        GameManager.instance.LoadLevel();

        AudioManager.instance.Play("ButtonUp");
    }
    #endregion
}
