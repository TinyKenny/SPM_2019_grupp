using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Superclass/interface for all EventListeners that makes sure all subclasses has an <see cref="Initialize"/> function.
/// The <see cref="EventCoordinator"/> initializes all EventListeners through their respective <see cref="Initialize"/> 
/// functions.
/// </summary>
public abstract class EventListenerInterface : MonoBehaviour
{
    /// <summary>
    /// Runs after the <see cref="EventCoordinator"/> is done with its awake method and its instance is set to initialize 
    /// subclasses/EventListeners.
    /// </summary>
    public virtual void Initialize()
    {

    }
}
