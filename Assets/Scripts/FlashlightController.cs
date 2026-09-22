using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    [Header("ライト設定")]
    [Tooltip("操作対象のSpot Lightオブジェクト")]
    [SerializeField] private GameObject flashlightObject;

    [Tooltip("ゲーム開始時に点灯しておくか")]
    [SerializeField] private bool startsOn = false;

    [Header("レシート連携 (任意)")]
    [Tooltip("レシート管理スクリプト（開いている間は切り替え無効）")]
    [SerializeField] private ReceiptController receiptController;

    [Header("効果音 (任意)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip toggleSound;

    private bool isOn = false;

    private void Start()
    {
        isOn = startsOn;
        if (flashlightObject != null)
        {
            flashlightObject.SetActive(isOn);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        // レシートが開いている間は操作不能にする
        if (receiptController != null && receiptController.IsReceiptOpen)
        {
            return;
        }

        // FキーでON/OFF切り替え
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            ToggleFlashlight();
        }
    }

    public void ToggleFlashlight()
    {
        if (flashlightObject == null) return;

        isOn = !isOn;
        flashlightObject.SetActive(isOn);

        if (audioSource != null && toggleSound != null)
        {
            audioSource.PlayOneShot(toggleSound);
        }
    }
}