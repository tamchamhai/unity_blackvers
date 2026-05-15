---
trigger: always_on
---

### Development Rules

Component Loading: When required to load a component, always create a dedicated loading function and invoke it within the LoadComponents override method.

Class Documentation: Always generate comprehensive documentation/comments for every Class.

Singleton Pattern: When implementing a Singleton, create a specific initialization function for the singleton instance and call it inside LoadComponents.

Internal Referencing: Always use the this. prefix when accessing internal variables or members.

Coding Style: Prefer Guard Clauses over nested if-else structures whenever possible.

Looping Restrictions: Strictly prohibit the use of while(true) {} loops.

Method Logic Separation: Do not write business logic directly inside Unity lifecycle methods (e.g., Start(), Update(), FixedUpdate()) or MasterBehaviour methods. Instead, encapsulate logic in separate, descriptive functions.

Naming Conventions: Do not use abbreviations for variable names (e.g., use SpriteRenderer spriteRenderer instead of SpriteRenderer sr).

Editor Setup: For setup code intended only for the Editor, use partial classes and place them in a separate file with the suffix Setup (e.g., for class PlanetSpawner, the setup file should be PlanetSpawnerSetup).

Language Standards:

    Always write code comments in English.

    Always create the implementation plan in Vietnamese.

Code Commit Protocols
Commit Trigger: Only commit code when explicitly requested. I will specify the required commit type.

Commit Message Format: [type][id] commit_message

      type: Must be one of feat, fix, or update.

      id: A unique 7-character alphanumeric hash (lowercase letters and numbers).

      commit_message: A concise summary of the changes in English.
