using BepInEx.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Henry.Utils
{
    class Observer : MonoBehaviour
    {
        public void Awake()
        {
            Debug.Log("Hello world");
        }

        public void FixedUpdate() {
            Destroy(base.gameObject);
        }
    }
}
