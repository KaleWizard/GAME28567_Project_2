using System.Collections.Generic;
using UnityEngine;

public class LadderRegister
{
    private static LadderRegister instance;

    public static LadderRegister Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = new LadderRegister();
            }

            return instance;
        }
    }

    public List<BoxCollider2D> ladderColliders = new();
}
