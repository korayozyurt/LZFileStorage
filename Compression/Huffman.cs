using System.Text;
using LZAlgorithms;

namespace Compression;

public class Huffman
{
    public class HuffmanNode
    {
        public byte Symbol { get; set; }
        public int Frequency { get; set; }
        public HuffmanNode Left { get; set; }
        public HuffmanNode Right { get; set; }

        public bool IsLeaf => Left == null && Right == null;
    }

    public class HuffmanNodeString
    {
        public char Symbol { get; set; }
        public int Frequency { get; set; }
        public HuffmanNodeString Left { get; set; }
        public HuffmanNodeString Right { get; set; }
        public bool IsLeaf => Left == null && Right == null;
    }
    #region LZ77Bytes
    // Build Huffman Table for LZ77 Tuple byte
    public Dictionary<byte, string> BuildHuffmanTable(List<LZ77.TuppleByte> tuples)
    {
        Dictionary<byte, int> frequencyTable = new Dictionary<byte, int>();

        // Sıkıştırılmış Tuple'ların frekansını hesapla (sadece NextByte için Huffman uygulanacak)
        foreach (var tuple in tuples)
        {
            if (frequencyTable.ContainsKey(tuple.NextByte))
                frequencyTable[tuple.NextByte]++;
            else
                frequencyTable[tuple.NextByte] = 1;
        }

        // Huffman Ağacı Oluşturma
        PriorityQueue<HuffmanNode, int> priorityQueue = new PriorityQueue<HuffmanNode, int>();
        foreach (var item in frequencyTable)
        {
            priorityQueue.Enqueue(new HuffmanNode { Symbol = item.Key, Frequency = item.Value }, item.Value);
        }

        while (priorityQueue.Count > 1)
        {
            HuffmanNode left = priorityQueue.Dequeue();
            HuffmanNode right = priorityQueue.Dequeue();
            HuffmanNode parent = new HuffmanNode
            {
                Frequency = left.Frequency + right.Frequency,
                Left = left,
                Right = right
            };
            priorityQueue.Enqueue(parent, parent.Frequency);
        }

        // Huffman Ağacı Oluştu
        HuffmanNode root = priorityQueue.Dequeue();
        Dictionary<byte, string> huffmanTable = new Dictionary<byte, string>();
        TraverseHuffmanTree(root, string.Empty, huffmanTable);

        return huffmanTable;
    }

    private void TraverseHuffmanTree(HuffmanNode node, string code, Dictionary<byte, string> huffmanTable)
    {
        if (node.IsLeaf)
        {
            huffmanTable[node.Symbol] = code;
        }
        else
        {
            TraverseHuffmanTree(node.Left, code + "0", huffmanTable);
            TraverseHuffmanTree(node.Right, code + "1", huffmanTable);
        }
    }

    // Huffman Kodlama
    public string HuffmanEncode(List<LZ77.TuppleByte> tuples, Dictionary<byte, string> huffmanTable)
    {
        StringBuilder encodedData = new StringBuilder();

        foreach (var tuple in tuples)
        {
            encodedData.Append(huffmanTable[tuple.NextByte]);
        }

        return encodedData.ToString();
    }
    #endregion

    public Dictionary<char, string> BuildHuffmanTable(List<LZ77.TupleString> tuples)
    {
        Dictionary<char, int> frequencyTable = new Dictionary<char, int>();

        // Sıkıştırılmış Tuple'ların nextChar'larının frekansını hesapla
        foreach (var tuple in tuples)
        {
            if (frequencyTable.ContainsKey(tuple.NextChar))
                frequencyTable[tuple.NextChar]++;
            else
                frequencyTable[tuple.NextChar] = 1;
        }

        // Huffman Ağacı Oluşturma
        PriorityQueue<HuffmanNodeString, int> priorityQueue = new PriorityQueue<HuffmanNodeString, int>();
        foreach (var item in frequencyTable)
        {
            priorityQueue.Enqueue(new HuffmanNodeString { Symbol = item.Key, Frequency = item.Value }, item.Value);
        }

        while (priorityQueue.Count > 1)
        {
            HuffmanNodeString left = priorityQueue.Dequeue();
            HuffmanNodeString right = priorityQueue.Dequeue();
            HuffmanNodeString parent = new HuffmanNodeString
            {
                Frequency = left.Frequency + right.Frequency,
                Left = left,
                Right = right
            };
            priorityQueue.Enqueue(parent, parent.Frequency);
        }

        // Huffman Ağacı Oluştu
        HuffmanNodeString root = priorityQueue.Dequeue();
        Dictionary<char, string> huffmanTable = new Dictionary<char, string>();
        TraverseHuffmanTree(root, string.Empty, huffmanTable);

        return huffmanTable;
    }

    private void TraverseHuffmanTree(HuffmanNodeString node, string code, Dictionary<char, string> huffmanTable)
    {
        if (node.IsLeaf)
        {
            huffmanTable[node.Symbol] = code;
        }
        else
        {
            TraverseHuffmanTree(node.Left, code + "0", huffmanTable);
            TraverseHuffmanTree(node.Right, code + "1", huffmanTable);
        }
    }

    // Huffman Kodlama
    public string HuffmanEncode(List<LZ77.TupleString> tuples, Dictionary<char, string> huffmanTable)
    {
        StringBuilder encodedData = new StringBuilder();

        foreach (var tuple in tuples)
        {
            encodedData.Append(huffmanTable[tuple.NextChar]);
        }

        return encodedData.ToString();
    }
}