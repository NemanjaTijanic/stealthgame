# Stealth Game
**Introduction**

This is a 3D stealth action-adventure game made in Unity. The purpose of this project was for me to get as familiar as possible with various elements and techniques that are used when designing and programming the AI and UI parts of a video game.

The game is fully playable and contains all the elements that you would expect from a regular video game, but since the main focus is on the AI and UI, other parts aren't as polished.

## Specifics
**Player**

Character Controller was used to implement player movement, while the camera movement was done using Cinemachine. The player can move (W) forward, (A) left and (D) right while using the mouse to rotate. He can (C) toggle walk/run and (Q) throw rocks to distract the enemy.

**AI**

Enemies move around using the NavMesh system. Their decision making is based on a custom State Machine model. They have five states which are guard, patrol, pursue, distracted and flee. They can hear and detect the player in a certain radius around them, provided he is walking and not running, while they use a vision cone to see the player.

Detecting the player while in the patrol or guard states will make them switch to pursue, chasing him and attacking if in melee range. Senses of hearing and sight are used to keep track of the player. If he manages to evade his pursuers, they will check his last known position, and if no detection occurs during that time, they will return to their original states.

Throwing rocks can be used to distract the enemies, prompting them to investigate the area where they heard the rock hit the ground. The AI will switch to the flee state if their health is reduced beneath a certain threshold. If the player is close enough to the enemies, they will run away from him, turning around after a certain time to check if he still pursues them.

**UI**

The UI features a splash screen, during which pressing any button will lead to the main menu. Play, options and credits sections can be accessed via the main menu. Options panel features a multitude of settings related to audio, display and gameplay. While the play menu has features such as mission select, starting a new game, (not implemented) loading a previous save, and continuing to the last played mission.

After the loading screen, the player will find himself in-game. As a part of the HUD there he can find a mini map, health bar, weapon display, number of rocks in possession and a no sound icon if he is walking. During story segments a dialogue box will be showed, displaying who is talking and his message. There is also a mini tutorial screen that shows and gives tips for playing the game. 

A (ESC) pause menu is also present, featuring continue, options and return to menu buttons. Every button is animated and has sound cues.

## Installation

Unity version 2021.3.17f1 or higher is required (64-bit).

- Create a new project in Unity.
- Download Assets and ProjectSettings and place them in the folder of your new project.
- Open the desired scene in the Scenes directory.

## Screenshots

![Main Menu](https://i.imgur.com/63SAyV5.png)
![Options Menu](https://i.imgur.com/vxg9s5i.png)
![Play Menu](https://i.imgur.com/xBAHYmG.png)
![Loading Screen](https://i.imgur.com/5Qfwd1G.png)
![Gameplay](https://i.imgur.com/7Zlf321.png)
![Tutorial](https://i.imgur.com/LwJdyWq.png)
