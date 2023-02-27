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

**3. Use out-of-the-box sprite sorting with pivot points**
[See Tutorial](https://www.youtube.com/watch?v=SlMJx3MWvfM)
- URP gives you out-of-the-box configuration to have all your sprites automatically sort their layer position by axis. In our case, we do it by Y-Axis for a top down game. Objects with higher Y-axis positions sort below objects closer to the bottom of the screen.
- We want to set custom pivot points on our sprites via the Sprite Editor to where we project the bottom of that object to be in imagined 3D space.
- When setting box colliders on objects, also project the collider size so that it is only covering the bottom face of the object (whats on the floor) in imagined 3D space. This makes it so we can walk behind objects where we expect to be able to, and for buildable objects, we don't cast the floor size of it to be larger than it should be.