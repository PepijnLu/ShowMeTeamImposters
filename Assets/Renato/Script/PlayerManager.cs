using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private int HP = 400; // (100 per bar) (can do something like increased damage intake at each new bar layer)
    [SerializeField] private int damage = 5;
    [SerializeField] private float layerHP = 100f; // Initialize with full layer capacity
    [SerializeField] private string playerName = "";
    
    // ComboManagement
    public ComboManagement comboManagement;
    
    // PlayerUIManagement
    public PlayerUIManagement playerUIManagement;
    
    void Start() 
    {
        if(playerName == "Player1") 
        {
            playerUIManagement.InstantiatePlayerUI(UIManager.instance.UIPrefabPlayer1, UIManager.instance.player1_UI);
        }
        
        if(playerName == "Player2") 
        {
            playerUIManagement.InstantiatePlayerUI(UIManager.instance.UIPrefabPlayer2, UIManager.instance.player2_UI);
        }
    }

    public void Attack(InputAction.CallbackContext ctx) 
    {
        if(ctx.performed) 
        {
            Debug.Log("Input Detected");
            comboManagement.CheckComboKeyPress(playerUIManagement, HP, layerHP, damage);
        }
    }    

    // public void DoDamage(int incomingDamage) 
    // {
    //     HP -= incomingDamage;
    //     HP = Mathf.Max(HP, 0);

    //     layerHP -= incomingDamage;
    //     layerHP = MathF.Max(layerHP, 0);

    //     // Debug.Log($"Layer HP: {layerHP} + Player HP: {HP}");

    //     playerUIManagement.UpdateHealthBar(HP, ref layerHP);

    //     if(HP <= 0) 
    //     {
    //         HP = 0;
    //         Debug.Log("Death...");
    //         // RestartPlayMode();
    //     }
    // }

    private void RestartPlayMode() 
    {
        if(EditorApplication.isPlaying) 
        {
            EditorApplication.isPlaying = false;
        }

        EditorApplication.isPlaying = true;
    }
}
