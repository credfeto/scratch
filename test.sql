CREATE TABLE Test.Test (
  Id INT IDENTITY (1, 1) NOT NULL,
  [Name] VARCHAR(66) NOT NULL,
  CONSTRAINT PK_Test PRIMARY KEY (Id),
  CONSTRAINT IXU_Name UNIQUE ([Name])
)

INSERT INTO Test.Test (
  [Name]
) VALUES ('Hello World')
SELECT
  [Id],
  [Name]
FROM Test.Test
ORDER BY [Name]

DELETE
FROM Test.Test
WHERE [Name] = 'Hello World'
