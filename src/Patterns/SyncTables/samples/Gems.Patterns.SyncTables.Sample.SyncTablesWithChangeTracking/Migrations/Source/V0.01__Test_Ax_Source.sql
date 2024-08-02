Use [Source];

CREATE TABLE dbo.Person
(
    [RecId]                   bigint              NOT NULL primary key identity,
    [PersonId]                uniqueidentifier    NOT NULL,
    [FirstName]               nvarchar(20)        NOT NULL,
    [LastName]                nvarchar(20)        NOT NULL,
    [Age]                     int                 NOT NULL,
    [Gender]                  int                 NOT NULL    
);

ALTER TABLE dbo.Person ENABLE CHANGE_TRACKING;