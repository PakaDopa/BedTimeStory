using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameUtil
{
    public static class GameUtil 
    {
        public static float GetSqrDistWith(this Vector3 currPos, Vector3 targetPos)
        {   
            return (targetPos - currPos).sqrMagnitude;
        }

        public static float GetDistWith(this Vector3 currPos, Vector3 targetPos)
        {   
            return Vector3.Distance(currPos, targetPos);
        }

        public static Vector3 WithFloorHeight(this Vector3 pos)
        {
            return new Vector3(pos.x,0,pos.z);
        }
        public static Vector3 WithPlayerWaistHeight(this Vector3 pos)
        {
            return new Vector3(pos.x,0.5f,pos.z);
        }

        public static Vector3 WithPlayerHeadHeight(this Vector3 pos)
        {
            return new Vector3(pos.x,1,pos.z);
        }

        
        public static Vector3 WithTestHeight(this Vector3 pos)
        {
            return new Vector3(pos.x,1.5f,pos.z);
        }
    }

    public static class GameConstants
    {
        public static int playerLayer =  1<<LayerMask.NameToLayer("Player");
        public static int enemyLayer =  1<<LayerMask.NameToLayer("Enemy");

        public static int towerLayer =  1<<LayerMask.NameToLayer("Tower");
    }
}

