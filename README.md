# Backend mappaszerkezet
```
EcoTrip/				→ fő projekt mappa
├── Controllers/						→ API végpontok
│   ├── AuthController.cs				
│   ├── BookingController.cs			
│   └── ...
│
├── Models/								→ adatmodellek
│   ├── DTOs/							→ adatátviteli objektumok
│   │   └── UserDto.cs
│   ├── User.cs
│   └── ...
│
├── Migrations/							→ adatbázis verziók
│   ├── ...
│
├── Properties/							→ projekt beállítások
│   ├── launchSettings.json
├── Services/
│   └── IServices/						→ service interfészek
│       └── IMail.cs					
│   ├── Mail.cs				
│
├── Program.cs
├── appsettings.Development.json
├── EcoTrip.http
├── appsettings.json
└── EcoTrip.csproj
```