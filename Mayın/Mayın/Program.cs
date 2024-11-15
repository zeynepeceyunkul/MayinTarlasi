using System;

class Program
{
    static void Main(string[] args)
    {
        // Sabit boyut ve mayın sayısı tanımlanıyor
        const int boyut = 20;
        const int mayinSayisi = 40;

        // Oyun tahtası ve durum dizileri oluşturuluyor
        char[,] tahta = new char[boyut, boyut];
        bool[,] mayinlar = new bool[boyut, boyut];
        bool[,] acildi = new bool[boyut, boyut];
        Random rastgele = new Random();

        // Tahta ve mayınlar başlatılıyor
        TahtayiBaslat(tahta, mayinlar, mayinSayisi, rastgele);

        // Oyun durum değişkenleri
        bool oyunBitti = false;
        int acilanHucreler = 0;
        int toplamGuvenliHucre = boyut * boyut - mayinSayisi;

        // Oyun döngüsü
        while (!oyunBitti && acilanHucreler < toplamGuvenliHucre)
        {
            Console.Clear();
            TahtayiGoster(tahta); // Tahta ekranda gösteriliyor
            Console.WriteLine("Hücre seç (satır ve sütun): ");

            try
            {
                // Kullanıcıdan hücre girişi alınıyor
                Console.Write("Satır (1-20): ");
                int satir = int.Parse(Console.ReadLine()) - 1;
                Console.Write("Sütun (1-20): ");
                int sutun = int.Parse(Console.ReadLine()) - 1;

                // Geçersiz giriş kontrolü
                if (satir < 0 || satir >= boyut || sutun < 0 || sutun >= boyut)
                {
                    Console.WriteLine("Geçersiz konum! Tekrar deneyin.");
                    continue;
                }

                // Zaten açılmış hücre kontrolü
                if (acildi[satir, sutun])
                {
                    Console.WriteLine("Bu hücre zaten açıldı! Başka bir hücre seçin.");
                    continue;
                }

                // Mayına basma durumu
                if (mayinlar[satir, sutun])
                {
                    oyunBitti = true;
                    Console.Clear();
                    MayinlariGoster(mayinlar); // Tüm mayınlar gösteriliyor
                    Console.WriteLine("Mayına bastınız! Oyun bitti.");
                }
                else
                {
                    // Çevredeki mayın sayısı hesaplanıyor
                    int cevredekiMayinSayisi = CevredekiMayinSayisiniBul(satir, sutun, mayinlar);
                    tahta[satir, sutun] = cevredekiMayinSayisi > 0 ? cevredekiMayinSayisi.ToString()[0] : ' ';
                    acildi[satir, sutun] = true;
                    acilanHucreler++;

                    // Çevresinde mayın olmayan hücreler otomatik açılıyor
                    if (cevredekiMayinSayisi == 0)
                    {
                        BosHucreleriAc(satir, sutun, tahta, mayinlar, acildi);
                    }
                }
            }
            catch
            {
                // Geçersiz giriş için hata mesajı
                Console.WriteLine("Geçersiz giriş! Lütfen doğru bir sayı girin.");
            }
        }

        // Tüm güvenli hücreler açıldığında oyun kazanılıyor
        if (acilanHucreler == toplamGuvenliHucre)
        {
            Console.Clear();
            TahtayiGoster(tahta);
            Console.WriteLine("Tebrikler! Tüm güvenli hücreleri açtınız.");
        }

        Console.WriteLine("Çıkmak için bir tuşa basın...");
        Console.ReadKey();
    }

    // Tahtayı başlatır ve mayınları rastgele yerleştirir
    static void TahtayiBaslat(char[,] tahta, bool[,] mayinlar, int mayinSayisi, Random rastgele)
    {
        int boyut = tahta.GetLength(0);

        // Tüm hücreler varsayılan değerlerle dolduruluyor
        for (int i = 0; i < boyut; i++)
        {
            for (int j = 0; j < boyut; j++)
            {
                tahta[i, j] = '*'; // Gizli hücre
                mayinlar[i, j] = false; // Başlangıçta tüm hücreler güvenli
            }
        }

        // Mayınları rastgele hücrelere yerleştir
        int yerlestirilenMayin = 0;
        while (yerlestirilenMayin < mayinSayisi)
        {
            int satir = rastgele.Next(boyut);
            int sutun = rastgele.Next(boyut);

            // Aynı hücreye birden fazla mayın yerleştirilmesini önler
            if (!mayinlar[satir, sutun])
            {
                mayinlar[satir, sutun] = true;
                yerlestirilenMayin++;
            }
        }
    }

    // Tahtayı ekranda gösterir
    static void TahtayiGoster(char[,] tahta)
    {
        int boyut = tahta.GetLength(0);

        // Sütun numaralarını yazdır
        Console.Write("   ");
        for (int i = 1; i <= boyut; i++)
        {
            Console.Write($"{i,2} ");
        }
        Console.WriteLine();

        // Satır numaraları ve hücre içeriklerini yazdır
        for (int i = 0; i < boyut; i++)
        {
            Console.Write($"{i + 1,2} ");
            for (int j = 0; j < boyut; j++)
            {
                Console.Write($"{tahta[i, j],2} ");
            }
            Console.WriteLine();
        }
    }

    // Tüm mayınları ekranda gösterir
    static void MayinlariGoster(bool[,] mayinlar)
    {
        int boyut = mayinlar.GetLength(0);

        // Sütun numaralarını yazdır
        Console.Write("   ");
        for (int i = 1; i <= boyut; i++)
        {
            Console.Write($"{i,2} ");
        }
        Console.WriteLine();

        // Mayın yerlerini 'X' olarak göster, diğer hücreleri '.' ile işaretle
        for (int i = 0; i < boyut; i++)
        {
            Console.Write($"{i + 1,2} ");
            for (int j = 0; j < boyut; j++)
            {
                Console.Write(mayinlar[i, j] ? "X " : ". ");
            }
            Console.WriteLine();
        }
    }

    // Çevredeki mayınların sayısını bulur
    static int CevredekiMayinSayisiniBul(int satir, int sutun, bool[,] mayinlar)
    {
        int sayi = 0;
        int boyut = mayinlar.GetLength(0);

        // Çevredeki hücreleri kontrol et
        for (int i = satir - 1; i <= satir + 1; i++)
        {
            for (int j = sutun - 1; j <= sutun + 1; j++)
            {
                if (i >= 0 && i < boyut && j >= 0 && j < boyut && mayinlar[i, j])
                {
                    sayi++;
                }
            }
        }

        return sayi;
    }

    // Çevresinde mayın olmayan hücreleri otomatik açar
    static void BosHucreleriAc(int satir, int sutun, char[,] tahta, bool[,] mayinlar, bool[,] acildi)
    {
        int boyut = tahta.GetLength(0);

        for (int i = satir - 1; i <= satir + 1; i++)
        {
            for (int j = sutun - 1; j <= sutun + 1; j++)
            {
                if (i >= 0 && i < boyut && j >= 0 && j < boyut && !acildi[i, j] && !mayinlar[i, j])
                {
                    acildi[i, j] = true;
                    int cevredekiMayinSayisi = CevredekiMayinSayisiniBul(i, j, mayinlar);
                    tahta[i, j] = cevredekiMayinSayisi > 0 ? cevredekiMayinSayisi.ToString()[0] : ' ';

                    // Eğer çevresinde mayın yoksa, komşu hücreleri de aç
                    if (cevredekiMayinSayisi == 0)
                    {
                        BosHucreleriAc(i, j, tahta, mayinlar, acildi);
                    }
                }
            }
        }
    }
}
