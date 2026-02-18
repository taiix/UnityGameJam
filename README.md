# Unity Game Jam Project

A first-person interaction game built in Unity featuring time-based ingredients, puzzle mechanics, and environmental interactions.

## 🎮 About

This is a Unity game jam project that combines exploration, interaction mechanics, and puzzle-solving elements. Players navigate a first-person environment, collecting ingredients from different time periods and interacting with various objects to progress through the game.

## ✨ Features

### Player Mechanics
- **First-Person Movement** - WASD movement with sprint capability
- **Camera Control** - Mouse look with customizable sensitivity
- **Jump System** - Physics-based jumping with ground detection
- **Crouch Mechanic** - Stealth and navigation option

### Interaction System
- **Object Pickup** - Hold and carry objects with physics
- **Environmental Interactions** - Open cupboards, move bookshelves, and discover secrets
- **Visual Feedback** - Interactive objects highlight on hover
- **Contextual UI** - Dynamic interaction prompts

### Ingredient & Crafting
The game features a unique time-based ingredient system:
- **Mushroom From The Past** 🍄
- **Fresh From The Present** 🌿
- **Powder From The Future** ✨
- **Swords** - Rusty Sword, Normal Sword, and Excalibur

### Audio System
- **Sound Manager** - Centralized audio management with named clips
- **Audio Mixing** - Separate channels for music and sound effects
- **Volume Control** - Master and music volume controls
- **Dynamic Sound Effects** - Contextual audio for interactions

## 🕹️ Controls

| Action | Input |
|--------|-------|
| Move | `W` `A` `S` `D` |
| Look | `Mouse` |
| Jump | `Space` |
| Sprint | `Left Shift` |
| Crouch | `Left Ctrl` |
| Interact | `F` |
| Grab/Hold | `E` (Hold) |
| Attack | `Left Mouse Button` |
| Previous/Next Item | `Mouse Wheel` / `Q` `E` |

## 🛠️ Technical Details

### Technologies
- **Engine**: Unity
- **Language**: C# (73.1%)
- **Shaders**: ShaderLab (26.9%)
- **Input System**: Unity's New Input System
- **Physics**: Unity Rigidbody-based character controller

### Key Systems

#### Character Movement (`CharacterMovement.cs`)
- Rigidbody-based movement with force application
- Slope detection and valid ground checking
- Cinemachine integration for smooth camera follow
- Sprint toggle system

#### Interaction Handler (`InteractionHandler.cs`)
- Raycast-based interaction detection
- Support for multiple interaction types
- UI feedback system
- Grab and hold mechanics

#### Sound Manager (`SoundManager.cs`)
- Singleton pattern for global access
- Dictionary-based clip lookup
- Dynamic audio source pooling
- Support for random clip variations

#### Environmental Objects
- **Bookshelf** - Timed sliding mechanics with audio feedback
- **Wood Cupboard** - Animated door opening with smooth rotations
- **Knight** - Auto-equips swords when player approaches

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── CharacterMovement.cs
│   ├── InteractionHandler.cs
│   ├── SoundManager.cs
│   ├── Ingredient.cs
│   ├── Knight.cs
│   ├── Bookshelf.cs
│   ├── WoodCupboardInteractable.cs
│   ├── PickupInteractable.cs
│   ├── SceneSwitch.cs
│   ├── SkyboxRotator.cs
│   └── HighlightOnHover.cs
├── Prefabs/
│   └── SoundManager.prefab
├── Audio/
│   └── MainMixer.mixer
└── InputSystem/
    ├── InputSystem_Actions.inputactions
    └── InputSystem.inputsettings.asset
```

## 🚀 Getting Started

### Prerequisites
- Unity 2021.3 LTS or later recommended
- Visual Studio or JetBrains Rider (optional)

### Installation
1. Clone this repository:
   ```bash
   git clone https://github.com/taiix/UnityGameJam.git
   ```
2. Open the project in Unity Hub
3. Let Unity import all assets and dependencies
4. Open the main scene
5. Press Play to start

### Building
1. Go to `File > Build Settings`
2. Select your target platform
3. Click `Build` or `Build and Run`

## 🎯 Game Jam Notes

This project was created for a game jam with a focus on:
- **Rapid prototyping** - Clean, modular systems
- **Reusable components** - Generic interaction interfaces
- **Polish** - Smooth animations and audio feedback
- **Time management** - Core mechanics implemented efficiently

## 🤝 Contributing

This is a game jam project, but feel free to fork and experiment! If you find bugs or have suggestions:
1. Fork the repository
2. Create a feature branch
3. Submit a pull request

## 📝 License

This project is open source and available for learning purposes.

## 👤 Author

**taiix**
- GitHub: [@taiix](https://github.com/taiix)

---

Made with ❤️ during a Unity Game Jam
