using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PCInteraction : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private GameObject pcScreenUI;

    [Header("プレイヤーオブジェクト（操作を停止するため）")]
    [SerializeField] private GameObject playerObject;

    private bool isUsingPC = false;
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
        if (pcScreenUI != null) pcScreenUI.SetActive(false);
    }

    void Update()
    {
        // PC使用中にESCキーを押しても閉じられるようにする
        if (isUsingPC)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                ClosePC();
            }
        }
    }

    // 視線スクリプトから呼ばれる切り替え処理
    public void TogglePC()
    {
        if (isUsingPC)
        {
            ClosePC();
        }
        else
        {
            OpenPC();
        }
    }

    public void OpenPC()
    {
        isUsingPC = true;
        if (pcScreenUI != null) pcScreenUI.SetActive(true);

        SetPlayerControl(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClosePC()
    {
        isUsingPC = false;
        if (pcScreenUI != null) pcScreenUI.SetActive(false);

        SetPlayerControl(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SetPlayerControl(bool isEnabled)
    {
        if (playerScripts == null) return;

        foreach (var script in playerScripts)
        {
            // 自分自身やインタラクト用スクリプトは除外
            if (script != this && !(script is PlayerInteraction))
            {
                script.enabled = isEnabled;
            }
        }
    }
}