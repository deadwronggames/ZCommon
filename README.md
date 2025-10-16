<p align="center">
  <img src="https://raw.githubusercontent.com/deadwronggames/ZSharedAssets/main/Banner_Zombie.jpg" alt="ZCommon Banner" style="width: 100%; max-width: 1200px; height: auto;">
</p>

# ZCommon
ZCommon is a foundational Unity package designed to provide reusable and flexible programming patterns across any project. It demonstrates modular architecture, type safety, and data-driven workflows.


## Overview and Design Highlights
- **Data-Binding-Friendly Variables**  
  ScriptableObject variables (`BaseVariableSO`, `FloatVariableSO`) emit events on value changes, support data binding, enabling reactive and decoupled systems.
- **Flexible Data References**  
  Allows seamless switching between constants and ScriptableObject variables (`FloatReference`, `BaseReference`) for flexible runtime configuration.
- **Type-Safe Addressables Wrappers**  
  `BaseAssetReferenceSO<TAsset, TAssetReference>` and inheritors for `AudioClip`, `Sprite`, `GameObject`, `SceneInstance` and more. Provides async loading, safe switching of assets, and editor tools for save handling of Addressable keys.
- **Modular & Extensible**  
  New types, variables, and asset references can be added easily by inheriting the base classes. The architecture encourages code reuse and consistency across projects.


## Installation
- Install via Unity Package Manager using the Git URL: https://github.com/deadwronggames/ZCommon
- Use the namespace: 
```csharp 
using DeadWrongGames.ZCommon;
```


## Usage Examples

### ScriptableObject Variables (FloatVariableSO, etc.)
1. Create a new ScriptableObject instance (`Create → Scriptable Objects → Variables → FloatVariable`).
2. Optionally assign a start value in the Inspector.
3. Assign the variable reference anywhere in your scene or to components that support it.
4. Any changes to the variable (via code or inspector) will trigger subscribed events.

```csharp
// Access the value in code
float speed = myFloatVariableSO.Value;
```

### Variable References
1. Add e.g. the FloatReference MonoBehaviour to a GameObject in your scene.
2. Assign either a constant value or a FloatVariableSO in the Inspector.
3. The reference can now be used anywhere as a float, implicitly or explicitly.

Example: connecting a `FloatReference` to a ZModularUI button to broadcast the value via ZServices EventChannel when clicked.

### Unity Addressables Wrapper
1. Create a new ScriptableObject instance<br>`(Create → Scriptable Objects → Addressables → ReferenceAudioClip / GameObject / Scene / Sprite, etc.)`.
2. Assign an Addressable asset in the Inspector using the built-in editor buttons.
3. The asset can now be safely used anywhere via the ScriptableObject instance in code.

To swap assets: delete the old asset, assign a new one with the same Addressables key. All references will automatically update. No need to manually find every usage.


## Notes
- All ScriptableObject variables and Addressable references support editor-time validation.
- Easily extendable: new types can be added for variables or Addressables by inheriting base classes.
- Developed for usage with **Odin Inspector**.
- **Work in progress**, some functionality may change.

