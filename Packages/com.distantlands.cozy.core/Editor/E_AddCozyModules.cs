//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace DistantLands.Cozy {
    [InitializeOnLoad]
    public class E_AddCozyModules : Editor
    {
        public E_AddCozyModules()
        {

            List<Type> listOfMods = (
          from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
          from type in domainAssembly.GetTypes()
          where typeof(CozyModule).IsAssignableFrom(type) && type != typeof(CozyModule)
          select type).ToList();

            listOfMods.Remove(typeof(CozyModule));


        }
    }
}