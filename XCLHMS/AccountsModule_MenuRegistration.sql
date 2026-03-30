-- =============================================
-- ACCOUNTS MODULE MENU REGISTRATION
-- =============================================

-- 1. Create Main "Accounts Module" Parent Menu
INSERT INTO [dbo].[Menu_List] ([menuParentID], [menuName], [controllerName], [actionName], [applicationPath])
VALUES (0, 'Accounts Module', 'COA', 'Index', '/Accounts/COA/Index');

DECLARE @ParentID INT = SCOPE_IDENTITY();

-- 2. Sub-modules
INSERT INTO [dbo].[Menu_List] ([menuParentID], [menuName], [controllerName], [actionName], [applicationPath])
VALUES (@ParentID, 'Chart of Accounts', 'COA', 'Index', '/Accounts/COA/Index');

INSERT INTO [dbo].[Menu_List] ([menuParentID], [menuName], [controllerName], [actionName], [applicationPath])
VALUES (@ParentID, 'Contingency Register', 'ContingencyRegister', 'Index', '/Accounts/ContingencyRegister/Index');

INSERT INTO [dbo].[Menu_List] ([menuParentID], [menuName], [controllerName], [actionName], [applicationPath])
VALUES (@ParentID, 'Cheque Register', 'ChequeRegister', 'Index', '/Accounts/ChequeRegister/Index');

INSERT INTO [dbo].[Menu_List] ([menuParentID], [menuName], [controllerName], [actionName], [applicationPath])
VALUES (@ParentID, 'Cash Book', 'CashBook', 'Index', '/Accounts/CashBook/Index');

INSERT INTO [dbo].[Menu_List] ([menuParentID], [menuName], [controllerName], [actionName], [applicationPath])
VALUES (@ParentID, 'Bank Reconciliation', 'BankReconciliation', 'Index', '/Accounts/BankReconciliation/Index');

INSERT INTO [dbo].[Menu_List] ([menuParentID], [menuName], [controllerName], [actionName], [applicationPath])
VALUES (@ParentID, 'Vendor Management', 'VendorManagement', 'Index', '/Accounts/VendorManagement/Index');

INSERT INTO [dbo].[Menu_List] ([menuParentID], [menuName], [controllerName], [actionName], [applicationPath])
VALUES (@ParentID, 'Tax Configuration', 'TaxConfiguration', 'Index', '/Accounts/TaxConfiguration/Index');

INSERT INTO [dbo].[Menu_List] ([menuParentID], [menuName], [controllerName], [actionName], [applicationPath])
VALUES (@ParentID, 'Expenditure Statement', 'ExpenditureStatement', 'Index', '/Accounts/ExpenditureStatement/Index');
GO
