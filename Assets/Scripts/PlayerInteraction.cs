using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private float interactDistance = 2.5f; // 調べられる距離（メートル）
    [SerializeField] private LayerMask interactLayer = ~0;   // 対象レイヤー（デフォルトは全て）
    [SerializeField] private Camera playerCamera;            // プレイヤーのカメラ

    private PCInteraction currentPC = null;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        // 画面中央から前方にレイ（視線）を飛ばす
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            // 見ているオブジェクト（または親オブジェクト）からPCInteractionを探す
            PCInteraction pc = hit.collider.GetComponentInParent<PCInteraction>();
            if (pc != null)
            {
                currentPC = pc;
            }
            else
            {
                currentPC = null;
            }
        }
        else
        {
            currentPC = null;
        }

        // Eキーが押された時の処理
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (currentPC != null)
            {
                currentPC.TogglePC();
            }
        }
    }
}