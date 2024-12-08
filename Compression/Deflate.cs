namespace Compression;

public class Deflate
{
    public class LZ77Tuple
    {
        public int Offset { get; set; }
        public int Length { get; set; }
        public char Symbol { get; set; }

        public override string ToString()
        {
            return $"({Offset},{Length},'{Symbol}')";
        }
    }

    public class HuffmanNode
    {
        public int? Value { get; set; }
        public int Frequency { get; set; }
        public HuffmanNode Left { get; set; }
        public HuffmanNode Right { get; set; }
    }
    /*
    static void Main()
    {
        string input = "ABABABABABCABABABABAB";

        // 1. LZ77 Sıkıştırması
        var lz77Compressed = LZ77Compress(input);
        Console.WriteLine("LZ77 Sıkıştırılmış Veri:");
        foreach (var tuple in lz77Compressed)
        {
            Console.WriteLine(tuple);
        }

        // 2. HuffmanBasic Kodlarını Oluştur
        var offsetCodes = BuildHuffmanCodes(lz77Compressed.Select(t => t.Offset));
        var lengthCodes = BuildHuffmanCodes(lz77Compressed.Select(t => t.Length));
        var symbolCodes = BuildHuffmanCodes(lz77Compressed.Select(t => (int)t.Symbol));

        // 3. LZ77 + HuffmanBasic Kodlarını Bit Düzeyinde Yaz
        string compressedFilePath = "lz77_huffman_compressed.bin";
        WriteLZ77HuffmanToBinaryFile(lz77Compressed, offsetCodes, lengthCodes, symbolCodes, compressedFilePath);
        Console.WriteLine($"Sıkıştırılmış veri {compressedFilePath} dosyasına yazıldı.");

        // 4. Dosyayı Oku ve Çöz
        var decompressed = ReadLZ77HuffmanFromBinaryFile(compressedFilePath, offsetCodes, lengthCodes, symbolCodes);
        Console.WriteLine("Çözümlenmiş Metin: " + decompressed);
    }
    */
    public static List<LZ77Tuple> LZ77Compress(string input)
    {
        List<LZ77Tuple> tuples = new List<LZ77Tuple>();
        int searchBuffer = 7;
        int lookaheadBuffer = 4;

        for (int i = 0; i < input.Length;)
        {
            int matchLength = 0;
            int matchDistance = 0;

            for (int j = Math.Max(0, i - searchBuffer); j < i; j++)
            {
                int length = 0;
                while (length < lookaheadBuffer &&
                       i + length < input.Length &&
                       input[j + length] == input[i + length])
                {
                    length++;
                }

                if (length > matchLength)
                {
                    matchLength = length;
                    matchDistance = i - j;
                }
            }

            char nextSymbol = i + matchLength < input.Length ? input[i + matchLength] : '\0';
            tuples.Add(new LZ77Tuple { Offset = matchDistance, Length = matchLength, Symbol = nextSymbol });
            i += matchLength + 1;
        }

        return tuples;
    }

    public static Dictionary<int, string> BuildHuffmanCodes(IEnumerable<int> values)
    {
        // Frekans tablosu oluştur
        var frequency = values.GroupBy(v => v).ToDictionary(g => g.Key, g => g.Count());

        // HuffmanBasic ağacı için öncelik sırasına göre düğümler oluştur
        var priorityQueue = new SortedSet<HuffmanNode>(Comparer<HuffmanNode>.Create((a, b) =>
        {
            int cmp = a.Frequency.CompareTo(b.Frequency);
            return cmp == 0 ? a.Value.GetValueOrDefault().CompareTo(b.Value.GetValueOrDefault()) : cmp;
        }));

        foreach (var kvp in frequency)
        {
            priorityQueue.Add(new HuffmanNode { Value = kvp.Key, Frequency = kvp.Value });
        }

        // HuffmanBasic ağacı oluştur
        while (priorityQueue.Count > 1)
        {
            var left = priorityQueue.Min; priorityQueue.Remove(left);
            var right = priorityQueue.Min; priorityQueue.Remove(right);

            priorityQueue.Add(new HuffmanNode
            {
                Frequency = left.Frequency + right.Frequency,
                Left = left,
                Right = right
            });
        }

        var root = priorityQueue.Min;
        var huffmanCodes = new Dictionary<int, string>();
        GenerateHuffmanCodes(root, "", huffmanCodes);

        return huffmanCodes;
    }

    public static void GenerateHuffmanCodes(HuffmanNode node, string code, Dictionary<int, string> huffmanCodes)
    {
        if (node == null) return;

        // Eğer yaprak düğümse, kodu ekle
        if (node.Value.HasValue)
        {
            huffmanCodes[node.Value.Value] = code;
            return;
        }

        // Sol ve sağ dallara devam et
        GenerateHuffmanCodes(node.Left, code + "0", huffmanCodes);
        GenerateHuffmanCodes(node.Right, code + "1", huffmanCodes);
    }

    public static void WriteLZ77HuffmanToBinaryFile(List<LZ77Tuple> tuples, Dictionary<int, string> offsetCodes, Dictionary<int, string> lengthCodes, Dictionary<int, string> symbolCodes, string filePath)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            byte currentByte = 0;
            int bitPosition = 0;

            void WriteBits(string bits)
            {
                foreach (char bit in bits)
                {
                    if (bit == '1')
                    {
                        currentByte |= (byte)(1 << (7 - bitPosition));
                    }

                    bitPosition++;
                    if (bitPosition == 8)
                    {
                        writer.Write(currentByte);
                        currentByte = 0;
                        bitPosition = 0;
                    }
                }
            }

            foreach (var tuple in tuples)
            {
                WriteBits(offsetCodes[tuple.Offset]);
                WriteBits(lengthCodes[tuple.Length]);
                WriteBits(symbolCodes[(int)tuple.Symbol]);
            }

            if (bitPosition > 0)
            {
                writer.Write(currentByte);
            }
        }
    }

    public static string ReadLZ77HuffmanFromBinaryFile(string filePath, Dictionary<int, string> offsetCodes, Dictionary<int, string> lengthCodes, Dictionary<int, string> symbolCodes)
    {
        // Ters HuffmanBasic kodlarını oluştur
        var reverseOffsetCodes = offsetCodes.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        var reverseLengthCodes = lengthCodes.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        var reverseSymbolCodes = symbolCodes.ToDictionary(kvp => kvp.Value, kvp => (char)kvp.Key);

        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            HuffmanNode current = null;
            string decoded = "";

            // Kodları çöz
            // TODO: LZ çözme kısmı
            return decoded;
        }
    }
}