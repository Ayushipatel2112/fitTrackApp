-- ============================================================
--  FitnessDb  –  Final 7-Table Ultra-Clean Design
--  All extra tables (Services, Blog, Profiles) removed.
--  ============================================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'FitnessDb')
BEGIN
    ALTER DATABASE FitnessDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE FitnessDb;
END
GO

CREATE DATABASE FitnessDb;
GO

USE FitnessDb;
GO

-- 1. Roles
CREATE TABLE Roles (
    Id               NVARCHAR(450) NOT NULL PRIMARY KEY,
    Name             NVARCHAR(256) NULL,
    NormalizedName   NVARCHAR(256) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL
);

-- 2. Users (Includes Profile Info)
CREATE TABLE Users (
    Id                   NVARCHAR(450) NOT NULL PRIMARY KEY,
    UserName             NVARCHAR(256) NULL,
    NormalizedUserName   NVARCHAR(256) NULL,
    Email                NVARCHAR(256) NULL,
    NormalizedEmail      NVARCHAR(256) NULL,
    EmailConfirmed       BIT           NOT NULL DEFAULT 0,
    PasswordHash         NVARCHAR(MAX) NULL,
    SecurityStamp        NVARCHAR(MAX) NULL,
    ConcurrencyStamp     NVARCHAR(MAX) NULL,
    PhoneNumber          NVARCHAR(MAX) NULL,
    PhoneNumberConfirmed BIT           NOT NULL DEFAULT 0,
    TwoFactorEnabled     BIT           NOT NULL DEFAULT 0,
    LockoutEnd           DATETIMEOFFSET NULL,
    LockoutEnabled       BIT           NOT NULL DEFAULT 1,
    AccessFailedCount    INT           NOT NULL DEFAULT 0,
    -- Profile shadow properties
    FullName             NVARCHAR(200) NULL,
    Phone                NVARCHAR(20)  NULL,
    Age                  INT           NULL,
    HeightCm             DECIMAL(5,2)  NULL,
    WeightKg             DECIMAL(5,2)  NULL,
    FitnessGoal          NVARCHAR(200) NULL,
    ActivityLevel        NVARCHAR(50)  NULL,
    JoinedDate           DATETIME2     NOT NULL DEFAULT GETDATE(),
    IsActive             BIT           NOT NULL DEFAULT 1
);

-- 3. UserRoles
CREATE TABLE UserRoles (
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE
);

-- 4. Workouts
CREATE TABLE Workouts (
    Id              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserId          NVARCHAR(450)     NOT NULL,
    Title           NVARCHAR(100)     NOT NULL,
    Description     NVARCHAR(MAX)     NOT NULL,
    WorkoutType     NVARCHAR(50)      NOT NULL DEFAULT 'strength',
    DurationMinutes INT               NOT NULL,
    CaloriesBurned  INT               NOT NULL DEFAULT 0,
    Sets            INT               NULL,
    Reps            INT               NULL,
    WeightKg        DECIMAL(8,2)      NULL,
    Notes           NVARCHAR(MAX)     NULL,
    Date            DATETIME2         NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- 5. Meals
CREATE TABLE Meals (
    Id       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserId   NVARCHAR(450)     NOT NULL,
    MealName NVARCHAR(200)     NOT NULL,
    MealType NVARCHAR(50)      NOT NULL DEFAULT 'Breakfast',
    Calories INT               NOT NULL DEFAULT 0,
    Protein  DECIMAL(8,2)      NULL,
    Carbs    DECIMAL(8,2)      NULL,
    Fat      DECIMAL(8,2)      NULL,
    Notes    NVARCHAR(MAX)     NULL,
    LoggedAt DATETIME2         NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- 6. SleepLogs
CREATE TABLE SleepLogs (
    Id         INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserId     NVARCHAR(450)     NOT NULL,
    SleepDate  DATE              NOT NULL,
    BedTime    NVARCHAR(10)      NOT NULL,
    WakeTime   NVARCHAR(10)      NOT NULL,
    HoursSlept DECIMAL(4,2)      NOT NULL,
    Quality    NVARCHAR(20)      NOT NULL DEFAULT 'Good',
    Notes      NVARCHAR(MAX)     NULL,
    CreatedAt  DATETIME2         NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- 7. ContactMessages
CREATE TABLE ContactMessages (
    Id      INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name    NVARCHAR(200)     NOT NULL,
    Email   NVARCHAR(300)     NOT NULL,
    Subject NVARCHAR(300)     NULL,
    Message NVARCHAR(MAX)     NOT NULL,
    SentAt  DATETIME2         NOT NULL DEFAULT GETDATE(),
    IsRead  BIT               NOT NULL DEFAULT 0
);

-- Internal Identity (Required)
CREATE TABLE UserClaims (Id INT IDENTITY PRIMARY KEY, UserId NVARCHAR(450) NOT NULL, ClaimType NVARCHAR(MAX), ClaimValue NVARCHAR(MAX), FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE);
CREATE TABLE UserLogins (LoginProvider NVARCHAR(128) NOT NULL, ProviderKey NVARCHAR(128) NOT NULL, ProviderDisplayName NVARCHAR(MAX), UserId NVARCHAR(450) NOT NULL, PRIMARY KEY (LoginProvider, ProviderKey), FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE);
CREATE TABLE UserTokens (UserId NVARCHAR(450) NOT NULL, LoginProvider NVARCHAR(128) NOT NULL, Name NVARCHAR(128) NOT NULL, Value NVARCHAR(MAX), PRIMARY KEY (UserId, LoginProvider, Name), FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE);
CREATE TABLE RoleClaims (Id INT IDENTITY PRIMARY KEY, RoleId NVARCHAR(450) NOT NULL, ClaimType NVARCHAR(MAX), ClaimValue NVARCHAR(MAX), FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE);

-- Seed Roles
INSERT INTO Roles (Id, Name, NormalizedName) VALUES ('r1', 'Admin', 'ADMIN'), ('r2', 'User', 'USER');

GO
PRINT 'FitnessDb created with exactly 7 Main Tables.';
GO
