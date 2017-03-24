
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 12/15/2011 13:12:01
-- Generated from EDMX file: C:\Users\Russell\Documents\Visual Studio 2010\Projects\ACPLogAnalyzer\ACPLogAnalyzer\ACPLogAnalyzerDataModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ACPLogAnalyzer];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_ObservatoryObserver]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Observer] DROP CONSTRAINT [FK_ObservatoryObserver];
GO
IF OBJECT_ID(N'[dbo].[FK_ObserverLog]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Log] DROP CONSTRAINT [FK_ObserverLog];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Observatory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Observatory];
GO
IF OBJECT_ID(N'[dbo].[Observer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Observer];
GO
IF OBJECT_ID(N'[dbo].[Log]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Log];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Observatory'
CREATE TABLE [dbo].[Observatory] (
    [ObservatoryID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Latitude] nvarchar(max)  NOT NULL,
    [Longitude] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Observer'
CREATE TABLE [dbo].[Observer] (
    [ObserverID] int IDENTITY(1,1) NOT NULL,
    [ObservatoryID] int  NOT NULL,
    [Surname] nvarchar(max)  NOT NULL,
    [Forename] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Log'
CREATE TABLE [dbo].[Log] (
    [LogID] int IDENTITY(1,1) NOT NULL,
    [ObserverID] int  NOT NULL,
    [ObservatoryID] int  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [LogStartDate] datetime  NOT NULL,
    [LogEndDate] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ObservatoryID] in table 'Observatory'
ALTER TABLE [dbo].[Observatory]
ADD CONSTRAINT [PK_Observatory]
    PRIMARY KEY CLUSTERED ([ObservatoryID] ASC);
GO

-- Creating primary key on [ObserverID] in table 'Observer'
ALTER TABLE [dbo].[Observer]
ADD CONSTRAINT [PK_Observer]
    PRIMARY KEY CLUSTERED ([ObserverID] ASC);
GO

-- Creating primary key on [LogID] in table 'Log'
ALTER TABLE [dbo].[Log]
ADD CONSTRAINT [PK_Log]
    PRIMARY KEY CLUSTERED ([LogID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ObservatoryID] in table 'Observer'
ALTER TABLE [dbo].[Observer]
ADD CONSTRAINT [FK_ObservatoryObserver]
    FOREIGN KEY ([ObservatoryID])
    REFERENCES [dbo].[Observatory]
        ([ObservatoryID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ObservatoryObserver'
CREATE INDEX [IX_FK_ObservatoryObserver]
ON [dbo].[Observer]
    ([ObservatoryID]);
GO

-- Creating foreign key on [ObserverID] in table 'Log'
ALTER TABLE [dbo].[Log]
ADD CONSTRAINT [FK_ObserverLog]
    FOREIGN KEY ([ObserverID])
    REFERENCES [dbo].[Observer]
        ([ObserverID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ObserverLog'
CREATE INDEX [IX_FK_ObserverLog]
ON [dbo].[Log]
    ([ObserverID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------