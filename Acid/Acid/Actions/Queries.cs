namespace Acid.Actions;

public class Queries
{
    public const string SQL_READ_COMMITTED = @"

BEGIN TRAN

DECLARE @bigBalance INT;
DECLARE @part1 INT;
DECLARE @part2 INT;

SELECT @bigBalance = Max(Balance) FROM MyAccounts
SELECT @part1 = @bigBalance / 2;
SELECT @part2 = @bigBalance - @part1;

UPDATE MyAccounts
SET Balance = Balance - @bigBalance
WHERE Id IN (
    SELECT Min(Id)
    FROM MyAccounts
    WHERE Balance = @bigBalance
);

UPDATE MyAccounts
SET Balance = Balance + @part1
WHERE Id IN (
    SELECT TOP 1 Id
    FROM MyAccounts
    ORDER BY NewId()
);

UPDATE MyAccounts
SET Balance = Balance + @part2
WHERE Id IN (
    SELECT TOP 1 Id
    FROM MyAccounts
    ORDER BY NewId()
);

COMMIT TRAN;";

    public const string SQL_TRANS_READ_COMMITTED = @"

BEGIN TRAN

DECLARE @bigBalance INT;
DECLARE @part1 INT;
DECLARE @part2 INT;

SELECT @bigBalance = Max(Balance) FROM MyAccounts
SELECT @part1 = @bigBalance / 2;
SELECT @part2 = @bigBalance - @part1;

UPDATE MyAccounts
SET Balance = Balance - @bigBalance
WHERE Id IN (
    SELECT Min(Id)
    FROM MyAccounts
    WHERE Balance = @bigBalance
);

UPDATE MyAccounts
SET Balance = Balance + @part1
WHERE Id IN (
    SELECT TOP 1 Id
    FROM MyAccounts
    ORDER BY NewId()
);

UPDATE MyAccounts
SET Balance = Balance + @part2
WHERE Id IN (
    SELECT TOP 1 Id
    FROM MyAccounts
    ORDER BY NewId()
);

COMMIT TRAN;";

    public const string SQL_TRANS_SERIALIZABLE = @"

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

BEGIN TRAN

DECLARE @bigBalance INT;
DECLARE @part1 INT;
DECLARE @part2 INT;

SELECT @bigBalance = Max(Balance) FROM MyAccounts
SELECT @part1 = @bigBalance / 2;
SELECT @part2 = @bigBalance - @part1;

UPDATE MyAccounts
SET Balance = Balance - @bigBalance
WHERE Id IN (
    SELECT Min(Id)
    FROM MyAccounts
    WHERE Balance = @bigBalance
);

UPDATE MyAccounts
SET Balance = Balance + @part1
WHERE Id IN (
    SELECT TOP 1 Id
    FROM MyAccounts
    ORDER BY NewId()
);

UPDATE MyAccounts
SET Balance = Balance + @part2
WHERE Id IN (
    SELECT TOP 1 Id
    FROM MyAccounts
    ORDER BY NewId()
);

COMMIT TRAN;";
}