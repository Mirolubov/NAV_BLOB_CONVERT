using Microsoft.SqlServer.Server;
using System;
using System.Data.SqlTypes;
using System.IO;
using System.IO.Compression;
using System.Text;

public class BlobToText
{
    [SqlFunction]
    static public SqlString BlobToNVarChar(SqlBytes sqlBytes)
    {
        SqlString str;
        byte[] bytes = new byte[1000];
        byte[] bytesOffset = new byte[1000];
        Stream bStream = sqlBytes.Stream;
        int n = bStream.Read(bytes, 0, 1000);
        Buffer.BlockCopy(bytes, 4, bytesOffset, 0, bytes.Length-4);
        MemoryStream inStream = new MemoryStream(bytesOffset) ;
        
        DeflateStream decompressionStream = new DeflateStream(inStream, CompressionMode.Decompress);
        
        MemoryStream outStream = new MemoryStream();
        decompressionStream.CopyTo(outStream);

        Encoding Rus = Encoding.GetEncoding("windows-1251");
        str = Rus.GetString(outStream.ToArray());
        return str;
    }
}
