````markdown
# FirebaseAESLib.Mobile

Secure AES-encrypted **Firebase Realtime Database** and **Firestore REST client** for **Xamarin** and **.NET Standard 2.0**.  
This library makes it easy to store and retrieve encrypted data from Firebase using **AES encryption**.

---

## ✨ Features
- 🔐 AES-256 encryption for Firebase data  
- 🔄 Push / Put / Patch / Delete support  
- 📥 Encrypted and decrypted object handling  
- 📱 Designed for Xamarin / Mobile apps  
- 🌐 Works with Realtime Database REST API  

---

## 📦 Installation

From **NuGet**:

```bash
dotnet add package FirebaseAESLib.Mobile
````

Or in **Visual Studio**:

```powershell
PM> Install-Package FirebaseAESLib.Mobile
```

---

## ⚙️ Setup

You need a Firebase project and an AES key/IV (Base64).
Generate a random 256-bit AES key and 128-bit IV:

```csharp
using System;
using System.Security.Cryptography;

var aes = Aes.Create();
aes.GenerateKey();
aes.GenerateIV();

string base64Key = Convert.ToBase64String(aes.Key);
string base64IV = Convert.ToBase64String(aes.IV);

Console.WriteLine($"Key: {base64Key}");
Console.WriteLine($"IV: {base64IV}");
```

Save these values securely in your app.

---

## 🚀 Usage

### 1. Initialize AES Encryptor

```csharp
using FirebaseAESLib.Mobile;

var encryptor = new AesEncryptor("<your-base64-key>", "<your-base64-iv>");
```

### 2. Create Realtime Client

```csharp
var firebase = new RealtimeRestClient(
    projectId: "your-project-id", 
    idToken: "<optional-firebase-idtoken>", 
    aes: encryptor
);
```

### 3. Push Encrypted Data

```csharp
var member = new { Name = "Kevin", Role = "Admin" };

string? key = await firebase.PushEncryptedAsync("Members", member);
Console.WriteLine($"New record key: {key}");
```

### 4. Get Encrypted String

```csharp
string? decrypted = await firebase.GetDecryptedStringAsync("Members/memberId/Name");
Console.WriteLine($"Decrypted Name: {decrypted}");
```

### 5. Update (PATCH) Encrypted

```csharp
await firebase.PatchEncryptedAsync("Members/memberId", new { Role = "User" });
```

### 6. Delete Node

```csharp
await firebase.DeleteAsync("Members/memberId");
```

---

# 📖 API Reference

## 🔐 AesEncryptor

AES-256 encryption / decryption helper.

### `string Encrypt(string plainText)`

Encrypts a string.

* **plainText** → Input text
* **Returns** → AES-encrypted Base64 string

---

### `string Decrypt(string cipherText)`

Decrypts a string.

* **cipherText** → AES-encrypted Base64 string
* **Returns** → Decrypted plain text

---

### `Dictionary<string, object> EncryptDictionary(Dictionary<string, object> plainDict)`

Encrypts all values in a dictionary.

* **plainDict** → Dictionary of key/value pairs
* **Returns** → Dictionary with encrypted values

---

### `Dictionary<string, object> DecryptDictionary(Dictionary<string, object> encryptedDict)`

Decrypts all values in a dictionary.

* **encryptedDict** → Dictionary with AES-encrypted values
* **Returns** → Dictionary with decrypted values

---

## 🌐 RealtimeRestClient

Encrypted Firebase Realtime Database REST client.

### `RealtimeRestClient(string projectId, string idToken, AesEncryptor aes)`

Initializes the client.

* **projectId** → Firebase project ID
* **idToken** → Firebase user ID token
* **aes** → `AesEncryptor` instance

---

### `Task<string> PushEncryptedAsync(string path, object data)`

Pushes a new encrypted record to Firebase (generates unique key).

* **path** → Database path (e.g. `"DemoApp/Members"`)
* **data** → Object/dictionary to encrypt and save
* **Returns** → Firebase generated key

---

### `Task PutEncryptedAsync(string path, object data)`

Creates or replaces data at a specific path.

* **path** → Full path (e.g. `"DemoApp/Members/memberKey"`)
* **data** → Object/dictionary to encrypt and save

---

### `Task PatchEncryptedAsync(string path, object data)`

Updates only the given fields (partial update).

* **path** → Full path (e.g. `"DemoApp/Members/memberKey"`)
* **data** → Object/dictionary containing updated fields

---

### `Task<T?> GetDecryptedAsync<T>(string path)`

Fetches and decrypts a node as an object.

* **path** → Full path (e.g. `"DemoApp/Members/memberKey"`)
* **Returns** → Decrypted object or `null`

---

### `Task<string?> GetDecryptedStringAsync(string path)`

Fetches and decrypts a single string value.

* **path** → Full path (e.g. `"DemoApp/Members/memberKey/Name"`)
* **Returns** → Decrypted string

---

### `Task DeleteAsync(string path)`

Deletes a record from Firebase.

* **path** → Full path (e.g. `"DemoApp/Members/memberKey"`)

---

### `Task<string> PushAsync(string path, object data)`

Pushes **raw (unencrypted)** data.

* **path** → Database path
* **data** → Object/dictionary
* **Returns** → Firebase generated key

---

### `Task PutAsync(string path, object data)`

Overwrites **raw (unencrypted)** data.

* **path** → Full path
* **data** → Object/dictionary

---

### `Task PatchAsync(string path, object data)`

Updates **raw (unencrypted)** data.

* **path** → Full path
* **data** → Object/dictionary

---

### `string BuildUrl(string path)`

Builds a Firebase REST API URL.

* **path** → Database path
* **Returns** → Full Firebase REST URL

---

## 🧩 Example Usage

```csharp
var aes = new AesEncryptor(key, iv);
var client = new RealtimeRestClient(projectId, idToken, aes);

// Push new member
var member = new { Name = "John", Age = "28" };
string key = await client.PushEncryptedAsync("DemoApp/Members", member);

// Update member
await client.PatchEncryptedAsync($"DemoApp/Members/{key}", new { Age = "29" });

// Replace member
await client.PutEncryptedAsync($"DemoApp/Members/{key}", new { Name = "John Doe", Age = "30" });

// Get decrypted
string? name = await client.GetDecryptedStringAsync($"DemoApp/Members/{key}/Name");

// Delete member
await client.DeleteAsync($"DemoApp/Members/{key}");

// Push raw (unencrypted)
string rawKey = await client.PushAsync("DemoApp/Logs", new { Event = "Login", Time = DateTime.UtcNow });

// Build Firebase URL
string url = client.BuildUrl("DemoApp/Members");
```

---

## 🏷️ Metadata

* **Package Id**: FirebaseAESLib.Mobile
* **Target Framework**: .NET Standard 2.0
* **Author**: Kevin Nyagi (RAVINCE LLC)
* **License**: MIT

---

## 📌 Notes

* Firebase REST API base URL:
  `https://<projectId>.firebaseio.com/`

* Pass `idToken` if Firebase rules require authenticated requests.

* Only **string/primitive values are encrypted**.
  Firebase structural JSON remains intact.

```

