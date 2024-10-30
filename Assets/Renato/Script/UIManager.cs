using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject UIPrefab;
    public Transform player1_UI, player2_UI;

    void Awake() 
    {
        if(instance != null && instance != this) 
        {
            Destroy(this);
        }
        else 
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void Start()
    {
        // StartCoroutine(playerUI.IntantiatePlayerUI(UIPrefab, player1_UI));
    }

    void Update()
    {
        
    }
}
