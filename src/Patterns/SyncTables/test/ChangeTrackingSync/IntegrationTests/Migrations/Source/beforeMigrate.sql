IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'Source')
    BEGIN
        CREATE DATABASE [Source];
        
        ALTER DATABASE [Source]
            SET CHANGE_TRACKING = ON
            (CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON);
    END