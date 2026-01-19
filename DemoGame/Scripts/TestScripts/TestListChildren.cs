using UnityEngine;

public class TestListChildren : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Terrain[] kids = gameObject.GetComponentsInChildren<Terrain>();
        print(kids.Length);
        for(int i = 0; i < kids.Length; i++) print(kids[i].gameObject.name);
        print(kids.Length);
    }
}
