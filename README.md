# Unity Game Project  
# LeapZone (Working Title: BackWay Escape)  
# Unity 6000.4.8f1  

---

## 🎮 Description  
BackWay Escape is a stylized 3D maze-based ball control game where the player must navigate through a timed environment, avoid obstacles, collect gems, use checkpoints, and reach the goal before the timer ends.  

The game focuses on **skill-based movement, time pressure, and environmental interaction** with a mix of puzzle navigation and physics-based gameplay.

---

## ✨ Key Features  

### 🎯 Core Gameplay
- Time-based objective system (2:30 min challenge)
- Ball control with player interaction mechanics
- Gem system (-3 seconds reward mechanic)
- Mud trap obstacle with escape interaction system
- Strategic checkpoint system (progress saving)

### 🧠 AI & Environment
- Enemy AI that chases and kicks the ball
- Patrol system with limited aggression cycles
- NavMesh-based navigation inside maze boundaries
- Stylized forest environment with terrain design, trees, fog, and lighting

### 🎮 Controls & Input System
- Unity New Input System implementation
- Supports Keyboard, Gamepad, Touch, and Accelerometer
- Dual mobile control modes:
  - Virtual Joystick
  - Tilt / Gyroscope controls
- Platform-based UI switching (Android / Windows)

### 🖥️ UI / UX System
- Live gameplay timer (00:00:00 format)
- Tutorial overlay before gameplay starts
- Pause system with checkpoint restart options
- Win / Lose screens with animated character preview (Render Texture)
- Character customization system:
  - Hat selection
  - Face expression selection
  - Color customization slider
- Haptic feedback system (UI + gameplay events)

### 🏆 Progression & Scoring
- Time-based scoring system
- Local leaderboard system (Top 5 players)
- JSON-based data saving system
- Name entry system (max 5 characters)
- Anti-cheat linked directly to timer system

### 🎨 Visual System
- Stylized forest environment design
- Terrain sculpting with painted textures
- Fog, bloom, ACES tone mapping
- Directional lighting setup
- Moving clouds with dynamic shadows
- Render Texture character preview system

### 🔊 Audio System
- Persistent background music across scenes
- SFX system:
  - Ball kick & dribble
  - Gem collection
  - Checkpoints
  - Goal completion
  - UI feedback sounds

### 📱 Platform Support
- Android (Primary)
- Windows PC
- Automatic UI switching based on platform
- Mobile-specific input + UI handling

---

## 🧩 Technical Systems
- Cinemachine dual camera system (Top-down + Follow camera)
- NavMesh AI navigation
- PlayerPrefs + JSON save system
- Scene persistence (audio + data)
- Screen awake system (prevents sleep during gameplay)
- Notification system (Android API 33+ support)
- Coroutine-based gameplay triggers

---

## 🎮 Controls  

### Keyboard / Gamepad (Windows)
- Move: WASD / Left Stick  
- Jump: Space / South Button  
- Kick: E / East Button  
- Camera Switch: C / RB

### Mobile (Android) / Gamepad(Android) same as Gamepad (Windows)
- Virtual Joystick / Tilt Movement  
- On-screen buttons for Jump, Kick, Camera Switch  
- Pause Button with full menu access  

---

## 👨‍💻 Author  
Dhruv Panchal  