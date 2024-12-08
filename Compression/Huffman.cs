using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Compression;

public class Huffman
{
    public Dictionary<string, int> frequencyTable = new Dictionary<string, int>();

    private PriorityQueue<HuffmanNode, int> huffmanTree = new PriorityQueue<HuffmanNode, int>();

    public Dictionary<string, (ulong code, int length)> huffmanTable = new Dictionary<string, (ulong code, int length)>();

    (List<byte>, int totalBits) encodedData = new(new List<byte>(), 0);

    public HuffmanNode RootHuffmanNode { get; set; }


    public async Task<(List<byte>, int totalBits)> Compress(string text, int groupSize)
    {

        encodedData = new(new List<byte>(), 0);
        huffmanTable = new Dictionary<string, (ulong code, int length)>();

        var frequencies = CalculateFrequency(text,groupSize);
        RootHuffmanNode = BuildHuffmanTree();
        GenerateHuffmanCodes(RootHuffmanNode, 0, 0, huffmanTable);

        return Encode(text, huffmanTable);
    }

    public class HuffmanNode
    {
        public string character { get; set; }
        public List<byte> code { get; set; }
        public int Frequency { get; set; }
        public HuffmanNode left { get; set; }
        public HuffmanNode right { get; set; }
    }

    private Dictionary<string, int> CalculateFrequency(string binaryText, int groupSize)
    {
        frequencyTable = new Dictionary<string, int>();

        // İkili diziyi belirtilen gruplara ayır
        for (int i = 0; i < binaryText.Length; i += groupSize)
        {
            string group = binaryText.Substring(i, Math.Min(groupSize, binaryText.Length - i));

            if (!frequencyTable.ContainsKey(group))
            {
                frequencyTable.Add(group, 1);
                continue;
            }

            frequencyTable[group]++;
        }

        // Frekans tablosunu sıralı bir şekilde döndür
        return frequencyTable.OrderByDescending(d => d.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    private (List<byte>, int totalBits) Encode(string text, Dictionary<string, (ulong code, int length)> huffmanTable)
    {
        List<byte> encodedBytes = new List<byte>();
        int bitPosition = 0;
        byte currentByte = 0;
        int totalBits = 0;

        int index = 0;
        while (index < text.Length)
        {
            foreach (var key in huffmanTable.Keys)
            {
                if (text.Substring(index).StartsWith(key))
                {
                    var (code, length) = huffmanTable[key];

                    for (int i = length - 1; i >= 0; i--)
                    {
                        if (((code >> i) & 1) == 1)
                        {
                            currentByte |= (byte)(1 << (7 - bitPosition));
                        }

                        bitPosition++;
                        totalBits++;

                        if (bitPosition == 8)
                        {
                            encodedBytes.Add(currentByte);
                            currentByte = 0;
                            bitPosition = 0;
                        }
                    }

                    index += key.Length;
                    break;
                }
            }
        }

        // Kalan bitleri ekle
        if (bitPosition > 0)
        {
            encodedBytes.Add(currentByte);
        }

        return (encodedBytes, totalBits);
    }


    public string Decode(List<byte> encodedBytes, int totalBits, HuffmanNode root)
    {
        HuffmanNode current = root;
        StringBuilder decodedData = new StringBuilder();
        int processedBits = 0;

        foreach (byte b in encodedBytes)
        {
            for (int i = 7; i >= 0; i--)
            {
                if (processedBits == totalBits)
                    return decodedData.ToString();

                current = ((b >> i) & 1) == 0 ? current.left : current.right;

                if (current.right == null && current.right == null)
                {
                    decodedData.Append(current.character);
                    current = root; 
                }

                processedBits++;
            }
        }

        return decodedData.ToString();
    }



    private void GenerateHuffmanCodes(HuffmanNode node, ulong currentCode, int depth, Dictionary<string, (ulong code, int length)> huffmanTable)
    {
        if (node == null) return;

        if (node.character != null) // Yaprak düğümde bir karakter varsa
        {
            huffmanTable[node.character] = (currentCode, depth);
        }

        GenerateHuffmanCodes(node.left, currentCode << 1, depth + 1, huffmanTable);

        GenerateHuffmanCodes(node.right, currentCode << 1 | 1, depth + 1, huffmanTable);
    }

    private HuffmanNode BuildHuffmanTree()
    {
        huffmanTree = new PriorityQueue<HuffmanNode, int>();
        foreach (var frequency in frequencyTable)
        {
            huffmanTree.Enqueue(new HuffmanNode() { character = frequency.Key, Frequency = frequency.Value }, frequency.Value);
        }

        while (huffmanTree.Count > 1)
        {
            var left = huffmanTree.Dequeue();
            var right = huffmanTree.Dequeue();

            var parent = new HuffmanNode() { left = left, right = right, Frequency = left.Frequency + right.Frequency };
            huffmanTree.Enqueue(parent, parent.Frequency);
        }

        return huffmanTree.Peek();
    }
}