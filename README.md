# 💸 Predictive Expense Tracker

![.NET](https://img.shields.io/badge/.NET-ASP.NET%20Core-blue)
![ML.NET](https://img.shields.io/badge/Machine%20Learning-ML.NET-yellowgreen)
![Bootstrap](https://img.shields.io/badge/Design-Bootstrap%205-purple)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-lightgrey)
![License](https://img.shields.io/badge/license-MIT-green)

> A smart, web-based expense tracker that allows users to manage their daily expenses, analyze spending, and predict future expenses using ML.NET.

---

## 🧾 Features

### 👤 User Module
- Register/Login using local session
- Add daily income and expense entries
- Categorize transactions by type
- Filter and view monthly reports
- **Download monthly reports as PDF or Excel**
- **ML-based next-month expense prediction**

### 🤖 Predictive Expense Model (ML.NET - Time Series)

- Trained using historical monthly expense data per user  
- Implements Time Series Forecasting using SSA (Singular Spectrum Analysis)  
- Predicts the next month's total expense with confidence intervals  
- Integrated with ADO.NET for data access  
- Output displayed in Razor Views and visualized using JavaScript (Chart.js)

---

## 🛠️ Technologies Used

| Layer       | Technology                       |
|------------|----------------------------------|
| Frontend   | HTML, CSS, Bootstrap 5           |
| Backend    | ASP.NET Core MVC (Razor Views)   |
| ML         | ML.NET Regression                |
| Data Access| ADO.NET + Stored Procedures      |
| DB         | Microsoft SQL Server             |
| Reports    | iTextSharp (PDF), EPPlus (Excel) |

---

## 🧩 Database Structure

- Users
- Categories (Income/Expense types)
- Transactions
- ML Predictions 
- Monthly Summary


---

## 🧪 Sample Stored Procedure

```sql
CREATE PROCEDURE usp_GetMonthlyExpenses
    @UserId INT,
    @Month INT,
    @Year INT
AS
BEGIN
    SELECT SUM(Amount) AS Total
    FROM tbl_Transactions
    WHERE UserId = @UserId AND MONTH(Date) = @Month AND YEAR(Date) = @Year
END
