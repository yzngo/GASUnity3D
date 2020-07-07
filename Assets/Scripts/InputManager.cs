﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour {
    public static List<GameObject> ActiveInputComponents = new List<GameObject>();

    public List<InputHandler> Inputs = new List<InputHandler>();

    // Start is called before the first frame update
    void Awake() {
        if (!ActiveInputComponents.Find(x => x == this)) {
            ActiveInputComponents.Add(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update() {
        for (int i = 0; i < Inputs.Count; i++) {
            bool triggerEvent = false;
            var input = Inputs[i];
            if (input.ButtonState.HasFlag(EButtonState.ButtonDown) &&
                Input.GetButtonDown(input.Button)) triggerEvent = true;

            if (input.ButtonState.HasFlag(EButtonState.ButtonUp) &&
                Input.GetButtonUp(input.Button)) triggerEvent = true;

            if (input.ButtonState.HasFlag(EButtonState.ButtonHeld) &&
                Input.GetButton(input.Button)) triggerEvent = true;

            if (triggerEvent) {
                input.Handler?.Invoke();
            }
        }
    }

    void OnDestroy() {
        ActiveInputComponents.Remove(this.gameObject);
    }
}

[Serializable]
public struct InputHandler {
    public string Button;
    public EButtonState ButtonState;
    public UnityEvent Handler;

}

[Flags]
public enum EButtonState {
    ButtonDown = 0x01,
    ButtonUp = 0x02,
    ButtonHeld = 0x04
}