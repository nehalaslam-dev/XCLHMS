-- =============================================
-- ACCOUNTS MODULE SCHEMA UPDATES
-- =============================================

-- 1. EXTEND CHART OF ACCOUNTS (COA) WITH BUDGET FIELDS
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'BudgetEstimates' AND Object_ID = OBJECT_ID('Budget'))
BEGIN
    ALTER TABLE [dbo].[Budget] ADD 
        [BudgetEstimates] DECIMAL(18, 2) DEFAULT 0,
        [RevisedBudget] DECIMAL(18, 2) DEFAULT 0,
        [AdditionalBudget] DECIMAL(18, 2) DEFAULT 0,
        [SupplementaryBudget] DECIMAL(18, 2) DEFAULT 0,
        [TotalRevisedBudget] DECIMAL(18, 2) DEFAULT 0,
        [ReAppropriationPositive] DECIMAL(18, 2) DEFAULT 0,
        [ReAppropriationNegative] DECIMAL(18, 2) DEFAULT 0,
        [TotalRevisedBudgetAfterReApp] DECIMAL(18, 2) DEFAULT 0,
        [FiscalYear] NVARCHAR(20);
END
GO

-- 2. CREATE CHEQUE REGISTER TABLE
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChequeRegister]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ChequeRegister] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [ChequeNo] NVARCHAR(50) NOT NULL,
        [Date] DATETIME NOT NULL,
        [Amount] DECIMAL(18, 2) NOT NULL,
        [BillNo] NVARCHAR(50),
        [VendorName] NVARCHAR(200),
        [ObjectHeadId] INT, -- Links to COA
        [IsCleared] BIT DEFAULT 0,
        [ClearedDate] DATETIME,
        [Status] NVARCHAR(50) DEFAULT 'Pending',
        [CreatedDate] DATETIME DEFAULT GETDATE(),
        FOREIGN KEY ([ObjectHeadId]) REFERENCES [dbo].[COA]([ID])
    );
END
GO

-- 3. EXTEND VENDORS TABLE
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'NTN' AND Object_ID = OBJECT_ID('Vendors'))
BEGIN
    ALTER TABLE [dbo].[Vendors] ADD 
        [NTN] NVARCHAR(50),
        [CNIC] NVARCHAR(15),
        [Status] NVARCHAR(50), -- Active/Inactive
        [PaymentSection] NVARCHAR(100); -- e.g., Goods/Services categories
END
GO

-- 4. TAX CONFIGURATION
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Accounts_TaxConfig]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Accounts_TaxConfig] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [TaxName] NVARCHAR(100) NOT NULL,
        [TaxPercentage] DECIMAL(5, 4) NOT NULL, -- e.g., 0.0035 for 0.35%
        [Description] NVARCHAR(255),
        [IsActive] BIT DEFAULT 1
    );
    
    -- Seed data
    INSERT INTO [dbo].[Accounts_TaxConfig] ([TaxName], [TaxPercentage], [Description])
    VALUES ('Stamp Duty', 0.0035, 'Stamp Duty tax at 0.35%');
END
GO

-- 5. EXPENDITURE STATEMENT (CAN BE A VIEW OR TABLE)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExpenditureStatement]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ExpenditureStatement] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [HeadId] INT,
        [FiscalYear] NVARCHAR(20),
        [MonthName] NVARCHAR(20), -- July to June
        [ExpenditureAmount] DECIMAL(18, 2) DEFAULT 0,
        [CreatedDate] DATETIME DEFAULT GETDATE(),
        FOREIGN KEY ([HeadId]) REFERENCES [dbo].[COA]([ID])
    );
END
GO

-- 6. CONTINGENCY REGISTER (ENHANCEMENT)
-- Using existing AccountRegister, adding missing columns if any
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'StampDuty' AND Object_ID = OBJECT_ID('AccountRegister'))
BEGIN
    ALTER TABLE [dbo].[AccountRegister] ADD 
        [StampDuty] DECIMAL(18, 2) DEFAULT 0,
        [GST] DECIMAL(18, 2) DEFAULT 0;
END
GO
