using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingBase : MonoBehaviour
{
    [SerializeField] EFaction _Faction;
    [SerializeField] int _MaxHealth = 100;
    [SerializeField] int _CurrentHealth = 100;
    [SerializeField] TextMeshProUGUI FeedbackLabel;

    public EFaction Faction => _Faction;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        UpdateHealthDisplay();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected void UpdateHealthDisplay()
    {
        FeedbackLabel.text = string.Format("{0} / {1}", _CurrentHealth, _MaxHealth);
    }

    public void TakeDamage(int amount)
    {
        _CurrentHealth = Mathf.Clamp(_CurrentHealth - amount, 0, _MaxHealth);
        UpdateHealthDisplay();
    }

    public void RepairDamage(int amount)
    {
        _CurrentHealth = Mathf.Clamp(_CurrentHealth + amount, 0, _MaxHealth);
        UpdateHealthDisplay(); 
    }
}
