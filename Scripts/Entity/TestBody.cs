using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg {


    public class TestBody : AbstractBody
    {
        [SerializeField] List<KeyedAttribute<List<KeyedAttribute<GameObject>>>> categories;


        public override void Refresh() {}
    }

}
