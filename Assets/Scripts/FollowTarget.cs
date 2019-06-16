using System;
using UnityEngine;
using UnityStandardAssets.Utility;

public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0f, 7.5f, 0f);


        private void FixedUpdate()
        {
			if (target) {
			transform.position = Vector3.Lerp(transform.position, target.position + offset, 0.3f);
			}
        }
    }

