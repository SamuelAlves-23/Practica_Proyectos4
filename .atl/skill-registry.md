---
name: skill-registry
description: Project skill registry for Practica_Proyectos4. Auto-resolved by orchestrator.
---

## Project Standards

*None detected — project uses standard Unity conventions without custom linting or formatting rules.*

## User Skills

| Trigger | Skill | When |
|---------|-------|------|
| Godot game, GDScript, Godot signals, scenes, state machines | `godot-gdscript-patterns` | File ends with `.gd`, or path contains `godot`/`Godot`, or task mentions GDScript, signals, scenes |
| Unity project | *(default — Unity conventions, standard MonoBehaviour patterns)* | File ends with `.cs`, or path contains `Assets/Scripts` |

## Skill Resolution Notes

- This is a **Unity 6 (6000.3.8f1) C# project**, NOT a Godot project. Do NOT load `godot-gdscript-patterns` for C# files.
- Unity conventions apply: `MonoBehaviour`, `Awake`, `OnEnable`, `OnDisable`, `FixedUpdate`, `Start`, `Update`.
- Test framework: `com.unity.test-framework` (NUnit-based). Test files follow `[Test]` / `[UnityTest]` attributes.
- Input: `com.unity.inputsystem` with `.inputactions` JSON assets.
- Rendering: URP 17.3.0 (`com.unity.render-pipelines.universal`).