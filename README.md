# MyCryptoPortfolio (Magang-2025-2)

**MyCryptoPortfolio** adalah aplikasi manajemen portofolio investasi modern berbasis web yang dibangun dengan **ASP.NET Core 9.0**. Aplikasi ini dirancang untuk memantau aset Kripto (dan Saham) secara real-time, menghitung keuntungan/kerugian (PnL), serta menyediakan visualisasi data yang intuitif.

Dibangun dengan prinsip **Clean Architecture**, aplikasi ini memisahkan logika bisnis, akses data, dan tampilan pengguna untuk memastikan kode yang rapi, mudah dirawat, dan *scalable*.

---

## Daftar Isi

- [Teknologi yang Digunakan](#teknologi-yang-digunakan)
- [Prasyarat](#prasyarat-prerequisites)
- [Instalasi dan Setup Awal](#instalasi--setup-awal)
  - [1. Clone Repository](#1-clone-repository)
  - [2. Konfigurasi Database](#2-konfigurasi-database)
  - [3. Terapkan Migrasi Database](#3-terapkan-migrasi-database-database-update)
- [Cara Menjalankan Aplikasi](#cara-menjalankan-aplikasi-build--run)
- [Panduan Fitur dan Penggunaan](#panduan-fitur--penggunaan)
- [Penjelasan Teknis](#penjelasan-teknis-under-the-hood)
- [Update Fitur: Autentikasi dan Autorisasi Pengguna](#update-fitur-autentikasi-dan-autorisasi-pengguna)
- [Troubleshooting](#troubleshooting-masalah-umum)


---

## Teknologi yang Digunakan

- **Framework:** .NET 9.0 (ASP.NET Core MVC)
- **Bahasa:** C#
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core
- **Frontend:** Razor Views, Tailwind CSS (via CDN), Phosphor Icons
- **Charting:** Chart.js, TradingView Widgets
- **API Data:** CoinGecko Public API (dengan sistem caching internal)

---

## Prasyarat (Prerequisites)

Sebelum memulai, pastikan komputer Anda telah terinstal:

1. **.NET 9.0 SDK** (Wajib)
2. **PostgreSQL** (Database Server)
3. **Code Editor** (Disarankan: Visual Studio Code atau Visual Studio 2022)
4. **Git** (Untuk cloning repo)

> Catatan: Untuk menjalankan perintah migrasi, Anda mungkin perlu memasang EF Tools:
>
> ```bash
> dotnet tool install --global dotnet-ef
> ```

---

## Instalasi & Setup Awal

Ikuti langkah-langkah ini secara berurutan untuk menjalankan aplikasi di komputer lokal Anda.

### 1. Clone Repository

Buka terminal dan jalankan:

```bash
git clone https://github.com/codenameyizzz/MyCryptoPortofolio.git
cd MyCryptoPortofolio
```

> Jika folder root project Anda bernama berbeda (mis. `MyCryptoPortfolio`), sesuaikan perintah `cd` di atas sesuai nama folder hasil clone.

---

### 2. Konfigurasi Database

Aplikasi ini membutuhkan koneksi ke PostgreSQL.

1. Buka file **`MyCryptoPortfolio.Web/appsettings.json`**.
2. Temukan bagian `"ConnectionStrings"`.
3. Sesuaikan `DefaultConnection` dengan kredensial PostgreSQL lokal Anda (Username, Password, dan Nama Database).

Contoh konfigurasi:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=MyCryptoPortoDb;Username=postgres;Password=password_anda"
}
```

*(Catatan: Pastikan user `postgres` dan password-nya benar. Database `MyCryptoPortoDb` akan dibuat otomatis di langkah selanjutnya.)*

---

### 3. Terapkan Migrasi Database (Database Update)

Kita perlu membuat database dan tabel-tabelnya berdasarkan kode (**Code-First Approach**).

Buka terminal di root folder project, lalu jalankan perintah ini:

```bash
# Membuat Database & Tabel berdasarkan migrasi yang sudah ada
dotnet ef database update -p MyCryptoPortfolio.Infrastructure -s MyCryptoPortfolio.Web
```

Jika berhasil, akan muncul pesan seperti **"Done."** dan database `MyCryptoPortoDb` telah siap digunakan.

---

## Cara Menjalankan Aplikasi (Build & Run)

Setelah setup database selesai, Anda siap menjalankan aplikasi.

1. Pastikan Anda berada di root folder project.
2. Jalankan perintah:

```bash
dotnet run --project MyCryptoPortfolio.Web
```

3. Tunggu hingga muncul pesan seperti:
   `Now listening on: http://localhost:5168`
4. Buka browser dan akses alamat tersebut (biasanya **http://localhost:5168**).

---

## Panduan Fitur & Penggunaan

Berikut adalah penjelasan mendalam mengenai setiap halaman dan fitur dalam aplikasi ini:

### 1. Halaman Utama (News & Landing Page)

- **Fungsi:** Menjadi gerbang utama aplikasi.
- **Fitur:** Menampilkan **TradingView News Widget** yang menyajikan berita pasar finansial dan kripto terkini secara real-time.
- **Navigasi:** Terdapat tombol **"Buka Portfolio Saya"** untuk masuk ke dashboard utama.

---

### 2. Dashboard Portfolio (Overview)

Halaman pusat untuk memantau kesehatan investasi Anda.

- **Total Balance:** Menampilkan total nilai seluruh aset Anda dalam Rupiah (IDR) berdasarkan harga pasar saat ini.
- **Allocation Chart (Pie Chart):**
  - Visualisasi persentase komposisi portofolio Anda (misal: 60% BTC, 40% ETH).
  - Persentase dihitung berdasarkan **Nilai Valuasi Pasar Saat Ini**, bukan berdasarkan modal beli.
- **Holdings Table (Tabel Aset):**
  - Daftar ringkas aset yang dimiliki.
  - **Live Price:** Harga terupdate dari CoinGecko.
  - **PnL (Profit/Loss):** Indikator warna (Hijau/Merah) untuk menunjukkan keuntungan atau kerugian yang belum direalisasikan (*Unrealized*).
  - **Data Alloc:** Bar progres mini yang menunjukkan dominasi aset tersebut.

---

### 3. Form Input Transaksi (Add Asset)

Formulir cerdas untuk mencatat pembelian atau penjualan.

- **Smart Ticker Search:**
  - Ketik nama koin (misal "Bit"), sistem akan memberikan rekomendasi (BTC - Bitcoin).
  - Menggunakan elemen `<datalist>` HTML5 yang didukung data dari backend.
- **Auto-Fetch Price:**
  - Saat Anda memilih Ticker yang valid, sistem otomatis mengambil harga pasar saat ini (IDR) dari API.
  - Anda tetap bisa mengedit harga tersebut secara manual jika harga beli Anda berbeda.
- **Fee (Biaya):** Kolom opsional untuk mencatat biaya transaksi (gas fee/broker fee) agar perhitungan profit lebih akurat.

---

### 4. Halaman Market (Live Chart)

- **Fungsi:** Alat analisis teknikal.
- **Fitur:** Mengintegrasikan **TradingView Advanced Chart Widget**. Anda bisa melihat grafik candlestick, menambahkan indikator (RSI, MACD), dan mengganti timeframe tanpa meninggalkan aplikasi.

---

### 5. Transaction History (Riwayat Transaksi)

Buku besar yang mencatat setiap aktivitas Anda.

- **Tabel Lengkap:** Menampilkan tanggal, tipe (Buy/Sell), harga, jumlah, fee, dan total.
- **Fitur Filter:** Filter transaksi berdasarkan rentang tanggal (Start Date - End Date).
- **Export Data:** Tombol untuk mengunduh laporan transaksi dalam format **CSV** (kompatibel dengan Microsoft Excel/Google Sheets).
- **Edit/Delete:** Ikon pensil dan tempat sampah untuk mengoreksi data jika terjadi kesalahan input.

---

## Penjelasan Teknis (Under the Hood)

### Integrasi API & Caching

Aplikasi ini menggunakan **CoinGecko Public API** untuk data harga.

- **Masalah Umum:** API gratis memiliki batas kecepatan (*Rate Limit*).
- **Solusi Kami:** Kami menerapkan **IMemoryCache** di backend (`CoinGeckoPriceService.cs`).
- Harga aset disimpan di memori server selama **10 menit**.
- Jika Anda me-refresh halaman, aplikasi mengambil data dari cache (instan & stabil), tidak memanggil API berulang kali.
- Ini mencegah data "berkedip" atau muncul error "API Busy".

---

### Clean Architecture Structure

- **Domain:** Berisi Entity (`Transaction`) dan Interface (`IPriceService`). Tidak memiliki ketergantungan ke luar.
- **Infrastructure:** Implementasi teknis. Berisi `ApplicationDbContext` (Database) dan `CoinGeckoPriceService` (Logika API & Cache).
- **Web:** Layer presentasi. Berisi Controller, View (Razor), dan konfigurasi awal (`Program.cs`).

---


## Update Fitur: Autentikasi dan Autorisasi Pengguna

Pembaruan ini mengintegrasikan ASP.NET Core Identity untuk mengamankan aplikasi. Pengguna harus melakukan registrasi dan login terlebih dahulu untuk mengakses halaman manajemen portofolio.

### Ringkasan Konsep

- Autentikasi (authentication) memastikan siapa pengguna yang sedang mengakses aplikasi (misalnya melalui login).
- Autorisasi (authorization) menentukan apa yang boleh dilakukan pengguna setelah terautentikasi (misalnya hanya pengguna login yang dapat mengakses halaman portofolio).

Pada implementasi ini, autentikasi menggunakan mekanisme cookie authentication bawaan ASP.NET Core Identity. Setelah login berhasil, server menerbitkan cookie autentikasi. Setiap request berikutnya membawa cookie tersebut untuk membuktikan sesi pengguna.

### Teknologi dan Library yang Digunakan

- ASP.NET Core Identity: framework untuk manajemen akun, login, register, dan keamanan pengguna.
- Entity Framework Core + IdentityDbContext: penyimpanan data pengguna dan otorisasi ke database.
- PostgreSQL: penyimpanan tabel-tabel Identity (AspNetUsers, AspNetRoles, AspNetUserRoles, AspNetUserClaims, dan sebagainya).
- Tailwind CSS (UI): perapihan tampilan halaman Login dan Register.

### Detail Implementasi Autentikasi

Bagian ini menjelaskan alur autentikasi yang umum pada ASP.NET Core Identity dan yang biasanya diimplementasikan pada controller Account.

#### 1. Registrasi (Register)

Alur registrasi pada umumnya:

1) Pengguna membuka halaman Register dan mengisi email serta password.
2) Data divalidasi melalui ViewModel (misalnya menggunakan DataAnnotations seperti Required, EmailAddress, StringLength, Compare).
3) Aplikasi membuat akun baru menggunakan UserManager.
4) Jika pembuatan akun berhasil, aplikasi dapat langsung melakukan sign-in (opsional) atau mengarahkan pengguna untuk login.

Komponen yang terlibat:

- RegisterViewModel: memegang field input dan aturan validasi.
- UserManager<IdentityUser>: membuat dan menyimpan user (password akan di-hash secara aman).

#### 2. Login

Alur login pada umumnya:

1) Pengguna membuka halaman Login dan mengisi kredensial.
2) Data divalidasi melalui LoginViewModel.
3) Aplikasi memverifikasi kredensial menggunakan SignInManager.
4) Jika sukses, aplikasi menerbitkan cookie autentikasi lalu mengarahkan ke halaman yang dituju.

Komponen yang terlibat:

- LoginViewModel: memegang field input login.
- SignInManager<IdentityUser>: melakukan verifikasi password dan mengelola sesi (cookie).
- Konfigurasi cookie: mengatur halaman redirect login, durasi cookie, dan perilaku akses tidak sah.

Catatan tentang keamanan:

- Identity menyimpan password dalam bentuk hash (bukan plaintext).
- Fitur lockout dapat diaktifkan agar akun terkunci sementara setelah beberapa kali gagal login.

#### 3. Logout

Logout umumnya dilakukan dengan:

- SignInManager.SignOutAsync() untuk menghapus cookie autentikasi.

Praktik yang disarankan:

- Melakukan logout melalui metode POST dan menambahkan antiforgery token untuk mencegah CSRF.

### Detail Implementasi Autorisasi

Autorisasi diterapkan untuk memastikan hanya pengguna yang sudah login yang dapat mengakses fitur tertentu.

#### 1. Proteksi Halaman dengan Atribut Authorize

Contoh penerapan:

- Menambahkan atribut [Authorize] pada controller atau action yang mengelola transaksi atau portofolio.

Jika ada halaman yang harus tetap publik:

- Tambahkan [AllowAnonymous] pada action tertentu (misalnya halaman Home atau halaman Login/Register).

#### 2. Redirect Otomatis ke Halaman Login

Saat pengguna mengakses halaman yang diproteksi tanpa login, aplikasi akan mengarahkan ke halaman Login. Perilaku ini biasanya dikonfigurasi pada cookie options (LoginPath) atau konfigurasi Identity.

### Konfigurasi Teknis yang Umum di Program.cs

Secara umum, konfigurasi yang diperlukan mencakup:

1) Registrasi DbContext yang mengarah ke PostgreSQL.
2) Registrasi Identity dan penyimpanan data Identity ke DbContext.
3) Mengaktifkan middleware authentication dan authorization.

