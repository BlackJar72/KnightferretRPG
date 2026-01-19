using UnityEngine;
using kfutils;
using kfutils.rpg;
using UnityEngine.InputSystem;
using kfutils.rpg.ui;

public class FakeCast : MonoBehaviour
{
    [SerializeField] EntityLiving livingEntity;
    [SerializeField] AbstractAction castAnimation;
    protected PlayerInput input;
    protected InputAction castSpellAction;
    protected PCActing pc;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = GetComponent<PlayerInput>();
        castSpellAction = input.actions["CastSpell"];
        castSpellAction.started += DummyCast;
        pc = GetComponent<PCActing>();
    }


    protected void DummyCast(InputAction.CallbackContext context) {
        livingEntity.mana.UseMana(10f);
        livingEntity.health.TakeDamage(DamageUtils.CalcDamageNoArmor(20));
        GameManager.Instance.UI.ShowToast("Casting Self Harm!");
        pc.ActionLayer.SetMask(castAnimation.mask);
        pc.ActionLayer.Play(castAnimation.anim).Time = 0; 
    }

    
}
