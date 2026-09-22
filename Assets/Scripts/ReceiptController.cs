using UnityEngine;
using UnityEngine.InputSystem;

public class ReceiptController : MonoBehaviour
{
    [Header("UI参照")]
    [Tooltip("開閉するレシートのパネルGameObject")]
    [SerializeField] private GameObject receiptPanel;

    [Header("サウンド (任意)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    private bool isReceiptOpen = false;
    public bool IsReceiptOpen => isReceiptOpen;

    private void Update()
    {
        if (Keyboard.current == null) return;

        // Tabキーで開閉トグル
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleReceipt();
            return;
        }

        // 開いているときにEscapeキーが押されたら閉じる
        if (isReceiptOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseReceipt();
        }
    }

    public void ToggleReceipt()
    {
        if (isReceiptOpen)
        {
            CloseReceipt();
        }
        else
        {
            OpenReceipt();
        }
    }

    public void OpenReceipt()
    {
        if (receiptPanel == null) return;

        isReceiptOpen = true;
        receiptPanel.SetActive(true);

        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseReceipt()
    {
        if (receiptPanel == null) return;

        isReceiptOpen = false;
        receiptPanel.SetActive(false);

        if (audioSource != null && closeSound != null)
        {
            audioSource.PlayOneShot(closeSound);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}