Contoh struktur konfigurasi (disesuaikan dengan implementasi Anda):

- services.AddDbContext<ApplicationDbContext>(...);
- services.AddIdentity<IdentityUser, IdentityRole>(options => { ... })
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

- app.UseAuthentication();
- app.UseAuthorization();

Opsi yang sering diatur pada Identity:

- Password policy: panjang minimal, kombinasi huruf besar, huruf kecil, angka, dan karakter non-alphanumeric.
- Lockout: jumlah percobaan login gagal sebelum terkunci sementara.
- User: require unique email, dan sebagainya.

Opsi yang sering diatur pada cookie:

- LoginPath: lokasi endpoint login (misalnya /Account/Login).
- AccessDeniedPath: lokasi endpoint ketika akses ditolak.
- ExpireTimeSpan dan SlidingExpiration: durasi sesi.

### Daftar Perubahan File (Modifikasi Utama)

Berikut adalah komponen utama yang ditambahkan atau dimodifikasi dalam pembaruan ini:

#### 1. Layer Web (MyCryptoPortfolio.Web)

- Controllers
  - AccountController.cs (baru): menangani logika Login, Register, dan Logout.
  - TransactionsController.cs: ditambahkan atribut [Authorize] sehingga hanya pengguna yang sudah login yang dapat mengaksesnya.
