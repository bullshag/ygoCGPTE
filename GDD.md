# Game Design Document (Living) — ygoCGPTE
_Last updated: 2025-10-16 22:15 UTC • Document owner: Codex
## 0. Executive Snapshot
- **Current Phase:** Porting Core Systems to Unity
- **Build Status:** Yellow — Windows-specific tests fail in current Linux environment.
- **This Week’s Objectives (3–5 bullets):**
  - Catalogue WinForms features for porting.
  - Establish Unity project structure and packages.
  - Define core gameplay loop skeleton.
  - Validate MySQL connectivity from Unity client.
  - Stand up data-driven location activity availability service.
  - Implement world map waypoint navigation with directional animations and city interaction triggers.
- **Top Risks & Blocks (max 5):**
  - Undefined Unity version — Owner: TBD, due 2025-09-20.
  - Missing Windows Desktop SDK in CI — Owner: TBD, due 2025-09-18.
  - Incomplete mapping of legacy features — Owner: TBD, due 2025-09-25.
  - Popup window usage inconsistent across scenes — Owner: Codex, due 2025-09-27 (Login and Register now share PopupWindow prefab; remaining scenes pending audit).
    - GUID corruption in `.meta` files may break asset references — Owner: Codex, due 2025-09-14.
  - Scene asset regression risk due to manual YAML edits — Owner: Codex, due 2025-09-30. Mitigation: Use Unity editor or verified templates when adjusting scene tags; Trigger: scene fails to open or loses GameObject data. Status 2025-09-26: Restored RPG scene from commit 5a21cfe after YAML truncation recurred; Unity re-save validation still pending.
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
- **Progress:** In progress, 8% (commit TBD — Owner: Codex, due 2025-09-14).

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
- **Tasks**
  - Integrate FrameAnimator with battle actions — Owner: TBD, Estimate: 2d, Dependencies: FEAT-ANI-001, Acceptance: attack animations play via FrameAnimator, Progress: 0% (due 2025-09-30).
- **Legacy reference**
  - PictureBox animations → FrameAnimator component (see mapping table).
- **Progress:** In progress, 15% (FrameAnimator in place).
- **Open decisions:**
  - None.

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

### World Map Navigation
- **Purpose**
  Move player character across the world map.
- **Rules & Data**
  Shift+Right Click sets destination; Shift+Left Click queues waypoint.
  Entering a city radius enables the city interaction panel.
  Pressing Enter while inside an area tooltip opens the LocationActivitiesPanel with the corresponding activity selected.
- **UX notes**
  Directional and idle animations reflect agent movement state.
  City interaction panel appears when entering city radius.
  Waypoint markers show queued clicks and a path line previews the route.
  Area tooltips appear over interactable locations, showing the area name with Info/Enter actions while the player remains inside the trigger.
  Location activities buttons highlight in yellow for the active selection and red for inactive options to reinforce focus state.
- **Technical notes**
  Utilizes NavMeshAgent with a queued `Vector3` path and emits `QueueEmptied`.
  PlayerNavigator instantiates waypoint marker prefabs and updates a LineRenderer for path preview.
  PlayerNavigator checks `CityNode` triggers to toggle `CityInteraction` UI.
  PlayerNavigator raycasts ignore tooltip trigger colliders so shift-clicking works within area volumes.

  AreaTooltip resides on the locationTooltip root, enforces a trigger SphereCollider (radius 143.2, center y -4.06), updates TMP labels, and orchestrates show/idle/hide animator states with UnityEvents for buttons.
  AreaTooltip now listens for `KeyCode.Return` / keypad enter while the player remains inside and latches input until LocationActivitiesPanel closes.
  LocationActivitiesPanel now mounts directly onto the handcrafted `locationInfoWindow`, reusing existing buttons/backgrounds, auto-generating placeholder content for unimplemented locations, and explicitly unlocks `AreaTooltip.LastActivatedTooltip` across all close paths (Escape, Back button, Cancel input) before/after hiding so the panel can reopen without leaving the trigger.
  LocationActivitiesPanel now reads availability via LocationActivityService, toggling buttons using the `location_activity_settings` table and enforcing #FF4949 idle / yellow active color states.
  LocationActivitiesPanel exposes a debug auto-refresh toggle that re-queries the database every 3 seconds for QA verification while the panel remains active.
  LocationActivityService logs each database availability query to the Unity console, listing activities and their enabled states for diagnostics.
  LocationActivitiesPanel scene wiring now seeds `nodeFortAurus` as the default locationId and binds Tavern, Shop, Temple, Academy, Graveyard, Arena, and Search buttons to their respective UI `Button` components so database availability toggles map correctly.
  World map player uses a CapsuleCollider (radius 0.35, height 1.8) and a kinematic Rigidbody to stay inside tooltip triggers without physics drift.
  Player GameObject now carries the `Player` tag (restored after YAML corruption), and TagManager explicitly includes it so AreaTooltip trigger checks succeed.

