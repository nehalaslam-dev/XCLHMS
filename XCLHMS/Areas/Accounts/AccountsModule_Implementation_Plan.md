# 🏥 Hospital Management System - Accounts Module Expansion

The Accounts Module has been expanded into a fully modular section with a premium, modern UI. This implementation follows the **Controller-Service-Repository** pattern and integrates seamlessly with the existing system while introducing significantly improved functionality and aesthetics.

## 📁 Modular Structure Overview
The module follows the requested sub-division under the `Areas/Accounts` directory:

| Sub-Module | Controller | Primary Components |
| :--- | :--- | :--- |
| **Charts of Accounts** | `COAController` | Object Head Code, Object Descriptions, Budget Fields |
| **Contingency Register** | `ContingencyRegisterController` | Bill tracking, Tax Deductions, Net Amount calculations |
| **Cheque Register** | `ChequeRegisterController` | Monitoring pending and cleared cheques |
| **Cash Book** | `CashBookController` | Daily ledger with Receipts & Payments and auto-closing balances |
| **Bank Reconciliation** | `BankReconciliationController` | Syncing internal accounts with bank statements |
| **Vendor Management** | `VendorManagementController` | Comprehensive vendor profiles (NTN, CNIC, Address, Status) |
| **Tax Configuration** | `TaxConfigurationController` | Set global tax percentages (IT, SST, GST, Stamp Duty) |
| **Expenditure Statement**| `ExpenditureStatementController`| Monthly expenditure analysis (July – June) |

## 🛠️ Data Infrastructure
Two comprehensive SQL scripts have been created to prepare the database for the new functionality:
1.  **[AccountsModule_Schema.sql](file:///c:/Users/Personal%20Computer/Downloads/XCLHMS/XCLHMS/AccountsModule_Schema.sql)**:
    - Extends `Budget` table with multi-phase budget fields (Estimates, Revised, Supplementary, Additional).
    - Creates `ChequeRegister`, `Accounts_TaxConfig`, and `ExpenditureStatement` tables.
    - Adds `NTN`, `CNIC`, `Status`, and `PaymentSection` to the `Vendors` table.
2.  **[AccountsModule_MenuRegistration.sql](file:///c:/Users/Personal%20Computer/Downloads/XCLHMS/XCLHMS/AccountsModule_MenuRegistration.sql)**:
    - Registers the "Accounts Module" as a parent menu in the database.
    - Adds all 8 sub-modules as distinct, navigable sections.

## 🎨 Design Aesthetics
A new layout **[_AccountsLayout.cshtml](file:///c:/Users/Personal%20Computer/Downloads/XCLHMS/XCLHMS/Areas/Accounts/Views/Shared/_AccountsLayout.cshtml)** has been introduced to achieve the modern medical-UI look (inspired by the provided screenshots).
- **Typography:** Uses *Outfit* and *Inter* fonts for a clean, professional feel.
- **Components:** Premium cards, sleek status badges, and interactive data tables.
- **Consistency:** Uses a unified color palette based on `sidebar-blue (#0f3460)` and `primary-blue (#3f8cfe)`.

## 🧮 Core Logic & Auto-Calculations
- **Budget Tracking:** Automated calculation of `Total Revised Budget` and `Re-Appropriation` balances.
- **Tax Processing:** Centralized net-amount logic (Gross Amount - Income Tax - SST - Stamp Duty).
- **Cash Book:** Auto-calculation of Opening and Closing balances on the client-side for real-time totals.

## 🚀 Deployment Instructions
1.  **Run SQL Scripts:** Execute both `.sql` files in your SQL Server database to update the schema and menu.
2.  **Re-login:** After running the menu registration script, re-log into the system to see the new **Accounts Module** section in the sidebar.
3.  **View Results:** All new pages are ready-to-use and pre-configured with the new modern design.
