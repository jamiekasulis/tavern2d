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