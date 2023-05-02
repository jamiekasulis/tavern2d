# Notes for Lex

## The basics
- This is a completely **solo** game development project that I worked on in January/February of this year. This is very much a **rough prototype** that I'm building to learn the ropes of Unity and game development in general, but it is also a game that I'm very excited about! 
- This is a cafe management game. You can do things like furnish the cafe, build relationships with the townsfolk, forage for rare ingredients, set the menu, and of course, make delicious food & drink to serve. In its current state, some basic systems are implemented such as inventory, build mode, player movement, interactions, item pick ups, etc.
- Each individual feature is intentionally far from polished. I've learned that over-engineering and over-polishing too early on leads to wasted work since I'm defining the requirements and experimenting with new details as I go.
- The code is written in C# and makes heavy use of Unity's libraries for things like physics, cameras, sprites, etc. Most of the coding "work" here is in the design: how objects will relate to each other, what patterns promote decoupling, how to structure game events, how to build reusable components that make building out the level easier, etc. There aren't really any complicated algorithms yet.
- The code is presently in the middle of a refactor. I commited it to main even though it's not complete just to get some comments in there for you to see, which I hope will make the intention behind some of the classes clearer. In most other projects I would be more strict about what I commit, but since this is just a way for me to learn, I've been more relaxed about it.
- **Please note:** The code can be found in the Assets/Scripts folder.

## Why I picked this project
This project has enabled me to build a solid foundation in game development fundamentals. I'm very proud of what I have built so far, including:
- Decoupled Inventory & Inventory UI, managed by an intermediary class following the Manager-Managed design pattern
- Observer pattern and event delegation for game events, which helps me decouple components and makes building levels more scalable
- Reusable components with smaller areas of concern that allow me to share functionality across game objects. For example, I designed MeshSwapper as a component that handles just the swapping of sprites based on the direction an entity is facing in the game world. This is currently used to handle rotating furniture as you arrange your cafe, but it was designed to also be leveraged for things like player characters and NPCs.
- Data schema design that allows the number of things in the game (like distinct items or crafting recipes) to scale efficiently in Unity. This is achieved using Unity's concept of ScriptableObjects, plus my version of a lightweight database/ScriptableObject registry that supports rapid prototyping without requiring me to decide just yet on a longterm DB solution.
- Abstracted UI components that can be reused when I have more UI. Some examples of these are my homegrown grid (Unity's UI engine doesn't have grid layouts natively) and grid cells. As I build more UI, further abstractions can be done.

This project is the site of experimentation with different design patterns and data models as I figure out better ways to build games through my own trial and error.

## Where it can be improved
- Further abstracting UI components for reusability. The inventory-specific tooltip can be made into a general customizable component, for example.
- More testing. There are some unit tests in the game for more low-level things like the underlying inventory data structure, but I would like to explore how I could make something like a feature test in Unity.
- Cleaning up the physics, collider, and raycasting code. I think this is the most illegible logic in the entire project. I want to learn to write code in this area that's more readable, especially because I am most likely to forget how these pieces work with time.
- Making naming and capitalization conventions consistent. I started off the project using conventions I was used to, but these conflict with the traditional C# conventions. Whichever I end up choosing, it should be consistent across the code base.

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