- ViewModels
  - LoginViewModel.cs: model untuk data input login.
  - RegisterViewModel.cs: model untuk validasi input pendaftaran akun.
- Views
  - Views/Account/Login.cshtml: tampilan halaman login.
  - Views/Account/Register.cshtml: tampilan halaman pendaftaran pengguna.
  - Views/Shared/_Layout.cshtml: navigasi diperbarui untuk menampilkan tombol Login/Register atau Logout sesuai status pengguna.
- Konfigurasi
  - Program.cs: registrasi service Identity dan middleware Authentication/Authorization.

#### 2. Layer Infrastructure (MyCryptoPortfolio.Infrastructure)

- Database Context
  - ApplicationDbContext.cs: diubah agar mewarisi IdentityDbContext untuk mendukung tabel pengguna.

### Panduan Instalasi dan Eksekusi Pembaruan

Jika Anda baru meng-clone pembaruan ini, lakukan langkah berikut agar database sinkron dengan fitur autentikasi.

#### Langkah 1: Install Paket NuGet (jika belum)

Jalankan perintah ini di dalam folder MyCryptoPortfolio.Web:

```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 9.0.0
```

Catatan: jika paket sudah tercantum di file project (.csproj), Anda dapat melewati langkah ini.

#### Langkah 2: Migrasi Database

