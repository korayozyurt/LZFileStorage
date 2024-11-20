using System.Text;

namespace LZAlgorithms;

public class LZ77
{
    public class Tuple
    {
        public int Offset { get; set; }
        public int Length { get; set; }

    }

    /// <summary>
    /// <b>Offset:</b> Kaç karakter geriye gidileceği <br/>
    /// <b>length:</b> Eşleşen karakter sayısı <br/>
    /// <b>nextChar:</b> Eşleşen karakterlerin ardından gelen karakter <br/>
    /// </summary>
    public class TupleString : Tuple
    {
        public char NextChar { get; set; }

        public TupleString(int offset, int length, char nextChar)
        {
            Offset = offset;
            Length = length;
            NextChar = nextChar;
        }

        public override string ToString()
        {
            return $"({Offset}, {Length}, '{NextChar}')";
        }
    }

    /// <summary>
    /// <b>Offset:</b> Kaç karakter geriye gidileceği <br/>
    /// <b>length:</b> Eşleşen karakter sayısı <br/>
    /// <b>nextChar:</b> Eşleşen karakterlerin ardından gelen byte <br/>
    /// </summary>
    public class TuppleByte: Tuple
    {
        public byte NextByte { get; set; }

        public TuppleByte(int offset, int length, byte nextByte)
        {
            Offset = offset;
            Length = length;
            NextByte = nextByte;
        }

        public override string ToString()
        {
            return $"({Offset}, {Length}, {NextByte})";
        }
    }

    public List<TupleString> Compress(string input, int windowSize, int lookaheadBufferSize)
    {
        List<TupleString> compressed = new List<TupleString>();
        int currentPosition = 0;

        while (currentPosition < input.Length)
        {
            int matchLength = 0;
            int matchPosition = 0;

            int searchWindowStart = Math.Max(0, currentPosition - windowSize);

            for (int i = searchWindowStart; i < currentPosition; i++)
            {
                int length = 0;
                while (length < lookaheadBufferSize && currentPosition + length < input.Length &&
                       input[i + length] == input[currentPosition + length])
                {
                    length++;
                }

                if (length > matchLength)
                {
                    matchLength = length;
                    matchPosition = i;
                }
            }

            if (matchLength > 0)
            {
                int offset = currentPosition - matchPosition;
                char nextChar = currentPosition + matchLength < input.Length ? input[currentPosition + matchLength] : '\0';
                compressed.Add(new TupleString(offset, matchLength, nextChar));
                currentPosition += matchLength + 1; 
            }
            else
            {
                compressed.Add(new TupleString(0, 0, input[currentPosition]));
                currentPosition++;
            }
        }

        return compressed;
    }

    public List<TuppleByte> Compress(byte[] input, int windowSize, int lookaheadBufferSize)
    {
        List<TuppleByte> compressed = new List<TuppleByte>();
        int currentPosition = 0;

        while (currentPosition < input.Length)
        {
            int matchLength = 0;
            int matchPosition = 0;

            int searchWindowStart = Math.Max(0, currentPosition - windowSize);

            for (int i = searchWindowStart; i < currentPosition; i++)
            {
                int length = 0;
                while (length < lookaheadBufferSize && currentPosition + length < input.Length &&
                       input[i + length] == input[currentPosition + length])
                {
                    length++;
                }

                if (length > matchLength)
                {
                    matchLength = length;
                    matchPosition = i;
                }
            }

            if (matchLength > 0)
            {
                int offset = currentPosition - matchPosition;
                byte nextByte = currentPosition + matchLength < input.Length ? input[currentPosition + matchLength] : (byte)0;
                compressed.Add(new TuppleByte(offset, matchLength, nextByte));
                currentPosition += matchLength + 1;
            }
            else
            {
                compressed.Add(new TuppleByte(0, 0, input[currentPosition]));
                currentPosition++;
            }
        }

        return compressed;

    }
    public string Decompress(List<TupleString> compressed)
    {
        StringBuilder decompressed = new StringBuilder();

        foreach (var tuple in compressed)
        {
            if (tuple.Length > 0)
            {
                int startPosition = decompressed.Length - tuple.Offset;
                for (int i = 0; i < tuple.Length; i++)
                {
                    decompressed.Append(decompressed[startPosition + i]);
                }
            }
            decompressed.Append(tuple.NextChar);
        }

        return decompressed.ToString();
    }

    public byte[] Decompress(List<TuppleByte> compressed)
    {
        List<byte> decompressed = new List<byte>();

        foreach (var tuple in compressed)
        {
            if (tuple.Length > 0)
            {
                int startPosition = decompressed.Count - tuple.Offset;
                for (int i = 0; i < tuple.Length; i++)
                {
                    decompressed.Add(decompressed[startPosition + i]);
                }
            }
            decompressed.Add(tuple.NextByte);
        }

        return decompressed.ToArray();
    }

    public void WriteToFile(string filePath, List<TuppleByte> compressed)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            foreach (var tuple in compressed)
            {
                writer.Write(tuple.Offset);
                writer.Write(tuple.Length);
                writer.Write(tuple.NextByte);
            }
        }
    }

    public List<TuppleByte> ReadFromFile(string filePath)
    {
        List<TuppleByte> compressed = new List<TuppleByte>();

        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                int offset = reader.ReadInt32();
                int length = reader.ReadInt32();
                byte nextByte = reader.ReadByte();
                compressed.Add(new TuppleByte(offset, length, nextByte));
            }
        }

        return compressed;
    }

    public string TuplesToString(List<TupleString> tuples)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var tuple in tuples)
        {
            sb.Append(tuple.Offset).Append(",").Append(tuple.Length).Append(",").Append(tuple.NextChar).Append(";");
        }

        return sb.ToString();
    }

    public List<TupleString> ConvertStringToLZ77Tuples(string input)
    {
        List<TupleString> tuples = new List<TupleString>();
        string[] tupleStrings = input.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (string tupleString in tupleStrings)
        {
            string[] parts = tupleString.Split(',');
            int offset = int.Parse(parts[0]);
            int length = int.Parse(parts[1]);
            char nextChar = parts[2][0];

            tuples.Add(new TupleString(offset, length, nextChar));
        }

        return tuples;
    }
}