# حفاظت از شاخه `test`

این کار **باید یک‌بار به‌صورت دستی** توسط مالک مخزن انجام شود؛ توکن ربات‌ها معمولاً اجازه تغییر
تنظیمات حفاظت شاخه را ندارد (خطای `403 Resource not accessible by integration`).

## مراحل

1. در GitHub به مسیر **Settings → Branches** بروید.
2. روی **Add branch protection rule** کلیک کنید.
3. در کادر **Branch name pattern** بنویسید: `test`
4. گزینه‌های زیر را فعال کنید:
   - **Require a pull request before merging**
     - `Require approvals`: ۱
     - `Dismiss stale pull request approvals when new commits are pushed`: فعال
   - **Require status checks to pass before merging**
     - `Require branches to be up to date before merging`: فعال
     - در فهرست status checkها، مورد زیر را انتخاب کنید:
       `Build, Package and Release` (از Workflow «Build and Release»)
   - **Require conversation resolution before merging**: فعال
   - **Do not allow bypassing the above settings**: فعال (در صورت نیاز به انتشار سریع، موقتاً خاموش کنید)
5. **Create** را بزنید. همین تنظیمات را برای `main` هم تکرار کنید (در صورت استفاده از این شاخه).

## نکته‌ها

- نام status check فقط پس از **یک‌بار اجرای موفق** Workflow روی همان شاخه در فهرست ظاهر می‌شود.
- پس از فعال‌سازی، Push مستقیم به `test` مسدود می‌شود و همه تغییرها باید از طریق Pull Request بروند.
- برای انتشار نسخه، تگ `v*` Push می‌شود؛ ساخت Tag از قوانین حفاظت شاخه مستثنی است.
- اگر مرحله «بررسی قالب‌بندی کد» را مسدودکننده کردید، توجه کنید که این مرحله عمداً
  `continue-on-error: true` است تا یک اختلاف قالب‌بندی، انتشار را متوقف نکند.
