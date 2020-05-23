﻿using UnityEngine;

public class StayCentered : MonoBehaviour
{
    [SerializeField]
    private float effort = 3;

    private new Rigidbody rigidbody;
    private float centerZ;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        centerZ = rigidbody.position.z;
    }

    private void FixedUpdate()
    {
        var zPos = Mathf.MoveTowards(rigidbody.position.z, centerZ, effort * Time.fixedDeltaTime);
        rigidbody.MovePosition(new Vector3(rigidbody.position.x, rigidbody.position.y, zPos));
    }
}