using System.Text;

namespace Compression;

public class LZ78
{
    public class Tuple
    {
        public int Index { get; set; }
    }

    public class TupleChar : Tuple
    {
        public char NextChar { get; set; }

        public TupleChar(int index, char nextChar)
        {
            Index = index;
            NextChar = nextChar;
        }

        public override string ToString()
        {
            return $"({Index}, '{NextChar}')";
        }
    }

    public class TupleByte : Tuple
    {
        public byte NextByte { get; set; }
        public TupleByte(int index, byte nextByte)
        {
            Index = index;
            NextByte = nextByte;
        }

        public override string ToString()
        {
            return $"({Index}, '{NextByte}')";
        }
    }

    public List<TupleChar> Compress(string input)
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        List<TupleChar> compressed = new List<TupleChar>();
        string currentString = String.Empty;
        int dictSize = 1;       // dictionary index numarası 1'den başlıyor

        foreach (char c in input)
        {
            string concatenated = currentString + c;

            if (dictionary.ContainsKey(concatenated))
            {
                currentString = concatenated;
            }
            else
            {
                var index = currentString == string.Empty ? 0 : dictionary[currentString];
                compressed.Add(new TupleChar(index, c));
                dictionary[concatenated] = dictSize++;
                currentString = string.Empty;
            }
        }

        if(currentString != string.Empty)
        {
            int index = dictionary[currentString];
            compressed.Add(new TupleChar(index, '\0'));     // Sonraki karakter olmadığı için null karakter eklenir.
        }

        return compressed;
    }

    public List<TupleByte> Compress(byte[] input)
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        List<TupleByte> compressed = new List<TupleByte>();
        string currentString = String.Empty;
        int dictSize = 1;

        foreach (byte b in input)
        {
            string concatenated = currentString + (char)b;
            if (dictionary.ContainsKey(concatenated))
            {
                currentString = concatenated;
            }
            else
            {
                var index = currentString == string.Empty ? 0 : dictionary[currentString];
                compressed.Add(new TupleByte(index, b));
                dictionary[concatenated] = dictSize++;
                currentString = string.Empty;
            }
        }
        if (currentString != string.Empty)
        {
            int index = dictionary[currentString];
            compressed.Add(new TupleByte(index, 0)); // Sonraki karakter olmadığı için 0 değeri ekliyoruz
        }

        return compressed;
    } 

    public string Decompress(List<TupleChar> compressed)
    {
        Dictionary<int, string> dictionary = new Dictionary<int, string>();
        StringBuilder decompressed = new StringBuilder();
        int dictSize = 1;

        foreach (TupleChar item in compressed)
        {
            string entry = item.Index == 0 ? string.Empty : dictionary[item.Index];
            string newEntry = entry + item.NextChar;

            decompressed.Append(newEntry);
            dictionary[dictSize++] = newEntry;
        }

        return decompressed.ToString();
    }

    public byte[] Decompress(List<TupleByte> compressed)
    {
        Dictionary<int, List<byte>> dictionary = new Dictionary<int, List<byte>>();
        List<byte> decompressed = new List<byte>();
        int dictSize = 1;

        foreach (TupleByte item in compressed)
        {
            List<byte> entry = item.Index == 0 ? new List<byte>() : dictionary[item.Index];
            List<byte> newEntry = new List<byte>(entry);
            newEntry.Add(item.NextByte);

            decompressed.AddRange(newEntry);
            dictionary[dictSize++] = newEntry;
        }

        return decompressed.ToArray();
    }

    public void WriteToFile(string filePath, List<TupleByte> compressed)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            foreach (var tuple in compressed)
            {
                writer.Write(tuple.Index);  
                writer.Write(tuple.NextByte);
            }
        }
    }

    public List<TupleByte> ReadFromFile(string filePath)
    {
        List<TupleByte> compressed = new List<TupleByte>();

        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                int index = reader.ReadInt32();
                byte nextByte = reader.ReadByte();
                compressed.Add(new TupleByte(index, nextByte));
            }
        }

        return compressed;
    }

}