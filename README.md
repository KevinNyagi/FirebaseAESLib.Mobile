# FirebaseAESLib.Mobile

**FirebaseAESLib.Mobile** is a Xamarin-compatible library that uses AES encryption to securely interact with Firebase **Realtime Database** and **Firestore** using the Firebase REST API.

---

## ✅ Features

- 🔐 AES-256 encryption/decryption
- 🔥 Works with Firestore (via REST)
- 🔁 Works with Realtime Database (via REST)
- 📱 Compatible with Xamarin, MAUI, .NET Standard 2.0
- ⚙️ No Admin SDK dependencies

---

## 📦 Installation

```bash
# Create the project (if not already done)
dotnet new classlib -n FirebaseAESLib.Mobile -f netstandard2.0
cd FirebaseAESLib.Mobile

# Add required packages
dotnet add package System.Security.Cryptography.Algorithms
dotnet add package System.Text.Json
```

Or just include the `.cs` files directly into your Xamarin shared project.

---

## 🔧 Configuration

Create a 256-bit AES key and IV, then Base64 encode them:

```csharp
var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
var iv = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
```

Store them securely (e.g., in `SecureStorage` or environment variables).

---

## 🔐 Usage Example

```csharp
using FirebaseAESLib.Mobile;

var aes = new AesEncryptor("base64-key", "base64-iv");

// Firestore
var firestore = new FirestoreRestClient("your-project-id", "your-api-key", aes);
await firestore.SetEncryptedAsync("users", "user1", new Dictionary<string, object>
{
    { "name", "Alice" },
    { "email", "alice@example.com" }
});

var result = await firestore.GetDecryptedAsync("users", "user1");

// Realtime Database
var realtime = new RealtimeRestClient("your-project-id", "optional-id-token", aes);
await realtime.SetEncryptedAsync("users/user2", new Dictionary<string, object>
{
    { "phone", "12345678" }
});

var realtimeResult = await realtime.GetDecryptedAsync("users/user2");
```

---

## 🔐 Firebase Setup Required

| Field               | Source                                        |
|--------------------|-----------------------------------------------|
| Project ID         | Firebase Console > Project Settings           |
| Web API Key        | Firebase Console > Project Settings > General |
| ID Token (optional)| Firebase Auth token for signed-in user        |

To use Firebase Authentication:
- Sign in using REST API or Firebase SDK
- Get the `idToken` and pass it to `RealtimeRestClient`

---

## 📁 Files

- `AesEncryptor.cs`: AES logic
- `FirestoreRestClient.cs`: Handles Firestore REST
- `RealtimeRestClient.cs`: Handles Realtime DB REST
- `HttpClientExtensions.cs`: Adds `PatchAsync()`

---

## 🧪 Roadmap / Coming Soon

- [ ] Token auto-refresh
- [ ] Firestore query support
- [ ] Support for structured (non-string) types
- [ ] Upload as NuGet package

---

## ✅ License

MIT License
