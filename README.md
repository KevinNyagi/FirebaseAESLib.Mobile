
# FirebaseAESLib.Mobile Xamarin.Forms Sample

This is a minimal sample Xamarin.Forms app using the `FirebaseAESLib.Mobile` NuGet package to securely encrypt and decrypt Firebase Realtime Database data with AES encryption.

## 🔧 Requirements
- Xamarin.Forms (>= 5.0.0)
- .NET Standard 2.0 compatible platform (Android/iOS)
- Firebase project with Realtime Database enabled

## 📦 Install Packages
```
# Add this package to your Xamarin shared project
nuget install FirebaseAESLib.Mobile -Version 1.0.3

# Or using .NET CLI:
dotnet add package FirebaseAESLib.Mobile --version 1.0.3

dotnet add package DotNetEnv
```

## 📁 Folder Structure
```
MyFirebaseAESApp/
├── Models/
│   ├── Member.cs
│   └── RawMember.cs
├── Views/
│   └── MainPage.xaml
├── MainPage.xaml.cs
├── .env
├── App.xaml.cs
└── App.xaml
```

## 🧪 Sample `.env` File
Place this in the root of your shared project:
```
AES_KEY=your-base64-encoded-256bit-key
AES_IV=your-base64-encoded-128bit-iv
FIREBASE_PROJECT_ID=your-project-id
FIREBASE_ID_TOKEN=your-firebase-user-id-token
```

## 🧬 How It Works
- 🔐 Encrypts each field using AES before pushing to Firebase
- 🔓 Decrypts data after reading it back
- 🔑 Uses `.env` for storing Firebase project credentials and AES keys

## 🧾 Code Overview

### 1. Define your encrypted `Member` model
`Models/Member.cs`
```csharp
public class Member
{
    public string Name { get; set; }
    public string Age { get; set; }
    public string Gender { get; set; }
    public string Phone { get; set; }
    public string Residence { get; set; }
    public string RegistrationDateTime { get; set; }
    public DateTime LogDetailDateTime { get; set; }

    public int AgeInt => int.TryParse(Age, out var r) ? r : 0;
}
```

### 2. Define a raw display model for encrypted content
`Models/RawMember.cs`
```csharp
public class RawMember
{
    public string Key { get; set; }
    public Dictionary<string, object> Fields { get; set; }

    public string Name => Get("Name");
    public string Age => Get("Age");

    private string Get(string key) =>
        Fields != null && Fields.TryGetValue(key, out var v) ? v?.ToString() ?? "" : "";
}
```

### 3. MainPage UI Layout
`Views/MainPage.xaml`
```xml
<ContentPage ...>
    <StackLayout Padding="10">
        <Entry x:Name="NameEntry" Placeholder="Enter Name" />
        <Entry x:Name="AgeEntry" Placeholder="Enter Age" Keyboard="Numeric" />
        <Button Text="Save Member" Clicked="OnSaveClicked" />
        <Button Text="Load Members" Clicked="OnLoadClicked" />
        <ListView x:Name="MainListView" />
    </StackLayout>
</ContentPage>
```

### 4. Logic: Save & Load Encrypted Data
`MainPage.xaml.cs`
```csharp
public partial class MainPage : ContentPage
{
    private RealtimeRestClient _firebase;
    private AesEncryptor _aes;

    public MainPage()
    {
        InitializeComponent();
        Env.Load(); // 🔄 Load .env variables

        var key = Environment.GetEnvironmentVariable("AES_KEY");
        var iv = Environment.GetEnvironmentVariable("AES_IV");
        var pid = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");
        var token = Environment.GetEnvironmentVariable("FIREBASE_ID_TOKEN");

        _aes = new AesEncryptor(key, iv);
        _firebase = new RealtimeRestClient(pid, token, _aes);
    }

    // 🔐 Encrypt and push data
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var member = new Dictionary<string, object>
        {
            { "Name", NameEntry.Text },
            { "Age", AgeEntry.Text },
            { "Gender", "MALE" },
            { "Phone", "+123456789" },
            { "Residence", "NAIROBI" },
            { "RegistrationDateTime", DateTime.Now.ToString("dd MMM yyyy") },
            { "LogDetailDateTime", DateTime.UtcNow.ToString("o") }
        };

        await _firebase.PushEncryptedAsync("DemoApp/Members", member);
        await DisplayAlert("Saved", "Member saved to Firebase.", "OK");
    }

    // 🔓 Read and decrypt data
    private async void OnLoadClicked(object sender, EventArgs e)
    {
        var url = _firebase.BuildUrl("DemoApp/Members");
        var json = await new HttpClient().GetStringAsync(url);

        var raw = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(json);
        var members = new List<Member>();

        foreach (var item in raw)
        {
            var decrypted = _aes.DecryptDictionary(item.Value);
            var memJson = JsonSerializer.Serialize(decrypted);
            var member = JsonSerializer.Deserialize<Member>(memJson);
            members.Add(member);
        }

        MainListView.ItemsSource = members;
    }
}
```

### 5. App Startup
`App.xaml.cs`
```csharp
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new MainPage());
    }

    protected override void OnStart() => DotNetEnv.Env.Load();
}
```

---

## ✅ Result
- AES encrypted member data is stored securely in Firebase.
- On retrieval, data is decrypted and shown in your mobile app.

## 📚 Learn More
🔗 GitHub: [FirebaseAESLibMobile](https://github.com/KevinNyagi/FirebaseAESLib.Mobile))

MIT License
