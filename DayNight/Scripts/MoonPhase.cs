using UnityEngine;


namespace kfutils.skies
{

    [System.Serializable]
    public class MoonPhase
    {
        [SerializeField] Cubemap moonSky;
        [SerializeField] int nextStart;
        [SerializeField] int nextPhaseIndex;
        [SerializeField] float brightness;

        public Cubemap MoonSky => moonSky;
        public int NextStart => nextStart;
        public int NextPhaseIndex => nextPhaseIndex;
        public float Brightness => brightness;
    }

}
