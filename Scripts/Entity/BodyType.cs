using UnityEngine;


namespace kfutils.rpg {


    [CreateAssetMenu(menuName = "KF-RPG/World/Body Type", fileName = "Body", order = -56)]
    public class BodyType : ScriptableObject
    {
        [Tooltip("This should hold the main object so it for special reasons.")]
        [SerializeField] GameObject MainBody;

        // Main body part options
        [SerializeField] GameObject[] heads;
        [SerializeField] GameObject[] torso;
        [SerializeField] GameObject[] hands;
        [SerializeField] GameObject[] legs;
        [SerializeField] GameObject[] feet;

        // Other part options
        [SerializeField] GameObject[] eyes;
        [SerializeField] GameObject[] hair;
        [SerializeField] GameObject[] brows;
        [SerializeField] GameObject[] beard;

        // Clothing and Special
        [SerializeField] GameObject[] headGear;
    }

}