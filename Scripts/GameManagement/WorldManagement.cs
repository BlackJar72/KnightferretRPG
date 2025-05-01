using System.Collections.Generic;
using kfutils.rpg;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace kfutils.rpg {

    public static class WorldManagement {

        private static Worldspace worldspace;


        public static Worldspace CurWorldspace => worldspace;
        public static float SeaLevel => worldspace == null ? float.MinValue : worldspace.SeaLevel;

        public static readonly Dictionary<string, TeleportMarker> teleportMarkers = new();


        public static void SetWorldspace(Worldspace world) {
            worldspace = world;
        }


        // This is not precise; it seems the origin, though always resolving to (0,0,0), is not 
        // the same between scenes, and a scene may not always load with the same basis relative
        // to others.
        //
        // I suspect that the way to fix this is to use teleport markers.  These would have labels
        // and registers themselves to a dictionary keyed to the label on Awake() (or perhaps during 
        // OnEnable()), and remove themsevle with OnDestroy() (or perhaps during OnDisable()).  On 
        // completing the workspace load (and unload of the previous worldspace) the player would be 
        // teleported to the coords of the teleport marker.  I would need to put the delay (mediated 
        // by the GameManager through a SpecialUpdate) back in, though, to give the teleport maker, 
        // along with the game objects, to be fully loaded and activate, as they are not available 
        // imediately after LoadScene() exits as would have expected.  This should hopefully allow 
        // for accurate and precise positioning of the player in the new worldspace.
        public static void TransferPC(PCMoving pc, Worldspace next, AbstractTransition transfer) {
            Time.timeScale = 0.0f;
            if(CurWorldspace != null) SceneManager.UnloadSceneAsync(CurWorldspace.ScenePath);
            SceneManager.LoadScene(next.ScenePath, LoadSceneMode.Additive);
            SetWorldspace(next);  
            transferData = new TransferData(pc, transfer.DestinationID);
            GameManager.Instance.specialUpdates.Add(TransferCountdown);
        }


        private class TransferData {
            public PCMoving pc;
            public string destinationID;
            public int coundDown = 3;
            public TransferData(PCMoving pc, string destination) {
                this.pc = pc;
                destinationID = destination;
            }
        }


        private static TransferData transferData = null;


        private static bool FinishTransferPC() {             
            transferData.pc.Teleport(teleportMarkers[transferData.destinationID].transform);
            Time.timeScale = 1.0f;
            transferData = null;
            return true;
        }


        public static bool CountdownTransfer() {
            if(transferData != null) {
                transferData.coundDown--;
                if(transferData.coundDown < 1) return FinishTransferPC();
                return false;
            } 
            return true;
        }


        public static GameManager.SpecialMethod TransferCountdown = CountdownTransfer;

    }


}