### Changed
- Use Unity `Debug.Log` instead of ZUtils logging (which has been removed).
- Some AssenceReference inheritors now throw instead of warn when misused by the developer.
### Fixed
- Went back to block-scoped namespaces because unity does not like file-scoped apparently.


## [1.0.0] - 2026-08-08
### First Release
- Data-Binding-Friendly Variables
- Flexible Data References
- Type-Safe Addressables Wrappers
- Predicate Class and interface and other commonly used types