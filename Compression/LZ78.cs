using System.Text;

namespace LZAlgorithms;

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
}