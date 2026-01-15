using System;
using UnityEngine;
using System.Collections.Generic;
using DistantLands.Cozy.Data;
using UnityEngine.Serialization;

namespace DistantLands.Cozy
{
    public class CozySearchable : PropertyAttribute
    {
        public string[] keywords;
        public bool deepSearch;

        public CozySearchable(params string[] keywords)
        {

            this.keywords = keywords;
            this.deepSearch = false;

        }
        public CozySearchable(bool deepSearch, params string[] keywords)
        {

            this.keywords = keywords;
            this.deepSearch = deepSearch;

        }
    }
}