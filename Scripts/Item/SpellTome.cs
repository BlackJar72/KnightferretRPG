using UnityEngine;


namespace kfutils.rpg
{


    public class SpellTome : ItemEquipt, IUsable
    {
        [SerializeField] protected Spell spell;


        public AbstractAction UseAnimation => null;
        public int StaminaCost => 0;
        public int PowerAttackCost => 0;


        public void OnEquipt(IActor actor) {}


        public void OnUnequipt() {}


        public void OnUse(IActor actor)
        {
            if(actor is PCTalking pc)
            {   
                if(pc.Spells.LearnSpell(spell, pc.attributes))
                {
                    GameManager.Instance.UI.ShowToast("You learned " + spell.Name);
                }
                else if(pc.Spells.HasItem(spell))
                {
                    GameManager.Instance.UI.ShowToast("You already know " + spell.Name);
                } else
                {
                    GameManager.Instance.UI.ShowToast("You are unable to learn " + spell.Name + " with current stats.");
                }
            }
            // Should I include a way for NPCs to learn them?  Probably not needed.
        }


        public void OnUseCharged(IActor actor)
        {
            OnUse(actor);
        }


        public void PlayEquipAnimation(IActor actor) {}


        public void PlayUseAnimation(IActor actor) {}



    }


}
