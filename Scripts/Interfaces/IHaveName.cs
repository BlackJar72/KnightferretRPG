using UnityEngine;


namespace kfutils.rpg
{

    public interface IHaveName
    {
        // The generic name of the entity type
        string GetName();

        // For entities with a personal name; for others this should wrap GetName()
        string GetPersonalName();  
    }


}