#### Location Activities Panel Refresh
- **Owner:** Codex — coordinating with UI/UX.
- **Progress:** In progress, 94% (database-driven availability online; contextual sub-panels pending) — due 2025-09-29.
- **Dependencies:** FEAT-WM-001, FEAT-UI-006, location metadata service (Owner: TBD, due 2025-09-27), `location_activity_settings` table (Owner: Codex, delivered 2025-09-25).
- **Acceptance Criteria:**
  - Activity list populates dynamically from `CityNode` metadata with keyboard/gamepad focus cycling preserved.
  - Selecting an activity opens the matching contextual sub-panel and returns focus to the root list on close.
  - Panel supports minimum 1080p/1440p layouts without overlapping the tooltip or blocking NavMesh clicks.
  - Locations without bespoke content fall back to autogenerated placeholder copy so the window remains informative.
- **Risks:**
  - InputAction conflicts may reoccur when sub-panels capture focus — Owner: Codex, due 2025-09-30.
  - Location metadata contract is still TBD and may slip — Owner: TBD, due 2025-09-27.
  - Animation transitions for new sub-panels are unbudgeted and could cause scope creep — Owner: Codex, due 2025-10-02.
  - Placeholder content must be replaced once metadata contract lands to avoid stale copy — Owner: Codex, due 2025-09-29.
  - SQL availability mismatches could hide required actions until data review — Owner: Codex, due 2025-09-28.

#### Tavern Sub-Panel Integration
- **Owner:** Codex (UI) with systems handoff to TavernManager.
- **Progress:** In progress, 5% (data binding notes captured; prefab stubs not yet created) — due 2025-10-01.
- **Dependencies:** FEAT-UI-004, FEAT-UI-007, TavernManager service API audit (Owner: TBD, due 2025-09-28).
- **Acceptance Criteria:**
  - Tavern sub-panel surfaces Hire, Mercenary Contract, and Work actions with stateful availability messaging.
  - Hire action reuses existing recruit detail overlay and closes sub-panel after successful hire or cancellation.
  - Tavern panel reports actionable telemetry event `tavern_subpanel_open` with location identifier.
- **Risks:**
  - Legacy TavernForm logic hides edge cases (e.g., depleted roster) that remain undocumented — Owner: TBD, due 2025-09-29.
  - UI layout may exceed safe area for smaller resolutions; responsive behavior unverified — Owner: Codex, due 2025-10-03.
  - Data refresh timing between TavernManager and CharacterService may double-trigger updates — Owner: Codex, due 2025-10-04.

- **Dependencies**
  - Unity NavMesh
  - Unity input system
- **Acceptance Criteria**
  - Shift+Left Click enqueues NavMesh hit point.
  - Shift+Right Click clears the queue and resets the agent.
  - Idle animation plays when the agent is stationary.
  - Idle agent consumes queued waypoints sequentially.
  - Area tooltip animator plays show → idle while player remains inside trigger and hide when they exit.
  - Pressing Enter (keyboard or keypad) while within a tooltip opens the location activities panel; Escape/Cancel closes it and allows re-opening without exiting the trigger.
- **Risks**
  - Shift-click input may conflict with UI focus, preventing waypoint capture.
  - Scene has not been re-opened in the target Unity editor post-merge; hidden serialization issues could persist until validated. Owner: Codex, due 2025-09-27 (RPG scene restored via commit 5a21cfe snapshot on 2025-09-26; Unity re-save still pending).
  - Future TagManager edits might drop the `Player` tag, breaking tooltip detection — Owner: Codex, due 2025-09-24.
  - Input axis "Cancel" may be missing on some control schemes, blocking Escape-equivalent close behavior — Owner: Codex, due 2025-09-28.
