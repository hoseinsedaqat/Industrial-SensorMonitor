# Industrial Sensor Monitor

## سامانه ثبت و مدیریت اطلاعات سنسورهای صنعتی

نرم‌افزاری دسکتاپ برای ویندوز که با **C# / Windows Forms / ADO.NET** نوشته شده و اطلاعات سنسورهای صنعتی را در **Microsoft SQL Server** (LocalDB یا Express) ذخیره، مدیریت و نمایش می‌دهد.

> **نسخه فعلی:** `1.0.0`
> **پلتفرم:** Windows 10 / 11 — 64 bit
> **فریم‌ورک:** .NET 10 (`net10.0-windows`)

---

## فهرست مطالب

1. [معرفی نرم‌افزار](#۱-معرفی-نرم‌افزار)
2. [امکانات اصلی برنامه](#۲-امکانات-اصلی-برنامه)
3. [تکنولوژی‌های استفاده‌شده](#۳-تکنولوژی‌های-استفاده‌شده)
4. [پیش‌نیازهای نصب](#۴-پیش‌نیازهای-نصب)
5. [نحوه نصب SQL Server LocalDB](#۵-نحوه-نصب-sql-server-localdb)
6. [نحوه نصب SQL Server Express](#۶-نحوه-نصب-sql-server-express)
7. [نحوه ایجاد دیتابیس](#۷-نحوه-ایجاد-دیتابیس)
8. [نحوه اجرای InitDatabase.sql](#۸-نحوه-اجرای-initdatabasesql)
9. [نحوه تغییر Connection String](#۹-نحوه-تغییر-connection-string)
10. [نحوه باز کردن پروژه در Visual Studio](#۱۰-نحوه-باز-کردن-پروژه-در-visual-studio)
11. [نحوه نصب پکیج‌های NuGet](#۱۱-نحوه-نصب-پکیجهای-nuget)
12. [نحوه Build کردن پروژه](#۱۲-نحوه-build-کردن-پروژه)
13. [نحوه انتشار فایل EXE](#۱۳-نحوه-انتشار-فایل-exe)
14. [نحوه ساخت Setup.exe](#۱۴-نحوه-ساخت-setupexe)
15. [نحوه اجرای GitHub Actions](#۱۵-نحوه-اجرای-github-actions)
16. [نحوه دانلود فایل‌ها از Actions](#۱۶-نحوه-دانلود-فایلها-از-actions)
17. [نحوه دانلود نسخه رسمی از Releases](#۱۷-نحوه-دانلود-نسخه-رسمی-از-releases)
18. [نحوه نصب و حذف نرم‌افزار](#۱۸-نحوه-نصب-و-حذف-نرمافزار)
19. [ساختار پوشه‌های پروژه](#۱۹-ساختار-پوشههای-پروژه)
20. [نسخه فعلی نرم‌افزار](#۲۰-نسخه-فعلی-نرمافزار)
21. [وضعیت ساخت و بررسی نسخه ۱٫۰٫۰](#وضعیت-ساخت-و-بررسی-نسخه-۱٫۰٫۰)

---

## ۱. معرفی نرم‌افزار

**Industrial Sensor Monitor** یک برنامه ویندوزی سبک و کاربردی برای ثبت و مشاهده اطلاعات سنسورهای صنعتی است.

کاربر نوع سنسور را از فهرست انتخاب می‌کند، مقدار اندازه‌گیری‌شده را وارد می‌کند و واحد اندازه‌گیری را (که به‌صورت خودکار پیشنهاد می‌شود) تأیید یا تغییر می‌دهد. با زدن دکمه «ثبت اطلاعات»:

- اطلاعات به‌صورت **پارامتری** (بدون خطر SQL Injection) در جدول `dbo.SensorData` ذخیره می‌شود.
- تاریخ و ساعت ثبت **توسط خود SQL Server** با تابع `SYSDATETIME()` تعیین می‌شود (نه توسط ساعت ویندوز کاربر).
- جدول اطلاعات بلافاصله به‌روزرسانی می‌شود و جدیدترین رکورد در بالاترین ردیف قرار می‌گیرد.

اطلاعات در SQL Server ذخیره می‌شوند و با بستن یا اجرای مجدد برنامه **از بین نمی‌روند**.

> برای استفاده معمولی از نرم‌افزار، **نیازی به Visual Studio نیست**. کافی است پیش‌نیاز دیتابیس (SQL Server LocalDB یا Express) را نصب کنید و فایل نصب یا نسخه Portable را از بخش **Releases** مخزن دانلود کنید.

---

## ۲. امکانات اصلی برنامه

- انتخاب نوع سنسور از ComboBox (دما، فشار، رطوبت، سرعت)
- نمایش خودکار واحد پیش‌فرض هر سنسور و امکان ویرایش آن
- ورود مقدار عددی صحیح یا اعشاری با **چهار رقم اعشار**، پشتیبانی از مقادیر منفی و مقدار صفر
- ذخیره مقدار به‌صورت `decimal` در C# و `DECIMAL(18,4)` در SQL Server (بدون تبدیل به `double`/`float`)
- ثبت خودکار تاریخ و ساعت توسط `SYSDATETIME()` در ستون `DATETIME2(7)`
- نمایش اطلاعات در `DataGridView` به‌صورت فقط‌خواندنی، راست‌چین و فارسی
- مرتب‌سازی نزولی بر اساس تاریخ ثبت (جدیدترین رکورد در ابتدا)
- پاک‌سازی خودکار ورودی‌ها پس از ثبت موفق و آماده‌سازی فرم برای ثبت بعدی
- اعتبارسنجی کامل ورودی‌ها پیش از ارسال به دیتابیس
- نمایش وضعیت اتصال به دیتابیس در سربرگ فرم
- ایجاد خودکار دیتابیس و جدول در اولین اجرا (در صورت وجود دسترسی)
- مدیریت خطاهای رایج SQL Server با پیام‌های فارسی و ثبت جزئیات فنی در فایل گزارش
- رابط کاربری فارسی، راست‌چین و مقیاس‌پذیر در رزولوشن‌های مختلف ویندوز
- نسخه اجرایی مستقل (Self-contained Single File) بدون نیاز به نصب .NET Runtime
- بسته نصبی ویندوز (Inno Setup) با میانبر Desktop و Start Menu و Uninstall استاندارد
- نسخه قابل حمل (Portable ZIP)
- ساخت خودکار نسخه‌ها با GitHub Actions و انتشار در GitHub Releases

---

## ۳. تکنولوژی‌های استفاده‌شده

| بخش | تکنولوژی |
|---|---|
| زبان برنامه‌نویسی | C# |
| رابط کاربری | Windows Forms |
| پلتفرم | Windows Desktop |
| فریم‌ورک | .NET 10 (`net10.0-windows`) |
| دیتابیس | Microsoft SQL Server (LocalDB / Express) |
| ارتباط با دیتابیس | ADO.NET (`SqlConnection` / `SqlCommand` / `SqlDataReader` / `SqlParameter`) |
| کتابخانه اتصال | `Microsoft.Data.SqlClient` |
| خواندن تنظیمات | `Microsoft.Extensions.Configuration` + `Microsoft.Extensions.Configuration.Json` |
| ساخت Installer | Inno Setup 6 |
| Build و انتشار | .NET CLI + GitHub Actions |

**پکیج‌های NuGet (نسخه‌ها در فایل csproj مشخص شده‌اند):**

```xml
<PackageReference Include="Microsoft.Data.SqlClient" Version="6.1.1" />
<PackageReference Include="Microsoft.Extensions.Configuration" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="10.0.0" />
```

از **Entity Framework** یا هیچ ORM دیگری استفاده نشده است. هیچ کتابخانه گرافیکی شخص ثالثی نیز به کار نرفته و تمام کنترل‌ها استاندارد Windows Forms هستند.

---

## ۴. پیش‌نیازهای نصب

برنامه برای اجرا به موارد زیر نیاز دارد:

| پیش‌نیاز | توضیح |
|---|---|
| Windows 10 یا 11 (64 bit) | سیستم‌عامل مقصد |
| Microsoft SQL Server Express LocalDB **یا** SQL Server Express | ذخیره‌سازی اطلاعات — **همراه برنامه نصب نمی‌شود** |
| .NET 10 Runtime | **لازم نیست** — فایل EXE از نوع Self-contained است |

> **مهم:** Self-contained بودن فایل EXE فقط به معنی همراه بودن .NET Runtime است. **SQL Server / LocalDB همراه برنامه نیست** و باید به‌صورت جداگانه نصب شود.

برای Build کردن پروژه از سورس:

| ابزار | توضیح |
|---|---|
| .NET SDK 10 | برای `dotnet build` و `dotnet publish` |
| Visual Studio 2022 (نسخه سازگار با .NET 10) یا Visual Studio Code | برای توسعه |
| Inno Setup 6 | فقط برای ساخت فایل نصب |

---

## ۵. نحوه نصب SQL Server LocalDB

LocalDB سبک‌ترین گزینه و مناسب استفادهٔ تک‌کاربره است.

**روش اول — نصب از طریق SQL Server Express Installer:**

1. نصب‌کننده SQL Server Express را از سایت رسمی مایکروسافت دانلود کنید.
2. در مرحله انتخاب امکانات (Feature Selection)، گزینه **LocalDB** را علامت بزنید.
3. نصب را کامل کنید.

**روش دوم — نصب فقط LocalDB:**

1. فایل `SqlLocalDB.msi` را از صفحه دانلود SQL Server Express دانلود کنید.
2. آن را اجرا و نصب را کامل کنید.

**بررسی صحیح بودن نصب:** در Command Prompt دستور زیر را اجرا کنید:

```text
sqllocaldb info
```

باید حداقل خروجی `MSSQLLocalDB` را ببینید. برای فعال کردن نمونه:

```text
sqllocaldb start MSSQLLocalDB
```

رشته اتصال پیش‌فرض برنامه از همین نمونه استفاده می‌کند:

```text
Server=(localdb)\MSSQLLocalDB;Database=IndustrialSensorDb;Trusted_Connection=True;TrustServerCertificate=True;
```

---

## ۶. نحوه نصب SQL Server Express

اگر چند کاربر باید به دیتابیس دسترسی داشته باشند یا می‌خواهید از SQL Server Management Studio به‌صورت سروری استفاده کنید:

1. نصب‌کننده **SQL Server Express** را از سایت رسمی مایکروسافت دانلود و اجرا کنید.
2. نوع نصب را **Basic** یا **Custom** انتخاب کنید.
3. پس از نصب، در **SQL Server Configuration Manager** سرویس `SQL Server (SQLEXPRESS)` را بررسی کنید؛ باید در حال اجرا باشد.
4. رشته اتصال برنامه را در `appsettings.json` به شکل زیر تغییر دهید:

```text
Server=.\SQLEXPRESS;Database=IndustrialSensorDb;Trusted_Connection=True;TrustServerCertificate=True;
```

**نکته:** برای ساختن دیتابیس با SQL Server Express معمولاً کاربر ویندوز باید در نقش `sysadmin` یا حداقل دارای دسترسی `dbcreator` باشد. اگر برنامه نتوانست دیتابیس را خودکار بسازد، اسکریپت `Database/InitDatabase.sql` را با کاربر دارای دسترسی اجرا کنید.

---

## ۷. نحوه ایجاد دیتابیس

نام دیتابیس: `IndustrialSensorDb`
نام جدول: `dbo.SensorData`

ساختار جدول:

| ستون | نوع داده | ویژگی |
|---|---|---|
| `Id` | `INT` | `PRIMARY KEY` + `IDENTITY(1,1)` |
| `SensorName` | `NVARCHAR(100)` | `NOT NULL` — نام سنسور |
| `Value` | `DECIMAL(18,4)` | `NOT NULL` — مقدار سنسور |
| `Unit` | `NVARCHAR(30)` | `NOT NULL` — واحد اندازه‌گیری |
| `RecordDate` | `DATETIME2(7)` | `NOT NULL` — پیش‌فرض `SYSDATETIME()` |

برای مرتب‌سازی سریع بر اساس زمان، ایندکس زیر ساخته می‌شود:

```text
IX_SensorData_RecordDate ON dbo.SensorData ([RecordDate] DESC)
```

**دو روش برای ایجاد دیتابیس وجود دارد:**

**روش اول (خودکار):** کافی است برنامه را اجرا کنید. برنامه ابتدا اتصال به SQL Server را بررسی می‌کند؛ اگر سرور در دسترس باشد ولی دیتابیس یا جدول وجود نداشته باشد، آن‌ها را می‌سازد. اگر به دلیل محدودیت دسترسی، ایجاد خودکار ممکن نباشد، پیام مناسب نمایش داده می‌شود و شما به اجرای اسکریپت SQL راهنمایی می‌شوید.

**روش دوم (دستی و مطمئن):** اجرای فایل `Database/InitDatabase.sql` در SQL Server Management Studio (بخش بعدی).

هیچ‌کدام از این دو روش دیتابیس یا اطلاعات موجود را حذف نمی‌کنند.

---

## ۸. نحوه اجرای InitDatabase.sql

1. **SQL Server Management Studio (SSMS)** را باز کنید (نسخه Express هم کافی است و رایگان دانلود می‌شود).
2. در پنجره Connect، به نمونه دیتابیس متصل شوید:
   - برای LocalDB: در قسمت Server name مقدار `(localdb)\MSSQLLocalDB` را وارد کنید و Authentication را روی **Windows Authentication** بگذارید.
   - برای Express: `.\SQLEXPRESS` را وارد کنید.
3. از منوی **File → Open → File** فایل `Database/InitDatabase.sql` را باز کنید.
4. دکمه **Execute** (یا کلید `F5`) را بزنید.
5. در پنجره Messages باید پیام موفقیت و ساختار جدول را ببینید.

اسکریپت **تکرارپذیر** است: اگر دیتابیس و جدول از قبل وجود داشته باشند، هیچ خطایی رخ نمی‌دهد و هیچ داده‌ای حذف یا بازنویسی نمی‌شود. در این اسکریپت از `DROP TABLE` و `DROP DATABASE` استفاده نشده است.

> **درباره زمان ثبت:** مقدار `RecordDate` با تابع `SYSDATETIME()` و بر اساس **ساعت محلی سرور دیتابیس** تعیین می‌شود؛ یعنی زمان ثبت به تنظیمات ساعت سیستم‌عاملی بستگی دارد که SQL Server روی آن اجرا می‌شود، نه ساعت ویندوز کاربر (در حالت LocalDB این دو معمولاً یکی هستند).

---

## ۹. نحوه تغییر Connection String

رشته اتصال در فایل `appsettings.json` و در کنار فایل اجرایی برنامه قرار دارد:

```json
{
  "ConnectionStrings": {
    "SensorDb": "Server=(localdb)\\MSSQLLocalDB;Database=IndustrialSensorDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**نمونه‌های آماده:**

```text
# SQL Server LocalDB
Server=(localdb)\MSSQLLocalDB;Database=IndustrialSensorDb;Trusted_Connection=True;TrustServerCertificate=True;

# SQL Server Express
Server=.\SQLEXPRESS;Database=IndustrialSensorDb;Trusted_Connection=True;TrustServerCertificate=True;

# SQL Server کامل با نام نمونه دلخواه
Server=MY-PC\SQLEXPRESS2019;Database=IndustrialSensorDb;Trusted_Connection=True;TrustServerCertificate=True;

# اتصال با نام کاربری و رمز عبور (SQL Authentication)
Server=.\SQLEXPRESS;Database=IndustrialSensorDb;User Id=sa;Password=***;TrustServerCertificate=True;
```

**نکات:**

- مسیر فایل تنظیمات:
  - نسخه نصبی: `C:\Program Files\IndustrialSensorMonitor\appsettings.json`
  - نسخه Portable: کنار فایل `IndustrialSensorMonitor.exe`
- پس از ذخیره فایل، برنامه را مجدداً اجرا کنید (تنظیمات در زمان اجرا خوانده می‌شود).
- اگر فایل `appsettings.json` وجود نداشته باشد یا مقدار `SensorDb` خالی باشد، برنامه از رشته اتصال پیش‌فرض LocalDB استفاده می‌کند تا اجرای فایل EXE مستقل نیز امکان‌پذیر باشد.
- اگر فایل تنظیمات ساختار JSON معتبری نداشته باشد، برنامه پیام هشدار فارسی نمایش می‌دهد و با مقدار پیش‌فرض اجرا می‌شود (بسته نمی‌شود).
- **رمز عبور یا اطلاعات محرمانه در سورس کد ذخیره نشده است.** در صورت استفاده از SQL Authentication، رمز را فقط در همین فایل تنظیمات قرار دهید.

---

## ۱۰. نحوه باز کردن پروژه در Visual Studio

1. Visual Studio سازگار با **.NET 10** را نصب کنید (workload **.NET Desktop Development** لازم است).
2. فایل `IndustrialSensorMonitor.sln` را از ریشه مخزن باز کنید.
3. Visual Studio دو پروژه را نشان می‌دهد:
   - `IndustrialSensorMonitor` (پروژه اصلی Windows Forms)
   - `IndustrialSensorMonitor.Tests` (پروژه تست با xUnit)
4. پروژه `IndustrialSensorMonitor` را به‌عنوان Startup Project تنظیم کنید.
5. کلید `F5` را بزنید؛ فرم اصلی برنامه باز می‌شود.

پکیج‌های NuGet در اولین Build به‌صورت خودکار Restore می‌شوند.

---

## ۱۱. نحوه نصب پکیج‌های NuGet

نسخه پکیج‌ها در فایل `src/IndustrialSensorMonitor/IndustrialSensorMonitor.csproj` ثابت شده است، بنابراین برای نصب/بازیابی کافی است یکی از دستورهای زیر را اجرا کنید:

```powershell
dotnet restore
```

یا در Visual Studio:

- راست‌کلیک روی Solution → **Restore NuGet Packages**، یا
- از منوی **Tools → NuGet Package Manager → Package Manager Console** و اجرای `Update-Package -Reinstall` در صورت نیاز.

برای افزودن دستی یک پکیج:

```powershell
dotnet add src/IndustrialSensorMonitor/IndustrialSensorMonitor.csproj package Microsoft.Data.SqlClient --version 6.1.1
```

---

## ۱۲. نحوه Build کردن پروژه

از ریشه مخزن:

```powershell
# بازیابی پکیج‌ها
dotnet restore IndustrialSensorMonitor.sln

# ساخت نسخه Debug
dotnet build IndustrialSensorMonitor.sln -c Debug

# ساخت نسخه Release
dotnet build IndustrialSensorMonitor.sln -c Release
```

اجرای تست‌ها:

```powershell
dotnet test tests/IndustrialSensorMonitor.Tests/IndustrialSensorMonitor.Tests.csproj -c Release
```

خروجی Build در مسیر `src/IndustrialSensorMonitor/bin/Release/net10.0-windows/` قرار می‌گیرد و برای اجرا باید فایل `appsettings.json` نیز کنار آن باشد (این فایل به‌صورت خودکار کپی می‌شود).

---

## ۱۳. نحوه انتشار فایل EXE

فایل اجرایی نهایی **Self-contained** و **Single File** است، بنابراین روی ویندوز مقصد به نصب .NET Runtime نیازی ندارد.

```powershell
dotnet publish src/IndustrialSensorMonitor/IndustrialSensorMonitor.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:PublishTrimmed=false `
  -o publish/win-x64
```

خروجی:

```text
publish/win-x64/
├── IndustrialSensorMonitor.exe      <- فایل اجرایی اصلی
└── appsettings.json                 <- فایل تنظیمات (قابل ویرایش)
```

همین تنظیمات در پروفایل آماده `src/IndustrialSensorMonitor/Properties/PublishProfiles/win-x64.pubxml` نیز ذخیره شده و از داخل Visual Studio با گزینه **Publish** قابل استفاده است.

---

## ۱۴. نحوه ساخت Setup.exe

پیش‌نیاز: نصب [Inno Setup 6](https://jrsoftware.org/isdl.php)

1. ابتدا فایل اجرایی را طبق بخش ۱۳ در مسیر `publish/win-x64` منتشر کنید.
2. کامپایلر Inno Setup را اجرا کنید:

```powershell
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer\Setup.iss /DMyAppVersion=1.0.0
```

3. خروجی در مسیر زیر ساخته می‌شود:

```text
dist/IndustrialSensorMonitor-Setup-x64.exe
```

**امکانات بسته نصبی:**

- نصب در مسیر استاندارد `C:\Program Files\IndustrialSensorMonitor` با امکان انتخاب مسیر دیگر
- ایجاد میانبر در **Desktop** و **Start Menu**
- نمایش نام برنامه، نسخه و نام ناشر
- نصب فایل `appsettings.json`، فایل `README.md` و `Database\InitDatabase.sql`
- امکان حذف کامل از طریق **Uninstall** و **Windows Settings → Apps**
- بررسی نبود LocalDB / SQL Express و نمایش هشدار راهنما (نصب را متوقف نمی‌کند)
- **حذف نرم‌افزار هیچ تأثیری روی دیتابیس ندارد**؛ جدول `SensorData` و اطلاعات ثبت‌شده باقی می‌مانند

---

## ۱۵. نحوه اجرای GitHub Actions

Workflow در مسیر `.github/workflows/build-release.yml` قرار دارد و با رویدادهای زیر اجرا می‌شود:

| رویداد | توضیح |
|---|---|
| `push` به شاخه `main` (و `test`) | ساخت خودکار نسخه در هر Push |
| `workflow_dispatch` | اجرای دستی از صفحه Actions |
| `push` تگ `v*` | ساخت نسخه + ایجاد GitHub Release |

**اجرای دستی:**

1. وارد مخزن در GitHub شوید.
2. تب **Actions** را باز کنید.
3. از ستون سمت چپ، workflow با نام **Build and Release** را انتخاب کنید.
4. دکمه **Run workflow** را بزنید، شاخه موردنظر را انتخاب و تأیید کنید.

**مراحل Workflow:**

1. آماده‌سازی Windows Runner و نصب .NET SDK 10
2. `dotnet restore` و `dotnet build` در حالت Release
3. اجرای تست‌های واحد و سپس تست‌های یکپارچه‌سازی دیتابیس (در صورت وجود LocalDB روی Runner) — در صورت شکست، انتشار متوقف می‌شود
4. `dotnet publish` با تنظیمات Self-contained + Single File (win-x64)
5. نصب Inno Setup و ساخت `IndustrialSensorMonitor-Setup-x64.exe`
6. ساخت `IndustrialSensorMonitor-Portable-x64.zip`
7. بارگذاری سه Artifact جداگانه
8. در صورت اجرا با Tag، ساخت GitHub Release و پیوست فایل‌ها

**انتشار نسخه جدید با Tag:**

```bash
git tag v1.0.0
git push origin v1.0.0
```

---

## ۱۶. نحوه دانلود فایل‌ها از Actions

1. وارد مخزن شوید و تب **Actions** را باز کنید.
2. روی آخرین اجرای موفق Workflow (علامت ✅) کلیک کنید.
3. در پایین صفحه، بخش **Artifacts** سه فایل قابل دانلود دارد:

| نام Artifact | فایل داخل آن |
|---|---|
| `IndustrialSensorMonitor-EXE` | `IndustrialSensorMonitor.exe` |
| `IndustrialSensorMonitor-Setup` | `IndustrialSensorMonitor-Setup-x64.exe` |
| `IndustrialSensorMonitor-Portable` | `IndustrialSensorMonitor-Portable-x64.zip` |

4. فایل ZIP مربوطه را دانلود و پس از استخراج، `Setup.exe` را اجرا کنید.

> **نکته:** Artifacts مدت نگهداری محدودی دارند (در این پروژه **۳۰ روز** تنظیم شده است) و جایگزین انتشار رسمی نسخه نیستند. برای نسخه پایدار از بخش **Releases** استفاده کنید.

---

## ۱۷. نحوه دانلود نسخه رسمی از Releases

1. وارد مخزن GitHub شوید.
2. از ستون سمت راست، بخش **Releases** را باز کنید (یا آدرس `.../releases/latest`).
3. آخرین نسخه (مثلاً `v1.0.0`) را انتخاب کنید.
4. یکی از فایل‌های زیر را دانلود کنید:

| فایل | توضیح |
|---|---|
| `IndustrialSensorMonitor-Setup-x64.exe` | **پیشنهادی** — بسته نصبی ویندوز |
| `IndustrialSensorMonitor.exe` | فایل اجرایی مستقل (بدون نصب) |
| `IndustrialSensorMonitor-Portable-x64.zip` | نسخه قابل حمل شامل EXE، تنظیمات، README و اسکریپت دیتابیس |

5. فایل `Setup-x64.exe` را اجرا کرده و مراحل نصب را کامل کنید.
6. پیش از اجرای برنامه، پیش‌نیاز SQL Server (بخش‌های ۵ و ۶) را فراهم کنید.
7. از میانبر Desktop یا Start Menu برنامه را اجرا کنید.

---

## ۱۸. نحوه نصب و حذف نرم‌افزار

**نصب:**

1. `IndustrialSensorMonitor-Setup-x64.exe` را اجرا کنید.
2. مسیر نصب را (در صورت نیاز) تغییر دهید و **Next** را بزنید.
3. در صورت نبود SQL Server LocalDB، پیام هشدار نمایش داده می‌شود؛ می‌توانید نصب را ادامه دهید و بعداً LocalDB را نصب کنید.
4. نصب را کامل کنید؛ میانبرها ساخته می‌شوند.
5. در اولین اجرا، اگر دیتابیس وجود نداشته باشد، برنامه تلاش می‌کند آن را بسازد؛ در غیر این صورت پیام راهنما نمایش داده می‌شود.

**نسخه Portable:**

فایل ZIP را در هر مسیری استخراج کنید (مثلاً `D:\Apps\IndustrialSensorMonitor`) و `IndustrialSensorMonitor.exe` را اجرا کنید. نیازی به نصب نیست، اما SQL Server همچنان پیش‌نیاز است.

**حذف:**

- از **Settings → Apps → Installed apps** گزینه `Industrial Sensor Monitor` را انتخاب و **Uninstall** کنید، یا
- از **Control Panel → Programs and Features** حذف کنید، یا
- فایل `unins000.exe` را در مسیر نصب اجرا کنید.

**در حین حذف:**

| مورد | نتیجه |
|---|---|
| فایل‌های اجرایی برنامه | حذف می‌شوند |
| میانبرهای Desktop و Start Menu | حذف می‌شوند |
| دیتابیس `IndustrialSensorDb` و جدول `SensorData` | **باقی می‌مانند** |
| اطلاعات ثبت‌شده کاربر | **باقی می‌مانند** |

اگر می‌خواهید اطلاعات را هم پاک کنید، به‌صورت آگاهانه و با دستور زیر در SSMS:

```sql
DROP DATABASE [IndustrialSensorDb];
```

> این دستور در هیچ‌کدام از اسکریپت‌ها و فرآیند نصب/حذف به‌صورت خودکار اجرا نمی‌شود.

---

## ۱۹. ساختار پوشه‌های پروژه

```text
IndustrialSensorMonitor/
│
├── .github/
│   └── workflows/
│       └── build-release.yml          # Build و انتشار خودکار + ساخت Release
│
├── IndustrialSensorMonitor.sln        # فایل Solution
│
├── src/
│   └── IndustrialSensorMonitor/
│       ├── Program.cs                 # نقطه شروع برنامه و خواندن appsettings.json
│       ├── MainForm.cs                # منطق رابط کاربری و اعتبارسنجی
│       ├── MainForm.Designer.cs       # طراحی فرم (کنترل‌ها، چیدمان، رنگ‌ها)
│       ├── MainForm.resx              # منابع فرم
│       ├── appsettings.json           # رشته اتصال دیتابیس
│       ├── IndustrialSensorMonitor.csproj
│       ├── Data/
│       │   ├── DatabaseConnection.cs          # مدیریت Connection String
│       │   ├── DatabaseConfigurationException.cs
│       │   ├── DatabaseStatusReport.cs        # وضعیت اتصال/دیتابیس/جدول
│       │   ├── RepositoryResult.cs            # نتیجه عملیات + پیام فارسی
│       │   └── SensorRepository.cs            # تمام عملیات دیتابیس (ADO.NET)
│       ├── Diagnostics/
│       │   └── AppLog.cs                      # ثبت گزارش خطا
│       ├── Models/
│       │   ├── SensorData.cs                  # مدل رکورد سنسور
│       │   └── SensorCatalog.cs               # فهرست سنسورها و واحدهای پیش‌فرض
│       ├── Validation/
│       │   ├── SensorInputValidator.cs        # اعتبارسنجی ورودی‌ها
│       │   └── ValidationResult.cs
│       └── Properties/
│           └── PublishProfiles/
│               └── win-x64.pubxml             # پروفایل انتشار Self-contained
│
├── Database/
│   └── InitDatabase.sql               # اسکریپت ایجاد دیتابیس، جدول و ایندکس
│
├── installer/
│   └── Setup.iss                      # اسکریپت Inno Setup
│
├── tests/
│   └── IndustrialSensorMonitor.Tests/
│       ├── IndustrialSensorMonitor.Tests.csproj
│       ├── SensorInputValidatorTests.cs
│       ├── SensorCatalogTests.cs
│       ├── DatabaseConnectionTests.cs
│       └── SensorRepositoryIntegrationTests.cs   # تست واقعی با SQL Server LocalDB
│
├── dist/                              # خروجی‌های نهایی (توسط GitHub Actions ساخته می‌شود)
├── README.md
└── .gitignore
```

**خروجی‌های Build در Git نگهداری نمی‌شوند:** پوشه‌های `bin`، `obj`، `publish`، `portable` و فایل‌های `dist` در `.gitignore` قرار دارند و تولید فایل‌های اجرایی توسط GitHub Actions انجام می‌شود.

---

## ۲۰. نسخه فعلی نرم‌افزار

| مورد | مقدار |
|---|---|
| نسخه | `1.0.0` |
| الگوی نسخه‌بندی | `Major.Minor.Patch` |
| محل تعریف نسخه | `src/IndustrialSensorMonitor/IndustrialSensorMonitor.csproj` و `installer/Setup.iss` |

برای انتشار نسخه جدید:

1. مقدار `<Version>` را در فایل csproj و مقدار پیش‌فرض `MyAppVersion` در `installer/Setup.iss` به‌روزرسانی کنید.
2. تغییرات را Commit و Push کنید.
3. Tag جدید بسازید و Push کنید:

```bash
git tag v1.0.1
git push origin v1.0.1
```

GitHub Actions به‌صورت خودکار نسخه را با شماره Tag می‌سازد و Release را منتشر می‌کند.

---

## وضعیت ساخت و بررسی نسخه ۱٫۰٫۰

این پروژه با **GitHub Actions** روی Runner ویندوزی (`windows-latest`) به‌صورت واقعی ساخته و بسته‌بندی شده است. نتیجه آخرین اجرای موفق:

| مرحله | نتیجه | جزئیات |
|---|---|---|
| Restore پکیج‌های NuGet | ✅ موفق | `Microsoft.Data.SqlClient 6.1.1`، `Microsoft.Extensions.Configuration(.Json) 10.0.0` |
| Build در حالت Release | ✅ موفق | بدون خطای کامپایل |
| اجرای تست‌های واحد (xUnit) | ✅ موفق | اعتبارسنجی ورودی‌ها، فهرست سنسورها و رشته اتصال |
| تست یکپارچه‌سازی دیتابیس (SQL Server LocalDB واقعی) | ✅ موفق | اجرای دو‌باره `InitDatabase.sql`، ایجاد خودکار دیتابیس/جدول، درج و بازخوانی متن فارسی و مقدار اعشاری `123.4567` و مقدار منفی، بررسی مرتب‌سازی نزولی |
| انتشار Self-contained Single File (`win-x64`) | ✅ موفق | `IndustrialSensorMonitor.exe` — حدود ۵۰ مگابایت |
| ساخت فایل نصب با Inno Setup | ✅ موفق | `IndustrialSensorMonitor-Setup-x64.exe` — حدود ۳۷ مگابایت |
| ساخت نسخه قابل حمل | ✅ موفق | `IndustrialSensorMonitor-Portable-x64.zip` — حدود ۵۰ مگابایت |
| بارگذاری Artifacts | ✅ موفق | سه Artifact جداگانه در تب Actions |
| انتشار GitHub Release | ✅ موفق | با ایجاد Tag از الگوی `v*` |

**آنچه در CI واقعاً آزمایش شده است:** اسکریپت `Database/InitDatabase.sql` روی یک نمونه واقعی SQL Server Express LocalDB اجرا شده (دو بار پشت سر هم، برای اثبات تکرارپذیر بودن)، سپس دیتابیس و جدول به‌صورت خودکار ساخته شده و کلاس `SensorRepository` رکوردهایی شامل **متن فارسی** و **اعداد اعشاری چهاررقمی** را در دیتابیس درج و مجدداً خوانده است؛ صحت مقدار `123.4567`، مقدار منفی `-45.6789`، واحد اندازه‌گیری، شناسه و مرتب‌سازی نزولی تاریخ ثبت تأیید شده است.

**نکته صادقانه درباره تست‌ها:** فرآیند Build، تست خودکار و بسته‌بندی در محیط CI ویندوزی انجام شده است. اجرای تعاملی برنامه روی دسکتاپ ویندوز (باز کردن فرم، ثبت واقعی رکورد در LocalDB و بررسی ظاهر رابط کاربری) نیازمند یک سیستم ویندوزی با SQL Server LocalDB است؛ سناریوهای جدول «تست عملکرد» را می‌توانید روی همان سیستم با استفاده از فایل نصب یا نسخه Portable انجام دهید. دیتابیس برنامه در زمان اجرا، در صورت وجود دسترسی مناسب، به‌صورت خودکار ساخته می‌شود و در غیر این صورت پیام راهنما نمایش داده می‌شود.

---

## پیوست: گزارش خطاها و رفع اشکال

**محل فایل گزارش:** `%LOCALAPPDATA%\IndustrialSensorMonitor\Logs\app-YYYYMMDD.log`

**خطاهای رایج:**

| پیام / نشانه | علت | راه‌حل |
|---|---|---|
| «نمونه SQL Server با نام ... پیدا نشد» | LocalDB نصب نیست یا نام نمونه اشتباه است | بخش ۵ را انجام دهید و `sqllocaldb info` را بررسی کنید |
| «اتصال به سرور ... برقرار نشد» | سرویس SQL Server متوقف است | سرویس `SQL Server (SQLEXPRESS)` را استارت کنید |
| «ورود به SQL Server ناموفق بود» | رشته اتصال با SQL Authentication تنظیم شده | `Trusted_Connection=True` را اضافه کنید یا کاربر/رمز صحیح بدهید |
| «دیتابیس ... وجود ندارد» | دیتابیس ساخته نشده و ایجاد خودکار ممکن نبود | `Database/InitDatabase.sql` را در SSMS اجرا کنید |
| «دسترسی کافی برای ایجاد خودکار دیتابیس وجود ندارد» | کاربر ویندوزی `dbcreator` نیست | اسکریپت را با کاربر دارای دسترسی اجرا کنید |
| «جدول SensorData ... وجود ندارد» | جدول حذف شده است | اسکریپت `InitDatabase.sql` را اجرا کنید |

---

**پایان مستندات — Industrial Sensor Monitor 1.0.0**
