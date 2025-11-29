using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Player UI")]
    public Slider healthBar;
    public GameObject curseEnergySymbol;
    public TMP_Text curseEnergyAmount;

    [Header("Black Screen")]
    public GameObject blackScreenPanel;
    public CanvasGroup blackScreenCanvasGroup;

    [Header("New Ability Screen")]
    public GameObject newAbilityPanel;
    public TMP_Text abilityGainedText;
    public TMP_Text abilityDescriptionText;
    public TMP_Text continueText;

    [Header("Game Prompt Panel")]
    public GameObject gamePromptPanel;
    public TMP_Text gamePromptText;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        blackScreenCanvasGroup.alpha = 0f;
    }

    public void UpdateHealthBar(float currentPlayerHealth)
    {
        healthBar.value = currentPlayerHealth;
    }

    public void UpdateCurseEnergyAmount()
    {
        curseEnergyAmount.text = GameManager.Instance.currentCurseEnergyAmount.ToString();
    }

    public IEnumerator FadeInBlackScreen(float fadeDuration)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            blackScreenCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        blackScreenCanvasGroup.alpha = 1f;
    }

    public IEnumerator FadeOutBlackScreen(float fadeDuration)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            blackScreenCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        blackScreenCanvasGroup.alpha = 0f;
    }

    public void SetupAbilityScreen(string abilityGained, string abilityDescription)
    {
        abilityGainedText.text = abilityGained;
        abilityDescriptionText.text = abilityDescription;
    }

    public IEnumerator AbilityGainedScreen(float fadeDuration)
    {
        float timer = 0f;
        Color currentColor = abilityGainedText.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            currentColor.a = alpha;
            abilityGainedText.color = currentColor;
            yield return null;
        }
        currentColor.a = 1f;
        abilityGainedText.color = currentColor;

        yield return new WaitForSeconds(0.5f);

        timer = 0f;
        currentColor = abilityDescriptionText.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            currentColor.a = alpha;
            abilityDescriptionText.color = currentColor;
            yield return null;
        }
        currentColor.a = 1f;
        abilityDescriptionText.color = currentColor;

        yield return new WaitForSeconds(2f);

        continueText.gameObject.SetActive(true);
    }

    public void ChangePlayerUIActive(bool isActive)
    {
        if (isActive)
        {
            healthBar.gameObject.SetActive(false);
            curseEnergySymbol.SetActive(false);
            curseEnergyAmount.gameObject.SetActive(false);
        }
        else
        {
            healthBar.gameObject.SetActive(true);
            curseEnergySymbol.SetActive(true);
            curseEnergyAmount.gameObject.SetActive(true);
        }
    }

    public void ChangeAbilityScreenActive(bool isActive)
    {
        if (isActive)
        {
            newAbilityPanel.SetActive(false);
        }
        else
        {
            newAbilityPanel.SetActive(true);
        }
    }

    public void ChangeGamePromptText(string newText)
    {
        gamePromptText.text = newText;
    }

    public void ChangeGamePromptPanelActive(bool isActive)
    {
        if (isActive)
        {
            gamePromptPanel.SetActive(false);
        }
        else
        {
            gamePromptPanel.SetActive(true);
        }
    }
}
