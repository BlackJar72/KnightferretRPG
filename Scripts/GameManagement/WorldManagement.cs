using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace kfutils.rpg {

    public static class WorldManagement
    {

        private static Worldspace worldspace;
        private static WorldspaceLogic worldspaceLogic;

        public static Worldspace CurWorldspace => worldspace;
        public static WorldspaceLogic WorldLogic => worldspaceLogic;
        public static float SeaLevel => worldspace == null ? float.MinValue : worldspace.SeaLevel;

        private static Dictionary<string, Worldspace> worldspaceRegistry = new();


        public static readonly Dictionary<string, TeleportMarker> teleportMarkers = new();

        private static Dictionary<string, ChunkData> chunkData = new();
        public static ChunkData GetChunkData(string id) => chunkData.ContainsKey(id) ? chunkData[id] : null;
        public static void StoreChunkData(ChunkData data) => chunkData.Add(data.ID, data);
        public static Dictionary<string, ChunkData> ChunkDataRegistry => chunkData;

        public delegate void GameReload();
        public static event GameReload GameReloaded;

        public delegate void PostLoad();
        public static event PostLoad OnPostLoad;


        public static void SetWorldspace(Worldspace world)
        {
            worldspace = world;
        }


        public static void SignalGameReloaded() {
            GameReloaded?.Invoke();
        }


        public static void SignalPostLoad()
        {
            OnPostLoad?.Invoke();            
        }


        public static void TransferPC(PCMoving pc, Worldspace next, AbstractTransition transfer)
        {
            Time.timeScale = 0.0f;
            GameManager.Instance.LoadingScreen.SetActive(true);
            transferData = new TransferData(pc, transfer.DestinationID, next, transfer);
            GameManager.Instance.specialUpdates.Add(TransferCountdown);
        }


        private class TransferData
        {
            public PCMoving pc;
            public string destinationID;
            public int coundDown = 3;
            public Worldspace next;
            public AbstractTransition transfer;
            public TransferData(PCMoving pc, string destination, Worldspace next, AbstractTransition transfer)
            {
                this.pc = pc;
                destinationID = destination;
                this.next = next;
                this.transfer = transfer;
            }
        }


        private static TransferData transferData = null;


        private static bool FinishTransferPC()
        {
            SetupWorldspace();
            transferData.pc.Teleport(teleportMarkers[transferData.destinationID].transform);
            System.GC.Collect();
            GameManager.Instance.LoadingScreen.SetActive(false);
            Time.timeScale = 1.0f;
            transferData = null;
            SignalPostLoad();
            return true;
        }


        public static bool CountdownTransfer()
        {
            if (transferData != null)
            {
                transferData.coundDown--;
                if(transferData.coundDown == 2) {                    
                    if (CurWorldspace != null) SceneManager.UnloadSceneAsync(CurWorldspace.ScenePath);
                    SceneManager.LoadScene(transferData.next.ScenePath, LoadSceneMode.Additive);
                    SetWorldspace(transferData.next);
                }
                if (transferData.coundDown < 1) return FinishTransferPC();
                return false;
            }
            return true;
        }


        public static void SetupWorldspace()
        {
            Scene scene = SceneManager.GetSceneByPath(worldspace.ScenePath);
            GameObject[] gos = scene.GetRootGameObjects();
            WorldspaceLogic logic = null;
            for (int i = 0; i < gos.Length; i++)
            {
                logic = gos[i].GetComponent<WorldspaceLogic>();
                if (logic != null) break;
            }
            worldspaceLogic = logic;
            if (logic != null)
            {
                logic.Init();
            }
            worldspace.LoadASGraphData();
        }


        public static void SetChunkData(Dictionary<string, ChunkData> loaded)
        {
            chunkData = loaded;
        }


        public static GameManager.SpecialMethod TransferCountdown = CountdownTransfer;


        public static ChunkManager GetChunkFromCoords(float x, float z)
        {
            int ix = Mathf.FloorToInt((x - worldspace.ChunkOffsetX) / worldspace.ChunkSize);
            int iz = Mathf.FloorToInt((z - worldspace.ChunkOffsetZ) / worldspace.ChunkSize);
            return worldspaceLogic.GetChunk(ix, iz);
        }


        public static ChunkManager GetChunkFromTransform(Transform trans)
        {
            return GetChunkFromCoords(trans.position.x, trans.position.z);
        }


        public static void SetupWorldspaceRegistry(Worldspace[] worldspaceArr)
        {
            worldspaceRegistry.Clear();
            for (int i = 0; i < worldspaceArr.Length; i++)
            {
                if (!worldspaceRegistry.ContainsKey(worldspaceArr[i].ID)) worldspaceRegistry.Add(worldspaceArr[i].ID, worldspaceArr[i]);
            }
        }


        public static string GetCurrentWorldspaceID()
        {
            return worldspace.ID;
        }


        public static void LoadWSFromSave(string wsid)
        {
            if (worldspaceRegistry.ContainsKey(wsid))
            {
                Worldspace loaded = worldspaceRegistry[wsid];
                loaded.LoadForSave(worldspace);
                GameManager.DoPostLoadForOther();
            }
            else
            {
                Debug.LogError("Could not find worldspace " + wsid + " from save file.");
            }
        }


    }


}