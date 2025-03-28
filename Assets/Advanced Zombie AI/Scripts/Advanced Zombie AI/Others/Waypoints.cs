using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMS_AdvancedZombieAI
{
    public class Waypoints : MonoBehaviour
    {
        public enum ToDo
        {
            Nothing,
            Scream,
            Idle,
            Custom,
        }
        public ToDo toDo = ToDo.Nothing;

        public float timeToWait = 5f;
        public AnimationClip customAnimation;
    }
}