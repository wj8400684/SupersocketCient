using System.Buffers;
using System.Text;
using SuperSocket.ProtoBase;

namespace SupersocketCient;

public sealed class StringPacketEncode : IPackageEncoder<StringPackageInfo>
{
    public int Encode(IBufferWriter<byte> writer, StringPackageInfo pack)
    {
        var s = Encoding.UTF8.GetBytes("SORT 10 7 3 8 6 43 23\r\n");
         writer.Write(s);

         return s.Length;
        var bodyArray = pack.Parameters;
        
        var totalLength = writer.Write(pack.Key, Encoding.UTF8);
        totalLength += writer.Write(" ", Encoding.UTF8);

        for (var i = 0; i < bodyArray.Length; i++)
        {
            totalLength += writer.Write(bodyArray[i], Encoding.UTF8);
            totalLength += writer.Write(" ", Encoding.UTF8);
        }

        totalLength += writer.Write("\r\n", Encoding.UTF8);

        return totalLength;
    }
}