using UnityEngine;

public class rotate : MonoBehaviour
{
    public GameObject tar;
    public bool only_begin;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!only_begin || Time.time < 2)
            transform.LookAt(tar.transform);
    }
}
