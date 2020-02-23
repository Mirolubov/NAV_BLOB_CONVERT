using Microsoft.SqlServer.Server;
using System;
using System.Data.SqlTypes;
using System.IO;
using System.IO.Compression;
using System.Text;

public class BlobToText
{
    [SqlFunction]
    public static SqlString BlobToNVarChar(SqlBytes sqlBytes)
    {
        SqlString str = "";
        if (sqlBytes.IsNull)
            return null;
        try
        {

            Stream bStream = sqlBytes.Stream;
            byte[] bytes = new byte[bStream.Length];

            Encoding Rus = Encoding.GetEncoding("windows-1251");
            int n = bStream.Read(bytes, 0, (int)bStream.Length);
            if (n > 4)
            {
                byte[] MAGIC_NUMBER = new byte[] { 0x02, 0x45, 0x7D, 0x5B };
                if (bytes[0] == MAGIC_NUMBER[0] && bytes[1] == MAGIC_NUMBER[1] && bytes[2] == MAGIC_NUMBER[2] && bytes[3] == MAGIC_NUMBER[3])
                {
                    byte[] bytesOffset = new byte[bytes.Length - 4];
                    Buffer.BlockCopy(bytes, 4, bytesOffset, 0, bytes.Length - 4);
                    MemoryStream inStream = new MemoryStream(bytesOffset);

                    DeflateStream decompressionStream = new DeflateStream(inStream, CompressionMode.Decompress);

                    MemoryStream outStream = new MemoryStream();
                    decompressionStream.CopyTo(outStream);

                    str = Rus.GetString(outStream.ToArray());
                }
                else
                {
                    str = Rus.GetString(bytes, 0, n);
                }
            }
            else
            {
                str = Rus.GetString(bytes, 0, n);
            }
        }
        catch
        {
            str = null;
        }
        return str;
    }
}
