# فرایند انتشار نسخه

این سند مراحل انتشار یک نسخه جدید را به‌ترتیب نشان می‌دهد. هدف این است که شماره نسخه در همه
جا یکسان بماند و یادداشت‌های انتشار به‌صورت خودکار از `CHANGELOG.md` ساخته شود.

## ۱. به‌روزرسانی شماره نسخه

همه موارد زیر باید **یکسان** باشند، وگرنه مرحله «بررسی سازگاری شماره نسخه» در CI با شکست متوقف می‌شود:

| محل | نمونه |
|---|---|
| `src/IndustrialSensorMonitor/IndustrialSensorMonitor.csproj` (`<Version>`, `AssemblyVersion`, `FileVersion`) | `1.3.0` / `1.3.0.0` |
| `installer/Setup.iss` (`#define MyAppVersion`) | `1.3.0` |
| بالاترین بخش `CHANGELOG.md` | `## [1.3.0] - 2026-11-05` |
| تگ Git (هنگام انتشار) | `v1.3.0` |

## ۲. نوشتن تغییرات در CHANGELOG

یک بخش جدید در بالای `CHANGELOG.md` اضافه کنید. متن همین بخش **بدون تغییر** در GitHub Release استفاده می‌شود.

```markdown
## [1.3.0] - 2026-11-05

### افزوده شد
- ...

### تغییر کرد
- ...

### رفع شد
- ...
```

## ۳. بررسی محلی (اختیاری)

```powershell
dotnet restore IndustrialSensorMonitor.sln
dotnet build IndustrialSensorMonitor.sln -c Release
dotnet test tests\IndustrialSensorMonitor.Tests\IndustrialSensorMonitor.Tests.csproj -c Release
```

## ۴. ادغام از طریق Pull Request

- Push به شاخه کاری و باز کردن PR به `test`.
- CI روی PR اجرا می‌شود (Build، تست‌ها، بررسی نسخه، پوشش کد).
- پس از سبز شدن، PR را Merge کنید.

## ۵. ساخت تگ و انتشار

```bash
git checkout test
git pull origin test
git tag v1.3.0
git push origin v1.3.0
```

با Push تگ:

1. Workflow «Build and Release» اجرا می‌شود.
2. شماره نسخه از تگ خوانده و با `csproj` مقایسه می‌شود.
3. خروجی‌ها ساخته می‌شوند: `Setup-x64.exe`، `IndustrialSensorMonitor.exe` و `Portable-x64.zip`.
4. یادداشت‌های انتشار از بخش `## [1.3.0]` در `CHANGELOG.md` ساخته و Release منتشر می‌شود.

## ۶. بررسی پس از انتشار

- صفحه Actions: مرحله «انتشار GitHub Release» باید موفق باشد.
- صفحه Releases: هر سه فایل باید موجود و حجم آن‌ها غیرصفر باشد.
- خلاصه اجرا باید تعداد تست‌های موفق و درصد پوشش کد (مرحله غیرمسدودکننده) را نشان دهد.

## نکته‌های مهم

- هیچ مرحله‌ای از انتشار روی محیط توسعه محلی انجام نمی‌شود؛ ساخت و بسته‌بندی کاملاً در GitHub Actions است.
- مرحله `dotnet format` و مرحله پوشش کد **غیرمسدودکننده** هستند و فقط گزارش تولید می‌کنند.
- اگر مرحله تست‌ها «هیچ تستی اجرا نشد» گزارش کند، اجرا عمداً شکست می‌خورد تا خرابی کشف تست‌ها پنهان نماند.
