using System.Collections.Generic;
using UnityEngine;

public class Cardbar : MonoBehaviour
{
    public static Cardbar Instance;
    
    [SerializeField] private Card taskCardPrefab;
    
    private List<Card> _cards = new List<Card>();

    private void Awake()
    {
        Instance = this;
    }
    
    public void ShowTaskCard(int key)
    {
        Clear();
        
        var card = Instantiate(taskCardPrefab, transform);
        card.Init(key);
        
        _cards.Add(card);
    }
    
    //public void ShowCollisionCard(string id)

    public void Clear()
    {
        foreach (var card in _cards)
            Destroy(card.gameObject);
        _cards.Clear();
    }
}
