using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackMove 
{
   public Vector2 position;
   public float startupFrames;
   public float activeFrames;
   public float recoveryFrames;
   public float hitbox;
   public float damage;
   public Vector2 launchAngle;
   public float launchStrength;
   public float hitstunFrames;
   public float hitstopFrames;
   public bool multiHit;
   
}
