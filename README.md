# Learnings

**1. Check for click-and-button combinations using Input.GetKey()**

When detecting click-and-button combinations such as Shift+Left Click, my testing shows that using `MouseDownEvent.shiftKey` is unreliable half the time! MouseDownEvent seems slow to detect changes to the non-mouse keys. Instead, use `Input.GetKey(KeyCode x)` which returns true while x is being pressed. For example:

```
// DO
MouseDownEvent evt;
bool shiftLeftClick = evt.button == 0 && Input.GetKey(KeyCode.LEFT_SHIFT);

// Do NOT
bool shiftLeftClick = evt.button == 0 && evt.shiftKey;
```

**2. Let the player rigidbody be DYNAMIC**
Previously this was kinematic since I did not want the player to be subject to gravity and physics. However, having the rigidbody be anything other than dynamic means that generally the object will not collide except under specific circumstances. A better approach is to let the player rigidbody be dynamic but set the project's physics settings to have no gravity in Edit -> Project Settings -> Physics 2D.
