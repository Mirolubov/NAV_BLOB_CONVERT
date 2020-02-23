
use Experimental;

DROP FUNCTION [dbo].BlobToNVarChar
DROP ASSEMBLY CLRBlobToText;

CREATE ASSEMBLY CLRBlobToText FROM 'C:\CLR\BlobToText.dll' 
go

CREATE FUNCTION [dbo].BlobToNVarChar(@sqlBinary varbinary(max))
RETURNS NVARCHAR (MAX) 
AS 
EXTERNAL NAME CLRBlobToText.BlobToText.BlobToNVarChar
