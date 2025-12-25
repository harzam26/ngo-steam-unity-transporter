# NGO Steam Transporter

A hybrid networking solution for Unity **Netcode for GameObjects (NGO)**. This package provides a seamless integration between **Facepunch.Steamworks** (Steam Relay) and **Unity Transport** (Relay/Direct), handling lobby management, connection logic, and player spawning automatically.

## 📦 Features

* **Hybrid Transport System:** Automatically selects the best transport method.
    * *Steam Mode:* Uses Valve's Steam Datagram Relay (SDR) via Facepunch.
    * *Unity Mode:* Uses Unity Relay or Direct IP connection when Steam is not available.
* **Lobby Management:** Built-in system for creating, searching, and joining lobbies.
* **Auto-Bootstrap:** Handles NetworkManager initialization and scene loading.
* **Player Spawning:** Includes logic for handling player persistence and spawning.

---

## 🚀 Installation

### 1. Install via Package Manager
Open Unity, go to **Window > Package Manager**, click the **"+"** icon, select **"Add package from git URL..."**, and paste the link below:

```text
[https://github.com/harzam26/ngo-steam-unity-transporter.git](https://github.com/harzam26/ngo-steam-unity-transporter.git)

```

*Dependencies (Netcode, Facepunch, Collections, etc.) will be installed automatically.*

---

## ⚙️ Setup & Configuration (Crucial!)

This package relies on specific **Layers** and **Physics** settings that are not imported automatically. You must configure these manually in your project.

### 1. Create Layers

Go to **Edit > Project Settings > Tags and Layers**. Add the following user layers (IDs can be anything):

* `Player`
* `CameraBounds`

### 2. Configure Physics Matrix

To ensure the camera logic functions correctly without interfering with gameplay physics:

1. Go to **Edit > Project Settings > Physics**.
2. Scroll down to the **Layer Collision Matrix**.
3. Find the `CameraBounds` row/column.
4. **Uncheck ALL boxes** for `CameraBounds`.
* *Note:* `CameraBounds` should not collide with anything (it is logic-only).



### 3. Steam App ID

Create a text file named `steam_appid.txt` in the root folder of your Unity project (next to the `Assets` folder, not inside it).

* Inside the file, write only your Steam App ID (e.g., `480` for SpaceWar test app).
* **Important:** Steam Client must be running in the background for the transport to work.

---

## 🛠 Usage

### Initialization

1. Drag the **`NetworkBootstrap`** prefab (located in `Packages/NGO Steam Transporter/Runtime/NetworkBase/`) into your startup scene.
2. Ensure you have a **NetworkManager** in the scene with `UnityTransport` assigned as the default transport.
3. The `TransportSelector` script will automatically swap the transport to **Facepunch** if Steam is detected.

### Using the Lobby System

You can access the lobby manager via the singleton:

```csharp
// Create a Lobby
await LobbyManager.Instance.CreateLobby("My Room", 4, isPrivate: false);

// Join a Lobby
await LobbyManager.Instance.JoinLobby(lobbyId);

```

---

## 🧩 Architecture

### Transport Selection Logic

The system checks for the presence of the Steam Client on startup:

* **If Steam is Running:** It swaps the NetworkManager's transport to `FacepunchTransport` and initializes the Steamworks API.
* **If Steam is Missing:** It falls back to `UnityTransport` (useful for console builds or local debugging).

### Folder Structure

* `Runtime/NetworkBase`: Core networking logic and bootstrapping.
* `Runtime/Menu`: UI logic for Lobbies and Main Menu.
* `Runtime/Player`: Player controller, states, and camera logic.
* `Runtime/ThirdParty`: Wrapper for Facepunch transport.

---

## 📄 License

This project is licensed under the MIT License.
