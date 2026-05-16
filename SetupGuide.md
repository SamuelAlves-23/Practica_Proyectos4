# Shadows of The Grove (SOTG) - Setup Guide

## Estado Actual
- Unity 6.0.3
- Sistema de input configurado (WASD/Gamepad)
- Movimiento del player con NavMeshAgent

---

## Configuración de Escena

### 1. Configuración Básica
- [ ] Crear **NavMesh** en la escena (Window → AI → Navigation)
- [ ] Crear **GameConfig** en `Assets/Resources/`: Create → SOTG → GameConfig

### 2. Player Setup
- [ ] GameObject con:
  - [ ] **NavMeshAgent** (componente)
  - [ ] **PlayerInputHandler** (script)
  - [ ] **PlayerMovement** (script)
  - [ ] Asignar referencia de PlayerInputHandler al campo _inputHandler en PlayerMovement

### 3. Cámaras
- [ ] Main Camera con ángulo top-down o third-person

---

## Sistemas Implementados

### ✅ Config (GameConfig.cs)
- Huevos: 10
- Intrusos: 5
- Ubicación: `Assets/Scripts/Mechanics/Config/GameConfig.cs`

### ✅ Input (PlayerInputHandler.cs + Player_InputAction.cs)
- Move: WASD / Left Stick
- Jump: Space / Button South
- Attack: E / (Gamepad configurable)
- Ubicación: `Assets/Scripts/Mechanics/Player/`

### ✅ Movimiento (PlayerMovement.cs)
- NavMeshAgent con velocidad por defecto
- Input convertido a dirección de cámara
- Ubicación: `Assets/Scripts/Mechanics/Player/`

### ✅ Huevos (EggEntity.cs + EggSpawner.cs)
- Spawn automático de 10 huevos en NavMesh
- Detección de raptado via evento OnKidnapped
- Ubicación: `Assets/Scripts/Mechanics/Egg/`

### ✅ Intrusos (Intruder.cs + IntruderSpawner.cs)
- AI con NavMesh que busca huevo más cercano
- Raptan huevo y desaparecen (0.5s delay)
- Ubicación: `Assets/Scripts/Mechanics/Intruder/`

### ✅ Ataque (PlayerAttack.cs)
- Melee attack con radio de 2m
- Cooldown de 0.5s
- Elimina intrusos en rango al presionar Attack (E)
- Ubicación: `Assets/Scripts/Mechanics/Player/`

---

## Pendiente

- [ ] GameManager
- [ ] Animaciones

---

## Setup para Prueba

### EggSpawner en escena
1. Crear GameObject vacío "EggSpawner"
2. Añadir script EggSpawner
3. Asignar Prefab de huevo al campo _eggPrefab
4. Ajustar _spawnRadius (default: 20)
5. Ejecutar - Aparecen 10 huevos en posiciones NavMesh aleatorias

### Prefab Egg
1. Crear esfera o modelo 3D
2. Añadir script EggEntity
3. Añadir Collider (Trigger = false)
4. Optional: Añadir Rigidbody (isKinematic = true)

### IntruderSpawner en escena
1. Crear GameObject vacío "IntruderSpawner"
2. Añadir script IntruderSpawner
3. Asignar Prefab de intruso al campo _intruderPrefab
4. Ajustar _spawnRadius (default: 20)
5. Ejecutar - Aparecen 5 intrusos en posiciones NavMesh aleatorias

### Prefab Intruder
1. Crear modelo 3D (cubo/esfera de otro color)
2. Añadir script Intruder
3. Añadir componente NavMeshAgent
4. Añadir Collider (Trigger = false)

### PlayerAttack Setup
1. Añadir script PlayerAttack al GameObject del player
2. Asignar PlayerInputHandler al campo _inputHandler
3. Ajustar _attackRange (default: 2m)
4. Ajustar _attackCooldown (default: 0.5s)
5. Presionar E (o botón de Attack) para eliminar intrusos cercanos