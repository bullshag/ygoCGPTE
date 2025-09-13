# Game Design Document (Living) — ygoCGPTE
_Last updated: 2025-09-13 16:56 UTC • Document owner: Codex_

## 0. Executive Snapshot
- **Current Phase:** Porting Core Systems to Unity
- **Build Status:** Yellow — Windows-specific tests fail in current Linux environment.
- **This Week’s Objectives (3–5 bullets):**
  - Catalogue WinForms features for porting.
  - Establish Unity project structure and packages.
  - Define core gameplay loop skeleton.
  - Validate MySQL connectivity from Unity client.
- **Top Risks & Blocks (max 5):**
  - Undefined Unity version — Owner: TBD, due 2025-09-20.
  - Missing Windows Desktop SDK in CI — Owner: TBD, due 2025-09-18.
  - Incomplete mapping of legacy features — Owner: TBD, due 2025-09-25.
- **Next Milestone:** Unity Prototype Build, 2025-10-01, exit criteria: player can start battle and load inventory from database.

## 1. Vision & Pillars
- **High-level pitch** (2–3 sentences).
  A turn-based RPG porting a legacy WinForms experience to Unity for modern presentation and cross-platform scalability.
- **Design pillars** (exactly 3–5, each with a “tests” sub-bullet describing how we know we upheld it).
  - Faithful Systems Parity
    - *Tests:* Unity interactions mirror WinForms mechanics in manual tests.
  - Fast Iteration via Modular SQL
    - *Tests:* All new database logic lives in standalone `.sql` files executed through `DatabaseClientUnity`.
  - Accessible Cross-platform UI
    - *Tests:* Menus navigable via keyboard and mouse; font sizes pass legibility checks.

## 2. Audience & Platforms
- **Target platforms:** Windows (Unity)
- **Player profile:** Fans of tactical party-based RPGs seeking quick session play.
- **Performance targets:** 60 FPS, <5 s scene load, <500 MB RAM.

## 3. Core Gameplay Loop
- **Loop summary (bullets)**
  - Explore world map.
  - Engage in battles.
  - Collect loot and experience.
  - Upgrade party and repeat.
- **Loop diagram (ASCII if needed)**
  Explore → Battle → Loot → Upgrade ↺
- **Progress:** In progress, 5% (commit d2c3bb8).

## 4. Systems & Mechanics
### Combat
- **Purpose**
  Resolve party encounters through turn-based actions.
- **Rules & Data**
  Enemy levels scale to player party total (see MANUAL_TEST_PLAN).
- **UX notes**
  Battle log lists enemy levels and actions.
- **Technical notes**
  Uses SQL script `unity_character_heal.sql` for healing.
- **Progress:** In progress, 10% (manual tests).
- **Open decisions:**
  - TBD: Define animation system. Owner: TBD, due 2025-09-27.

### Progression
- **Purpose**
  Track character experience, levels, and skills.
- **Rules & Data**
  Level scaling formulas from WinForms.
- **UX notes**
  Level-up window requires redesign.
- **Technical notes**
  Data persisted via MySQL.
- **Progress:** Not started, 0%.
- **Open decisions:**
  - TBD: Choose skill tree layout. Owner: TBD, due 2025-09-30.

### Economy
- **Purpose**
  Manage gold and item transactions.
- **Rules & Data**
  Shop prices from `ShopForm` logic.
- **UX notes**
  Shop UI to use grid layout.
- **Technical notes**
  Separate SQL scripts for add/remove item.
- **Progress:** Not started, 0%.
- **Open decisions:**
  - TBD: Currency inflation strategy. Owner: TBD, due 2025-10-05.

### UI/UX
- **Purpose**
  Provide screens for login, inventory, battle, and settings.
- **Rules & Data**
  Interface must support keyboard and mouse.
- **UX notes**
  Requires pixel font `Thaleah_PixelFont`.
- **Technical notes**
  Built with Unity UGUI.