Karena ada penambahan tabel Identity (User dan Role), Anda perlu memastikan migrasi sudah dibuat dan diterapkan.

Pastikan Anda berada di terminal folder MyCryptoPortfolio.Web, lalu jalankan:

1) Membuat file migrasi (jika belum ada di repo)

```bash
dotnet ef migrations add AddIdentitySchema --project ../MyCryptoPortfolio.Infrastructure --startup-project .
```

2) Menerapkan ke database (update database)

```bash
dotnet ef database update --project ../MyCryptoPortfolio.Infrastructure --startup-project .
```

Catatan:
- Parameter --project mengarah ke project yang berisi DbContext (Infrastructure), sedangkan --startup-project mengarah ke Web.
- Untuk Windows PowerShell, Anda juga bisa memakai path ..\MyCryptoPortfolio.Infrastructure.

#### Langkah 3: Jalankan Aplikasi

Setelah database terupdate, jalankan aplikasi seperti biasa:

```bash
dotnet run
```

### Alur Penggunaan

1. Register: buka menu Register, masukkan email dan password. Akun akan dibuat dan disimpan pada tabel AspNetUsers.
2. Login: gunakan akun yang baru dibuat untuk masuk. Jika berhasil, menu Portfolio dapat diakses.
3. Portfolio (secured): halaman portofolio terkunci. Jika mengakses tanpa login melalui URL, sistem mengalihkan ke halaman Login.
4. Logout: klik tombol Logout di navbar untuk mengakhiri sesi.

