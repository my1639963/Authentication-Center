-- ============================================
-- EF 迁移脚本 - 在 MySQL 中执行
-- ============================================

-- 1. 标记基线迁移为已应用（所有业务表已存在，跳过重复创建）
INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) 
VALUES ('20260916040535_Baseline', '10.0.12');

-- 2. 创建 OpenIddict 表
CREATE TABLE `OpenIddictApplications` (
    `Id` varchar(255) NOT NULL,
    `ApplicationType` varchar(50) NULL,
    `ClientId` varchar(100) NULL,
    `ClientSecret` longtext NULL,
    `ClientType` varchar(50) NULL,
    `ConcurrencyToken` varchar(50) NULL,
    `ConsentType` varchar(50) NULL,
    `DisplayName` longtext NULL,
    `DisplayNames` longtext NULL,
    `JsonWebKeySet` longtext NULL,
    `Permissions` longtext NULL,
    `PostLogoutRedirectUris` longtext NULL,
    `Properties` longtext NULL,
    `RedirectUris` longtext NULL,
    `Requirements` longtext NULL,
    `Settings` longtext NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_OpenIddictApplications_ClientId` (`ClientId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `OpenIddictScopes` (
    `Id` varchar(255) NOT NULL,
    `ConcurrencyToken` varchar(50) NULL,
    `Description` longtext NULL,
    `Descriptions` longtext NULL,
    `DisplayName` longtext NULL,
    `DisplayNames` longtext NULL,
    `Name` varchar(200) NULL,
    `Properties` longtext NULL,
    `Resources` longtext NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_OpenIddictScopes_Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `OpenIddictAuthorizations` (
    `Id` varchar(255) NOT NULL,
    `ApplicationId` varchar(255) NULL,
    `ConcurrencyToken` varchar(50) NULL,
    `CreationDate` datetime(6) NULL,
    `Properties` longtext NULL,
    `Scopes` longtext NULL,
    `Status` varchar(50) NULL,
    `Subject` varchar(400) NULL,
    `Type` varchar(50) NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type` (`ApplicationId`, `Status`, `Subject`, `Type`),
    CONSTRAINT `FK_OpenIddictAuthorizations_OpenIddictApplications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `OpenIddictApplications` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `OpenIddictTokens` (
    `Id` varchar(255) NOT NULL,
    `ApplicationId` varchar(255) NULL,
    `AuthorizationId` varchar(255) NULL,
    `ConcurrencyToken` varchar(50) NULL,
    `CreationDate` datetime(6) NULL,
    `ExpirationDate` datetime(6) NULL,
    `Payload` longtext NULL,
    `Properties` longtext NULL,
    `RedemptionDate` datetime(6) NULL,
    `ReferenceId` varchar(100) NULL,
    `Status` varchar(50) NULL,
    `Subject` varchar(400) NULL,
    `Type` varchar(150) NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_OpenIddictTokens_ApplicationId_Status_Subject_Type` (`ApplicationId`, `Status`, `Subject`, `Type`),
    KEY `IX_OpenIddictTokens_AuthorizationId` (`AuthorizationId`),
    UNIQUE KEY `IX_OpenIddictTokens_ReferenceId` (`ReferenceId`),
    CONSTRAINT `FK_OpenIddictTokens_OpenIddictApplications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `OpenIddictApplications` (`Id`),
    CONSTRAINT `FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId` FOREIGN KEY (`AuthorizationId`) REFERENCES `OpenIddictAuthorizations` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 3. 标记 OpenIddict 迁移为已应用
INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) 
VALUES ('20260916040652_AddOpenIddictTables', '10.0.12');
