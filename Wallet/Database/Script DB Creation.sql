CREATE DATABASE WALLET
GO
USE WALLET
GO
CREATE TABLE Users(
Id int not null primary key identity(1,1),	   
First_Name nvarchar(50) not null,
Last_Name nvarchar(50) not null,
Email nvarchar(100) not null UNIQUE,
Password nvarchar(MAX) not null
)
GO
CREATE TABLE Accounts(
Id int not null identity(1,1) primary key, 
Currency nvarchar(3) CHECK(Currency='USD' or Currency='ARS'),
User_Id int not null foreign key references Users(Id)
)
GO
CREATE TABLE Transactions(
Id int not null identity(1,1) primary key,
Amount float not null check(Amount>0),
Concept nvarchar(50) not null,
Date Datetime not null,
Type nvarchar(10) not null CHECK(Type='Topup' or Type='Payment'),
Account_Id int not null foreign key references Accounts(Id)
)
GO
CREATE TABLE FixedTermDeposit(
Id int not null identity(1,1) primary key,
Amount float not null check(Amount>0),
Creation_Date Datetime not null DEFAULT(GETDATE()),
Closing_Date Datetime null,
Account_Id int not null FOREIGN KEY REFERENCES Accounts(Id)
)