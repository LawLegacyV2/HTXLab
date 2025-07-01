This WPF application simulates a swarm of flying insects using a modified Boid algorithm.

It includes:

- Realistic insect motion with attraction to a moving light source
- Mouse-controlled light attractor
- Burst behavior triggered by mouse clicks
- Wings simulated using triangle trails
- Export to video/gif supported (see comments in code for extension)
- Clean architecture with adjustable parameters

---

## 💻 How to Run

1. Clone or download the solution.
2. Set `Question4_InsectSwarm` as the startup project.
3. Build and run in **Release mode** for smoother visuals.
4. Move your mouse to attract the swarm.
5. **Click the left mouse button** to trigger a burst behavior (flies scatter and reform).
6. Press **Escape** to close the application.

---

## 🧪 How It Works

The boid algorithm is a simplified biological model of swarm behavior. Each "insect" follows:

- A **cohesion rule** (move toward others)
- A **separation rule** (avoid crowding)
- A **target attraction rule** (chase the light)

The result is a fluid, emergent animation that mimics natural swarm dynamics.

---

## 📷 Preview
 

🎬 [YouTube that best exemplifies swarm behavior](https://youtu.be/-wK-Zx2P9TQ)


## ✅ Requirements

- Windows 10/11
- .NET 8 SDK
- Visual Studio 2022+

---