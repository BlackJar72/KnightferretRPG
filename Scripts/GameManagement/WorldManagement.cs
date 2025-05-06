using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace kfutils.rpg {

    public static class WorldManagement {   

        private static Worldspace worldspace;
        private static WorldspaceLogic worldspaceLogic;

        public static Worldspace CurWorldspace => worldspace;
        public static WorldspaceLogic WorldLogic => worldspaceLogic;
        public static float SeaLevel => worldspace == null ? float.MinValue : worldspace.SeaLevel;

        public static readonly Dictionary<string, TeleportMarker> teleportMarkers = new();


        public static void SetWorldspace(Worldspace world) {
            worldspace = world;
        }


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
            SetupWorldspace();
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


        public static void SetupWorldspace() { 
            Scene scene = SceneManager.GetSceneByPath(worldspace.ScenePath);
            GameObject[] gos = scene.GetRootGameObjects();
            WorldspaceLogic logic = null;
            for(int i = 0; i < gos.Length; i++) {
                logic = gos[i].GetComponent<WorldspaceLogic>();
                if(logic != null) break;
            }
            worldspaceLogic = logic;
            if(logic != null) {
                logic.Init();
            }
        }


        public static GameManager.SpecialMethod TransferCountdown = CountdownTransfer;


        public static ChunkManager GetChunkFromCoords(float x, float z) {
            int ix = Mathf.FloorToInt((x - worldspace.ChunkOffsetX) / worldspace.ChunkSize); 
            int iz = Mathf.FloorToInt((z - worldspace.ChunkOffsetZ ) / worldspace.ChunkSize); 
            return worldspaceLogic.GetChunk(ix, iz);             
        }


        public static ChunkManager GetChunkFromTransform(Transform trans) {
            return GetChunkFromCoords(trans.position.x, trans.position.z);
        }


        


    }


}