using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static AttackShape;

public class CombatButton : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    public AttackKeys attack;
    public bool isTempAttack;
    DemoPlayer playerUI;

    [SerializeField] private Image img;
    private Sprite icon;
    public Sprite Icon
    {
        get => icon;
        set
        {
            icon = value;
            img.sprite = value;
        }
    }

    public Toggle.ToggleEvent onValueChanged { get => toggle.onValueChanged; }

    public void Initialize(AttackKeys attack, bool isTempAttack, DemoPlayer playerUI)
    {
        this.attack = attack;
        this.isTempAttack = isTempAttack;
        Icon = TileEffectLibrary.Instance.TileUIs[AttackDictionary[attack].TileAnimation];
        this.playerUI = playerUI;
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(bool isOn)
    {
        playerUI.ClickedCombatButton(this, isOn);
    }

    public void Deactivate()
    {
        //toggle.isOn = false;
        toggle.SetIsOnWithoutNotify(false);
    }
}