- **Tasks**
  - Add path preview line — Owner: Codex, Estimate: 1d, Dependencies: FEAT-WM-001, Acceptance: preview line renders for queued waypoints, Progress: 100% (due 2025-09-30).
  - Implement area tooltip interactions — Owner: Codex, Estimate: 1d, Dependencies: FEAT-WM-001, Acceptance: Tooltip displays area name, plays show/idle/hide states, and invokes Info/Enter events, Progress: 100% (due 2025-09-16).
  - Implement location activities panel with keyboard/gamepad highlighting — Owner: Codex, Estimate: 1d, Dependencies: FEAT-WM-001, Acceptance: Enter opens panel, Escape/Cancel closes, active button shows yellow highlight, Progress: 100% (due 2025-09-24).
- **Legacy reference**
  - WorldMapForm → WorldMap scene + PlayerNavigator (see mapping table).
- **Progress:** In progress, 88% (scene reopen pending Unity validation; location activities panel refresh and Tavern sub-panel wiring outstanding — Owner: Codex, due 2025-09-30).
- **Open decisions:**
  - TBD: Authenticate WebSocket connections. Owner: TBD, due 2025-09-30.

#### World-Space UI Interaction
- **Owner:** Codex
- **Progress:** In progress, 92% (2025-10-16: LocationActivitiesPanel close handlers now unlock tooltips before/after hide so Enter can reopen immediately; Play Mode verification still pending due to headless environment limits) — due 2025-10-18.
- **Dependencies:** FEAT-WM-001; locationTooltip Canvas hierarchy; Unity world-space Canvas event system configuration.
- **Acceptance Criteria:**
  - locationTooltip world-space Canvas receives pointer events while facing away from the camera.
  - Enter button click triggers AreaTooltip `EnterClicked` UnityEvent and logs interaction in Play Mode.
  - Visual orientation of tooltip children remains correct after Canvas configuration change.
- **Risks:**
  - Play Mode confirmation blocked by headless CI; requires on-device validation. Owner: Codex, due 2025-10-18.
  - Future prefab overrides may revert GraphicRaycaster settings, reintroducing missed clicks — Owner: Codex, due 2025-10-25.
  - If tooltip orientation flips again, Info button alignment could break; needs regression test pass — Owner: Codex, due 2025-10-22.

### Camera Controls
- **Purpose**
  Provide free-fly camera movement for debugging and exploration.
- **Control scheme**
  W/A/S/D keys pan the camera.
- **Dependencies**
  FEAT-WM-001
- **Owner**
  Codex
- **Progress:** In progress, 10%.
- **Acceptance criteria**
  - WASD translates camera at inspector-defined speed.
  - Smoothing parameter produces damped motion.
- **Risks**
  - Unbounded movement may disorient players or leave scene bounds.


### Environmental Effects
- **Purpose**
  Provide atmospheric depth with drifting cloud shadows across terrain.
- **Rules & Data**
  Clouds spawn at random intervals on the right and drift left at varied speeds.
- **UX notes**
  Moving shadows sweep over the land for parallax.
- **Technical notes**
  CloudSpawner and CloudMover scripts instantiate shadow-casting cloud prefabs.
- **Dependencies**
  - FEAT-WM-001
- **Acceptance Criteria**
  - Clouds spawn randomly at set interval range on the right.
  - Clouds drift right-to-left and destroy themselves off-screen.
  - SpriteRenderer casts shadows onto terrain.
- **Risks**
  - Excessive clouds may reduce performance.
- **Legacy reference**
  - None (new feature).
- **Progress:** Done, 100% (Owner: Codex).
- **Open decisions:**
  - TBD: Tweak cloud density and speed. Owner: TBD, due 2025-09-21.

### UI/UX
- **Purpose**
  Provide screens for login, inventory, battle, and settings.
- **Rules & Data**
  Interface must support keyboard and mouse.
- **UX notes**
  Requires pixel font `Thaleah_PixelFont`.
  LocationActivitiesPanel provides keyboard/gamepad friendly navigation for city activities.
- **Technical notes**
  Built with Unity UGUI.
- **Progress:** In progress, 30% (popup window prefab, database-driven location activities panel, city interaction wiring, and tavern recruitment scaffolding).
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
  Support chat and player state sharing.
