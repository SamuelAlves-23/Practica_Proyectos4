# Protect the Forest - Setup Guide

## Estado Actual
- Unity 6.0.3
- Sistema de input configurado (WASD/Gamepad)
- Movimiento del player con NavMeshAgent

---

## Configuración de Escena

### 1. Configuración Básica
- [ ] Crear **NavMesh** en la escena (Window → AI → Navigation)
- [ ] Crear **GameConfig** en `Assets/Resources/`: Create → ProtectTheForest → GameConfig

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

### ✅ Huevos (Egg.cs + EggSpawner.cs)
- Spawn automático de 10 huevos en NavMesh
- Detección de raptado via evento OnKidnapped
- Ubicación: `Assets/Scripts/Mechanics/Egg/`

---

## Pendiente

- [ ] Sistema de Intrusos (Intruder)
- [ ] Sistema de Ataque (Attack) - Melee para eliminar intrusos
- [ ] GameManager
- [ ] Animaciones

---

## Setup para Prueba de Huevos

### EggSpawner en escena
1. Crear GameObject vacío "EggSpawner"
2. Añadir script EggSpawner
3. Asignar Prefab de huevo al campo _eggPrefab
4. Ajustar _spawnRadius (default: 20)
5. Ejecutar - Aparecen 10 huevos en posiciones NavMesh aleatorias

### Prefab Egg
1. Crear esfera o modelo 3D
2. Añadir script Egg
3. Añadir Collider (Trigger = false)
4. Optional: Añadir Rigidbody (isKinematic = true)