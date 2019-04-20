﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NavBox : MonoBehaviour, IEquatable<NavBox>
{
    public List<NavBox> Neighbours = new List<NavBox>();
    
    public bool Equals(NavBox obj)
    {

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        
        return gameObject.Equals(obj.gameObject);
    }

    public override int GetHashCode()
    {
        return gameObject.GetHashCode();
    }
}