- **Rules & Data**
  Chat and player positions stored in MySQL tables.
- **UX notes**
  Chat window scrollback; shows last 25 messages; scroll resizes only on login or when sending a message.

- **Technical notes**
  `ChatService` to be rewritten for Unity. `PlayerStateUploader` and `PlayerStateDownloader` poll MySQL for world map positions.
- **Owner:** Codex
- **Dependencies:** ChatService backend, ScrollRect hookup, `player_position` table
- **Progress:** In progress, 20% ([PR #288](https://github.com/bullshag/ygoCGPTE/pull/288); player state sync prototype)
- **Acceptance criteria:**
  - Send/receive chat messages; auto-scroll to latest message on login
  - Local player position posted every second; remote positions retrieved
- **Risks:**
  - Message flow still unverified may block broader networking features
  - High update frequency may impact database performance
- **Open decisions:**
  - TBD: Evaluate real-time networking framework. Owner: TBD, due 2025-10-15.

#### Player State Sync
- **Dependencies:** `player_position` table, `DatabaseClientUnity`, NavMeshAgent
- **Acceptance Criteria:**
  - Uploader persists current position.
  - Downloader retrieves other players' positions.
  - RemotePlayerManager spawns markers for online players, sets destinations to next waypoints, and toggles online state each poll.
- **Risks:** Stale or missing rows may desync remote markers.
- **Progress:** In progress, 50% — Owner: Codex, due 2025-09-21

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
| TavernForm | TavernPanel + TavernRecruitDetailPanel | In progress | Codex | Search for Party Members flow live; Mercenaries/Work buttons disabled pending specs |
| PictureBox animations | FrameAnimator component | In progress | Codex | Handles sprite-frame playback |

| WorldMapForm | WorldMap scene + PlayerNavigator | In progress | Codex | Shift-click waypoints queue |
| WorldMap remote party markers | RemotePlayer entities | In progress | Codex | NavMesh markers spawn and follow downloaded waypoints; WebSocket state sync with interpolation |
| WorldMap location tooltip | AreaTooltip prefab controller | Complete | Codex | Collider-driven show/idle/hide animations with area name and button events; requires `Player` tag retention |
| WorldMap city activities panel | LocationActivitiesPanel | In progress | Codex | Scene restored from stable YAML; Enter triggers LocationActivitiesPanel.SetLocation + Open; database-driven availability toggles with #FF4949 idle / yellow active palette while Tavern/Shop hooks remain TODO |
| LocationDialog | LocationActivitiesPanel Refresh | In progress | Codex | Migration delta: WinForms modal buttons were static; Unity refresh must bind dynamic location metadata and drive contextual sub-panels |
| (New) Free camera navigation | FreeCameraController | In progress | Codex | WASD panning with smoothing |
| TravelLogService | PlayerStateUploader/Downloader | In progress | Codex | Periodic SQL sync of player positions |
| Tavern interactions (Hire/Mercenary/Work) | Tavern Sub-Panel + TavernManager integration | In progress | Codex | Migration delta: WinForms triggered direct DB updates; Unity needs sub-panel flows, telemetry, and CharacterService refresh sequencing |

### Navigation Controls Mapping

| Legacy Control | Unity System | Progress | Owner | Dependencies | Acceptance Criteria | Risks |
|---|---|---|---|---|---|---|
| Arrow key map scroll | FreeCameraController (WASD) | 10% | Codex | Unity input system | WASD pans camera with smoothing | Camera bounds undefined may allow scrolling outside map |
| Mouse click travel path | WaypointNavAgent + PlayerNavigator | 100% | Codex | Unity NavMesh; FEAT-WM-001 | Shift+Left queues waypoint; Shift+Right clears path; agent consumes queue | Shift-click conflicts with UI may block waypoint capture |


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

### Register Screen Feedback
- **Owner:** Codex
- **Dependencies:** PopupWindow prefab, DatabaseClientUnity, Register scene Canvas
- **Progress:** Complete, 100%
- **Acceptance Criteria:**
  - Register attempts display PopupWindow messaging for success and failure states.
  - Success popup transitions to the Login scene after player acknowledgment.
  - Validation and duplicate username/nickname errors use the popup prefab.
- **Risks:** PopupWindow reference could be lost during scene merges — Owner: Codex, due 2025-09-27.

### Tavern Panel
- **Owner:** Codex
- **Dependencies:** TavernManager, CharacterService cache, RPGManager.RefreshPartyUIAsync, ScrollRect candidate list prefab, TavernRecruitDetailPanel overlay
- **Progress:** In progress, 45%
- **Acceptance Criteria:**
  - "Search for Party Members" generates three recruits through PartyMemberGenerator and populates ScrollRect buttons labeled "NAME – COST gold".
  - Selecting a recruit opens a modal detail overlay with stats plus Hire and Cancel actions while hiding the list.
  - Hire triggers TavernManager.HireAsync, updates CharacterService party cache, removes the recruit from available results, and refreshes RPGManager's party display.
- **Risks:** Detail panel prefab requires Unity editor wiring before QA — Owner: Codex, due 2025-09-25.

## 9. Data & Persistence
- **Save format** (JSON/ScriptableObject/etc.)
  - TBD
- **Cloud/local strategy**
  - MySQL cloud database.
- **Backwards compatibility policy**
  - TBD
- **Test cases for migration**
  - TBD

### player_position Table
- **Schema**
  | Field | Type | Notes |
  |---|---|---|
  | player_id | INT PK | references accounts.id |
  | current_pos | VARCHAR(255) | "x,y,z" position |
  | is_traveling | TINYINT(1) | 1 when agent has path |
  | next_waypoint | VARCHAR(255) | nullable "x,y,z" of next waypoint |
  | timestamp | TIMESTAMP | auto-updated
- **Dependencies:** `DatabaseClientUnity`
- **Acceptance Criteria:** table created via `db/migrations/update_player_position.sql`; uploader persists and updates rows.
- **Risks:** high write frequency may strain database.
- **Owner:** Codex
- **Progress:** In progress, 50%

### location_activity_settings Table
- **Schema**
  | Field | Type | Notes |
  |---|---|---|
  | id | INT AUTO_INCREMENT PK | Surrogate key |
  | location_id | VARCHAR(50) | FK to nodes.id |
  | activity_key | VARCHAR(32) | Enum: tavern/shop/temple/academy/graveyard/arena/search_for_enemies |
  | is_enabled | TINYINT(1) | 1 when activity is available |
  | created_at | TIMESTAMP | Default CURRENT_TIMESTAMP |
  | updated_at | TIMESTAMP | Auto-updated on change |
- **Dependencies:** `create_location_activity_settings.sql`, LocationActivityService (Owner: Codex, delivered 2025-09-25)
- **Acceptance Criteria:** Schema script executes without error; Unity LocationActivitiesPanel reflects enabled activities per row data.
- **Risks:** Data entry errors could disable mandatory actions until reviewed — Owner: Codex, due 2025-09-28.
- **Owner:** Codex
- **Progress:** Complete, 100% (initial schema delivered 2025-09-25)

### accounts Schema Rebuild Script
- **Artifact:** `recreate_accounts_tables.sql`
- **Owner:** Codex
- **Dependencies:** MySQL CLI access, Unity client schema contracts
- **Acceptance Criteria:** Running the script on a fresh `accounts` database creates every required table, stored procedure, and trigger without errors.
- **Risks:** Future migrations may diverge from this consolidated script if not mirrored (Medium).
- **Progress:** Complete, 100%

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

| ID | Title | Type | Owner | Estimate | Dependencies | Acceptance Criteria | Status | Progress | Links | Priority |
|---|---|---|---|---|---|---|---|---|---|---|
| SYS-ARCH-001 | Record Unity version | chore | TBD | 1d | None | `ProjectVersion.txt` committed | To Do | 0% | - | Must |
| FEAT-INV-001 | Port Inventory UI | feature | TBD | 5d | SYS-ARCH-001 | Items/equipment load/save via SQL | To Do | 0% | - | Must |
| FEAT-CBT-001 | Create BattleScene | feature | TBD | 7d | SYS-ARCH-001 | Player can start and resolve battle | To Do | 0% | - | Should |
| FEAT-UI-002 | Implement popup window prefab | feature | Codex | 1d | SYS-ARCH-001 | Popup shows login errors with OK dismissal | Done | 100% | PR TBD | Should |
| FEAT-UI-003 | Register success popup flow | feature | Codex | 0.5d | PopupWindow prefab, Register scene Canvas | Register screen shows popup on success/failure and returns to Login after confirmation | Done | 100% | PR TBD | Should |
| FEAT-UI-004 | Tavern recruit search & detail overlay | feature | Codex | 2d | TavernManager, CharacterService cache, RPGManager refresh hook | Search button spawns 3 recruits, modal displays stats, Hire updates party UI | In Progress | 45% | - | Should |
| FEAT-UI-005 | Activate Tavern mercenary/work actions | feature | TBD | 3d | FEAT-UI-004 | Mercenary and Work buttons enabled with dedicated flows | To Do | 0% | - | Could |
| FEAT-UI-006 | Location activities panel refresh | feature | Codex | 2d | FEAT-WM-001; location metadata service (Owner: TBD, due 2025-09-27); `location_activity_settings` table (Owner: Codex, delivered 2025-09-25) | Populate activities from `CityNode` metadata, surface database-driven availability, support Enter/Escape focus return, maintain 1080p/1440p layout safety | In Progress | 60% | - | Should |
| FEAT-UI-007 | Tavern sub-panel integration | feature | Codex | 3d | FEAT-UI-004; FEAT-UI-006; TavernManager API audit (Owner: TBD, due 2025-09-28) | Hire/Mercenary/Work actions available with telemetry `tavern_subpanel_open` firing on open and CharacterService refresh on hire | In Progress | 5% | - | Should |
| FEAT-ANI-001 | Sprite Frame Animator component | feature | Codex | 2d | SYS-ARCH-001 | Lists animate per state at frameRate with tests | Done | 100% | PR TBD | Should |
| FEAT-WM-001 | World map shift-click navigation | feature | Codex | 2d | SYS-ARCH-001 | Shift+Right sets destination; Shift+Left queues waypoint; agent animates per direction | Done | 100% | PR TBD | Should |
| FEAT-WM-002 | City node interaction panel | feature | Codex | 1d | FEAT-WM-001 | CityInteraction panel toggles when entering/exiting CityNode radius | Done | 100% | PR TBD | Should |
| FEAT-CBT-002 | Integrate FrameAnimator with battle actions | feature | TBD | 2d | FEAT-ANI-001, FEAT-CBT-001 | Attack and damage sequences play via FrameAnimator | To Do | 0% | - | Should |
| FEAT-WM-003 | Navigation path preview line | feature | Codex | 1d | FEAT-WM-001 | Queued waypoints display a preview line | Done | 100% | PR TBD | Should |
| FEAT-WM-004 | Area tooltip interactions | feature | Codex | 1d | FEAT-WM-001 | Tooltip shows area name, plays show/idle/hide states, and raises Info/Enter events | Done | 100% | PR TBD | Should |
| FEAT-CAM-001 | Free camera controller | feature | Codex | 1d | FEAT-WM-001 | WASD pans camera with smoothing | In Progress | 10% | - | Should |
| FEAT-NET-001 | Player state upload service | feature | Codex | 1d | player_position table | Uploader posts position every second | Done | 100% | - | Should |
| FEAT-NET-002 | Player state download service | feature | Codex | 1d | player_position table, FEAT-NET-001 | Downloader fetches other players' positions | Done | 100% | - | Should |
| FEAT-ENV-001 | Cloud shadow spawner | feature | Codex | 1d | FEAT-WM-001 | Clouds spawn randomly and drift left casting terrain shadows | Done | 100% | - | Could |


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

### UI Feature Quality Tracking

| Feature | Progress | Owner | Dependencies | Acceptance Criteria | Risks |
|---|---|---|---|---|---|
| Location Activities Panel Refresh | 94% (due 2025-09-29) | Codex | FEAT-WM-001; FEAT-UI-006; location metadata service (Owner: TBD, due 2025-09-27); `location_activity_settings` table (Owner: Codex, delivered 2025-09-25) | Reuses handcrafted `locationInfoWindow` layout with dynamic activity selection;<br>Loads availability via LocationActivityService and database toggles;<br>Supports 1080p/1440p layouts without blocking map input;<br>Debug toggle enables 3-second polling for QA | InputAction focus clashes during sub-panel open (Owner: Codex, due 2025-09-30);<br>Metadata contract TBD may delay integration (Owner: TBD, due 2025-09-27);<br>Placeholder copy must transition to live data once metadata lands (Owner: Codex, due 2025-09-29);<br>SQL data drift could hide critical actions (Owner: Codex, due 2025-09-28) |
| Tavern Sub-Panel Integration | 5% (due 2025-10-01) | Codex | FEAT-UI-004; FEAT-UI-007; TavernManager API audit (Owner: TBD, due 2025-09-28) | Hire, Mercenary, Work actions expose stateful buttons;<br>Hire reuses recruit overlay and closes cleanly;<br>`tavern_subpanel_open` telemetry fires with location ID | Legacy edge cases from TavernForm undocumented (Owner: TBD, due 2025-09-29);<br>Responsive layout for small resolutions unverified (Owner: Codex, due 2025-10-03);<br>CharacterService refresh timing may double-trigger updates (Owner: Codex, due 2025-10-04) |

## 15. Risks & Mitigations
- Missing Unity version info (Likely/Medium) — Owner: TBD — Mitigation: inspect project settings; Trigger: build fails.
- SQL scripts diverge from WinForms logic (Possible/High) — Owner: TBD — Mitigation: cross-review with legacy code; Trigger: test mismatch.
- CI lacks Windows components (Likely/High) — Owner: TBD — Mitigation: install WindowsDesktop SDK or adjust tests; Trigger: build pipeline failure.
- Chat message flow unverified (Possible/Medium) — Owner: TBD — Mitigation: end-to-end test with database; Trigger: messages fail to appear.
- Popup windows not standardized across scenes (Possible/Low) — Owner: Codex — Mitigation: centralize prefab usage; Trigger: inconsistent UI messaging.
- Sprite animation lists may be incomplete (Possible/Low) — Owner: TBD — Mitigation: context menu to append frames; Trigger: missing frames during play.
- Waypoint queue may desync with NavMesh (Possible/Low) — Owner: TBD — Mitigation: monitor agent stalls and clear queue; Trigger: agent stops unexpectedly.
- Shift-click input may conflict with UI elements (Possible/Low) — Owner: TBD — Mitigation: require map focus; Trigger: waypoints fail to enqueue.
- Player position sync may saturate database (Possible/Low) — Owner: Codex — Mitigation: throttle upload interval; Trigger: DB performance degradation.
- GUID corruption in `.meta` files may break asset references (Possible/Low) — Owner: Codex — Mitigation: regenerate or reset GUIDs; Trigger: assets reference missing scripts.

## 16. Changelog (Auto-Appended)
- 2025-09-13: Created initial GDD skeleton covering all sections. — Codex
- 2025-09-13: Updated Networking progress and ChatService migration status after chat auto-scroll work (PR #288). — Codex
- 2025-09-13: Verified chatScrollRect wiring and documented auto-scroll on login. — Codex
- 2025-09-13: Enabled dynamic chat content resizing when new messages arrive. — Codex
- 2025-09-13: Restricted chat view to 25 messages and resized scroll only on login/send. — Codex
- 2025-09-13: Added popup window prefab and integrated into login screen for invalid credential messages. — Codex
- 2025-09-13: Introduced FrameAnimator component with context menus and tests to manage sprite animations. — Codex
- 2025-09-13: Added PlayerNavigator with shift-click waypoints and NavMesh queue for world map movement. — Codex
- 2025-09-13: Implemented CityNodeData and city interaction triggers with UI panel toggling. — Codex
- 2025-09-13: Added remote player state packets, WebSocket broadcasting, interpolation, and sync tests. — Codex
- 2025-09-14: Documented tasks for FrameAnimator combat integration and navigation path preview line; referenced mapping table. — Codex
- 2025-09-14: Added FreeCameraController with WASD camera panning and smoothing; documented control scheme. — Codex

- 2025-09-14: Regenerated Unity .meta GUIDs for FrameAnimator assets (FEAT-ANI-001) to prevent reference corruption. — Codex
- 2025-09-14: Added WaypointNavAgent with queued NavMesh waypoints and shift-click clearing. — Codex
- 2025-09-14: Added player_position schema and PlayerState upload/download services for world map sync. — Codex
- 2025-09-14: Added navigation control mappings from WinForms to Unity camera/agent systems with progress, dependencies, acceptance criteria, and risks. — Codex
- 2025-09-14: Introduced RemotePlayerMarker prefab and manager for NavMesh waypoint syncing and online/offline tracking. — Codex
- 2025-09-14: Updated PlayerNavigator to switch to idle animation when NavMeshAgent stops. — Codex

- 2025-09-14: Added CloudSpawner and CloudMover with cloud prefab for random shadow-casting clouds. — Codex
- 2025-09-26: Wired LocationActivitiesPanel scene bindings to nodeFortAurus locationId and mapped all seven activity buttons to their Unity `Button` components for availability refreshes. — Codex

- 2025-09-14: Added waypoint marker prefab and path preview line rendering for queued waypoints. — Codex

- 2025-09-16: Authored `recreate_accounts_tables.sql` to rebuild the accounts schema and captured maintenance guidance in the GDD. — Codex
- 2025-09-16: Added AreaTooltip controller for world map location overlays and documented collider-driven show/idle/hide flow. — Codex
- 2025-09-16: Rewired the locationTooltip root with AreaTooltip, a trigger SphereCollider sized to the location radius, and assigned TMP label, buttons, and animator triggers. — Codex

- 2025-09-16: Routed RegisterManager to PopupWindow prefab for validation, duplicate checks, and success confirmation that returns to Login. — Codex

- 2025-09-16: Added CapsuleCollider and kinematic Rigidbody to the world map player for tooltip triggers and documented the setup. — Codex
- 2025-09-16: Restored RPG scene YAML after merge conflict, reattached AreaTooltip wiring, and reinstated player physics components pending Unity re-save. — Codex
- 2025-09-23: Documented structured TavernPanel flow, recruit detail overlay, CharacterService party cache updates, and RPGManager refresh hook. — Codex

- 2025-09-22: Updated PlayerNavigator raycast to ignore tooltip triggers, restoring waypoint placement inside area volumes. — Codex
- 2025-09-22: Tagged world map player and restored `Player` tag in TagManager so AreaTooltip triggers fire on entry/exit. — Codex

- 2025-09-23: Recovered RPG scene from backup commit, reinstated the `player` GameObject tag, and logged scene YAML edit risk mitigation. — Codex

- 2025-09-23: Restored RPG scene from clean snapshot, reattached LocationActivitiesPanel hierarchy, and wired AreaTooltip Enter events to open the panel. — Codex

- 2025-09-24: Added LocationActivitiesPanel with yellow/red highlighting, Enter/Escape input flow, and updated AreaTooltip to re-arm interactions per keyboard accessibility pillar. — Codex

- 2025-09-24: Scoped LocationActivitiesPanel refresh and Tavern sub-panel integration, recorded new dependencies, acceptance criteria, and risks, and noted TBD follow-ups for location metadata service contract (Owner: TBD, due 2025-09-27) and TavernManager API audit (Owner: TBD, due 2025-09-28) to unblock implementation steps. — Codex
- 2025-10-16: Documented tooltip unlock fix on LocationActivitiesPanel close (Escape/Back/Cancel), updated World-Space UI Interaction progress, and noted headless Play Mode verification gap. — Codex

- 2025-09-24: Rewired PlayerNavigator and AreaTooltip to the existing `locationInfoWindow`, migrated LocationActivitiesPanel onto it, and generated placeholder content for yet-to-be-specified locations. — Codex
- 2025-09-24: Restored RPG scene from commit 0ea6a56 after corruption and re-populated LocationActivitiesPanel defaults in YAML. — Codex

- 2025-09-25: Rebuilt LocationActivitiesPanel with database-driven availability, delivered LocationActivityService, and authored `create_location_activity_settings.sql` for per-location toggles. — Codex
- 2025-09-26: Restored RPG scene from commit 5a21cfe after corruption resurfaced and re-verified LocationActivitiesPanel bindings; scheduled Unity re-save validation follow-up. — Codex
- 2025-09-26: Added LocationActivitiesPanel debug auto-refresh toggle to poll database availability every 3 seconds for QA verification. — Codex
- 2025-09-26: Updated LocationActivityService to emit console logs enumerating activities and enabled states whenever availability is queried. — Codex
- 2025-10-16: Disabled locationTooltip Canvas GraphicRaycaster Ignore Reversed Graphics to restore Enter button clicks and documented world-space UI verification follow-up. — Codex
