Use [Source];

CREATE TABLE dbo.SourceData
(
    RecId                     bigint              NOT NULL primary key identity,
    ItemId                    nvarchar(20)        NOT NULL,
    TextData                  nvarchar(20)        NOT NULL,
    NumericData               numeric(32, 16)     NOT NULL
);

ALTER TABLE dbo.SourceData ENABLE CHANGE_TRACKING;