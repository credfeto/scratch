CREATE TABLE Test.Test (
    Id INT IDENTITY(1, 1) NOT NULL,
    Name VARCHAR(66) NOT NULL,
    CONSTRAINT PK_Test PRIMARY KEY (Id),

CONSTRAINT IXU_Name UNIQUE (name)
    )
