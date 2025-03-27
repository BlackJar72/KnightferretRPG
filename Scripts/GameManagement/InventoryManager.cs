using UnityEngine;
using kfutils.rpg.ui;
using System.Collections.Generic;
using Unity.VisualScripting;



namespace kfutils.rpg {

    public static class InventoryManager {

        private static List<IRedrawing> waitingToRedraw = new ();


        public static void Initialize() {
            waitingToRedraw = new List<IRedrawing>();
        }


        public static void AddRedraw(IRedrawing uiElement) {
            waitingToRedraw.Add(uiElement);
        }


        public static void DoRedraws() {
            for(int i = waitingToRedraw.Count - 1; i > -1; i--) {
                waitingToRedraw[i].DoRedraw();
                waitingToRedraw.RemoveAt(i);
            }
        }

    }


}
