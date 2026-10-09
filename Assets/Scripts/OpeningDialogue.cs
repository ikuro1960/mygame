using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // 新しいInput Systemを使用

public class OpeningDialogue : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private GameObject openingPanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("プレイヤーオブジェクト（Hierarchyから直接入れる）")]
    [SerializeField] private GameObject playerObject;

    [Header("セリフリスト")]
    [TextArea(2, 5)]
    [SerializeField] private string[] dialogueLines = new string[]
    {
        "――時計は深夜2時を回っていた。",
        "締め切り間近の原稿。フリーライターになってから、こんな夜更かしばかりだ。",
        "最近、街で囁かれている「とある集団」の噂……記事のネタにはなりそうだが。",
        "……腹が減ったな。少し頭を冷やすついでに、近所で夜食でも買ってくるか。"
    };

    private int currentIndex = 0;
    private MonoBehaviour[] playerScripts;

    void Awake()
    {
        if (playerObject != null)
        {
            playerScripts = playerObject.GetComponentsInChildren<MonoBehaviour>();
        }
    }

    void Start()
    {
        SetPlayerControl(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (dialogueLines.Length > 0 && dialogueText != null)
        {
            dialogueText.text = dialogueLines[0];
        }
    }

    void Update()
    {
        bool isClicked = false;

        // 新しいInput Systemでのマウス左クリック・キー入力判定
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            isClicked = true;
        }

        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame ||
                Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.eKey.wasPressedThisFrame)
            {
                isClicked = true;
            }
        }

        if (isClicked)
        {
            AdvanceDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        currentIndex++;

        if (currentIndex < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentIndex];
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        if (openingPanel != null) openingPanel.SetActive(false);

        SetPlayerControl(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        this.enabled = false;
    }

    private void SetPlayerControl(bool isEnabled)
    {
        if (playerScripts == null) return;

        foreach (var script in playerScripts)
        {
            if (script != this)
            {
                script.enabled = isEnabled;
            }
        }
    }
}