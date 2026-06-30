using System;
using UnityEngine;

public abstract class PlayerBase : MonoBehaviour {
    public event Action OnTouched;
    public event Action OnArrowLocked;

   
    public abstract void SetMovementLocked(bool value);
    protected void RaiseTouched() {
        OnTouched?.Invoke();
    }
    protected void OnArrowLock()
    {
        OnArrowLocked?.Invoke();
    }
    
}