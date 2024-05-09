using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotBehaviour : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int index;
    public GameManager gameManager;
    private SlotState state = SlotState.Neutral;

    [Header("Graphics")]
    [SerializeField] private Image image;
    [SerializeField] private Sprite PlayerSprite;
    [SerializeField] private Sprite NeutralSprite;
    [SerializeField] private Sprite AiSprite;

    public SlotState State { 
        get => state;
        set {
            state = value;
            gameManager.stateBoard[index.x, index.y] = value;
            UpdateTile();
            } 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(State == SlotState.Neutral && gameManager.playersTurn)
        {
            State = SlotState.Player;
            gameManager.stateBoard[index.x, index.y] = SlotState.Player;
            gameManager.Flip(index,state);
            gameManager.PlayerHasMoved();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        image.sprite = NeutralSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateTile() {

        switch (State)
        {
            case SlotState.Player:
                image.sprite = PlayerSprite;
                break;

            case SlotState.AI:
                image.sprite = AiSprite;
                break;

            case SlotState.Neutral:
                image.sprite = NeutralSprite;
                break;

            default:
                break;
        }
    }
}
