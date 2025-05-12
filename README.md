# ⚡ SockerLocatorBot

**Telegram Bot to Share and Find Free Electrical Socket (or any other) Locations — With Location, Image, and Map Support**

SockerLocatorBot is a geolocation-aware Telegram bot that helps users **add** or **find nearby** locations. Users can contribute by sharing GPS coordinates and uploading an image, which is then stored on **Google Drive**. All location data is stored in a **PostgreSQL database with PostGIS**, enabling powerful spatial queries.

---

## 🚀 Features

- 📍 **Add New Socket Location** — Send location + photo to register a free-to-use socket.
- 🔍 **Find Closest Sockets Nearby** — Bot uses PostGIS to find the 3 closest locations within a configurable radius (e.g., 100km).
- 🖼️ **Photo Uploads** — Each socket location supports multiple images, stored in **Google Drive**.
- 🗃️ **PostGIS-Powered Search** — Efficient geospatial queries using PostgreSQL + PostGIS.
- ♻️ **Asynchronous Background Processing** — Designed for scalability using a background worker.
- 🧩 **Modular Architecture** — Separation of concerns with distinct modules for bot logic, database access, and Google Drive integration.

---

## 🛠 Tech Stack

| Layer           | Technology                     |
|----------------|---------------------------------|
| Language        | C# .NET 9                      |
| Bot Framework   | Telegram.Bot                  |
| Storage         | Google Drive API               |
| Database        | PostgreSQL + PostGIS           |
| ORM             | Entity Framework Core          |
| Hosting Option  | .NET Worker Service / Docker   |
| DI              | Microsoft.Extensions.DependencyInjection |

---

## ⚙️ Getting Started

### ✅ Prerequisites

- .NET 9 SDK
- PostgreSQL with PostGIS extension
- Google Drive API credentials (Service Account)
- Telegram Bot Token (via BotFather)

---

### 🔧 Installation

1. **Clone the Repository**

```bash
git clone https://github.com/Fargesta/SockerLocatorBot.git
cd SockerLocatorBot
```

2. **Configure Environment**

Create a `.env` or `appsettings.json` with required settings:

```json
{
  "TelegramBot": {
    "Token": "YOUR_BOT_TOKEN"
  },
  "GoogleDrive": {
    "CredentialsPath": "PATH_TO_JSON",
    "ApplicationName": "SockerLocatorBot",
    "DirectoryId": "YOUR_FOLDER_ID"
  },
  "Postgres": {
    "ConnectionString": "Host=localhost;Port=5432;Database=sockets;Username=postgres;Password=yourpassword;SSL Mode=Prefer"
  },
  "Radius": 100
}
```

3. **Apply EF Core Migrations**

Make sure your DB is up and PostGIS is enabled:

```sql
CREATE EXTENSION postgis;
```

Then:

```bash
dotnet ef database update
```

4. **Run the Bot**

```bash
dotnet run --project SockerLocatorBot
```

Or use Docker (optional):

```bash
docker build -t socker-bot .
docker run --env-file .env socker-bot
```

---

## 🧪 How It Works

### Adding a Socket:

1. Share your **live location**
2. Upload **one or more photos**
3. Bot saves GPS + images to DB (PostGIS + Google Drive)

### Finding Sockets Nearby:

- Bot calculates distance using PostGIS
- Sends back closest location(s) with location + images
- Radius is configurable

---

## 🗂 Project Structure

```
SockerLocatorBot/
│
├── BotHandlers/             # Individual Telegram handlers
├── DbManager/               # Entity Framework + PostGIS logic
├── DriveManager/            # Google Drive integration
├── Models/                  # EF Core models
├── Worker/                  # Background processing logic
├── Program.cs               # Entry point
└── appsettings.json         # Configuration
```

---

## 📸 Image Handling via Google Drive

- Files are uploaded via Service Account using official Drive API
- File metadata (ID, size, name) is stored in DB
- On retrieval, the bot downloads and sends image(s) back to the user

---

## 🧱 Database Schema Overview

- `LocationModel`: stores geometry (PostGIS Point), metadata, timestamps
- `TGImageModel`: stores file ID, location reference, etc.

---

## 📜 License

MIT — see [LICENSE.txt](LICENSE.txt) for details.

---

## 🤝 Contributing

Pull requests are welcome. For major changes, open an issue first to discuss what you'd like to change.

---
