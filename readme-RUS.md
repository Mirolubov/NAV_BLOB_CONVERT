Чтение NAV compressed blob через sql

Одно из преимуществ Ms Dynamics nav это открытая физическая модель данных. Она позвjляет нам формировать внешнюю OLTP отчётность, выгружать данные для хранилищ OLAP простыми запросами SQL без дополнительных операций извлечения и расшифровки данных.
Все бы ничего, пока мы не натыкаемся на поля с типом Blob (в SQL это тип Image).
Длина текстового поля в NAV ограничена 250 символами, поэтому длинные строки MS рекомендует хранить в поле с типом BLOB. Пример, добавленные в 2017й версии поле Work Description в таблицу Sales Header.
И если данные данного типа легко Транс формировать в текст посредством выражения cast(cast(imageColumn as varbinary(max)) as varchar(max)), то прочитать данные с признаком compressed=Yes стандартными средствами уже не удастся.
Спасибо комьюнити мы теперь знаем, что данные в данном формате начинаются с магического числа 0x01447C5A (hex) 
Для  решения задачи чтения данных типа compressed BLOB из SQL я решил написать CLR функцию, что позволит использовать её напрямую в запросах к базе данных. Технология SQL CLR добавлена в MS SQL Server, начиная с версии 2005. Эта технология позволяет расширять функциональность SQL сервера с помощью кода, написанного на .NET.

Функция принимает в качестве параметра значение varbinary, определяет наличие компрессии по магическому числу, и проводит декомпрессию данных, возвращает nvarchar строку. 

Установка:

Скомпилируем dll. Путь к компилятору свой. (можно пропустить и взять уже скомпилированный вариант из папки Release)
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\Roslyn\csc.exe" /t:library /out:BlobToText.dll BlobToText.cs

Получаем BlobToText.dll, переносим ее на SQL сервер, в примере в папку C:\CLR\

Теперь в SQL Management Studio

необходимо разрешить использование CLR в SQL Server.
sp_configure 'clr enabled', 1
go
reconfigure
go


Выбераем базу 
use Experimental;

Создаем сборку
CREATE ASSEMBLY CLRBlobToText FROM 'C:\CLR\BlobToText.dll' 
go

И наконец создаем функцию
CREATE FUNCTION [dbo].BlobToNVarChar(@sqlBinary varbinary(max))
RETURNS NVARCHAR (MAX) 
AS 
EXTERNAL NAME CLRBlobToText.BlobToText.BlobToNVarChar