### Catatan Pengembangan Lanjutan (Opsional)

Jika Anda ingin memperketat otorisasi atau memisahkan data per pengguna, beberapa pengembangan yang umum dilakukan:

- Role-based authorization:
  - Tambahkan role (misalnya Admin, User) dan gunakan [Authorize(Roles = "Admin")] pada halaman admin.
- Policy-based authorization:
  - Definisikan policy (misalnya OnlyVerifiedUser) lalu gunakan [Authorize(Policy = "OnlyVerifiedUser")].
- Kepemilikan data (data ownership):
  - Tambahkan kolom OwnerUserId pada entitas portofolio/transaksi, lalu filter query berdasarkan User.Identity agar setiap pengguna hanya melihat datanya sendiri.
## Troubleshooting (Masalah Umum)

**Q: Saya mengetik Ticker koin tapi harganya tidak muncul otomatis/Error?**
**A:** Pastikan koin tersebut sudah didaftarkan di dalam file `CoinGeckoPriceService.cs` (Dictionary `SupportedCoins`). API CoinGecko membutuhkan ID spesifik (misal: `"SHIB"` -> `"shiba-inu"`). Jika belum ada, Anda bisa menambahkannya manual di file tersebut.

**Q: Total Balance saya tiba-tiba 0 atau grafik alokasi hilang?**
**A:** Ini biasanya terjadi jika API CoinGecko sedang *down* atau IP Anda terkena *rate limit* sesaat sebelum cache aktif. Tunggu 1–2 menit dan refresh halaman. Sistem cache akan menstabilkan ini.

**Q: Tabel "Your Assets" terpotong di HP?**
**A:** Tabel sudah didesain *responsive* dengan fitur *scroll horizontal*. Cukup geser tabel ke samping untuk melihat kolom PnL atau Value.

---

**Dibuat oleh Yizreel Schwartz Sipahutar sebagai prasayarat penyelesaian magang-4**
