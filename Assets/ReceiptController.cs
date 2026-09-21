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

    void Update()
    {
        if (Keyboard.current == null) return;

        // Tabキーで開閉切り替え
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleReceipt();
        }
    }

    public void ToggleReceipt()
    {
        if (receiptPanel == null) return;

        isReceiptOpen = !isReceiptOpen;
        receiptPanel.SetActive(isReceiptOpen);

        if (isReceiptOpen)
        {
            OnOpenReceipt();
        }
        else
        {
            OnCloseReceipt();
        }
    }

    private void OnOpenReceipt()
    {
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnCloseReceipt()
    {
        if (audioSource != null && closeSound != null)
        {
            audioSource.PlayOneShot(closeSound);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}