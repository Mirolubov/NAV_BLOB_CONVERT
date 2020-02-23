# DB CLR converting Navision compressed BLOB to NVARCHAR

One of the benefits of Ms Dynamics nav is its open physical data model. It allows us to generate external OLTP reporting, upload data for OLAP repositories with simple SQL queries without additional data extraction and decryption operations. Everything would be fine until we come across fields with the Blob type (in SQL, this is the Image type). The length of the text field in NAV is limited to 250 characters, so MS recommends storing long lines in a field of type BLOB. An example (added in the 2017 version) is the "Work Description" field in the [Sales Header] table. Data of this type can be easily transformed into text using the expression cast (cast (imageColumn as varbinary (max)) as varchar (max)), until we have compressed = Yes attribute. 
Thanks to the community, we now know that data in this format starts with a magic number 0x01447C5A (hex) To solve the problem of reading compressed BLOB data from SQL, I decided to write a CLR function, which will allow it to be used directly in database queries. SQL CLR technology has been added to MS SQL Server since version 2005. This technology allows you to extend the functionality of SQL server using code written in .NET.

The function takes a varbinary value as a parameter, determines whether compression is by magic number, and decompresses the data, returns an nvarchar string.

# Installation:

Compile dll. Change compiler path. (You can skip this step and take the already compiled version from the Release folder)
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\Roslyn\csc.exe" /t:library /out:BlobToText.dll BlobToText.cs

You will get BlobToText.dll, move it to SQL server, for example dir C:\CLR\

Now in SQL Management Studio

First enable CLR on SQL Server:
sp_configure 'clr enabled', 1
go
reconfigure
go

Select database:
use Experimental;

Create assemble:
CREATE ASSEMBLY CLRBlobToText FROM 'C:\CLR\BlobToText.dll' 
go

And create function:
CREATE FUNCTION [dbo].BlobToNVarChar(@sqlBinary varbinary(max))
RETURNS NVARCHAR (MAX) 
AS 
EXTERNAL NAME CLRBlobToText.BlobToText.BlobToNVarChar

#Example
select top 10 [dbo].BlobToNVarChar([Work Description]) from [dbo].[ARIMA$Sales Header] where [Work Description] is not null

