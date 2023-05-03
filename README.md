# Notes for Lex

**Please note: The code can be found in the Assets/Scripts folder, in the files ending with .cs . Unity creates a bunch of other files automatically which can be ignored, such as all the .meta ones.**

I'll also highlight some specific files to look at in case that helps focus the repo a bit:

- [Inventory.cs](https://github.com/jamiekasulis/tavern2d/blob/main/Assets/Scripts/Inventory/Inventory.cs), [Item.cs](https://github.com/jamiekasulis/tavern2d/blob/main/Assets/Scripts/Item/Item.cs), [ItemQuantity.cs](https://github.com/jamiekasulis/tavern2d/blob/main/Assets/Scripts/Item/ItemQuantity.cs), [InventoryManager.cs](https://github.com/jamiekasulis/tavern2d/blob/main/Assets/Scripts/Inventory/InventoryManager.cs), and [InventoryMenu.cs](https://github.com/jamiekasulis/tavern2d/blob/main/Assets/Scripts/InventoryMenu/InventoryMenu.cs) work together to compose the inventory system
- [Mesh2D.cs](https://github.com/jamiekasulis/tavern2d/blob/main/Assets/Scripts/BuildMode/Mesh2D.cs) and [MeshSwapper.cs](https://github.com/jamiekasulis/tavern2d/blob/main/Assets/Scripts/BuildMode/MeshSwapper.cs) set up functionality for swapping the visual representation of a game object according to the direction it's facing. (Think rotating furniture, or changing walking directions)
- [BuildMode.cs](https://github.com/jamiekasulis/tavern2d/blob/main/Assets/Scripts/BuildMode/BuildMode.cs), [BuildableGridArea.cs](https://github.com/jamiekasulis/tavern2d/blob/main/Assets/Scripts/BuildMode/BuildableGridArea.cs), and [PlaceableObject.cs](https://github.com/jamiekasulis/tavern2d/blob/main/Assets/Scripts/BuildMode/PlaceableObject.cs) work together to compose the build mode system, where users can move around furniture in a given bounds.

## The basics
- This is a completely **solo** game development project that I worked on in January/February of this year. This is very much a **rough prototype** that I'm building to learn the ropes of Unity and game development in general, but it is also a game that I'm very excited about! 
- This is a cafe management game. You can do things like furnish the cafe, build relationships with the townsfolk, forage for rare ingredients, set the menu, and of course, make delicious food & drink to serve. In its current state, some basic systems are implemented such as inventory, build mode, player movement, interactions, item pick ups, etc.
- Each individual feature is intentionally far from polished. I've learned that over-engineering and over-polishing too early on leads to wasted work since I'm defining the requirements and experimenting with new details as I go.
- The code is written in C# and makes heavy use of Unity's libraries for things like physics, cameras, sprites, etc. Most of the coding "work" here is in the design: how objects will relate to each other, what patterns promote decoupling, how to structure game events, how to build reusable components that make building out the level easier, etc. There aren't really any complicated algorithms yet.
- The code is presently in the middle of a refactor. I commited it to main even though it's not complete just to get some comments in there for you to see, which I hope will make the intention behind some of the classes clearer. In most other projects I would be more strict about what I commit, but since this is just a way for me to learn, I've been more relaxed about it.

## Why I picked this project
This project has enabled me to build a solid foundation in game development fundamentals. It has been the site of experimentation with different design patterns and data models as I figure out better ways to build games through my own trial and error. I'm very proud of what I have built so far, including:
- Decoupled Inventory & Inventory UI, managed by an intermediary class following the Manager-Managed design pattern
- Observer pattern and event delegation for game events, which helps me decouple components and makes building levels more scalable
- Reusable components with smaller areas of concern that allow me to share functionality across game objects. For example, I designed MeshSwapper as a component that handles just the swapping of sprites based on the direction an entity is facing in the game world. This is currently used to handle rotating furniture as you arrange your cafe, but it was designed to also be leveraged for things like player characters and NPCs.
- Data schema design that allows the number of things in the game (like distinct items or crafting recipes) to scale efficiently in Unity. This is achieved using Unity's concept of ScriptableObjects, plus my version of a lightweight database/ScriptableObject registry that supports rapid prototyping without requiring me to decide just yet on a longterm DB solution.
- Abstracted UI components that can be reused when I have more UI. Some examples of these are my homegrown grid (Unity's UI engine doesn't have grid layouts natively) and grid cells. As I build more UI, further abstractions can be done.
- And lastly, I have also been learning/practicing pixel art and 3D modeling for the game!

## Where it can be improved
- Further abstracting UI components for reusability. The inventory-specific tooltip can be made into a general customizable component, for example.
- More testing. There are some unit tests in the game for more low-level things like the underlying inventory data structure, but I would like to explore how I could make something like a feature test in Unity.
- Cleaning up the physics, collider, and raycasting code. I think this is the most illegible logic in the entire project. I want to learn to write code in this area that's more readable, especially because I am most likely to forget how these pieces work with time.
- Making naming and capitalization conventions consistent. I started off the project using conventions I was used to, but these conflict with the traditional C# conventions. Whichever I end up choosing, it should be consistent across the code base.
- Continuing to refactor to promote clean separations of concern.