- **Progress:** In progress, 5% (assets imported).
- **Open decisions:**
  - TBD: Finalize navigation flow. Owner: TBD, due 2025-09-25.

### Save/Load
- **Purpose**
  Persist player progress.
- **Rules & Data**
  Save format TBD.
- **UX notes**
  Save confirmation dialog.
- **Technical notes**
  Planned use of ScriptableObjects and JSON.
- **Progress:** Not started, 0%.
- **Open decisions:**
  - TBD: Determine save encryption. Owner: TBD, due 2025-10-10.

### Networking
- **Purpose**
  Support chat and potential multiplayer.
- **Rules & Data**
  Chat via MySQL tables.
- **UX notes**
  Chat window scrollback; shows last 25 messages; scroll resizes only on login or when sending a message.
- **Technical notes**
  `ChatService` to be rewritten for Unity.
- **Owner:** TBD
- **Dependencies:** ChatService backend, ScrollRect hookup
  - **Progress:** In progress, 10% ([PR #288](https://github.com/bullshag/ygoCGPTE/pull/288))
- **Acceptance criteria:** Send/receive chat messages; auto-scroll to latest message on login
- **Risks:** Message flow still unverified may block broader networking features
- **Open decisions:**
  - TBD: Evaluate real-time networking framework. Owner: TBD, due 2025-10-15.

## 5. Content
- **Units/Characters/Items/Levels**

| ID | Name | Role | Status | Owner | Notes |
|----|------|------|--------|-------|-------|
| TBD | TBD | TBD | Not started | TBD | Define initial roster |

- **Pipelines:** Content authored in SQL and imported via Unity scripts; validation via manual tests.
- **Placeholders vs. final assets:**
  - Placeholder pixel art → final spritesheet ETA 2025-10-05.

## 6. Tech Architecture
- **Unity version & packages**
  - Version: TBD
  - Key packages: 2D Sprite, Tilemap, Ads, Analytics, UGUI.
- **Project structure** (folders, namespaces)
  - `Assets/Scripts`, `Assets/Scenes`, `Assets/Prefabs`, `Assets/sql`.
- **Key patterns** (e.g., ECS, ScriptableObjects, DI, event bus)
  - ScriptableObjects for config; direct component references.
- **Data layer:** persistence strategy, schemas
  - MySQL via `MySql.Data`; SQL stored in root `.sql` files.
- **Third-party services** (analytics, crash, backend)
  - Unity Analytics, Unity Ads.
- **Progress & links to diagrams**
  - Initial manifest committed (d2c3bb8).

## 7. Port Plan: Windows Forms → Unity
- **Legacy inventory:** features in WinForms (list with brief behavior)
  - `ChatService` – retrieve and send chat messages.
  - `InventoryForm` – manage items and equipment.
  - `BattleForm` – turn-based combat encounters.
  - `ShopForm` – purchase items with gold.

- **Mapping table:**

| Legacy Feature | Unity Equivalent | Status | Owner | Notes |
|---|---|---|---|---|
| ChatService | Chat UI & service script | In progress | TBD | Auto-scroll on login/send; shows 25 latest messages; send/receive pending |
| InventoryForm | InventoryUI | Not started | TBD | Requires `unity_inventory_load.sql` |
| BattleForm | BattleScene | In progress | TBD | Basic scene scaffold |
| ShopForm | ShopUI | Not started | TBD | Pricing logic TBD |

- **Migration order:**
  1. Set up database access layer (acceptance: Unity reads `create_user.sql`).
  2. Port InventoryForm to InventoryUI (acceptance: load/save items).
  3. Implement BattleScene (acceptance: win/lose conditions match WinForms).
  4. Integrate ChatService (acceptance: send/receive messages) — In progress ([PR #288](https://github.com/bullshag/ygoCGPTE/pull/288)); message flow verification pending.

- **Deprecations:**
  - FriendService features will not be ported due to low usage.

## 8. UI/UX
- **Screens & flows**
  - Login → Main Menu → World Map → Battle → Loot → Menu
- **Wireframes**
  - TBD (Owner: TBD)
- **Input model:** keyboard/mouse.
- **Accessibility:** requirements and status
  - TBD: colorblind mode, text scaling. Owner: TBD, due 2025-10-12.

## 9. Data & Persistence
- **Save format** (JSON/ScriptableObject/etc.)
  - TBD
- **Cloud/local strategy**
  - MySQL cloud database.
- **Backwards compatibility policy**
  - TBD
- **Test cases for migration**
  - TBD

## 10. Tools & Pipelines
- **Build pipeline:** CI TBD; must compile Unity client and run `dotnet test` for backend.
- **Asset pipeline:** Assets imported with default settings; compression TBD.
- **Testing pipeline:** Unit tests via `dotnet test`; manual battle scaling tests.

## 11. Schedule & Milestones
- **Roadmap table:**

| Milestone | Date | Scope | Exit Criteria | Risk |
|---|---|---|---|---|
| Unity Prototype Build | 2025-10-01 | Core loop, inventory, battle | Player completes one battle in Unity | Medium |

- **Critical path:**  
  1. Determine Unity version.  
  2. Port InventoryForm.  
  3. Implement BattleScene.  
  4. Wire MySQL connectivity.  
  5. Integrate chat.

## 12. Task Backlog (Engineer-Ready)

| ID | Title | Type | Owner | Estimate | Dependencies | Acceptance Criteria | Status | Links | Priority |
|---|---|---|---|---|---|---|---|---|---|
| SYS-ARCH-001 | Record Unity version | chore | TBD | 1d | None | `ProjectVersion.txt` committed | To Do | - | Must |
| FEAT-INV-001 | Port Inventory UI | feature | TBD | 5d | SYS-ARCH-001 | Items/equipment load/save via SQL | To Do | - | Must |
| FEAT-CBT-001 | Create BattleScene | feature | TBD | 7d | SYS-ARCH-001 | Player can start and resolve battle | To Do | - | Should |

## 13. Non-Goals & Constraints
- No mobile or console ports in current scope.
- Budget limits prevent dedicated backend server development.
- Compliance with MySQL-only data storage.

## 14. Quality Bar
- **Definition of Done**
  - Design: approved wireframes and specs.
  - Code: PR reviewed, unit tests passing, SQL in separate files.
  - QA: manual test cases executed.
  - Docs: GDD updated with changes.
- **Performance budgets**
  - CPU/GPU usage under 60% on mid-tier PC.
- **Stability goals**
  - 99% crash-free sessions.
- **Telemetry needed**
  - Analytics events for battles, inventory actions, crashes.

## 15. Risks & Mitigations
- Missing Unity version info (Likely/Medium) — Owner: TBD — Mitigation: inspect project settings; Trigger: build fails.
- SQL scripts diverge from WinForms logic (Possible/High) — Owner: TBD — Mitigation: cross-review with legacy code; Trigger: test mismatch.
- CI lacks Windows components (Likely/High) — Owner: TBD — Mitigation: install WindowsDesktop SDK or adjust tests; Trigger: build pipeline failure.
- Chat message flow unverified (Possible/Medium) — Owner: TBD — Mitigation: end-to-end test with database; Trigger: messages fail to appear.

## 16. Changelog (Auto-Appended)
- 2025-09-13: Created initial GDD skeleton covering all sections. — Codex
- 2025-09-13: Updated Networking progress and ChatService migration status after chat auto-scroll work (PR #288). — Codex
- 2025-09-13: Verified chatScrollRect wiring and documented auto-scroll on login. — Codex
- 2025-09-13: Enabled dynamic chat content resizing when new messages arrive. — Codex
- 2025-09-13: Restricted chat view to 25 messages and resized scroll only on login/send. — Codex
