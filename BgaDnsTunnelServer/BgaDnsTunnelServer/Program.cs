using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Dns;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApplication12
{

    class Program
    {
        private const int InByteSize = 8;

        private const int OutByteSize = 5;

        private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        private static PacketCommunicator communicator;
        private static List<string> kontrol = new List<string>();
        private static List<string> LinkKontrol = new List<string>();

        private static List<string> yedekList = new List<string>();
        private static List<string> islemList = new List<string>();

        private static List<string> Domain_List = new List<string>();
        private static string[] DomainListesi;
        private static string[] domainAdi;
        private static int sayac;
        private static int aaa = 0;
        private static int kontroll = 0;
        static void Main(string[] args)
        {

            string dosyaYolu;
            string dosyaYoluDomain;
            Console.WriteLine("Domain yolunu giriniz");
            dosyaYoluDomain = Console.ReadLine();
            domainAdi = File.ReadAllLines(dosyaYoluDomain);
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine("Dosya yolunu giriniz");
            dosyaYolu = Console.ReadLine();
            Console.WriteLine("-------------------------------------------------------------------");
            DomainListesi = File.ReadAllLines(dosyaYolu);

            foreach (string i in DomainListesi)
            {

                Domain_List.Add("." + i);
            }

            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;

            if (allDevices.Count == 0)
            {
                Console.WriteLine("Hata");
                return;
            }

            // Print the list
            for (int i = 0; i != allDevices.Count; ++i)
            {
                LivePacketDevice device = allDevices[i];
                Console.Write((i + 1) + ". " + device.Name);
                if (device.Description != null)
                    Console.WriteLine(" (" + device.Description + ")");
                else
                    Console.WriteLine(" (No description available)");
            }

            int deviceIndex = 0;
            do
            {
                Console.WriteLine("Enter the interface number (1-" + allDevices.Count + "):");
                string deviceIndexString = Console.ReadLine();
                if (!int.TryParse(deviceIndexString, out deviceIndex) ||
                    deviceIndex < 1 || deviceIndex > allDevices.Count)
                {
                    deviceIndex = 0;
                }
            } while (deviceIndex == 0);

            // Take the selected adapter
            PacketDevice selectedDevice = allDevices[deviceIndex - 1];

            // Open the device
            using (communicator =
                selectedDevice.Open(65536,

                                    PacketDeviceOpenAttributes.Promiscuous,
                                    10000))
            {

                if (communicator.DataLink.Kind != DataLinkKind.Ethernet)
                {
                    Console.WriteLine("This program works only on Ethernet networks.");
                    return;
                }

                using (BerkeleyPacketFilter filter = communicator.CreateFilter("ip and udp"))
                {

                    communicator.SetFilter(filter);
                }

                Console.WriteLine("Listening on " + selectedDevice.Description + "...");


                communicator.ReceivePackets(0, PacketHandler);


            }


        }

        private static void PacketHandler(Packet packet)
        {


            IpV4Datagram ip = packet.Ethernet.IpV4;
            UdpDatagram udp = ip.Udp;
            DnsDatagram dnsBilgi = udp.Dns;

            DnsLayer dnsa = new DnsLayer();

            ReadOnlyCollection<DnsQueryResourceRecord> a = dnsBilgi.Queries;



            if (udp.DestinationPort == 53)

            {


                string yazi = udp.Dns.Queries[udp.Dns.Queries.Count - 1].DomainName.ToString();


                char[] aa = yazi.ToCharArray();
                string[] lines = { };

                for (int i = 0; i < Domain_List.Count; i++)
                {
                    if (yazi.IndexOf(Domain_List[i]) > 0)
                    {
                        lines = Regex.Split(yazi, Domain_List[i]);
                        aaa = 2;
                        goto gir;


                    }
                    else
                    {
                        aaa = 0;
                    }


                }
            gir:
                if (aaa > 1)
                {
                    for (int d = 0; d < domainAdi.Length; d++)
                    {

                        if (kontrol.Contains(lines[0]) == false && yazi.IndexOf(domainAdi[d]) > 0 && lines.Count() > 1)
                        {
                            LinkKontrol.Add(yazi);
                            kontrol.Add(lines[0]);
                            Console.WriteLine(ip.Source + ":" + udp.SourcePort + " -> " + ip.Destination + ":" + udp.DestinationPort + " " + lines[0]);
                        }
                    }

                    if (lines[0].Contains("bitti") == true)
                    {
                        Console.WriteLine("Kaçan dosya için ");
                        string[] surekliDinle = Regex.Split(lines[0], "-");
                        string surekliDinleNor = surekliDinle[0].Substring(surekliDinle[0].Length - 3);
                        var newList = kontrol.ToList();

                        foreach (string i in kontrol)
                        {
                            if (i.Contains(surekliDinleNor) == true)
                            {
                                islemList.Add(i);
                                newList.Remove(i);
                            }



                        }
                        kontrol = newList;
                        if (islemList.Count > 2)
                        {
                            DosyaYarat();
                        }
                        else
                        {
                            Sifirla();
                            Console.WriteLine("Geçersiz işlem algılandı");
                        }
                    }

                }
                if (aaa < 1)
                {

                    Console.WriteLine("gecersiz baglanti girdi");

                }

            }


            Sifirla();

        }
        public static string okan(PcapDotNet.Packets.Dns.DnsDomainName a)
        {
            string yazi = a.ToString();
            return yazi;

        }

        public static void Durdur()
        {

            communicator.Break();

        }
        public static string Kontrol(string a)
        {
            return a;
        }
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        static string GetString(byte[] bytes)
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
        internal static byte[] FromBase32String(string base32String)
        {
            // Check if string is null
            if (base32String == null)
            {
                return null;
            }
            // Check if empty
            else if (base32String == string.Empty)
            {
                return new byte[0];
            }

            // Convert to upper-case
            string base32StringUpperCase = base32String.ToUpperInvariant();

            // Prepare output byte array
            byte[] outputBytes = new byte[base32StringUpperCase.Length * OutByteSize / InByteSize];

            // Check the size
            if (outputBytes.Length == 0)
            {
                throw new ArgumentException("Specified string is not valid Base32 format because it doesn't have enough data to construct a complete byte array");
            }

            // Position in the string
            int base32Position = 0;

            // Offset inside the character in the string
            int base32SubPosition = 0;

            // Position within outputBytes array
            int outputBytePosition = 0;

            // The number of bits filled in the current output byte
            int outputByteSubPosition = 0;

            // Normally we would iterate on the input array but in this case we actually iterate on the output array
            // We do it because output array doesn't have overflow bits, while input does and it will cause output array overflow if we don't stop in time
            while (outputBytePosition < outputBytes.Length)
            {
                // Look up current character in the dictionary to convert it to byte
                int currentBase32Byte = Base32Alphabet.IndexOf(base32StringUpperCase[base32Position]);

                // Check if found
                if (currentBase32Byte < 0)
                {
                    throw new ArgumentException(string.Format("Specified string is not valid Base32 format because character \"{0}\" does not exist in Base32 alphabet", base32String[base32Position]));
                }

                // Calculate the number of bits we can extract out of current input character to fill missing bits in the output byte
                int bitsAvailableInByte = Math.Min(OutByteSize - base32SubPosition, InByteSize - outputByteSubPosition);

                // Make space in the output byte
                outputBytes[outputBytePosition] <<= bitsAvailableInByte;

                // Extract the part of the input character and move it to the output byte
                outputBytes[outputBytePosition] |= (byte)(currentBase32Byte >> (OutByteSize - (base32SubPosition + bitsAvailableInByte)));

                // Update current sub-byte position
                outputByteSubPosition += bitsAvailableInByte;

                // Check overflow
                if (outputByteSubPosition >= InByteSize)
                {
                    // Move to the next byte
                    outputBytePosition++;
                    outputByteSubPosition = 0;
                }

                // Update current base32 byte completion
                base32SubPosition += bitsAvailableInByte;

                // Check overflow or end of input array
                if (base32SubPosition >= OutByteSize)
                {
                    // Move to the next character
                    base32Position++;
                    base32SubPosition = 0;
                }
            }

            return outputBytes;
        }
        public static void DosyaYarat()
        {
            Console.WriteLine("Başarıyla sonlandi");
            char[] a = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
        KontrolNoktasi:

            string IslemNo = islemNo();
            if (IslemNo == "1")
            {

                try
                {
                    string yazi = "";
                    for (int i = 3; i < islemList.Count - 1; i++)
                    {
                        if (islemList[i].IndexOf("sifr") > 0)
                        {
                            string[] ba = Regex.Split(islemList[i], "sifr");

                            yazi += ba[0];

                        }
                        else
                        {
                            string[] ba = Regex.Split(islemList[i], "-");
                            yazi += ba[1];

                        }
                    }
                    byte[] yeni4 = GetBytes(yazi);

                    string[] dosya0 = Regex.Split(islemList[0], "-");
                    File.WriteAllBytes(dosya0[1] + ".txt", yeni4);
                    StreamReader oku;
                    oku = File.OpenText("dns.txt");
                    string metin2 = oku.ReadLine();
                    byte[] metin3 = Convert.FromBase64String(metin2);
                    string[] klasor = Regex.Split(islemList[0], "-");
                    byte[] klasorB = Convert.FromBase64String(klasor[1]);
                    string klasorS = GetString(klasorB);
                    string[] dosya = Regex.Split(islemList[1], "-");
                    byte[] dosyaB = Convert.FromBase64String(dosya[1]);
                    string dosyaS = GetString(dosyaB);

                    Directory.CreateDirectory(klasorS);
                    string dosyaYolu = klasorS + "\\" + dosyaS;
                    File.WriteAllBytes(dosyaYolu, metin3);
                    oku.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata:" + ex.Message);
                }




            }
            else if (IslemNo == "2")
            {
                try
                {
                    string yazi = "";
                    for (int i = 3; i < islemList.Count - 1; i++)
                    {
                        if (islemList[i].IndexOf("sifr") > 0)
                        {
                            string[] ba = Regex.Split(islemList[i], "sifr");

                            yazi += ba[0];

                        }
                        else
                        {
                            string[] ba = Regex.Split(islemList[i], "-");
                            yazi += ba[1];




                        }
                    }
                    StreamWriter aa = new StreamWriter("dns.txt");
                    aa.WriteLine(yazi);
                    aa.Close();
                    StreamReader oku;
                    oku = File.OpenText("dns.txt");
                    string metin2 = oku.ReadLine();
                    metin2 = metin2.Replace("arti", "+");
                    metin2 = metin2.Replace("bolu", "/");
                    metin2 = metin2.Replace(" ", "+");
                    int modAl = metin2.Length % 4;
                    if (modAl > 0)
                    {
                        metin2 += new string('=', 4 - modAl);
                    }
                    byte[] metin3 = Convert.FromBase64String(metin2);

                    string[] klasor = Regex.Split(islemList[0], "-");
                    int modAl1 = klasor[1].Length % 4;
                    if (modAl1 > 0)
                    {
                        klasor[1] += new string('=', 4 - modAl1);
                    }
                    byte[] klasorB = Convert.FromBase64String(klasor[1]);
                    string klasorS = GetString(klasorB);
                    string[] dosya = Regex.Split(islemList[1], "-");
                    int modAl2 = dosya[1].Length % 4;
                    if (modAl2 > 0)
                    {
                        dosya[1] += new string('=', 4 - modAl2);
                    }
                    byte[] dosyaB = Convert.FromBase64String(dosya[1]);
                    string dosyaS = GetString(dosyaB);
                    Directory.CreateDirectory(klasorS);
                    string dosyaYolu = klasorS + "\\" + dosyaS;
                    File.WriteAllBytes(dosyaYolu, metin3);
                    oku.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata:" + ex.Message);
                }


            }
            else if (IslemNo == "3")
            {
                try
                {
                    string yazi = "";
                    for (int i = 3; i < islemList.Count - 1; i++)
                    {
                        if (islemList[i].IndexOf("sifr") > 0)
                        {
                            string[] ba = Regex.Split(islemList[i], "sifr");

                            yazi += ba[0];

                        }
                        else
                        {
                            string[] ba = Regex.Split(islemList[i], "-");
                            yazi += ba[1];



                        }
                    }
                    StreamWriter aa = new StreamWriter("dns.txt");
                    aa.WriteLine(yazi);
                    aa.Close();
                    StreamReader oku;
                    oku = File.OpenText("dns.txt");
                    string metin2 = oku.ReadLine();
                    metin2 = metin2.Replace("arti", "+");
                    metin2 = metin2.Replace("bolu", "/");
                    metin2 = metin2.Replace(" ", "+");

                    byte[] metin3 = FromBase32String(metin2);

                    string[] klasor = Regex.Split(islemList[0], "-");
                    byte[] klasorB = FromBase32String(klasor[1]);
                    string klasorS = GetString(klasorB);
                    string[] dosya = Regex.Split(islemList[1], "-");
                    byte[] dosyaB = FromBase32String(dosya[1]);
                    string dosyaS = GetString(dosyaB);
                    Directory.CreateDirectory(klasorS);
                    string dosyaYolu = klasorS + "\\" + dosyaS;
                    File.WriteAllBytes(dosyaYolu, metin3);
                    oku.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata:" + ex.Message);
                }


            }
            else
            {
                Console.WriteLine("Hatali Secim");
            }

        }
        public static void Sifirla()
        {
            islemList.Clear();
            yedekList.Clear();
        }
        public static void AsilSifirla()
        {
            kontrol.Clear();
        }
        public static string islemNo()
        {
            string[] islemNo = Regex.Split(islemList[2], "-");
            return islemNo[1];
        }

    }
}
