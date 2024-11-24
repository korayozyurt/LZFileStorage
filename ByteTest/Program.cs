string text = "(0,0,'A')";

int unicodeCharacterCount = 128; 
int bitsRequired = (int)Math.Ceiling(Math.Log2(unicodeCharacterCount));
Console.WriteLine($"Her karakter için kullanılacak bit sayısı: {bitsRequired}");

ulong code = 0;
int totalBits = 0;
foreach (char c in text)
{
    int charCode = c; // A=0, B =1, C=2, D=3, E=4..
    code = (code << bitsRequired) | (ulong)charCode; //kod ekleme
    totalBits += bitsRequired;
}

Console.WriteLine($"Kodlanmış: {Convert.ToString((long)code, 2).PadLeft(totalBits, '0')}");

// çözme
for (int i = totalBits - bitsRequired; i >= 0; i-= bitsRequired)
{
    long charCode =  (long)((code >> i) & ((1UL << bitsRequired) - 1));
    char decodedChar = (char)(charCode); // A ya göre geri çöz
    Console.WriteLine(decodedChar);
}

return 0;
void writeBytetoString(byte mByte)
{
    Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
    Console.WriteLine($"Değer: {Convert.ToString(mByte)}");
    Console.WriteLine($"Değer Binary format: {Convert.ToString(mByte, 2)}");
    Console.WriteLine($"Değer Binary format: {Convert.ToString(mByte, 2).PadLeft(16, '0')}");
}

void writeLongBytetoString(ulong code)
{
    Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
    Console.WriteLine($"Değer: {Convert.ToString(code)}");
    Console.WriteLine($"Değer Binary format: {Convert.ToString((long)code, 2)}");
    Console.WriteLine($"Değer Binary format: {Convert.ToString((long)code, 2).PadLeft(16, '0')}");
}