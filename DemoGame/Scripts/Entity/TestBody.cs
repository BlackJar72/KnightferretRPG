using System.Collections.Generic;
using UnityEngine;
using kfutils.rpg;


namespace rpg.verslika {


    public class TestBody : AbstractBody
    {
        [SerializeField] List<KeyedAttribute<List<KeyedAttribute<GameObject>>>> categories;


        public override void Refresh() {}
    }

}
