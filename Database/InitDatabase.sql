/* =====================================================================
   Industrial Sensor Monitor
   اسکریپت ایجاد دیتابیس و جدول اصلی برنامه
   ---------------------------------------------------------------------
   نام دیتابیس : IndustrialSensorDb
   نام جدول    : dbo.SensorData
   ---------------------------------------------------------------------
   نکات مهم:
   * این اسکریپت کاملاً تکرارپذیر (Idempotent) است؛ اجرای چندباره آن
     خطا ایجاد نمی‌کند و هیچ اطلاعاتی را حذف نمی‌کند.
   * در این اسکریپت از DROP TABLE و DROP DATABASE استفاده نشده است.
   * برای اجرا: SQL Server Management Studio را باز کنید، به نمونه
     SQL Server (LocalDB یا Express) متصل شوید، این فایل را باز کنید
     و کلید F5 را بزنید.
   ===================================================================== */

USE [master];
GO

/* ۱) ایجاد دیتابیس در صورت نبود آن */
IF DB_ID(N'IndustrialSensorDb') IS NULL
BEGIN
    PRINT N'در حال ایجاد دیتابیس IndustrialSensorDb ...';
    CREATE DATABASE [IndustrialSensorDb];
END
ELSE
BEGIN
    PRINT N'دیتابیس IndustrialSensorDb از قبل وجود دارد.';
END
GO

/* ۲) انتخاب دیتابیس برنامه */
USE [IndustrialSensorDb];
GO

/* ۳) ایجاد جدول SensorData در صورت نبود آن */
IF OBJECT_ID(N'dbo.SensorData', N'U') IS NULL
BEGIN
    PRINT N'در حال ایجاد جدول dbo.SensorData ...';

    CREATE TABLE dbo.[SensorData]
    (
        /* شناسه رکورد: کلید اصلی و افزایش خودکار */
        [Id]         INT           IDENTITY(1,1) NOT NULL,

        /* نام سنسور: متن فارسی، حداکثر ۱۰۰ کاراکتر */
        [SensorName] NVARCHAR(100) NOT NULL,

        /* مقدار سنسور: عدد اعشاری با دقت ۴ رقم اعشار */
        [Value]      DECIMAL(18,4) NOT NULL,

        /* واحد اندازه‌گیری: حداکثر ۳۰ کاراکتر */
        [Unit]       NVARCHAR(30)  NOT NULL,

        /* تاریخ و ساعت ثبت: به‌صورت خودکار توسط SQL Server تعیین می‌شود */
        [RecordDate] DATETIME2(7)  NOT NULL
            CONSTRAINT [DF_SensorData_RecordDate] DEFAULT (SYSDATETIME()),

        /* کلید اصلی */
        CONSTRAINT [PK_SensorData] PRIMARY KEY CLUSTERED ([Id]),

        /* محدودیت‌های اعتبارسنجی در سطح دیتابیس */
        CONSTRAINT [CK_SensorData_Value] CHECK ([Value] >= -1000000 AND [Value] <= 1000000),
        CONSTRAINT [CK_SensorData_Unit] CHECK (LEN(LTRIM(RTRIM([Unit]))) > 0),
        CONSTRAINT [CK_SensorData_SensorName] CHECK (LEN(LTRIM(RTRIM([SensorName]))) > 0)
    );
END
ELSE
BEGIN
    PRINT N'جدول dbo.SensorData از قبل وجود دارد.';
END
GO

/* ۴) ایجاد ایندکس برای مرتب‌سازی سریع بر اساس تاریخ ثبت */
IF NOT EXISTS (SELECT 1
               FROM sys.indexes
               WHERE [name] = N'IX_SensorData_RecordDate'
                 AND [object_id] = OBJECT_ID(N'dbo.[SensorData]'))
BEGIN
    PRINT N'در حال ایجاد ایندکس IX_SensorData_RecordDate ...';

    CREATE NONCLUSTERED INDEX [IX_SensorData_RecordDate]
        ON dbo.[SensorData] ([RecordDate] DESC);
END
ELSE
BEGIN
    PRINT N'ایندکس IX_SensorData_RecordDate از قبل وجود دارد.';
END
GO

/* ۵) گزارش نهایی ساختار جدول (فقط برای بررسی، تغییری ایجاد نمی‌کند) */
PRINT N'--- ساختار جدول dbo.SensorData ---';
SELECT [COLUMN_NAME], [DATA_TYPE], [CHARACTER_MAXIMUM_LENGTH], [NUMERIC_PRECISION], [NUMERIC_SCALE], [IS_NULLABLE]
FROM [INFORMATION_SCHEMA].[COLUMNS]
WHERE [TABLE_NAME] = N'SensorData'
ORDER BY [ORDINAL_POSITION];
GO

PRINT N'آماده‌سازی دیتابیس IndustrialSensorDb با موفقیت پایان یافت.';
GO
