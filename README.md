# Asterobitus - Modern Asteroids Game

A modern remake of the classic Asteroids arcade game built with Unity. Navigate through space, destroy asteroids, and survive as long as possible while racking up points!

## Table of Contents
- [Game Overview](#game-overview)
- [Features](#features)
- [How to Play](#how-to-play)
- [Game Mechanics](#game-mechanics)
- [Technical Architecture](#technical-architecture)
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [Development](#development)

## Game Overview

Asterobitus is a 2D space shooter where players control a spaceship navigating through an asteroid field. The objective is to destroy asteroids while avoiding collisions, earning points, and surviving increasingly challenging waves of asteroids.

## Features

### Core Gameplay
- **Classic Asteroids Mechanics**: Navigate a spaceship through space destroying asteroids
- **Multiple Asteroid Sizes**: Large, medium, and small asteroids with different behaviors
- **Dynamic Difficulty**: Asteroid spawn rate increases over time
- **Score System**: Earn points for destroying asteroids, with high score tracking
- **Health System**: Player has multiple lives with invulnerability periods after taking damage

### Weapons System
- **Standard Weapon**: Single projectile with balanced damage and speed
- **Double Shot Weapon**: Fires two projectiles simultaneously with spread pattern
- **Weapon Switching**: Toggle between weapons during gameplay with Q key

### Audio & Visual Effects
- **Sound Effects**: Distinct audio for different asteroid destructions and weapon firing
- **Particle Effects**: Explosion effects for destroyed asteroids and player
- **Screen Shake**: Dynamic camera shake for impact feedback
- **Visual Feedback**: Player invulnerability blinking effect

### Game States
- **Menu System**: Start game, view scores
- **Pause Functionality**: Pause and resume gameplay
- **Game Over Screen**: Final score display and restart options

## How to Play

### Controls

#### Movement
- **W** or **Up Arrow**: Thrust forward
- **A/D** or **Left/Right Arrows**: Rotate spaceship left/right

#### Combat
- **Space** or **Left Mouse Button**: Fire weapon
- **Q**: Switch between Standard and Double Shot weapons

#### Game Management
- **Escape**: Pause/Resume game during gameplay

### Objective
1. **Survive**: Avoid collisions with asteroids
2. **Destroy**: Shoot asteroids to earn points and clear the field
3. **Score**: Achieve the highest score possible

### Scoring System
- **Large Asteroids**: Higher points, split into medium asteroids when destroyed
- **Medium Asteroids**: Moderate points, split into small asteroids when destroyed  
- **Small Asteroids**: Lower points, completely destroyed when hit
- **High Score**: Your best score is automatically saved and displayed

### Gameplay Tips
- Large asteroids require multiple hits and split into smaller ones
- Use the Double Shot weapon for crowd control
- Take advantage of invulnerability periods after taking damage
- The game becomes progressively more difficult with faster spawn rates

## Game Mechanics

### Asteroid Behavior
- **Three Size Categories**: Large, Medium, Small
- **Splitting Mechanics**: Large asteroids split into 2 medium asteroids, medium asteroids split into smaller pieces
- **Variable Health**: Different asteroid sizes have different health values
- **Dynamic Movement**: Asteroids move with physics-based momentum and rotation

### Player Mechanics
- **Physics-Based Movement**: Realistic thrust and momentum system
- **Health System**: Multiple hit points with visual feedback
- **Invulnerability Frames**: Temporary protection after taking damage
- **Screen Wrapping**: Player and asteroids wrap around screen edges

### Weapon Systems
- **Cooldown-Based Firing**: Prevents spam shooting with balanced fire rates
- **Two Weapon Types**:
  - Standard: Single, powerful shots
  - Double Shot: Twin projectiles with spread angle
- **Configurable Parameters**: Speed, damage, and lifetime settings

## Technical Architecture

### Core Systems
- **Event-Driven Architecture**: Centralized event system for game state management
- **Factory Pattern**: Asteroid and projectile creation through factory classes
- **Object Pooling**: Efficient memory management for frequently created/destroyed objects
- **Singleton Managers**: Game state, configuration, and score management

### Key Components
- **GameManager**: Handles overall game state transitions
- **ScoreManager**: Tracks and persists player scores  
- **ConfigLoader**: Loads game settings from JSON configuration files
- **AsteroidSpawner**: Manages asteroid creation and difficulty scaling
- **Player System**: Modular player components (Movement, Shooting, Health, Setup)

### Design Patterns Used
- **Component System**: Modular player functionality
- **Factory Pattern**: Object creation management
- **Observer Pattern**: Event-driven communication
- **Singleton Pattern**: Global manager access

## Project Structure

```
Assets/
├── AsteroidsModern/
│   ├── Animations/          # Animation assets
│   ├── Fonts/              # Text rendering fonts
│   ├── Prefabs/            # Game object templates
│   ├── Scenes/
│   │   └── Gameplay.unity  # Main game scene
│   ├── Scripts/
│   │   ├── Asteroids/      # Asteroid-related classes
│   │   │   ├── AsteroidBase.cs
│   │   │   ├── AsteroidSpawner.cs
│   │   │   ├── LargeAsteroid.cs
│   │   │   ├── MediumAsteroid.cs
│   │   │   └── SmallAsteroid.cs
│   │   ├── Audio/          # Audio management
│   │   ├── Core/           # Core game systems
│   │   │   ├── GameBootstrap.cs
│   │   │   ├── GameEvents.cs
│   │   │   ├── GameSettings.cs
│   │   │   └── ObjectPool.cs
│   │   ├── Effects/        # Visual effects
│   │   ├── Enums/          # Game enumerations
│   │   ├── Interfaces/     # Abstract interfaces
│   │   ├── Managers/       # Game managers
│   │   │   ├── GameManager.cs
│   │   │   ├── ScoreManager.cs
│   │   │   └── ConfigLoader.cs
│   │   ├── Player/         # Player components
│   │   │   ├── PlayerMovement.cs
│   │   │   ├── PlayerShooting.cs
│   │   │   ├── PlayerHealth.cs
│   │   │   └── PlayerSetup.cs
│   │   ├── UI/             # User interface
│   │   ├── Utils/          # Utility functions
│   │   └── Weapons/        # Weapon systems
│   │       ├── WeaponBase.cs
│   │       ├── StandardWeapon.cs
│   │       ├── DoubleShotWeapon.cs
│   │       └── Projectile.cs
│   ├── Sounds/             # Audio files
│   │   ├── bangLarge.wav   # Large asteroid destruction
│   │   ├── bangMedium.wav  # Medium asteroid destruction
│   │   ├── bangSmall.wav   # Small asteroid destruction
│   │   └── fire.wav        # Weapon firing sound
│   └── Sprites/            # Visual assets
│       ├── Asteroids/      # Asteroid sprites
│       ├── Background/     # Background elements
│       ├── Explosion/      # Explosion animations
│       ├── Ship/           # Player ship sprites
│       ├── VFX/            # Visual effects
│       └── Weapons/        # Weapon/projectile sprites
├── Settings/               # Unity project settings
├── StreamingAssets/        # Runtime-loadable assets
└── TextMesh Pro/           # Text rendering system
```

## Configuration

The game uses a JSON configuration system located in `StreamingAssets/game_config.json`. This allows for easy tweaking of game parameters without recompilation:

### Configurable Parameters
- **Player Settings**: Movement speed, rotation speed, health, invulnerability duration
- **Weapon Settings**: Fire rates, projectile speeds, damage values, lifetimes
- **Asteroid Settings**: Health, speed, and score values for each size category  
- **Spawn Settings**: Initial spawn rate, spawn rate increase, maximum asteroids on screen

## Development

### Requirements
- **Unity 2022.3 LTS** or newer
- **TextMesh Pro** package (included)
- **2D Physics** and **2D Renderer** packages

### Getting Started
1. Clone the repository
2. Open the project in Unity
3. Load the `Gameplay` scene from `Assets/AsteroidsModern/Scenes/`
4. Press Play to start the game

### Key Scripts to Understand
- [`GameManager.cs`](Assets/AsteroidsModern/Scripts/Managers/GameManager.cs): Central game state management
- [`PlayerMovement.cs`](Assets/AsteroidsModern/Scripts/Player/PlayerMovement.cs): Player physics and input handling
- [`AsteroidSpawner.cs`](Assets/AsteroidsModern/Scripts/Asteroids/AsteroidSpawner.cs): Dynamic asteroid generation
- [`GameSettings.cs`](Assets/AsteroidsModern/Scripts/Core/GameSettings.cs): Configuration data structure

### Customization
The game is designed to be easily customizable through:
- JSON configuration files for gameplay parameters
- Modular component architecture for extending functionality
- Event-driven system for adding new features
- Factory patterns for creating new asteroid or weapon types

---

**Enjoy playing Asterobitus!** 🚀🎮
