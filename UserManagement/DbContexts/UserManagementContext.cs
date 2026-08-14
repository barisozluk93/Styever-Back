using Microsoft.EntityFrameworkCore;
using UserManagement.Entity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserManagement.DbContexts
{
    public class UserManagementContext : DbContext
    {
        public UserManagementContext(DbContextOptions<UserManagementContext> options) : base(options)
        {
        }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<UserPayment> UserPayments { get; set; }
        public DbSet<UserVoucher> UserVouchers { get; set; }
        public DbSet<ShopierPayment> ShopierPayments { get; set; }
        public DbSet<UserAgreementAcceptance> UserAgreementAcceptances { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<LegalContent> LegalContents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShopierPayment>().HasIndex(x => x.Reference).IsUnique();
            modelBuilder.Entity<ShopierPayment>().HasIndex(x => x.ShopierOrderId).IsUnique();
            modelBuilder.Entity<UserAgreementAcceptance>().HasIndex(x => new { x.UserId, x.AcceptedDate });

            modelBuilder.Entity<Plan>().HasData(
                new Plan { Id = 2, Currency = "₺", IsDeleted = false, IsPopular = false, Name = "Origin", NameEn = "Origin", Period = "Yıl", PeriodEn = "Year", Price = 499.0, Properties = "1 Anı Sayfası\n1 Fotoğraf\n1 Anı Yazısı Alanı (1000 Karakter)\nHatırlatma Bildirimleri", PropertiesEn = "1 Memory Page\n1 Picture\n1 Memoir Section (1000 char.)\nReminder Notifications", SortOrder = 1 },
                new Plan { Id = 3, Currency = "₺", IsDeleted = false, IsPopular = true, Name = "Heart", NameEn = "Heart", Period = "Yıl", PeriodEn = "Year", Price = 699.0, Properties = "1 Anı Sayfası\n4 Fotoğraf\n2 Video\n2 Youtube Video\n1 Anı Yazısı Alanı (5000 Karakter)\nHatırlatma Bildirimleri", PropertiesEn = "1 Memory Page\n4 Pictures\n2 Videos\n2 Youtube Videos\n1 Memoir Section (5000 char.)\nReminder Notifications", SortOrder = 2 },
                new Plan { Id = 4, Currency = "₺", IsDeleted = false, IsPopular = false, Name = "Family", NameEn = "Family", Period = "Yıl", PeriodEn = "Year", Price = 1299.0, Properties = "4 Anı Sayfası\n4 Fotoğraf/anı sayfası\n2 Video/anı sayfası\n2 Youtube Video/anı sayfası\n1 Anı Yazısı Alanı (5000 Karakter)/anı sayfası\nHatırlatma Bildirimleri", PropertiesEn = "4 Memory Pages\n4 Pictures/memory page\n2 Videos/memory page\n2 Youtube Videos/memory page\n1 Memoir Section (5000 char.)/memory page\nReminder Notifications", SortOrder = 3 }    
            );

            modelBuilder.Entity<LegalContent>().HasIndex(x => x.Slug).IsUnique();
            modelBuilder.Entity<LegalContent>().HasData(
                new LegalContent { Id = 1, Slug = "terms-of-use", Category = "Legal", Title = @"Kullanım Şartları ve Üyelik Sözleşmesi", TitleEn = @"Terms of Use and Membership Agreement", Content = @"<section class=""mb-8"">
          <h2>1. Taraflar ve Onay</h2>
          <p>İşbu Web Sitesi Kullanım Şartları ve Üyelik Sözleşmesi (Bundan böyle ""Sözleşme"" olarak anılacaktır), www.styever.com internet sitesine (Bundan böyle ""Platform"" veya ""İnternet Sitesi"" olarak anılacaktır) üye olan, içerik sağlayan veya siteyi ziyaret eden kullanıcılar (Bundan böyle ""Kullanıcı"" veya ""Alıcı"" olarak anılacaktır) ile platformun yasal sahibi olan Styever (Bundan böyle ""Styever"" veya ""Satıcı"" olarak anılacaktır) arasında elektronik ortamda onaylandığı tarihte yürürlüğe girmiştir. Platforma giriş yapılması, üyelik oluşturulması veya platform üzerinden dijital hizmet satın alınması, işbu sözleşme şartlarının eksiksiz olarak kabul edildiği anlamına gelir.</p>
        </section>

        <section class=""mb-8"">
          <h2>2. Platformun Tanımı ve Hizmetin Niteliği</h2>
          <p>Styever, kullanıcıların hayatını kaybeden evcil hayvanları ve sevgili dostları için tamamen dijital ortamda özel anı sayfaları oluşturmasına, bu sayfalara fotoğraf/video yüklemesine, anı metinleri yazmasına ve üçüncü şahıslardan dijital taziye mesajları ile anma ritüelleri (dijital mum yakma vb.) almasına olanak sağlayan, web tabanlı bir yazılım ve barındırma platformudur. Sunulan tüm hizmetler soyut, dijital ve anında ifa edilen niteliktedir.</p>
        </section>

        <section class=""mb-8"">
          <h2>3. Hesap Oluşturma ve Güvenlik</h2>
          <p>Platformun sunduğu dijital anı sayfası oluşturma ve özelleştirme özelliklerinden yararlanmak için üyelik hesabı açılması zorunludur. Kullanıcılar, kayıt formunda talep edilen bilgileri doğru, eksiksiz, güncel ve gerçeğe uygun şekilde beyan etmekle yükümlüdür. Hesap şifresinin güvenliği, gizli tutulması ve üçüncü kişilerle paylaşılmaması sorumluluğu tamamen Kullanıcı’ya aittir. Yetkisiz hesap kullanımından doğabilecek her türlü hukuki ve cezai sorumluluk Kullanıcı'nın kendisine aittir.</p>
        </section>

        <section class=""mb-8"">
          <h2>4. Kullanıcı İçeriği ve Hukuki Sorumluluk</h2>
          <p>4.1. Kullanıcılar; platform üzerinde oluşturdukları anı sayfalarına yükledikleri, paylaştıkları veya taziye mesajı olarak bıraktıkları her türlü fotoğraf, video, yazı, yorum, isim ve görsel içerikten (Bundan böyle ""Kullanıcı İçeriği"" olarak anılacaktır) şahsen ve tamamen sorumludur.</p>
          <p>4.2. Kullanıcı, yüklediği içeriklerin 5846 sayılı Fikir ve Sanat Eserleri Kanunu başta olmak üzere yürürlükteki mevzuata uygun olduğunu, üçüncü kişilerin telif haklarını, mülkiyet haklarını, ticari sırlarını, kişisel verilerini veya kişilik haklarını ihlal etmediğini peşinen kabul, beyan ve taahhüt eder. İçeriklerden doğacak her türlü hukuki, cezai ve mali yaptırım doğrudan ilgili Kullanıcı’ya rücu edilir.</p>
          <p>4.3. Kullanıcı, platforma yüklediği içeriklerin yalnızca anı sayfasının görüntülenmesi ve hizmetin yürütülmesi amacıyla İşletici tarafından barındırılmasına, işlenmesine ve teknik olarak sunulmasına gayrikabili rücu muvafakat eder.</p>
        </section>

        <section class=""mb-8"">
          <h2>5. Yasaklı İçerikler ve Uyar-Kaldır Prensibi</h2>
          <p>5.1. Platform üzerinde genel ahlaka, kamu düzenine ve hukuka aykırı; hakaret, küfür, tehdit, nefret söylemi, ırkçılık, yasa dışı faaliyetleri övücü içerikler, spam, reklam, 6698 sayılı Kişisel Verilerin Korunması Kanunu’na aykırı paylaşımlar yapılması kesinlikle yasaktır.</p>
          <p>5.2. Styever, 5651 sayılı İnternet Ortamında Yapılan Yayınların Düzenlenmesi ve Bu Yayınlar Yoluyla İşlenen Suçlarla Mücadele Edilmesi Hakkında Kanun uyarınca ""Yer Sağlayıcı"" statüsündedir. Styever’ın, kullanıcılar tarafından yüklenen içerikleri önceden kontrol etme, hukuka aykırılık araştırması yapma veya editoryal olarak inceleme yükümlülüğü bulunmamaktadır.</p>
          <p>5.3. Styever, mevzuat uyarınca ""Uyar-Kaldır"" prensibini benimsemiştir. Hak ihlali, telif hakkı veya yasa dışı içerik bildirimleri info@styever.com adresine yapıldığı takdirde, Styever ilgili içeriği derhal ve hiçbir ön bildirimde bulunmaksızın yayından kaldırma, silme veya hesabı askıya alma hakkını saklı tutar.</p>
        </section>

        <section class=""mb-8"">
          <h2>6. Ödemeler ve Aktif Abonelik Sistemi</h2>
          <p>6.1. Platform üzerindeki premium anı sayfası kurulumu ve dijital anma özellikleri ücrete tabi olup, hizmet paketlerinin fiyatları sipariş ve ödeme ekranlarında açıkça listelenmektedir.</p>
          <p>6.2. Tüm tahsilat ve ödeme işlemleri, lisanslı ödeme altyapısı sağlayıcısı olan Shopier (Shopier Yazılım A.Ş.) üzerinden 256-bit SSL şifreli ve 3D Secure (güvenli doğrulama) protokolü ile gerçekleştirilir. Styever, Kullanıcı'nın kredi kartı/banka kartı bilgilerini hiçbir şekilde kendi sunucularında saklamaz ve işlemez.</p>
          <p>6.3. Kullanıcı, satın aldığı abonelik paketlerini ve yenileme süreçlerini platform üzerindeki ""Hesabım"" sekmesi üzerinden dilediği zaman yönetebilir veya iptal edebilir. Abonelik iptal edildiğinde, içinde bulunulan aktif dönemin sonuna kadar hizmet açık kalır, bir sonraki dönem karttan çekim yapılmaz.</p>
        </section>

        <section class=""mb-8"">
          <h2>7. Cayma Hakkı ve İade Politikası</h2>
          <p>7.1. Mesafeli Sözleşmeler Yönetmeliği’nin ""Cayma Hakkının İstisnaları"" başlıklı 15. maddesinin 1. fıkrasının (ğ) bendi uyarınca; ""Elektronik ortamda anında ifa edilen hizmetler veya tüketiciye anında teslim edilen gayrimaddi mallara ilişkin sözleşmeler"" kapsamında, dijital anı sayfası aktivasyonu ve hizmet alımı tamamlandığı andan itibaren kanunen cayma ve ücret iade hakkı bulunmamaktadır.</p>
          <p>7.2. Ancak Styever’ın kurumsal müşteri memnuniyeti politikası gereği, ilk satın alma tarihinden itibaren 7 (yedi) günlük bir koşulsuz iptal ve iade süresi tanınmıştır. İlk 7 gün içerisinde info@styever.com üzerinden yapılan iptal başvurularında ücret kesintisiz iade edilir ve ilgili anı sayfası sunuculardan kalıcı olarak silinir. 7 günlük deneme süresi aşıldıktan sonra yapılan iptallerde kesinlikle ücret iadesi yapılmaz.</p>
        </section>

        <section class=""mb-8"">
          <h2>8. Hizmetin Sürekliliği, Platformun Kapatılması ve Mücbir Sebepler</h2>
          <p>8.1. Styever, platformun kesintisiz ve yüksek performansla çalışması için gerekli teknik altyapı yatırımlarını yapar. Ancak, sunulan hizmetlerin sonsuza kadar, hiçbir kesinti olmaksızın veya ömür boyu yayında kalacağına dair mutlak bir yasal taahhüt veya garanti verilmemektedir.</p>
          <p>8.2. Styever; tamamen kendi ticari tasarrufuyla, ekonomik gerekçelerle, teknik imkansızlıklarla, şirket tasfiyesiyle veya stratejik kararlarla platform faaliyetlerini tamamen durdurma, internet sitesini kapatma ve sunulan tüm dijital anı sayfası hizmetlerini kalıcı olarak sonlandırma hakkını saklı tutar.</p>
          <p>8.3. Platformun tamamen kapatılması kararı alınması halinde Styever, aktif aboneliği bulunan veya geçmişte hizmet almış olan tüm kullanıcılara, sistemde kayıtlı e-posta adresleri üzerinden en az 30 (otuz) gün önceden yazılı bildirim yapacaktır.</p>
          <p>8.4. Yapılan kapatma bildirim süresi (30 gün) içerisinde, Kullanıcılar platform üzerinde oluşturdukları anı sayfalarında yer alan kendilerine ait tüm fotoğrafları, videoları ve metinleri yedeklemekle, kendi yerel depolama cihazlarına indirmekle tamamen kendileri yükümlüdür. Bildirim süresinin sona ermesiyle birlikte tüm sunucular kalıcı olarak kapatılacak, veritabanları imha edilecek ve içerikler geri döndürülemeyecek şekilde silinecektir.</p>
          <p>8.5. Platformun işbu maddede belirtilen yasal bildirim süresine (30 gün) uyularak tamamen kapatılması, hizmetin sonlandırılması veya verilerin sunuculardan silinmesi durumunda; aktif aboneliği bitmiş ve ücretsiz statüde sayfası yayınlanan kullanıcılara veya kalan döneme ait ücreti iade edilen aktif abonelere karşı Styever’ın hiçbir hukuki, cezai, mali veya idari sorumluluğu bulunmamaktadır.</p>
          <p>8.6. Kullanıcı; platformun tamamen kapanması veya hizmetin sona ermesi gerekçesiyle Styever'a karşı maddi veya manevi tazminat davası açmayacağını, ""anıların kaybolduğu, silindiği veya manevi zarara uğranıldığı"" iddiasıyla herhangi bir adli, idari veya tüketici merciine başvuruda bulunmayacağını gayrikabili rücu kabul, beyan ve taahhüt eder.</p>
          <p>8.7. İnternet servis sağlayıcılarından, siber saldırılardan, sunucu arızalarından veya üçüncü taraf altyapı tedarikçilerinden kaynaklanan geçici erişim engelleri mücbir sebep sayılır ve Styever bu kesintilerden ötürü sorumlu tutulamaz.</p>
        </section>

        <section class=""mb-8"">
          <h2>9. Uygulanacak Hukuk ve Yetkili Mahkeme</h2>
          <p>İşbu Sözleşme ve platform kullanımından doğacak her türlü uyuşmazlık Türkiye Cumhuriyeti kanunlarına tabidir. Sözleşme’den doğan uyuşmazlıklarda, Ticaret Bakanlığı tarafından her yıl ilan edilen parasal sınırlar dahilinde, Kullanıcı’nın yerleşim yerindeki veya Satıcı'nın ticari merkezinin bulunduğu yerdeki Tüketici Hakem Heyetleri ile Tüketici Mahkemeleri yetkilidir.</p>
        </section>

        <section>
          <h2>10. Satıcı ve İletişim Bilgileri</h2>
          <ul>
            <li>Resmi Ünvan / Ad Soyadı: Styever</li>
            <li>İş Yeri Adresi: ALACAATLI MAH. 3381/2 SK. A-4 ÇANKAYA / ANKARA</li>
            <li>Vergi Dairesi ve No: DOĞANBEY VERGİ DAİRESİ / 0990426667</li>
            <li>E-posta Adresi: info@styever.com</li>
            <li>Müşteri Şikayetleri: Tüm şikayet, öneri ve itirazlarınızı info@styever.com adresine iletebilirsiniz. Talepleriniz en geç 30 iş günü içerisinde incelenerek tarafınıza yazılı geri dönüş sağlanacaktır.</li>
          </ul>
        </section>", ContentEn = @"<section class=""mb-8"">
          <h2>1. Parties and Acceptance</h2>
          <p>This Website Terms of Use and Membership Agreement (hereinafter referred to as the ""Agreement"") enters into force on the date it is electronically approved between users who become members of, provide content to, or visit www.styever.com (hereinafter referred to as the ""Platform"" or ""Website"") (hereinafter referred to as the ""User"" or ""Buyer"") and Styever, the legal owner of the platform (hereinafter referred to as ""Styever"" or the ""Seller""). Accessing the Platform, creating a membership, or purchasing digital services through the Platform means that these Agreement terms are accepted in full.</p>
        </section>

        <section class=""mb-8"">
          <h2>2. Definition of the Platform and Nature of the Service</h2>
          <p>Styever is a web-based software and hosting platform that enables users to create private digital memorial pages for their deceased pets and beloved companions, upload photos/videos to these pages, write memorial texts, and receive digital condolence messages and commemoration rituals such as lighting a digital candle from third parties. All services offered are intangible, digital, and performed instantly.</p>
        </section>

        <section class=""mb-8"">
          <h2>3. Account Creation and Security</h2>
          <p>Creating a membership account is mandatory in order to use the digital memorial page creation and customization features offered by the Platform. Users are obliged to provide the information requested in the registration form accurately, completely, up to date, and truthfully. The security and confidentiality of the account password and ensuring that it is not shared with third parties are entirely the User's responsibility. Any legal and criminal liability arising from unauthorized account use belongs solely to the User.</p>
        </section>

        <section class=""mb-8"">
          <h2>4. User Content and Legal Responsibility</h2>
          <p>4.1. Users are personally and fully responsible for all photographs, videos, texts, comments, names, and visual content they upload, share, or leave as condolence messages on the memorial pages they create on the Platform (hereinafter referred to as ""User Content"").</p>
          <p>4.2. The User accepts, declares, and undertakes in advance that the content they upload complies with applicable legislation, particularly Law No. 5846 on Intellectual and Artistic Works, and does not infringe the copyrights, property rights, trade secrets, personal data, or personality rights of third parties. Any legal, criminal, and financial sanctions arising from such content shall be directly attributable to the relevant User.</p>
          <p>4.3. The User irrevocably consents to the Operator hosting, processing, and technically presenting the content uploaded to the Platform solely for the purpose of displaying the memorial page and providing the service.</p>
        </section>

        <section class=""mb-8"">
          <h2>5. Prohibited Content and Notice-and-Takedown Principle</h2>
          <p>5.1. It is strictly prohibited to share content on the Platform that is contrary to public morality, public order, or law, including insults, profanity, threats, hate speech, racism, content praising illegal activities, spam, advertisements, and posts contrary to Law No. 6698 on the Protection of Personal Data.</p>
          <p>5.2. Styever has the status of a ""Hosting Provider"" pursuant to Law No. 5651 on the Regulation of Publications on the Internet and Combating Crimes Committed through Such Publications. Styever has no obligation to pre-screen content uploaded by users, investigate unlawfulness, or conduct editorial review.</p>
          <p>5.3. Styever adopts the ""Notice-and-Takedown"" principle in accordance with applicable legislation. If notifications regarding rights violations, copyright infringement, or unlawful content are submitted to info@styever.com, Styever reserves the right to immediately remove or delete the relevant content or suspend the account without prior notice.</p>
        </section>

        <section class=""mb-8"">
          <h2>6. Payments and Active Subscription System</h2>
          <p>6.1. Premium memorial page setup and digital commemoration features on the Platform are subject to fees, and service package prices are clearly listed on the order and payment screens.</p>
          <p>6.2. All collection and payment transactions are carried out through Shopier (Shopier Yazılım A.Ş.), a licensed payment infrastructure provider, using 256-bit SSL encryption and the 3D Secure authentication protocol. Styever does not store or process the User's credit card/debit card information on its own servers in any way.</p>
          <p>6.3. The User may manage or cancel purchased subscription packages and renewal processes at any time through the ""My Account"" tab on the Platform. When a subscription is cancelled, the service remains active until the end of the current active period, and no charge is made to the card for the next period.</p>
        </section>

        <section class=""mb-8"">
          <h2>7. Right of Withdrawal and Refund Policy</h2>
          <p>7.1. Pursuant to subparagraph (ğ) of paragraph 1 of Article 15 titled ""Exceptions to the Right of Withdrawal"" of the Distance Contracts Regulation, digital memorial page activation and service purchase fall within the scope of ""services performed instantly in electronic environment or intangible goods delivered instantly to the consumer""; therefore, once activation and service delivery are completed, there is legally no right of withdrawal or refund.</p>
          <p>7.2. However, under Styever's corporate customer satisfaction policy, an unconditional cancellation and refund period of 7 (seven) days is granted from the date of the first purchase. For cancellation requests submitted via info@styever.com within the first 7 days, the fee is refunded in full and the relevant memorial page is permanently deleted from the servers. No refund will be made for cancellations submitted after the 7-day trial period has expired.</p>
        </section>

        <section class=""mb-8"">
          <h2>8. Service Continuity, Platform Closure, and Force Majeure</h2>
          <p>8.1. Styever makes the necessary technical infrastructure investments to ensure that the Platform operates uninterruptedly and with high performance. However, no absolute legal commitment or guarantee is given that the services offered will remain online forever, without interruption, or for a lifetime.</p>
          <p>8.2. Styever reserves the right, entirely at its own commercial discretion, to cease Platform operations completely, close the Website, and permanently terminate all digital memorial page services due to economic reasons, technical impossibilities, company liquidation, or strategic decisions.</p>
          <p>8.3. If a decision is made to completely close the Platform, Styever shall provide written notice at least 30 (thirty) days in advance via the e-mail addresses registered in the system to all users with active subscriptions or who have previously received services.</p>
          <p>8.4. During the 30-day closure notice period, Users are solely responsible for backing up and downloading to their own local storage devices all photographs, videos, and texts belonging to them on the memorial pages they created on the Platform. At the end of the notice period, all servers will be permanently shut down, databases will be destroyed, and content will be irreversibly deleted.</p>
          <p>8.5. If the Platform is completely closed, the service is terminated, or data is deleted from the servers in compliance with the 30-day legal notice period specified in this article, Styever shall have no legal, criminal, financial, or administrative liability toward users whose active subscriptions have expired and whose pages are published under free status, or toward active subscribers whose fees for the remaining period have been refunded.</p>
          <p>8.6. The User irrevocably accepts, declares, and undertakes that they will not file a claim for material or non-pecuniary damages against Styever due to the complete closure of the Platform or termination of the service, and will not apply to any judicial, administrative, or consumer authority on the grounds that ""memories were lost or deleted, or emotional harm was suffered.""</p>
          <p>8.7. Temporary access interruptions caused by internet service providers, cyberattacks, server failures, or third-party infrastructure providers shall be considered force majeure, and Styever shall not be held liable for such interruptions.</p>
        </section>

        <section class=""mb-8"">
          <h2>9. Applicable Law and Authorized Court</h2>
          <p>This Agreement and any disputes arising from use of the Platform are subject to the laws of the Republic of Türkiye. For disputes arising from the Agreement, within the monetary limits announced annually by the Ministry of Trade, the Consumer Arbitration Committees and Consumer Courts at the User's place of residence or at the Seller's commercial headquarters shall have jurisdiction.</p>
        </section>

        <section>
          <h2>10. Seller and Contact Information</h2>
          <ul>
            <li>Official Title / Name and Surname: Styever</li>
            <li>Business Address: ALACAATLI MAH. 3381/2 SK. A-4 ÇANKAYA / ANKARA</li>
            <li>Tax Office and Number: DOĞANBEY TAX OFFICE / 0990426667</li>
            <li>E-mail Address: info@styever.com</li>
            <li>Customer Complaints: You may send all complaints, suggestions, and objections to info@styever.com. Your requests will be reviewed and a written response will be provided within no later than 30 business days.</li>
          </ul>
        </section>", SortOrder = 1, IsDeleted = false },
                new LegalContent { Id = 2, Slug = "distance-sales-agreement", Category = "Legal", Title = @"Mesafeli Satış Sözleşmesi", TitleEn = @"Distance Sales Agreement", Content = @"<section class=""mb-8"">
          <h2>BÖLÜM I: ÖN BİLGİLENDİRME FORMU</h2>

          <h3>1. Satıcı Bilgileri</h3>
          <ul>
            <li>Resmi Ünvanı: Styever</li>
            <li>İş Yeri Adresi: ALACAATLI MAH. 3381/2 SK. A-4 ÇANKAYA / ANKARA</li>
            <li>Vergi Dairesi ve No: DOĞANBEY VERGİ DAİRESİ / 0990426667</li>
            <li>E-posta Adresi: info@styever.com</li>
          </ul>

          <h3>2. Sözleşme Konusu Hizmetin Özellikleri ve Fiyatı</h3>
          <p>Platform üzerinden satın alınan hizmet; vefat eden evcil hayvanlar için dijital anı sayfası oluşturma, yazılım fonksiyonlarını kullanma, barındırma (hosting) ve dijital anma araçlarından (dijital mum yakma, taziye bırakma vb.) yararlanma hakkını içeren soyut ve dijital bir hizmettir. Hizmetin tüm vergiler dahil toplam satış bedeli, Alıcı’nın satın alma anında ödeme ekranında gördüğü ve adına düzenlenen faturada yer alan tutardır. Tamamen dijital ortamda ifa edildiğinden kargo veya lojistik masrafı bulunmamaktadır.</p>

          <h3>3. Ödeme ve Teslimat Bilgileri</h3>
          <p>Ödeme işlemleri, lisanslı ödeme altyapısı sağlayıcısı olan Shopier (Shopier Yazılım A.Ş.) altyapısı üzerinden kredi kartı, banka kartı veya ön ödemeli kartlar ile 256-bit SSL ve 3D Secure güvencesiyle gerçekleştirilir. Sözleşme konusu dijital hizmet, Alıcı'nın ödeme adımlarını tamamlayıp onay vermesini takiben herhangi bir fiziksel teslimat gerektirmeksizin elektronik ortamda anında ifa edilir ve Alıcı'nın kullanımına açılır.</p>

          <h3>4. Cayma Hakkı ve İstisnaları</h3>
          <p>27 Kasım 2014 tarihli Resmi Gazete'de yayımlanan Mesafeli Sözleşmeler Yönetmeliği’nin “Cayma Hakkının İstisnaları” başlıklı 15. maddesinin 1. fıkrasının (ğ) bendi uyarınca; “Elektronik ortamda anında ifa edilen hizmetler veya tüketiciye anında teslim edilen gayrimaddi mallara ilişkin sözleşmeler” yasal olarak cayma hakkının istisnası kapsamındadır. Alıcı, satın aldığı dijital hizmetin anında teslim edilen bir dijital içerik olduğunu ve bu nedenle yasal olarak cayma ve ücret iade hakkının bulunmadığını peşinen kabul eder. (Sadece Satıcı'nın müşteri memnuniyeti politikası gereği tanıdığı ilk 7 günlük koşulsuz iptal hakkı saklıdır).</p>

          <h3>5. Şikayet ve Çözüm Mekanizması</h3>
          <p>Alıcı, hizmete ilişkin her türlü talep ve şikayetini info@styever.com adresine iletebilir. İşbu Ön Bilgilendirme Formu'ndan doğan uyuşmazlıklarda, Ticaret Bakanlığı tarafından her yıl ilan edilen yasal parasal sınırlar dahilinde Alıcı’nın yerleşim yerindeki veya Satıcı'nın ticari merkezinin bulunduğu yerdeki Tüketici Hakem Heyetleri ile Tüketici Mahkemeleri yetkilidir.</p>
        </section>

        <section class=""mb-8"">
          <h2>BÖLÜM II: MESAFELİ SATIŞ SÖZLEŞMESİ</h2>
          <h3>Taraflar</h3>
          <p>İşbu Sözleşme, aşağıda belirtilen şartlar ve hükümler dahilinde, www.styever.com internet sitesi (Bundan böyle “İnternet Sitesi” veya “Platform” olarak anılacaktır) üzerinden elektronik ortamda hizmet satın alan Alıcı ile hizmeti sağlayan Satıcı arasında, Alıcı'nın sipariş onay adımlarını tamamlayarak sözleşmeyi elektronik ortamda onayladığı tarihte yürürlüğe girmiştir.</p>
        </section>

        <section class=""mb-8"">
          <h2>1. Taraf Bilgileri ve Tanımlar</h2>

          <h3>1.1. Satıcı Bilgileri</h3>
          <ul>
            <li>Resmi Ünvanı: Styever</li>
            <li>Marka İsmi: Styever</li>
            <li>İş Yeri Adresi: ALACAATLI MAH. 3381/2 SK. A-4 ÇANKAYA / ANKARA</li>
            <li>T.C. Kimlik Numarası: 50335068022</li>
            <li>Vergi Dairesi ve No: DOĞANBEY VERGİ DAİRESİ / 0990426667</li>
            <li>E-posta: info@styever.com</li>
          </ul>

          <h3>1.2. Alıcı Bilgileri</h3>
          <p>Satıcı'ya ait www.styever.com internet sitesi üzerinden elektronik ortamda sipariş veren, üyelik başlatan, dijital hizmet satın alan; fatura, sipariş ve üyelik formlarında adı-soyadı, T.C. Kimlik Numarası (varsa) ve iletişim bilgileri yer alan gerçek veya tüzel kişidir (Bundan böyle “Alıcı” veya “Tüketici” olarak anılacaktır).</p>

          <h3>1.3. Tanımlar</h3>
          <p>İşbu sözleşmenin uygulanmasında ve yorumlanmasında, aşağıda yazılı terimler karşılarındaki yazılı açıklamaları ifade edeceklerdir:</p>
          <ul>
            <li>Satıcı: Styever</li>
            <li>Alıcı: İnternet sitesini kullanarak dijital hizmet, yazılım ve anı sayfası alanı satın alan gerçek veya tüzel kişi.</li>
            <li>Hizmet: Alıcı’nın platform üzerinden satın aldığı, anında ifa edilen dijital içerikleri, yazılım fonksiyonlarını ve barındırma hizmetlerini kapsayan soyut servisler.</li>
            <li>Sözleşme: Satıcı ve Alıcı arasında kurulan işbu Mesafeli Satış Sözleşmesi.</li>
          </ul>
        </section>

        <section class=""mb-8"">
          <h2>2. Sözleşmenin Konusu ve Kapsamı</h2>
          <p>İşbu Sözleşme’nin konusu, Alıcı’nın Satıcı’ya ait Styever platformu üzerinden elektronik ortamda siparişini verdiği, nitelikleri, kapsamı ve satış fiyatı platformun ilgili sayfalarında belirtilen dijital ürün ve hizmetlerin (Dijital Anı Sayfası Oluşturma, Dijital Anı ve Taziye Bırakma, Dijital Mum Yakma Özellikleri vb.) satışı, teslimi ve ifası ile ilgili olarak, 6502 sayılı Tüketicinin Korunması Hakkında Kanun ve Mesafeli Sözleşmeler Yönetmeliği hükümleri uyarınca tarafların karşılıklı hak ve yükümlülüklerinin belirlenmesidir.</p>
        </section>

        <section class=""mb-8"">
          <h2>3. Sözleşme Konusu Hizmet, Fiyat ve Ödeme Bilgileri</h2>
          <p>3.1. Hizmetin temel nitelikleri, paket içerikleri ve sağlanan dijital alanın kapsamı www.styever.com internet sitesinde yayınlanmaktadır.</p>
          <p>3.2. Listelenen ve sitede ilan edilen fiyatlar satış fiyatıdır. İlan edilen fiyatlar ve vaatler güncelleme yapılana ve değiştirilene kadar geçerlidir. Süreli olarak ilan edilen fiyatlar ise belirtilen süre sonuna kadar geçerlidir.</p>
          <p>3.3. Ödeme Metodu: Kredi Kartı / Banka Kartı veya Ön Ödemeli Kartlar (Shopier - Shopier Yazılım A.Ş. Altyapısı ile).</p>
          <p>3.4. Hizmet Bedeli: Alıcı'nın siparişi onayladığı anda ekranda belirtilen, sipariş özetinde gösterilen ve elektronik ortamda Alıcı'ya iletilen faturada yer alan tüm vergiler (KDV vb.) dahil toplam tutardır. İşbu hizmet tamamen dijital ortamda teslim edildiğinden kargo veya lojistik masrafı bulunmamaktadır.</p>
        </section>

        <section class=""mb-8"">
          <h2>4. Genel Hükümler</h2>
          <p>4.1. Alıcı, Styever platformunda Sözleşme konusu dijital hizmetin temel nitelikleri, satış fiyatı, ödeme şekli ve teslimata ilişkin ön bilgileri (Ön Bilgilendirme Formu) okuyup bilgi sahibi olduğunu ve elektronik ortamda gerekli teyidi verdiğini kabul, beyan ve taahhüt eder.</p>
          <p>4.2. Sözleşme konusu hizmet, Alıcı’nın elektronik ortamda ödeme ve sipariş onay adımlarını tamamlamasını takiben, herhangi bir fiziksel teslimat gerektirmeksizin, elektronik ortamda (kullanıcı paneli aktivasyonu ve e-posta bildirimi ile) anında teslim edilir ve Alıcı'nın kullanımına açılır.</p>
          <p>4.3. Satıcı, Sözleşme konusu hizmetin ayıpsız, platformda belirtilen teknik niteliklere uygun, kesintisiz ve taahhüt edilen sınırlar dahilinde Alıcı'ya sunulmasından sorumludur. Sitenin teknik altyapısında meydana gelebilecek kısa süreli bakım ve onarım çalışmaları esnasındaki kesintilerden Satıcı sorumlu tutulamaz.</p>
          <p>4.4. Ödeme İşlemleri Güvenliği: Alıcı, ödemelerini lisanslı ödeme kuruluşu Shopier (Shopier Yazılım A.Ş.) altyapısı üzerinden gerçekleştirir. Kart güvenliği, veri şifreleme (256-bit SSL), 3D Secure doğrulama süreçleri tamamen Shopier ve ilgili bankaların sorumluluğundadır. Satıcı, Alıcı’nın kredi kartı numarası, son kullanma tarihi ve CVC kodu gibi kritik kart bilgilerini kendi sistemlerinde kesinlikle tutmaz, kaydetmez ve saklamaz.</p>
          <p>4.5. Alıcı, sisteme kayıt olurken veya içerik yüklerken paylaştığı tüm bilgilerin (fotoğraflar, anı yazıları vb.) yasal sorumluluğunun kendisine ait olduğunu, telif hakları veya kişilik hakları ihlallerinden doğacak tüm hukuki sorumluluğun doğrudan kendisinde olduğunu kabul eder.</p>
        </section>

        <section class=""mb-8"">
          <h2>5. Cayma Hakkı ve İstisnaları</h2>
          <p>5.1. 27 Kasım 2014 tarihli Resmi Gazete'de yayımlanan Mesafeli Sözleşmeler Yönetmeliği’nin “Cayma Hakkının İstisnaları” başlıklı 15. maddesinin 1. fıkrasının (ğ) bendi uyarınca; “Elektronik ortamda anında ifa edilen hizmetler veya tüketiciye anında teslim edilen gayrimaddi mallara ilişkin sözleşmeler” yasal olarak cayma hakkının tamamen istisnası kapsamındadır.</p>
          <p>5.2. Alıcı, Styever üzerinden satın aldığı Dijital Anı Sayfası kurulumunun, aktivasyonunun ve dijital anma özelliklerinin “anında ifa edilen bir dijital içerik ve yazılım hizmeti” olduğunu, sipariş onayıyla birlikte hizmetin kendisine tamamen teslim edildiğini ve bu yasal nitelik gereği mevzuata göre cayma ve ücret iade hakkının bulunmadığını peşinen kabul, beyan ve taahhüt eder.</p>
          <p>5.3. Platformun müşteri memnuniyeti politikası gereği sunduğu “7 Günlük Koşulsuz İptal Hakkı”, Satıcı'nın tamamen kendi inisiyatifiyle sağladığı kurumsal bir jest olup, bu 7 günlük sürenin aşılmasının ardından mevzuat gereği herhangi bir hak veya bedel iadesi talep edilemez.</p>
        </section>

        <section class=""mb-8"">
          <h2>6. Hizmetin Sürekliliği, Platformun Kapatılması ve Sorumluluk Sınırı</h2>
          <p>6.1. Satıcı, www.styever.com platformunun ve bu platform üzerinden sağlanan tüm dijital anı sayfalarının en yüksek erişilebilirlik ve teknik performans standartlarında yayında kalması için gerekli özeni gösterir. Ancak Satıcı, platformun sonsuza kadar kesintisiz, teknik olarak kusursuz veya ömür boyu yayında kalacağına dair mutlak bir yasal taahhüt veya garanti vermez.</p>
          <p>6.2. Satıcı; tamamen kendi ticari tasarrufuyla, ekonomik/finansal gerekçelerle, teknik altyapı imkansızlıklarıyla, şirket tasfiyesiyle, ortaklık yapısı değişiklikleriyle veya stratejik kararlarla platform faaliyetlerini tamamen durdurma, internet sitesini kapatma ve sunulan tüm dijital anı sayfası hizmetlerini kalıcı olarak sonlandırma hakkını saklı tutar.</p>
          <p>6.3. Platformun ticari veya teknik sebeplerle tamamen kapatılması ve hizmetlerin kalıcı olarak sonlandırılması kararı alınması halinde Satıcı, aktif aboneliği bulunan veya geçmişte hizmet almış olan tüm kullanıcılara, sistemde kayıtlı e-posta adresleri üzerinden en az 30 (otuz) gün önceden yazılı bildirim yapmakla yükümlüdür.</p>
          <p>6.4. Yapılan kapatma bildirim süresi (30 gün) içerisinde, Alıcılar platform üzerinde oluşturdukları anı sayfalarında yer alan kendilerine ait fotoğrafları, videoları, metinleri ve tüm dijital verileri yedeklemekle, kendi yerel depolama cihazlarına indirmekle tamamen kendileri yükümlüdür. Bildirim süresinin sona ermesiyle birlikte tüm sunucular kalıcı olarak kapatılacak, veritabanları imha edilecek ve içerikler geri döndürülemeyecek şekilde silinecektir. Yedekleme işleminin Alıcı tarafından zamanında yapılmamasından kaynaklanan veri kayıplarından Satıcı sorumlu tutulamaz.</p>
          <p>6.5. Platformun işbu maddede belirtilen yasal bildirim süresine (30 gün) uyularak tamamen kapatılması, hizmetin sonlandırılması veya verilerin sunuculardan silinmesi durumunda; aktif aboneliği bitmiş ve ücretsiz statüde sayfası yayınlanan kullanıcılara, aboneliği geçmişte sonlandırılmış kişilere veya bildirim tarihi itibarıyla kalan döneme ait ücreti iade edilen aktif abonelere karşı Satıcı’nın hiçbir hukuki, cezai, mali veya idari sorumluluğu bulunmamaktadır.</p>
          <p>6.6. Alıcı; platformun tamamen kapanması, hizmetin sona ermesi veya içeriklerin silinmesi gerekçesiyle Satıcı'ya karşı maddi veya manevi tazminat davası açmayacağını, geriye dönük ödediği hizmet/abonelik bedellerini talep etmeyeceğini, “anıların kaybolduğu, silindiği veya manevi zarara uğranıldığı” iddiasıyla herhangi bir adli, idari veya tüketici merciine başvuruda bulunmayacağını gayrikabili rücu kabul, beyan ve taahhüt eder.</p>
        </section>

        <section>
          <h2>7. Delil Sözleşmesi ve Yetkili Mahkeme</h2>
          <p>7.1. İşbu Sözleşme’den doğabilecek uyuşmazlıklarda Satıcı’nın sistem kayıtları, sunucu logları, Shopier işlem dökümleri ve elektronik e-posta yazışmaları 6100 sayılı Hukuk Muhakemeleri Kanunu’nun 193. maddesi uyarınca kesin ve bağlayıcı delil niteliğindedir.</p>
          <p>7.2. İşbu Sözleşme’den doğan uyuşmazlıklarda, Ticaret Bakanlığı tarafından her yıl ilan edilen ve yasal olarak bağlayıcı olan parasal sınırlar dahilinde, Alıcı’nın yerleşim yerindeki veya Satıcı'nın ticari merkezinin bulunduğu yerdeki Tüketici Hakem Heyetleri ile Tüketici Mahkemeleri yetkilidir.</p>
        </section>", ContentEn = @"<section class=""mb-8"">
          <h2>SECTION I: PRELIMINARY INFORMATION FORM</h2>

          <h3>1. Seller Information</h3>
          <ul>
            <li>Legal Title: Styever</li>
            <li>Business Address: ALACAATLI MAH. 3381/2 SK. A-4 ÇANKAYA / ANKARA</li>
            <li>Tax Office and No.: DOĞANBEY TAX OFFICE / 0990426667</li>
            <li>E-mail Address: info@styever.com</li>
          </ul>

          <h3>2. Characteristics and Price of the Service Subject to the Agreement</h3>
          <p>The service purchased through the Platform is an intangible and digital service that includes the right to create a digital memorial page for deceased pets, use software functions, receive hosting services, and use digital remembrance tools such as lighting a digital candle and leaving condolences. The total sales price including all taxes is the amount displayed to the Buyer on the payment screen at the time of purchase and stated on the invoice issued in the Buyer's name. Since the service is performed entirely in a digital environment, there are no shipping or logistics costs.</p>

          <h3>3. Payment and Delivery Information</h3>
          <p>Payments are processed through Shopier (Shopier Yazılım A.Ş.), a licensed payment infrastructure provider, using credit cards, debit cards or prepaid cards with 256-bit SSL encryption and 3D Secure protection. After the Buyer completes and approves the payment steps, the digital service subject to the Agreement is performed instantly in electronic form without requiring physical delivery and is made available to the Buyer.</p>

          <h3>4. Right of Withdrawal and Exceptions</h3>
          <p>Pursuant to Article 15/1(ğ) of the Distance Contracts Regulation published in the Official Gazette on 27 November 2014, contracts concerning services performed instantly in electronic form or intangible goods delivered instantly to the consumer are legally exempt from the right of withdrawal. The Buyer acknowledges in advance that the purchased digital service is digital content delivered instantly and therefore there is no statutory right of withdrawal or refund. The unconditional 7-day cancellation right granted solely under the Seller's customer satisfaction policy is reserved.</p>

          <h3>5. Complaints and Dispute Resolution</h3>
          <p>The Buyer may submit any request or complaint concerning the service to info@styever.com. For disputes arising from this Preliminary Information Form, Consumer Arbitration Committees and Consumer Courts at the Buyer's place of residence or the Seller's commercial center shall have jurisdiction within the statutory monetary limits announced annually by the Ministry of Trade.</p>
        </section>

        <section class=""mb-8"">
          <h2>SECTION II: DISTANCE SALES AGREEMENT</h2>
          <h3>Parties</h3>
          <p>This Agreement enters into force on the date the Buyer completes the order confirmation steps and electronically approves the Agreement for a service purchased through www.styever.com (hereinafter the “Website” or “Platform”), between the Buyer and the Seller providing the service, subject to the terms and conditions set out below.</p>
        </section>

        <section class=""mb-8"">
          <h2>1. Party Information and Definitions</h2>

          <h3>1.1. Seller Information</h3>
          <ul>
            <li>Legal Title: Styever</li>
            <li>Brand Name: Styever</li>
            <li>Business Address: ALACAATLI MAH. 3381/2 SK. A-4 ÇANKAYA / ANKARA</li>
            <li>Turkish ID Number: 50335068022</li>
            <li>Tax Office and No.: DOĞANBEY TAX OFFICE / 0990426667</li>
            <li>E-mail: info@styever.com</li>
          </ul>

          <h3>1.2. Buyer Information</h3>
          <p>The natural or legal person who places an electronic order, starts a membership or purchases a digital service through the Seller's www.styever.com website and whose name, surname, Turkish ID Number (if applicable) and contact information appear on the invoice, order and membership forms (hereinafter the “Buyer” or “Consumer”).</p>

          <h3>1.3. Definitions</h3>
          <p>For the implementation and interpretation of this Agreement, the following terms shall have the meanings stated below:</p>
          <ul>
            <li>Seller: Styever</li>
            <li>Buyer: A natural or legal person who purchases digital services, software and memorial page space through the Website.</li>
            <li>Service: Intangible services purchased by the Buyer through the Platform, including instantly performed digital content, software functions and hosting services.</li>
            <li>Agreement: This Distance Sales Agreement concluded between the Seller and the Buyer.</li>
          </ul>
        </section>

        <section class=""mb-8"">
          <h2>2. Subject and Scope of the Agreement</h2>
          <p>The subject of this Agreement is to determine the mutual rights and obligations of the parties under Law No. 6502 on Consumer Protection and the Distance Contracts Regulation regarding the sale, delivery and performance of digital products and services ordered electronically by the Buyer through the Seller's Styever platform, whose characteristics, scope and sales price are specified on the relevant pages of the Platform, including Digital Memorial Page Creation, Digital Memorial and Condolence Messages, Digital Candle Lighting features and similar services.</p>
        </section>

        <section class=""mb-8"">
          <h2>3. Service, Price and Payment Information</h2>
          <p>3.1. The basic characteristics of the service, package contents and scope of the digital space provided are published on www.styever.com.</p>
          <p>3.2. Prices listed and announced on the Website are sales prices. Announced prices and commitments remain valid until updated or changed. Prices announced for a limited period remain valid until the end of the specified period.</p>
          <p>3.3. Payment Method: Credit Card / Debit Card or Prepaid Cards (through Shopier - Shopier Yazılım A.Ş. infrastructure).</p>
          <p>3.4. Service Fee: The total amount including all taxes (VAT, etc.) displayed when the Buyer confirms the order, shown in the order summary and stated on the invoice electronically delivered to the Buyer. Since the service is delivered entirely in digital form, there are no shipping or logistics costs.</p>
        </section>

        <section class=""mb-8"">
          <h2>4. General Provisions</h2>
          <p>4.1. The Buyer accepts, declares and undertakes that they have read and understood the preliminary information concerning the basic characteristics, sales price, payment method and delivery of the digital service subject to the Agreement on the Styever Platform and have provided the necessary electronic confirmation.</p>
          <p>4.2. After the Buyer completes the electronic payment and order confirmation steps, the service subject to the Agreement is delivered instantly in electronic form through user panel activation and e-mail notification, without requiring physical delivery, and is made available to the Buyer.</p>
          <p>4.3. The Seller is responsible for providing the service free from defects, in accordance with the technical characteristics stated on the Platform, continuously and within the committed limits. The Seller shall not be held responsible for interruptions occurring during short-term maintenance and repair work on the Website's technical infrastructure.</p>
          <p>4.4. Payment Transaction Security: The Buyer makes payments through the infrastructure of the licensed payment provider Shopier (Shopier Yazılım A.Ş.). Card security, data encryption (256-bit SSL) and 3D Secure verification processes are entirely under the responsibility of Shopier and the relevant banks. The Seller does not retain, record or store critical card information such as the Buyer's credit card number, expiration date or CVC code in its own systems.</p>
          <p>4.5. The Buyer accepts that they bear legal responsibility for all information and content shared while registering or uploading content, including photographs and memorial texts, and that all legal responsibility arising from copyright or personality-right violations belongs directly to the Buyer.</p>
        </section>

        <section class=""mb-8"">
          <h2>5. Right of Withdrawal and Exceptions</h2>
          <p>5.1. Pursuant to Article 15/1(ğ) of the Distance Contracts Regulation published in the Official Gazette on 27 November 2014, contracts concerning services performed instantly in electronic form or intangible goods delivered instantly to the consumer are completely exempt from the statutory right of withdrawal.</p>
          <p>5.2. The Buyer accepts, declares and undertakes in advance that the installation and activation of the Digital Memorial Page and digital remembrance features purchased through Styever constitute an instantly performed digital content and software service, that the service is fully delivered upon order confirmation, and that due to this legal nature there is no statutory right of withdrawal or refund.</p>
          <p>5.3. The “7-Day Unconditional Cancellation Right” offered under the Platform's customer satisfaction policy is a corporate goodwill gesture provided entirely at the Seller's discretion; after this 7-day period expires, no right or refund may be claimed under the applicable legislation.</p>
        </section>

        <section class=""mb-8"">
          <h2>6. Service Continuity, Platform Closure and Limitation of Liability</h2>
          <p>6.1. The Seller exercises due care to keep www.styever.com and all digital memorial pages provided through the Platform available at high accessibility and technical performance standards. However, the Seller does not provide an absolute legal commitment or guarantee that the Platform will remain uninterrupted, technically flawless or available for life.</p>
          <p>6.2. The Seller reserves the right, at its sole commercial discretion, to cease Platform operations entirely, close the Website and permanently terminate all digital memorial page services due to economic or financial reasons, technical infrastructure impossibilities, company liquidation, changes in partnership structure or strategic decisions.</p>
          <p>6.3. If a decision is made to permanently close the Platform and terminate the services for commercial or technical reasons, the Seller shall provide written notice at least 30 (thirty) days in advance to all users with active subscriptions or who have previously received services, using their registered e-mail addresses.</p>
          <p>6.4. During the 30-day closure notice period, Buyers are solely responsible for backing up and downloading to their local storage devices all photographs, videos, texts and digital data contained in their memorial pages. At the end of the notice period, all servers will be permanently shut down, databases destroyed and content irreversibly deleted. The Seller shall not be responsible for data loss resulting from the Buyer's failure to make timely backups.</p>
          <p>6.5. If the Platform is completely closed, services are terminated or data is deleted from the servers in compliance with the 30-day notice period specified herein, the Seller shall have no legal, criminal, financial or administrative liability toward users whose active subscriptions have expired and whose pages remain published under free status, persons whose subscriptions were previously terminated, or active subscribers whose remaining subscription fees as of the notification date have been refunded.</p>
          <p>6.6. The Buyer irrevocably accepts, declares and undertakes that they will not file material or moral damages claims against the Seller, seek repayment of previously paid service/subscription fees, or apply to any judicial, administrative or consumer authority on the grounds that memories were lost, deleted or caused emotional harm due to the complete closure of the Platform, termination of the service or deletion of content.</p>
        </section>

        <section>
          <h2>7. Evidence Agreement and Competent Court</h2>
          <p>7.1. In disputes arising from this Agreement, the Seller's system records, server logs, Shopier transaction records and electronic e-mail correspondence shall constitute conclusive and binding evidence pursuant to Article 193 of Law No. 6100 on Civil Procedure.</p>
          <p>7.2. For disputes arising from this Agreement, Consumer Arbitration Committees and Consumer Courts at the Buyer's place of residence or the Seller's commercial center shall have jurisdiction within the legally binding monetary limits announced annually by the Ministry of Trade.</p>
        </section>", SortOrder = 2, IsDeleted = false },
                new LegalContent { Id = 3, Slug = "cancellation-refund-policy", Category = "Legal", Title = @"İptal ve İade Koşulları", TitleEn = @"Cancellation and Refund Policy", Content = @"<section class=""mb-8"">
          <h2>1. Giriş veya Hizmetin Niteliği</h2>
          <p>1.1. Styever (Bundan böyle ""Satıcı"" olarak anılacaktır) tarafından sunulan hizmetler; internet sitesi (www.styever.com) üzerinden elektronik ortamda anında ifa edilen ve Alıcı’ya anında teslim edilen dijital içerikleri, yazılım hizmetlerini ve web tabanlı barındırma alanlarını (Dijital Anı Sayfası Kurulumu, Dijital Anı Bırakma Hizmeti, Sayfa Özelleştirme ve Dijital Anma Özellikleri vb.) kapsamaktadır.</p>
          <p>1.2. Alıcı, platform üzerinden satın alma işlemini gerçekleştirerek bu sözleşme koşullarının yanı sıra, hizmetin tamamen dijital, soyut ve anında ifa edilen bir niteliğe sahip olduğunu peşinen kabul, beyan ve taahhüt eder.</p>
        </section>

        <section class=""mb-8"">
          <h2>2. 7 Günlük Deneme Süresi ve İptal Hakkı</h2>
          <p>2.1. Satıcı, kullanıcı memnuniyeti odaklı hizmet politikası kapsamında, Alıcı'ya ilk satın alma (hizmetin başlatılma) tarihinden itibaren başlamak üzere 7 (yedi) günlük bir deneme ve koşulsuz iptal süresi tanımaktadır.</p>
          <p>2.2. Alıcı, satın aldığı dijital hizmetten memnun kalmaması veya devam etmek istememesi durumunda, ilk satın alma tarihinden itibaren 7 (yedi) gün içinde hiçbir cezai şart ödemeksizin ve herhangi bir gerekçe göstermeksizin platform üzerindeki kullanıcı paneli aracılığıyla veya info@styever.com resmi e-posta adresi üzerinden iptal talebinde bulunma hakkına sahiptir.</p>
          <p>2.3. İptal ve cayma hakkının bu 7 (yedi) günlük süre içinde usulüne uygun olarak kullanılması durumunda, Alıcı'dan tahsil edilen hizmet bedeli, ödeme kuruluşu altyapı sağlayıcısı Shopier (Shopier Yazılım A.Ş.) ve ilgili bankaların iade prosedürlerine uygun olarak Alıcı'nın işlem yaptığı karta aynen ve kesintisiz iade edilir.</p>
          <p>2.4. İptal işlemi sistem tarafından onaylandığı an, Alıcı için oluşturulan ilgili Dijital Anı Sayfası, içerisindeki tüm verilerle birlikte tamamen yayından kaldırılır, sunucu veri akışı durdurulur ve Alıcı platformda hiç abonelik başlatmamış statüsüne geri döndürülür.</p>
        </section>

        <section class=""mb-8"">
          <h2>3. Abonelik Sonlandırılması ve Dijital Anı Sayfası Statüsü</h2>
          <p>3.1. İlk satın alma tarihinin üzerinden 7 (yedi) günlük deneme süresi geçtikten sonra yapılan iptal, tek taraflı fesih ve abonelik sonlandırma taleplerinde, içinde bulunulan aktif döneme (ay/yıl) ait ücret iadesi kesinlikle yapılmaz. Alıcı, dilediği zaman bir sonraki döneme ait otomatik yenilemeyi durdurma (abonelik sonlandırma) hakkına sahiptir.</p>
          <p>3.2. Aboneliğini sonlandıran (yenilemesini kapatan) Alıcıların platform üzerinde daha önce oluşturmuş olduğu Dijital Anı Sayfası, Alıcı tarafından tamamen silinmesi talep edilmediği ve Satıcı platform faaliyetlerini sürdürdüğü müddetçe, platformda ""ziyarete açık"" statüde yayında kalmaya devam edecektir.</p>
          <p>3.3. Aboneliği sonlandırılmış, aktif ödeme dönemi bitmiş ve ücretsiz/statik statüye geçmiş olan Alıcıların, ilgili dijital anı sayfası üzerindeki düzenleme, yeni görsel/video yükleme, anı metni değiştirme, silme veya güncelleme (edit) yetkileri sistem tarafından otomatik olarak dondurulur. Alıcı, sayfa internette yayında olsa dahi üzerinde yeni bir editoryal değişiklik yapamaz.</p>
          <p>3.4. Alıcı, aboneliğini platform üzerinden dilediği zaman yeniden başlattığı ve ilgili dönem ödeme döngüsünü aktif hale getirdiği andan itibaren, anı sayfası üzerindeki tüm teknik düzenleme, görsel/medya yükleme ve metin değiştirme yetkilerine yeniden kavuşur.</p>
        </section>

        <section class=""mb-8"">
          <h2>4. Cayma Hakkı İstisnaları Beyanı</h2>
          <p>4.1. 27 Kasım 2014 tarihli Resmi Gazete'de yayımlanan Mesafeli Sözleşmeler Yönetmeliği’nin ""Cayma Hakkının İstisnaları"" başlıklı 15. maddesinin 1. fıkrasının (ğ) bendi uyarınca; ""Elektronik ortamda anında ifa edilen hizmetler veya tüketiciye anında teslim edilen gayrimaddi mallara ilişkin sözleşmeler"" yasal olarak cayma hakkının istisnası kapsamındadır ve bu tür hizmetlerde tüketicinin kanuni cayma ve iade hakkı bulunmamaktadır.</p>
          <p>4.2. Styever tarafından Alıcı'ya sunulan 7 günlük koşulsuz iptal/deneme süresi ve abonelik sonrasında sayfanın statik olarak yayında tutulmaya devam edilmesi, mevzuattan doğan bir zorunluluk olmayıp, tamamen Satıcı'nın Alıcı'ya sunduğu ek bir müşteri memnuniyeti taahhüdüdür. 7 günlük sürenin aşılmasının ardından mevzuat uyarınca yasal iade yapılma yükümlülüğü bulunmamaktadır.</p>
        </section>

        <section class=""mb-8"">
          <h2>5. İade Prosedürü ve Yansıma Süresi</h2>
          <p>5.1. 7 günlük yasal süre içinde usulüne uygun yapılan ve Satıcı tarafından onaylanan ücret iadeleri, Satıcı tarafından Alıcı'nın ödeme yaptığı kredi/banka kartına, lisanslı ödeme altyapısı sağlayıcısı Shopier (Shopier Yazılım A.Ş.) üzerinden tek seferde ve kesintisiz olarak iletilir.</p>
          <p>5.2. İade edilen tutarın Alıcı'nın banka veya kredi kartı hesabına, ekstrelerine yansıma süresi; Alıcı'nın hizmet aldığı bankanın iç prosedürlerine, kart türüne (kredi kartı veya banka/debit kart), hesap kesim tarihlerine ve uluslararası kart kuruluşlarının kurallarına göre değişiklik gösterebilir (genellikle 1 ila 7 iş günü). Satıcı tarafından iade talimatı Shopier sistemine anında verilir; bu aşamadan sonra meydana gelebilecek banka kaynaklı gecikmelerden, bloke sürelerinden veya teknik aksaklıklardan Satıcı doğrudan veya dolaylı olarak sorumlu tutulamaz.</p>
        </section>

        <section>
          <h2>6. Hizmetin Sürekliliği, Platformun Kapatılması ve Sorumluluk Sınırı</h2>
          <p>6.1. Satıcı, www.styever.com üzerinden sunulan dijital anı sayfası hizmetlerini, ticari imkanları, teknik altyapı sürdürülebilirliğini ve sunucu kapasitelerini göz önünde bulundurarak en yüksek kalitede sunmayı amaçlar. Ancak Satıcı, platformun sonsuza kadar, hiçbir kesinti olmaksızın veya ömür boyu yayında kalacağına dair mutlak ve taahhüt niteliğinde bir yasal garanti vermemektedir.</p>
          <p>6.2. Satıcı; tamamen kendi ticari tasarrufuyla, ekonomik gerekçelerle, teknik imkansızlıklarla, şirket tasfiyesiyle veya stratejik kararlarla platform faaliyetlerini tamamen durdurma, siteyi kapatma ve sunulan tüm dijital anı sayfası hizmetlerini kalıcı olarak sonlandırma hakkını saklı tutar.</p>
          <p>6.3. Platformun tamamen kapatılması veya hizmetlerin kalıcı olarak sonlandırılması kararı alınması halinde Satıcı, aktif aboneliği bulunan veya geçmişte abonelik başlatmış tüm kullanıcılara, sistemde kayıtlı olan e-posta adresleri üzerinden en az 30 (otuz) gün önceden yazılı bildirim yapmakla yükümlüdür.</p>
          <p>6.4. Yapılan kapatma bildirim süresi (30 gün) içerisinde, Alıcılar platform üzerinde oluşturdukları anı sayfalarında yer alan kendilerine ait fotoğrafları, videoları, metinleri ve verileri yedeklemekle, kendi bilgisayar veya harici depolama cihazlarına indirmekle tamamen kendileri yükümlüdür. Bildirim süresinin sona ermesiyle birlikte tüm sunucular kapatılacak ve veriler geri döndürülemeyecek şekilde silinecektir. Yedekleme işleminin Alıcı tarafından yapılmamasından veya geç yapılmasından kaynaklanan veri kayıplarından Satıcı sorumlu tutulamaz.</p>
          <p>6.5. Platformun ticari veya teknik zorunluluklar nedeniyle tamamen kapatılması, hizmetin sonlandırılması veya verilerin silinmesi durumunda;</p>

          <ul class=""ps-6 mb-4"">
            <li>Hizmeti geçmişte kullanmış, aboneliğini sonlandırmış ve ücretsiz/statik statüde sayfası yayınlanan kullanıcılara,</li>
            <li>Aktif ödeme dönemi bitmiş olan kullanıcılara,</li>
            <li>Bildirim tarihi itibarıyla aktif aboneliği bulunup kalan döneme ait bedeli iade edilen kullanıcılara karşı, Satıcı’nın hiçbir hukuki, cezai veya mali sorumluluğu bulunmamaktadır.</li>
          </ul>

          <p>6.6. Alıcı, platformun tamamen kapanması veya hizmetin yasal bildirim süresine uyularak sonlandırılması gerekçesiyle Satıcı'ya karşı maddi veya manevi tazminat davası açmayacağını, geriye dönük ödediği abonelik bedellerini talep etmeyeceğini, ""anıların kaybolduğu, silindiği veya manevi zarara uğranıldığı"" iddiasıyla herhangi bir adli veya idari merciye başvuruda bulunmayacağını gayrikabili rücu kabul, beyan ve taahhüt eder.</p>
        </section>", ContentEn = @"<section class=""mb-8"">
          <h2>1. Introduction or Nature of the Service</h2>
          <p>1.1. The services provided by Styever (hereinafter referred to as the ""Seller"") include digital content, software services, and web-based hosting spaces that are performed instantly in an electronic environment and delivered immediately to the Buyer through the website (www.styever.com), including Digital Memorial Page Setup, Digital Memorial Sharing Service, Page Customization, and Digital Commemoration Features.</p>
          <p>1.2. By completing a purchase through the Platform, the Buyer accepts, declares, and undertakes in advance, in addition to these agreement terms, that the service is entirely digital, intangible, and instantly performed.</p>
        </section>

        <section class=""mb-8"">
          <h2>2. 7-Day Trial Period and Right of Cancellation</h2>
          <p>2.1. As part of its customer satisfaction-oriented service policy, the Seller grants the Buyer a 7 (seven) day trial and unconditional cancellation period starting from the date of the first purchase, namely the date the service is initiated.</p>
          <p>2.2. If the Buyer is not satisfied with the purchased digital service or does not wish to continue, the Buyer has the right to request cancellation within 7 (seven) days from the first purchase date, without paying any penalty and without providing any reason, through the user panel on the Platform or via the official e-mail address info@styever.com.</p>
          <p>2.3. If the cancellation and withdrawal right is duly exercised within this 7 (seven) day period, the service fee collected from the Buyer shall be refunded in full and without deduction to the card used for the transaction, in accordance with the refund procedures of the payment infrastructure provider Shopier (Shopier Yazılım A.Ş.) and the relevant banks.</p>
          <p>2.4. Once the cancellation is approved by the system, the relevant Digital Memorial Page created for the Buyer shall be completely removed from publication together with all data contained therein, server data flow shall be stopped, and the Buyer shall be returned to the status of having never initiated a subscription on the Platform.</p>
        </section>

        <section class=""mb-8"">
          <h2>3. Subscription Termination and Digital Memorial Page Status</h2>
          <p>3.1. For cancellations, unilateral terminations, and subscription termination requests made after the 7 (seven) day trial period from the first purchase date has expired, no refund shall be made for the current active period (month/year). The Buyer has the right to stop automatic renewal for the next period at any time.</p>
          <p>3.2. The Digital Memorial Page previously created on the Platform by Buyers who terminate their subscription or turn off renewal shall remain published on the Platform in a ""publicly accessible"" status unless the Buyer requests complete deletion and provided that the Seller continues its Platform activities.</p>
          <p>3.3. For Buyers whose subscription has been terminated, whose active payment period has expired, and whose page has moved to free/static status, permissions to edit, upload new images/videos, change memorial text, delete, or update the relevant Digital Memorial Page are automatically frozen by the system. Even if the page remains online, the Buyer may not make any new editorial changes.</p>
          <p>3.4. Once the Buyer restarts the subscription through the Platform and activates the relevant payment cycle, all technical editing, visual/media uploading, and text modification permissions on the memorial page are restored.</p>
        </section>

        <section class=""mb-8"">
          <h2>4. Statement on Exceptions to the Right of Withdrawal</h2>
          <p>4.1. Pursuant to subparagraph (ğ) of paragraph 1 of Article 15 titled ""Exceptions to the Right of Withdrawal"" of the Distance Contracts Regulation published in the Official Gazette dated 27 November 2014, contracts regarding ""services performed instantly in an electronic environment or intangible goods delivered instantly to the consumer"" are legally exempt from the right of withdrawal, and consumers do not have a statutory right of withdrawal or refund for such services.</p>
          <p>4.2. The 7-day unconditional cancellation/trial period offered by Styever to the Buyer and the continued static publication of the page after subscription termination are not statutory obligations but additional customer satisfaction commitments offered entirely by the Seller. After the 7-day period expires, there is no statutory obligation to provide a refund.</p>
        </section>

        <section class=""mb-8"">
          <h2>5. Refund Procedure and Processing Time</h2>
          <p>5.1. Refunds duly requested within the 7-day period and approved by the Seller are sent by the Seller in a single transaction and without deduction to the credit/debit card used by the Buyer for payment through the licensed payment infrastructure provider Shopier (Shopier Yazılım A.Ş.).</p>
          <p>5.2. The time required for the refunded amount to appear in the Buyer's bank or credit card account or statement may vary depending on the internal procedures of the Buyer's bank, the card type (credit card or debit card), statement dates, and the rules of international card organizations, generally between 1 and 7 business days. The Seller immediately submits the refund instruction to the Shopier system; after this stage, the Seller cannot be held directly or indirectly liable for bank-related delays, blocking periods, or technical failures.</p>
        </section>

        <section>
          <h2>6. Service Continuity, Platform Closure, and Limitation of Liability</h2>
          <p>6.1. The Seller aims to provide the digital memorial page services offered through www.styever.com at the highest quality, taking into account commercial capabilities, technical infrastructure sustainability, and server capacities. However, the Seller does not provide an absolute or legally binding guarantee that the Platform will remain online forever, without interruption, or for a lifetime.</p>
          <p>6.2. The Seller reserves the right, entirely at its own commercial discretion, to completely cease Platform activities, close the Website, and permanently terminate all digital memorial page services offered due to economic reasons, technical impossibilities, company liquidation, or strategic decisions.</p>
          <p>6.3. If a decision is made to completely close the Platform or permanently terminate the services, the Seller is obliged to notify all users with active subscriptions or users who have previously initiated a subscription in writing at least 30 (thirty) days in advance via the e-mail addresses registered in the system.</p>
          <p>6.4. During the 30-day closure notice period, Buyers are fully responsible for backing up and downloading to their own computers or external storage devices the photographs, videos, texts, and data belonging to them on the memorial pages they created on the Platform. Upon expiry of the notice period, all servers shall be shut down and the data shall be irreversibly deleted. The Seller cannot be held liable for data losses resulting from the Buyer's failure to back up or delayed backup.</p>
          <p>6.5. If the Platform is completely closed, the service is terminated, or data is deleted due to commercial or technical necessities;</p>

          <ul class=""ps-6 mb-4"">
            <li>Users who previously used the service, terminated their subscription, and whose pages remain published in free/static status,</li>
            <li>Users whose active payment period has expired,</li>
            <li>Users who have an active subscription as of the notification date and whose remaining period fee has been refunded; the Seller shall have no legal, criminal, or financial liability toward such users.</li>
          </ul>

          <p>6.6. The Buyer irrevocably accepts, declares, and undertakes that they shall not file any material or moral compensation lawsuit against the Seller due to the complete closure of the Platform or termination of the service in compliance with the legal notification period, shall not claim previously paid subscription fees retroactively, and shall not apply to any judicial or administrative authority on the grounds that ""memories were lost, deleted, or moral damage was suffered.""</p>
        </section>", SortOrder = 3, IsDeleted = false },
                new LegalContent { Id = 4, Slug = "privacy-policy", Category = "Legal", Title = @"Gizlilik ve Güvenlik Politikası", TitleEn = @"Privacy and Security Policy", Content = @"<section class=""mb-8"">
          <h2>1. Veri Sorumlusu</h2>
          <p>6698 sayılı Kişisel Verilerin Korunması Kanunu (KVKK) uyarınca www.styever.com internet sitesinin ve Styever markasının yasal sahibi olan Styever (Bundan böyle ""Styever"" veya ""Veri Sorumlusu"" olarak anılacaktır), Veri Sorumlusu sıfatıyla hareket etmektedir. Styever, kullanıcıların kişisel verilerinin gizliliğine ve güvenliğine en üst düzeyde önem vermektedir.</p>
        </section>
        <section class=""mb-8"">
          <h2>2. Toplanan Kişisel Veriler</h2>
          <p>Platform hizmetlerinden yararlanırken aşağıdaki kişisel verileriniz işlenebilir:</p>
          <ul>
            <li>Kimlik ve İletişim Bilgileri: Ad, soyad, e-posta adresi, telefon numarası.</li>
            <li>Müşteri İşlem ve Finansal Bilgiler: Fatura detayları, sipariş geçmişi, işlem tutarları.</li>
            <li>Kullanıcı İçerik Verileri: Yüklenen fotoğraf, video, anı metinleri, yorumlar ve dijital taziye mesajları.</li>
            <li>İşlem Güvenliği ve Teknik Veriler: IP adresi, cihaz ve tarayıcı bilgileri, sistem erişim log kayıtları ve çerez (cookie) verileri.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>3. Kişisel Verilerin İşlenme Amaçları ve Hukuki Sebepleri</h2>
          <p>Toplanan kişisel verileriniz, KVKK’nın 5. ve 6. maddelerinde belirtilen kişisel veri işleme şartları dahilinde aşağıdaki amaçlar ve hukuki sebeplerle işlenmektedir:</p>
          <ul>
            <li>Sözleşmenin Kurulması ve İfası: Dijital anı sayfası hizmetlerinin sunulması, üyelik süreçlerinin yürütülmesi ve kullanıcı paneli erişiminin sağlanması.</li>
            <li>Hukuki Yükümlülüklerin Yerine Getirilmesi: Fatura düzenlenmesi, muhasebe süreçlerinin takibi ve 5651 sayılı Kanun uyarınca trafik (log) kayıtlarının tutulması.</li>
            <li>Meşru Menfaat: Platform güvenliğinin sağlanması, teknik destek süreçlerinin yürütülmesi, hileli işlemlerin önlenmesi ve kullanıcı deneyiminin geliştirilmesi.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>4. Ödeme Güvenliği ve Kredi Kartı Güvenlik Protokolü</h2>
          <p>4.1. Kredi kartı, banka kartı ve ödeme bilgileri Styever sunucularında kesinlikle tutulmaz, kaydedilmez ve saklanmaz.</p>
          <p>4.2. Tüm ödeme işlemleri, lisanslı ödeme altyapısı sağlayıcısı Shopier (Shopier Yazılım A.Ş.) üzerinden 256-bit SSL şifreleme ve Uluslararası Veri Güvenliği Standardı olan PCI DSS uyumlu altyapı ile gerçekleştirilir.</p>
          <p>4.3. Ödeme süreçlerinde kart güvenliğinin sağlanması amacıyla 3D Secure (güvenli doğrulama) protokolü zorunlu tutulmaktadır.</p>
        </section>
        <section class=""mb-8"">
          <h2>5. Verilerin Üçüncü Kişilerle Paylaşılması</h2>
          <p>Toplanan kişisel verileriniz ticari, reklam veya pazarlama amaçlarıyla üçüncü kişilere satılmaz veya devredilmez. Verileriniz yalnızca aşağıdaki taraflarla ve belirtilen amaçlarla sınırlı olarak paylaşılabilir:</p>
          <ul>
            <li>Shopier (Shopier Yazılım A.Ş.): Ödeme işlemlerinin güvenli bir şekilde tamamlanması ve tahsilatın yapılması amacıyla.</li>
            <li>Hizmet Sağlayıcılar: Barındırma (hosting), sunucu, e-posta gönderimi ve veri tabanı altyapısı sunan yetkili teknik tedarikçiler.</li>
            <li>Yetkili Kamu Kurum ve Kuruluşları: Yasal yükümlülükler uyarınca yetkili mahkemeler, icra daireleri, kolluk kuvvetleri ve idari makamlarca usulüne uygun talep edilmesi halinde.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>6. Veri Saklama Süresi, Sunucu Altyapısı ve Platformun Kapatılması Durumu</h2>
          <p>6.1. Verileriniz, ilgili yasal mevzuatta öngörülen saklama süreleri boyunca veya işleme amacının gerektirdiği süre kadar güvenli sunucularda muhafaza edilir.</p>
          <p>6.2. Tüm veriler teknik güvenlik önlemleri alınmış güvenli sunucu altyapılarında barındırılır.</p>
          <p>6.3. Platform faaliyetlerinin kalıcı olarak sonlandırılması kararı alınması halinde, kullanıcılara sistemde kayıtlı e-posta adresleri üzerinden en az 30 (otuz) gün önceden bildirim yapılır. Bildirim süresinin sonunda tüm kişisel veriler ve kullanıcı içerikleri KVKK'ya uygun olarak kalıcı olarak silinir, imha edilir veya anonim hale getirilir.</p>
        </section>
        <section class=""mb-8"">
          <h2>7. KVKK Kapsamındaki Kullanıcı Hakları ve Başvuru Usulü</h2>
          <p>KVKK’nın 11. maddesi uyarınca veri sahibi olarak aşağıdaki haklara sahipsiniz:</p>
          <ul>
            <li>Kişisel verilerinizin işlenip işlenmediğini öğrenme,</li>
            <li>Kişisel verileriniz işlenmişse buna ilişkin bilgi talep etme,</li>
            <li>Kişisel verilerinizin işlenme amacını ve amacına uygun kullanılıp kullanılmadığını öğrenme,</li>
            <li>Yurt içinde veya yurt dışında kişisel verilerinizin aktarıldığı üçüncü kişileri bilme,</li>
            <li>Kişisel verilerinizin eksik veya yanlış işlenmiş olması hâlinde bunların düzeltilmesini isteme,</li>
            <li>KVKK m. 7 çerçevesinde kişisel verilerinizin silinmesini veya yok edilmesini isteme,</li>
            <li>Aktarıldığı üçüncü kişilere yukarıdaki düzeltme ve silme işlemlerinin bildirilmesini isteme,</li>
            <li>İşlenen verilerin münhasıran otomatik sistemler vasıtasıyla analiz edilmesi suretiyle aleyhinize bir sonucun ortaya çıkmasına itiraz etme,</li>
            <li>Kişisel verilerin kanuna aykırı olarak işlenmesi sebebiyle zarara uğramanız hâlinde zararın giderilmesini talep etme.</li>
          </ul>
          <p>Haklarınıza ilişkin taleplerinizi Veri Sorumlusuna Başvuru Usul ve Esasları Hakkında Tebliğ'e uygun olarak info@styever.com adresine e-posta ile iletebilirsiniz. Talepleriniz en geç 30 (otuz) gün içerisinde ücretsiz olarak sonuçlandırılacaktır.</p>
        </section>
        <section>
          <h2>8. Çerezler (Cookies) ve Politika Güncellemeleri</h2>
          <p>Platform, sitenin işlevselliğini sağlamak ve kullanıcı deneyimini iyileştirmek amacıyla zorunlu ve analitik çerezler kullanmaktadır. İşbu Gizlilik ve Güvenlik Politikası, yasal mevzuattaki değişiklikler veya teknik gereklilikler doğrultusunda güncellenebilir. Güncel politika yayımı tarihinden itibaren geçerlilik kazanır.</p>
        </section>", ContentEn = @"<section class=""mb-8"">
          <h2>1. Data Controller</h2>
          <p>Pursuant to Turkish Personal Data Protection Law No. 6698 (KVKK), Styever, the legal owner of the www.styever.com website and the Styever brand (hereinafter referred to as ""Styever"" or the ""Data Controller""), acts as the Data Controller. Styever attaches the highest importance to the privacy and security of users' personal data.</p>
        </section>
        <section class=""mb-8"">
          <h2>2. Personal Data Collected</h2>
          <p>The following personal data may be processed while you use the Platform services:</p>
          <ul>
            <li>Identity and Contact Information: Name, surname, e-mail address, telephone number.</li>
            <li>Customer Transaction and Financial Information: Invoice details, order history, transaction amounts.</li>
            <li>User Content Data: Uploaded photographs, videos, memorial texts, comments and digital condolence messages.</li>
            <li>Transaction Security and Technical Data: IP address, device and browser information, system access logs and cookie data.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>3. Purposes and Legal Grounds for Processing Personal Data</h2>
          <p>Your collected personal data is processed for the following purposes and legal grounds within the personal data processing conditions specified in Articles 5 and 6 of the KVKK:</p>
          <ul>
            <li>Establishment and Performance of the Agreement: Providing digital memorial page services, carrying out membership processes and providing access to the user panel.</li>
            <li>Fulfillment of Legal Obligations: Issuing invoices, carrying out accounting processes and retaining traffic logs pursuant to Law No. 5651.</li>
            <li>Legitimate Interest: Ensuring Platform security, carrying out technical support processes, preventing fraudulent transactions and improving the user experience.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>4. Payment Security and Credit Card Security Protocol</h2>
          <p>4.1. Credit card, debit card and payment information is never retained, recorded or stored on Styever servers.</p>
          <p>4.2. All payment transactions are carried out through the licensed payment infrastructure provider Shopier (Shopier Yazılım A.Ş.) using 256-bit SSL encryption and infrastructure compliant with the international PCI DSS data security standard.</p>
          <p>4.3. The 3D Secure authentication protocol is mandatory in payment processes to ensure card security.</p>
        </section>
        <section class=""mb-8"">
          <h2>5. Sharing Data with Third Parties</h2>
          <p>Your collected personal data is not sold or transferred to third parties for commercial, advertising or marketing purposes. Your data may only be shared with the following parties and limited to the stated purposes:</p>
          <ul>
            <li>Shopier (Shopier Yazılım A.Ş.): For the secure completion of payment transactions and collection of payments.</li>
            <li>Service Providers: Authorized technical suppliers providing hosting, server, e-mail delivery and database infrastructure.</li>
            <li>Authorized Public Institutions and Authorities: Where duly requested by authorized courts, enforcement offices, law enforcement authorities and administrative authorities pursuant to legal obligations.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>6. Data Retention Period, Server Infrastructure and Platform Closure</h2>
          <p>6.1. Your data is retained on secure servers for the retention periods prescribed by applicable legislation or for as long as required by the purpose of processing.</p>
          <p>6.2. All data is hosted on secure server infrastructures protected by technical security measures.</p>
          <p>6.3. If a decision is made to permanently terminate Platform activities, users will be notified at least 30 (thirty) days in advance via their registered e-mail addresses. At the end of the notification period, all personal data and user content will be permanently deleted, destroyed or anonymized in accordance with the KVKK.</p>
        </section>
        <section class=""mb-8"">
          <h2>7. User Rights under the KVKK and Application Procedure</h2>
          <p>Pursuant to Article 11 of the KVKK, as a data subject you have the following rights:</p>
          <ul>
            <li>To learn whether your personal data is being processed,</li>
            <li>To request information if your personal data has been processed,</li>
            <li>To learn the purpose of processing your personal data and whether it is used in accordance with that purpose,</li>
            <li>To know the third parties to whom your personal data is transferred domestically or abroad,</li>
            <li>To request correction if your personal data has been processed incompletely or incorrectly,</li>
            <li>To request deletion or destruction of your personal data within the framework of Article 7 of the KVKK,</li>
            <li>To request notification of the above correction and deletion operations to third parties to whom your data has been transferred,</li>
            <li>To object to a result arising against you through the analysis of processed data exclusively by automated systems,</li>
            <li>To request compensation for damages if you suffer damage due to unlawful processing of personal data.</li>
          </ul>
          <p>You may submit requests concerning your rights to info@styever.com by e-mail in accordance with the Communiqué on the Procedures and Principles of Application to the Data Controller. Your requests will be concluded free of charge within 30 (thirty) days at the latest.</p>
        </section>
        <section>
          <h2>8. Cookies and Policy Updates</h2>
          <p>The Platform uses necessary and analytical cookies to ensure website functionality and improve the user experience. This Privacy and Security Policy may be updated in line with changes in applicable legislation or technical requirements. The updated policy becomes effective as of its publication date.</p>
        </section>", SortOrder = 4, IsDeleted = false },
                new LegalContent { Id = 5, Slug = "kvkk", Category = "Legal", Title = @"Kişisel Verilerin Korunması Kanunu (KVKK) Aydınlatma Metni", TitleEn = @"Personal Data Protection Law (KVKK) Information Notice", Content = @"<section class=""mb-8"">
          <h2>1. Veri Sorumlusu</h2>
          <p>6698 sayılı Kişisel Verilerin Korunması Kanunu (KVKK) uyarınca kişisel verileriniz, Veri Sorumlusu sıfatıyla Styever markasının yasal sahibi olan Styever (Bundan böyle ""Styever"" veya ""Veri Sorumlusu"" olarak anılacaktır) tarafından; hukuka ve dürüstlük kurallarına uygun, doğru ve gerektiğinde güncel, belirli, açık ve meşru amaçlar doğrultusunda işlenmektedir.</p>
        </section>

        <section class=""mb-8"">
          <h2>2. İşlenen Kişisel Veri Kategorileri ve İşleme Amaçları</h2>
          <p>Platformumuz (www.styever.com) üzerinden hizmet alırken, üyelik oluştururken veya sistemi ziyaret ederken toplanan kişisel verileriniz aşağıdaki kategoriler ve amaçlar dahilinde işlenmektedir:</p>
          <ul>
            <li>Kimlik ve İletişim Verileri (Ad, soyad, e-posta adresi, telefon numarası): Dijital anı sayfası kurulumu, üyelik hesabının oluşturulması, kullanıcı doğrulama ve iletişim süreçlerinin yürütülmesi.</li>
            <li>Müşteri İşlem ve Finansal Veriler (Fatura bilgileri, sipariş geçmişi, ödeme tutarı): Sipariş ve abonelik işlemlerinin lisanslı ödeme altyapısı sağlayıcısı Shopier (Shopier Yazılım A.Ş.) altyapısı üzerinden yürütülmesi, faturalandırma ve muhasebe süreçlerinin takibi.</li>
            <li>Görsel, İşitsel ve İçerik Verileri (Yüklenen fotoğraf, video, anı metinleri, taziye mesajları): Dijital anı sayfalarının sözleşmeye uygun şekilde oluşturulması, barındırılması ve platformda yayınlanması.</li>
            <li>İşlem Güvenliği Verileri (IP adresi, cihaz bilgileri, log kayıtları, çerezler): 5651 sayılı Kanun uyarınca trafik kayıtlarının tutulması, platform güvenliğinin sağlanması ve yetkisiz işlemlerin önlenmesi.</li>
          </ul>
        </section>

        <section class=""mb-8"">
          <h2>3. Kişisel Verilerin Aktarılması</h2>
          <p>Toplanan kişisel verileriniz hiçbir surette ticari, reklam veya pazarlama amacıyla üçüncü kişilere satılmaz veya devredilmez. Verileriniz yalnızca aşağıdaki taraflara ve amaçlarla aktarılabilir:</p>
          <ul>
            <li>Shopier (Shopier Yazılım A.Ş.): Ödeme işlemlerinin güvenli bir şekilde gerçekleştirilmesi ve tahsilat süreçlerinin tamamlanması amacıyla.</li>
            <li>Hizmet Sağlayıcılar: İnternet sitesi yayını, barındırma (hosting), veri tabanı ve sunucu hizmeti sağlayan yetkili teknik altyapı tedarikçilerine.</li>
            <li>Yetkili Kamu Kurum ve Kuruluşları: Yasal yükümlülüklerin yerine getirilmesi amacıyla mahkemeler, savcılıklar, Tüketici Hakem Heyetleri ve ilgili idari makamlara.</li>
          </ul>
          <p>Kişisel verileriniz ve yüklediğiniz anı içerikleri Türkiye Cumhuriyeti sınırları içerisindeki güvenli sunucularda saklanmakta olup açık rızanız olmaksızın yurt dışına aktarılmamaktadır.</p>
        </section>

        <section class=""mb-8"">
          <h2>4. Kişisel Veri Toplamanın Yöntemi ve Hukuki Sebebi</h2>
          <p>Kişisel verileriniz üyelik formları, sipariş ve ödeme ekranları, anı sayfası yükleme panelleri, iletişim formları, çerezler (cookies) ve sistem erişim logları aracılığıyla tamamen elektronik ortamda toplanmaktadır. Bu veriler, KVKK’nın 5. maddesinde belirtilen aşağıdaki hukuki sebeplere dayanılarak işlenmektedir:</p>
          <ul>
            <li>Sözleşmenin Kurulması veya İfası: Kullanım Şartları, Üyelik Sözleşmesi ve Mesafeli Satış Sözleşmesi kapsamındaki hizmetlerin sunulabilmesi ve siparişlerin teslimi için veri işlenmesinin zorunlu olması.</li>
            <li>Hukuki Yükümlülük: Türk Ticaret Kanunu, Vergi Usul Kanunu, 5651 sayılı Kanun ve tüketici mevzuatı kapsamındaki yasal sorumlulukların yerine getirilmesi.</li>
            <li>Meşru Menfaat: Platform güvenliğinin sağlanması, sistem performansının artırılması ve hileli işlemlerin tespiti amacıyla Veri Sorumlusunun meşru menfaatlerinin bulunması.</li>
          </ul>
        </section>

        <section class=""mb-8"">
          <h2>5. Platformun Kapatılması ve Verilerin Silinmesi Prosedürü</h2>
          <p>5.1. Styever platformu; ticari, hukuki veya teknik gerekçelerle faaliyetlerini tamamen durdurma ve internet sitesini kapatma hakkını saklı tutar.</p>
          <p>5.2. Platformun kalıcı olarak kapatılması durumunda kullanıcılara, sistemde kayıtlı e-posta adresleri üzerinden en az 30 (otuz) gün önceden bildirim yapılacaktır.</p>
          <p>5.3. Bildirim süresinin sona ermesinin ardından tüm kişisel veriler, yüklenen fotoğraflar, videolar, anı metinleri ve sistem yedekleri Kişisel Verilerin Silinmesi, Yok Edilmesi veya Anonim Hale Getirilmesi Hakkında Yönetmelik’e uygun olarak geri döndürülemeyecek şekilde silinecek, yok edilecek veya anonim hale getirilecektir.</p>
        </section>

        <section>
          <h2>6. İlgili Kişinin (Veri Sahibinin) Hakları ve Başvuru Usulü</h2>
          <p>KVKK'nın 11. maddesi uyarınca Veri Sahibi olarak aşağıdaki haklara sahipsiniz:</p>
          <ul>
            <li>Kişisel verilerinizin işlenip işlenmediğini öğrenme,</li>
            <li>Kişisel verileriniz işlenmişse buna ilişkin bilgi talep etme,</li>
            <li>Kişisel verilerinizin işlenme amacını ve amaca uygun kullanılıp kullanılmadığını öğrenme,</li>
            <li>Yurt içinde veya yurt dışında kişisel verilerinizin aktarıldığı üçüncü kişileri bilme,</li>
            <li>Kişisel verilerinizin eksik veya yanlış işlenmiş olması hâlinde bunların düzeltilmesini isteme,</li>
            <li>KVKK m. 7 çerçevesinde kişisel verilerinizin silinmesini veya yok edilmesini isteme,</li>
            <li>Aktarıldığı üçüncü kişilere yukarıdaki düzeltme ve silme işlemlerinin bildirilmesini isteme,</li>
            <li>İşlenen verilerin münhasıran otomatik sistemler vasıtasıyla analiz edilmesi suretiyle aleyhinize bir sonucun ortaya çıkmasına itiraz etme,</li>
            <li>Kişisel verilerin kanuna aykırı olarak işlenmesi sebebiyle zarara uğramanız hâlinde zararın giderilmesini talep etme.</li>
          </ul>
          <p>Haklarınıza ilişkin taleplerinizi Veri Sorumlusuna Başvuru Usul ve Esasları Hakkında Tebliğ'e uygun olarak, sistemimizde kayıtlı e-posta adresiniz üzerinden info@styever.com adresine iletebilirsiniz. Başvurunuz en geç 30 (otuz) gün içerisinde ücretsiz olarak yanıtlanacaktır.</p>
        </section>", ContentEn = @"<section class=""mb-8"">
          <h2>1. Data Controller</h2>
          <p>Pursuant to Personal Data Protection Law No. 6698 (KVKK), your personal data is processed by Styever, the legal owner of the Styever brand, acting as Data Controller (hereinafter referred to as ""Styever"" or the ""Data Controller""), in accordance with the law and principles of good faith, accurately and when necessary up to date, for specific, explicit and legitimate purposes.</p>
        </section>

        <section class=""mb-8"">
          <h2>2. Categories of Personal Data Processed and Purposes of Processing</h2>
          <p>Your personal data collected while receiving services, creating a membership or visiting our system through our Platform (www.styever.com) is processed within the following categories and purposes:</p>
          <ul>
            <li>Identity and Contact Data (Name, surname, e-mail address, telephone number): Establishment of the digital memorial page, creation of the membership account, user verification and communication processes.</li>
            <li>Customer Transaction and Financial Data (Invoice information, order history, payment amount): Carrying out order and subscription transactions through the infrastructure of the licensed payment provider Shopier (Shopier Yazılım A.Ş.), and following invoicing and accounting processes.</li>
            <li>Visual, Audio and Content Data (Uploaded photographs, videos, memorial texts, condolence messages): Creating, hosting and publishing digital memorial pages on the Platform in accordance with the agreement.</li>
            <li>Transaction Security Data (IP address, device information, log records, cookies): Retaining traffic records pursuant to Law No. 5651, ensuring Platform security and preventing unauthorized transactions.</li>
          </ul>
        </section>

        <section class=""mb-8"">
          <h2>3. Transfer of Personal Data</h2>
          <p>Your collected personal data is never sold or transferred to third parties for commercial, advertising or marketing purposes. Your data may only be transferred to the following parties for the stated purposes:</p>
          <ul>
            <li>Shopier (Shopier Yazılım A.Ş.): For the secure execution of payment transactions and completion of collection processes.</li>
            <li>Service Providers: Authorized technical infrastructure suppliers providing website publication, hosting, database and server services.</li>
            <li>Authorized Public Institutions and Authorities: Courts, prosecutors' offices, Consumer Arbitration Committees and relevant administrative authorities for the fulfillment of legal obligations.</li>
          </ul>
          <p>Your personal data and the memorial content you upload are stored on secure servers within the borders of the Republic of Türkiye and are not transferred abroad without your explicit consent.</p>
        </section>

        <section class=""mb-8"">
          <h2>4. Method and Legal Basis of Personal Data Collection</h2>
          <p>Your personal data is collected entirely electronically through membership forms, order and payment screens, memorial page upload panels, contact forms, cookies and system access logs. This data is processed based on the following legal grounds specified in Article 5 of the KVKK:</p>
          <ul>
            <li>Establishment or Performance of a Contract: Processing is necessary to provide services under the Terms of Use, Membership Agreement and Distance Sales Agreement and to deliver orders.</li>
            <li>Legal Obligation: Fulfillment of legal responsibilities under the Turkish Commercial Code, Tax Procedure Law, Law No. 5651 and consumer legislation.</li>
            <li>Legitimate Interest: The Data Controller has legitimate interests in ensuring Platform security, improving system performance and detecting fraudulent transactions.</li>
          </ul>
        </section>

        <section class=""mb-8"">
          <h2>5. Platform Closure and Data Deletion Procedure</h2>
          <p>5.1. The Styever Platform reserves the right to completely cease its activities and close the Website for commercial, legal or technical reasons.</p>
          <p>5.2. If the Platform is permanently closed, users will be notified at least 30 (thirty) days in advance via the e-mail addresses registered in the system.</p>
          <p>5.3. After the notification period expires, all personal data, uploaded photographs, videos, memorial texts and system backups will be irreversibly deleted, destroyed or anonymized in accordance with the Regulation on the Deletion, Destruction or Anonymization of Personal Data.</p>
        </section>

        <section>
          <h2>6. Rights of the Data Subject and Application Procedure</h2>
          <p>Pursuant to Article 11 of the KVKK, as a Data Subject you have the following rights:</p>
          <ul>
            <li>To learn whether your personal data is being processed,</li>
            <li>To request information if your personal data has been processed,</li>
            <li>To learn the purpose of processing your personal data and whether it is used in accordance with that purpose,</li>
            <li>To know the third parties to whom your personal data is transferred domestically or abroad,</li>
            <li>To request correction if your personal data has been processed incompletely or incorrectly,</li>
            <li>To request deletion or destruction of your personal data within the framework of Article 7 of the KVKK,</li>
            <li>To request notification of the above correction and deletion operations to third parties to whom your data has been transferred,</li>
            <li>To object to a result arising against you through the analysis of processed data exclusively by automated systems,</li>
            <li>To request compensation for damages if you suffer damage due to unlawful processing of personal data.</li>
          </ul>
          <p>You may submit requests concerning your rights to info@styever.com from the e-mail address registered in our system, in accordance with the Communiqué on the Procedures and Principles of Application to the Data Controller. Your application will be answered free of charge within 30 (thirty) days at the latest.</p>
        </section>", SortOrder = 5, IsDeleted = false },
                new LegalContent { Id = 6, Slug = "cookie-policy", Category = "Legal", Title = @"Çerez Politikası", TitleEn = @"Cookie Policy", Content = @"<section class=""mb-8"">
          <h2>1. Giriş ve Politikanın Amacı</h2>
          <p>İşbu Çerez Politikası; www.styever.com internet sitesinin (Bundan böyle ""Styever"" veya ""Platform"" olarak anılacaktır) yasal sahibi olan Styever (Veri Sorumlusu) tarafından, platformu ziyaret eden, üye olan veya dijital anı sayfası hizmetlerini kullanan kişilerin (Bundan böyle ""Kullanıcı"" veya ""Ziyaretçi"" olarak anılacaktır) gizliliğini korumak ve internet sitesinin teknik altyapısını en verimli şekilde kullanabilmesini sağlamak amacıyla hazırlanmıştır. İşbu politika, sitemizde hangi çerezlerin, hangi amaçlarla kullanıldığını ve bu çerezlerin nasıl yönetilebileceğini 6698 sayılı Kişisel Verilerin Korunması Kanunu (KVKK) çerçevesinde şeffaf bir dille açıklamaktadır.</p>
        </section>
        <section class=""mb-8"">
          <h2>2. Çerez (Cookie) Nedir?</h2>
          <p>Çerezler, bir internet sitesini ziyaret ettiğinizde tarayıcınız (Chrome, Safari, Edge, Firefox vb.) aracılığıyla cihazınıza (bilgisayar, akıllı telefon, tablet) yerleştirilen, küçük boyutlu metin dosyalarıdır. Çerezler, internet sitelerinin sizin siteyi tekrar ziyaret ettiğinizi hatırlamasına, oturumunuzun güvenli bir şekilde açık kalmasına ve size daha stabil, hızlı ve kişiselleştirilmiş bir dijital deneyim sunulmasına yardımcı olur.</p>
        </section>
        <section class=""mb-8"">
          <h2>3. Çerezlerin Kullanım Amaçları ve Hukuki Sebepleri</h2>
          <p>Styever platformunda çerezler, reklam, hedefleme veya üçüncü taraf ticari pazarlama amacıyla kesinlikle kullanılmaz. Çerezler yalnızca aşağıdaki yasal ve teknik sebeplerle işlenmektedir:</p>
          <ul>
            <li>Oturum Yönetimi ve İşlevsellik (Sözleşmenin İfası - KVKK m. 5/2-c): Kullanıcıların üye paneline giriş yaptıklarında oturumlarının açık kalmasını sağlamak, her sayfa geçişinde yeniden şifre girme zorunluluğunu ortadan kaldırmak ve platformdaki dil/görünüm tercihlerini hatırlamak.</li>
            <li>Sistem ve Ödeme Güvenliği (Veri Sorumlusunun Meşru Menfaati ve Kanunlarda Öngörülme - KVKK m. 5/2-e, f): Platformun siber güvenliğini korumak, sahte üyelikleri veya yetkisiz hesap erişimlerini engellemek. Özellikle Shopier (Shopier Yazılım A.Ş.) altyapısı üzerinden gerçekleştirilen sanal POS ödeme formlarının güvenli, SSL şifreli ve hileli işlemlerden uzak bir şekilde çalışmasını sağlamak.</li>
            <li>Performans ve Teknik Analiz (Veri Sorumlusunun Meşru Menfaati - KVKK m. 5/2-f): Sitemizin açılış hızını optimize etmek, hangi sayfaların daha stabil çalıştığını tespit etmek ve sunucu kaynaklarımızı kullanıcı yoğunluğuna göre doğru yönetebilmek amacıyla tamamen anonim (isimsiz) istatistiksel veriler toplamak.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>4. Platformumuzda Kullanılan Çerez Türleri</h2>
          <ul>
            <li>Zorunlu ve Teknik Çerezler: İnternet sitesinin düzgün şekilde çalışabilmesi, sayfalar arası geçiş yapılabilmesi, üyelik girişlerinin doğrulanması ve Shopier ödeme adımlarının güvenle tamamlanabilmesi için kullanımı zorunlu olan çerezlerdir. Bu çerezler devre dışı bırakıldığında platformun ana fonksiyonları ve ödeme altyapısı çalışamaz hale gelir.</li>
            <li>Performans ve Analitik Çerezler: Sitenin ziyaretçi sayılarını, sayfada kalma sürelerini ve teknik hata alınan alanları tamamen anonim (kullanıcı kimliğiyle eşleştirilemeyen) olarak tespit eden çerezlerdir. Bu veriler yalnızca sitenin hızını ve kullanıcı deneyimini iyileştirmek amacıyla kullanılır.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>5. Çerezlerin Yönetilmesi ve Devre Dışı Bırakılması</h2>
          <p>Kullanıcılar ve ziyaretçiler, platformumuzu kullandıkları internet tarayıcılarının (browser) ayarlarını değiştirerek çerezleri tamamen engelleme, sınırlandırma veya cihazlarına kaydedildiğinde uyarı alma hakkına sahiptir. Tarayıcınızın ayarlar menüsünden geçmiş çerez verilerini dilediğiniz zaman silebilirsiniz.</p>
          <p>Sık kullanılan tarayıcılarda çerez yönetimi için aşağıdaki adımları izleyebilirsiniz:</p>
          <ul>
            <li>Google Chrome: Ayarlar > Gizlilik ve Güvenlik > Çerezler ve Diğer Site Verileri</li>
            <li>Safari: Tercihler > Gizlilik > Çerezleri ve Web Sitesi Verilerini Engelle</li>
            <li>Mozilla Firefox: Seçenekler > Gizlilik ve Güvenlik > Çerezler ve Site Verileri</li>
            <li>Microsoft Edge: Ayarlar > Gizlilik, Arama ve Hizmetler > Çerezler</li>
          </ul>
          <p><strong>Önemli Not: Zorunlu ve teknik çerezlerin tarayıcı ayarları üzerinden tamamen devre dışı bırakılması veya silinmesi durumunda, Styever platformundaki üyelik panelinize giriş yapamayabilir, anı sayfalarınızı düzenleyemeyebilir veya Shopier ödeme ekranlarında teknik aksaklıklar yaşayabilirsiniz.</strong></p>
        </section>
        <section>
          <h2>6. Güncellemeler ve Yürürlük</h2>
          <p>Styever, yasal mevzuattaki değişiklikler, Kişisel Verileri Koruma Kurulu kararları veya ödeme kuruluşlarının teknik güvenlik kriterleri doğrultusunda işbu Çerez Politikası’nı gerekli gördüğü durumlarda güncelleme hakkını saklı tutar. Güncellenmiş politika sitede yayınlandığı andan itibaren yürürlüğe girer.</p>
          <p>Çerez kullanımı ve gizliliğinizle ilgili her türlü soru, görüş veya talebinizi info@styever.com resmi e-posta adresimize her zaman yazılı olarak iletebilirsiniz.</p>
        </section>", ContentEn = @"<section class=""mb-8"">
          <h2>1. Introduction and Purpose of the Policy</h2>
          <p>This Cookie Policy has been prepared by Styever (Data Controller), the legal owner of the www.styever.com website (hereinafter referred to as ""Styever"" or the ""Platform""), in order to protect the privacy of persons who visit the Platform, become members or use digital memorial page services (hereinafter referred to as ""Users"" or ""Visitors"") and to ensure the most efficient use of the Website's technical infrastructure. This policy transparently explains which cookies are used on our Website, for what purposes they are used and how they can be managed within the framework of Personal Data Protection Law No. 6698 (KVKK).</p>
        </section>
        <section class=""mb-8"">
          <h2>2. What is a Cookie?</h2>
          <p>Cookies are small text files placed on your device (computer, smartphone, tablet) through your browser (Chrome, Safari, Edge, Firefox, etc.) when you visit a website. Cookies help websites remember that you have visited the site again, keep your session securely open and provide you with a more stable, faster and personalized digital experience.</p>
        </section>
        <section class=""mb-8"">
          <h2>3. Purposes and Legal Grounds for the Use of Cookies</h2>
          <p>Cookies on the Styever Platform are never used for advertising, targeting or third-party commercial marketing purposes. Cookies are processed solely for the following legal and technical reasons:</p>
          <ul>
            <li>Session Management and Functionality (Performance of the Contract - KVKK Art. 5/2-c): To keep users' sessions open after they log in to the member panel, eliminate the need to re-enter passwords on every page transition and remember language/display preferences on the Platform.</li>
            <li>System and Payment Security (Legitimate Interest of the Data Controller and Provided by Law - KVKK Art. 5/2-e, f): To protect the Platform's cybersecurity and prevent fake memberships or unauthorized account access. In particular, to ensure that virtual POS payment forms operated through the Shopier (Shopier Yazılım A.Ş.) infrastructure function securely, with SSL encryption and free from fraudulent transactions.</li>
            <li>Performance and Technical Analysis (Legitimate Interest of the Data Controller - KVKK Art. 5/2-f): To collect fully anonymous statistical data in order to optimize the Website's loading speed, identify which pages operate more stably and manage server resources appropriately according to user traffic.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>4. Types of Cookies Used on Our Platform</h2>
          <ul>
            <li>Necessary and Technical Cookies: Cookies required for the Website to function properly, enable navigation between pages, verify membership logins and securely complete Shopier payment steps. If these cookies are disabled, the Platform's core functions and payment infrastructure may become unavailable.</li>
            <li>Performance and Analytics Cookies: Cookies that identify visitor numbers, time spent on pages and areas where technical errors occur in a fully anonymous manner that cannot be matched with user identity. This data is used solely to improve Website speed and user experience.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>5. Managing and Disabling Cookies</h2>
          <p>Users and visitors have the right to completely block or restrict cookies, or receive notifications when cookies are stored on their devices, by changing the settings of the internet browser they use to access our Platform. You may delete previously stored cookie data at any time through your browser settings.</p>
          <p>You can follow the steps below to manage cookies in commonly used browsers:</p>
          <ul>
            <li>Google Chrome: Settings > Privacy and Security > Cookies and Other Site Data</li>
            <li>Safari: Preferences > Privacy > Block Cookies and Website Data</li>
            <li>Mozilla Firefox: Options > Privacy and Security > Cookies and Site Data</li>
            <li>Microsoft Edge: Settings > Privacy, Search and Services > Cookies</li>
          </ul>
          <p><strong>Important Note: If necessary and technical cookies are completely disabled or deleted through browser settings, you may be unable to log in to your membership panel on the Styever Platform, edit your memorial pages or may experience technical problems on Shopier payment screens.</strong></p>
        </section>
        <section>
          <h2>6. Updates and Entry into Force</h2>
          <p>Styever reserves the right to update this Cookie Policy when deemed necessary in line with changes in applicable legislation, decisions of the Personal Data Protection Board or technical security criteria of payment institutions. The updated policy enters into force as soon as it is published on the Website.</p>
          <p>You may always submit any questions, opinions or requests regarding cookie usage and your privacy in writing to our official e-mail address info@styever.com.</p>
        </section>", SortOrder = 6, IsDeleted = false },
                new LegalContent { Id = 7, Slug = "legal-warning", Category = "Legal", Title = @"Yasal Uyarı ve Sorumluluk Reddi Beyanı", TitleEn = @"Legal Notice and Disclaimer", Content = @"<section class=""mb-8"">
          <h2>1. Hizmetin Niteliği ve Kabul</h2>
          <p>www.styever.com internet sitesini (Bundan böyle ""Platform"" veya ""Styever"" olarak anılacaktır) ziyaret eden, üye olan, dijital anı sayfası oluşturan veya platformdaki içeriklerle etkileşime giren tüm kullanıcılar, işbu Yasal Uyarı ve Sorumluluk Reddi Beyanı’nda yer alan tüm şartları peşinen okumuş, anlamış ve kayıtsız şartsız kabul etmiş sayılırlar.</p>
          <p>Styever, vefat eden evcil hayvanların anılarını yaşatmak amacıyla tamamen dijital ortamda barındırma (hosting), yazılım ve sayfa özelleştirme hizmeti sunan ticari bir platformdur. Platform bünyesinde kesinlikle bir dernek, vakıf, yardım toplama veya bağış faaliyeti yürütülmemektedir.</p>
        </section>
        <section class=""mb-8"">
          <h2>2. Kullanıcı İçerikleri ve Yer Sağlayıcı Statüsü</h2>
          <p>2.1. Styever platformu üzerinde oluşturulan dijital anı sayfalarında yayınlanan tüm fotoğraflar, videolar, yazılar, anı metinleri ve ziyaretçiler tarafından bırakılan taziye yorumları (Bundan böyle ""Kullanıcı İçeriği"" olarak anılacaktır) tamamen ilgili kullanıcıların ve ziyaretçilerin kendi inisiyatifleriyle sisteme yüklenmektedir.</p>
          <p>2.2. Styever, 5651 sayılı İnternet Ortamında Yapılan Yayınların Düzenlenmesi ve Bu Yayınlar Yoluyla İşlenen Suçlarla Mücadele Edilmesi Hakkında Kanun uyarınca ""Yer Sağlayıcı"" sıfatına sahiptir. Mevzuat gereği, Styever’ın kullanıcılar tarafından yüklenen içeriklerin doğruluğunu, güvenilirliğini, hukuka uygunluğunu veya telif haklarına uygunluğunu önceden kontrol etme, editoryal olarak inceleme veya araştırma yükümlülüğü kesinlikle bulunmamaktadır.</p>
          <p>2.3. Platform üzerinde paylaşılan içeriklerin ve yazılan yorumların tüm hukuki, cezai, mali ve idari sorumluluğu doğrudan o içeriği yükleyen veya yorumu yapan kişiye aittir. Styever, kullanıcıların veya üçüncü şahısların platformdaki beyanlarından, iddialarından veya paylaşımlarından ötürü doğrudan, dolaylı ya da müteselsilen sorumlu tutulamaz.</p>
        </section>
        <section class=""mb-8"">
          <h2>3. Uyar-Kaldır Mekanizması ve Müdahale Yetkisi</h2>
          <p>Styever, platformun huzurlu, saygılı ve yasalara uygun bir anma alanı olarak kalmasını hedefler. Bu doğrultuda, genel ahlaka, kamu düzenine aykırı, kişilik haklarını ihlal eden, hakaret veya telif hakkı ihlali içeren herhangi bir içerik tespit edildiğinde ya da info@styever.com adresi üzerinden hak sahipleri tarafından haklı bir şikayet (uyar-kaldır bildirimi) ulaştırıldığında; Styever, söz konusu içeriği hiçbir ön bildirimde bulunmaksızın yayından kaldırma, silme, erişimi engelleme veya ilgili kullanıcı hesabını askıya alma/kapatma hakkını saklı tutar.</p>
        </section>
        <section class=""mb-8"">
          <h2>4. Sorumluluğun Sınırlandırılması</h2>
          <p>Styever, aşağıdaki durumlardan dolayı doğrudan veya dolaylı, maddi ya da manevi hiçbir zarar, ziyan veya tazminat talebinden sorumlu tutulamaz:</p>
          <ul>
            <li>Kullanıcılar veya ziyaretçiler tarafından hukuka aykırı şekilde paylaşılan telifli görseller, videolar, hakaret içeren metinler veya kişilik hakları ihlalleri,</li>
            <li>Platformun altyapısını oluşturan veri barındırma (hosting) şirketlerinden, internet servis sağlayıcılarından veya siber saldırılardan (hacking, DDoS vb.) kaynaklanabilecek geçici veya kalıcı teknik aksaklıklar, veri kayıpları ya da erişim kesintileri,</li>
            <li>Ödeme altyapısı sağlayıcısı Shopier (Shopier Yazılım A.Ş.) veya bankaların sistemlerinde meydana gelebilecek teknik gecikmeler, POS hataları, kesintiler veya kart işlem aksaklıkları,</li>
            <li>Kullanıcıların kendi cihazlarından, tarayıcılarından, veri depolama eksikliklerinden veya internet bağlantılarından kaynaklanan erişim problemleri.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>5. Platformun Kapatılması ve Veri Silme Hakkı</h2>
          <p>Styever’ın yasal sahibi (Styever), tamamen kendi ticari kararı, ekonomik lüzumlar veya teknik imkansızlıklar doğrultusunda platform faaliyetlerini tamamen sonlandırma ve internet sitesini kalıcı olarak kapatma hakkına sahiptir. Platformun kapatılması durumunda, yasal sözleşmelerde belirtilen 30 (otuz) günlük ön bildirim süresi içinde kendi verilerini ve fotoğraflarını yedeklemeyen kullanıcıların uğrayacağı veri kayıplarından Styever sorumlu tutulamaz. Sistemlerin tamamen kapatılması ve verilerin KVKK mevzuatına uygun olarak kalıcı şekilde silinmesi nedeniyle Styever'a karşı ""manevi hatıraların kaybolduğu"", ""anıların silindiği"" veya ""zarara uğranıldığı"" iddiasıyla hiçbir maddi veya manevi tazminat davası açılamaz.</p>
        </section>
        <section class=""mb-8"">
          <h2>6. Fikri Mülkiyet ve Telif Hakları</h2>
          <p>Styever platformunun tasarımı, yazılım kodları, logosu, ""Styever"" markası, alan adı ve platform bünyesinde sunulan tüm görsel/grafik materyaller üzerindeki tüm telif ve mülkiyet hakları Styever’a aittir. 5846 sayılı Fikir ve Sanat Eserleri Kanunu ile 6769 sayılı Sınai Mülkiyet Kanunu uyarınca, Styever'ın yazılı izni olmaksızın platform bileşenlerinin kopyalanması, çoğaltılması veya ticari amaçla kullanılması yasaktır.</p>
        </section>
        <section>
          <h2>7. Güncellemeler, Uygulanacak Hukuk ve Yetkili Mahkeme</h2>
          <p>7.1. Styever, işbu Yasal Uyarı metninde yer alan maddeleri, değişen ulusal kanunlar, e-ticaret mevzuatları veya teknik gereksinimler doğrultusunda dilediği zaman tek taraflı olarak güncelleme hakkını saklı tutar. Güncel metin sitede yayınlandığı andan itibaren tüm kullanıcılar için bağlayıcı hale gelir.</p>
          <p>7.2. İşbu metnin uygulanmasından ve yorumlanmasından doğacak her türlü uyuşmazlıkta Türkiye Cumhuriyeti kanunları uygulanır. Uyuşmazlıkların çözümünde, Ticaret Bakanlığı tarafından her yıl ilan edilen yasal parasal sınırlar dahilinde Tüketici Hakem Heyetleri ile Ankara (Çankaya) Mahkemeleri ve İcra Daireleri yetkilidir.</p>
        </section>", ContentEn = @"<section class=""mb-8"">
          <h2>1. Nature and Acceptance of the Service</h2>
          <p>All users who visit the www.styever.com website (hereinafter referred to as the ""Platform"" or ""Styever""), become members, create a digital memorial page or interact with content on the Platform are deemed to have read, understood and unconditionally accepted all terms contained in this Legal Notice and Disclaimer in advance.</p>
          <p>Styever is a commercial platform that provides fully digital hosting, software and page customization services for preserving the memories of deceased pets. The Platform does not conduct any association, foundation, fundraising or donation activities.</p>
        </section>
        <section class=""mb-8"">
          <h2>2. User Content and Hosting Provider Status</h2>
          <p>2.1. All photographs, videos, writings, memorial texts and condolence comments left by visitors and published on digital memorial pages created on the Styever Platform (hereinafter referred to as ""User Content"") are uploaded to the system entirely at the discretion of the relevant users and visitors.</p>
          <p>2.2. Styever acts as a ""Hosting Provider"" pursuant to Law No. 5651 on the Regulation of Publications on the Internet and Combating Crimes Committed by Means of Such Publications. Under applicable legislation, Styever has no obligation to pre-screen, editorially review or investigate the accuracy, reliability, legality or copyright compliance of content uploaded by users.</p>
          <p>2.3. All legal, criminal, financial and administrative responsibility for content shared and comments posted on the Platform belongs directly to the person who uploaded the content or posted the comment. Styever cannot be held directly, indirectly or jointly liable for statements, claims or posts made on the Platform by users or third parties.</p>
        </section>
        <section class=""mb-8"">
          <h2>3. Notice-and-Takedown Mechanism and Right to Intervene</h2>
          <p>Styever aims to keep the Platform a peaceful, respectful and lawful memorial space. Accordingly, when any content contrary to public morals or public order, violating personality rights, containing insults or infringing copyright is detected, or when a justified complaint (notice-and-takedown notification) is submitted by rights holders via info@styever.com, Styever reserves the right to remove or delete the relevant content, block access to it, or suspend/terminate the relevant user account without prior notice.</p>
        </section>
        <section class=""mb-8"">
          <h2>4. Limitation of Liability</h2>
          <p>Styever cannot be held liable for any direct or indirect, material or non-material loss, damage or compensation claims arising from the following circumstances:</p>
          <ul>
            <li>Copyrighted images or videos, insulting texts or violations of personality rights unlawfully shared by users or visitors,</li>
            <li>Temporary or permanent technical failures, data loss or access interruptions arising from hosting companies forming the Platform infrastructure, internet service providers or cyberattacks (hacking, DDoS, etc.),</li>
            <li>Technical delays, POS errors, interruptions or card transaction failures that may occur in the systems of the payment infrastructure provider Shopier (Shopier Yazılım A.Ş.) or banks,</li>
            <li>Access problems arising from users' own devices, browsers, insufficient data storage or internet connections.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>5. Platform Closure and Right to Delete Data</h2>
          <p>The legal owner of Styever has the right to completely terminate Platform activities and permanently close the Website based entirely on her own commercial decision, economic necessities or technical impossibilities. In the event of Platform closure, Styever cannot be held responsible for data losses suffered by users who fail to back up their own data and photographs within the 30 (thirty) day advance notice period specified in the legal agreements. No material or non-material compensation claim may be brought against Styever on the grounds that ""sentimental memories were lost"", ""memories were deleted"" or ""damage was suffered"" as a result of the complete shutdown of the systems and permanent deletion of data in accordance with KVKK legislation.</p>
        </section>
        <section class=""mb-8"">
          <h2>6. Intellectual Property and Copyright</h2>
          <p>All copyright and proprietary rights in the design, software code, logo, ""Styever"" trademark, domain name and all visual/graphic materials offered within the Styever Platform belong to Styever. Pursuant to Law No. 5846 on Intellectual and Artistic Works and Industrial Property Law No. 6769, copying, reproducing or commercially using Platform components without Styever's written permission is prohibited.</p>
        </section>
        <section>
          <h2>7. Updates, Applicable Law and Competent Courts</h2>
          <p>7.1. Styever reserves the right to unilaterally update the provisions of this Legal Notice at any time in line with changing national laws, e-commerce legislation or technical requirements. The updated text becomes binding on all users as soon as it is published on the Website.</p>
          <p>7.2. The laws of the Republic of Türkiye shall apply to all disputes arising from the implementation and interpretation of this text. Within the statutory monetary limits announced annually by the Ministry of Trade, Consumer Arbitration Committees and the Courts and Enforcement Offices of Ankara (Çankaya) shall have jurisdiction over the resolution of disputes.</p>
        </section>", SortOrder = 7, IsDeleted = false },
                new LegalContent { Id = 8, Slug = "community-rules", Category = "Community", Title = @"Topluluk Kuralları", TitleEn = @"Community Guidelines", Content = @"<section class=""mb-8"">
          <p>Styever, kaybettiğimiz can dostlarımızın hatıralarını saygılı, huzurlu ve güvenli bir ortamda yaşatmak amacıyla kurulmuştur. Platformumuzu ziyaret eden, anı sayfası oluşturan ve etkileşimde bulunan herkes, bu ortak alanın huzurunu ve yasalara uygunluğunu korumak adına aşağıdaki temel kurallara uymayı peşinen kabul eder.</p>
        </section>
        <section class=""mb-8"">
          <h2>1. Saygı ve Anı Politikası</h2>
          <p>Kayıp dostlarımızın ardından tutulan yasa ve geride kalan hatıralara yaklaşırken her zaman saygılı olmak ana kuralımızdır. Anı sayfalarına bırakılacak taziye mesajları ve yorumlar yapıcı, nazik ve saygılı olmalı; buranın sessiz ve huzurlu bir hatıra alanı olduğu unutulmamalıdır. Hakaret, tehdit, taciz, kişisel saldırı ve nefret söylemi içeren hiçbir içeriğe platformda geçit verilmez.</p>
        </section>
        <section class=""mb-8"">
          <h2>2. İçerik Sınırları ve Denetim</h2>
          <p>Styever anı sayfaları, yalnızca evcil hayvanlarımızın anılarını yaşatmak amacıyla kullanılabilir. Sayfalarda, profil alanlarında veya yorumlarda:</p>
          <ul>
            <li>Siyasi propaganda, ideolojik sembol veya tartışmalar,</li>
            <li>Ticari reklam, ürün satışı, ilan veya izinsiz yönlendirici linkler,</li>
            <li>6698 sayılı KVKK kapsamına aykırı şekilde üçüncü kişilere ait kişisel veriler (telefon numarası, açık adres, özel hayatın gizliliğini ihlal eden görseller vb.),</li>
            <li>Genel ahlaka, kamu düzenine ve yürürlükteki mevzuata aykırı görsel veya metinler paylaşılması kesinlikle yasaktır.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>3. Platformda Yasaklanan Davranışlar</h2>
          <p>Aşağıda belirtilen eylemler Topluluk Kuralları'nın doğrudan ihlali sayılır ve yaptırıma tabidir:</p>
          <ul>
            <li>Saldırgan Dil ve Hakaret: Küfür, hakaret, aşağılama, tehdit veya taciz içeren yorum ve paylaşımlar,</li>
            <li>Ayrımcılık ve Nefret Söylemi: Irk, din, dil, cinsiyet veya sosyal statü temelinde ayrımcılık içeren ya da toplumsal kutuplaşmayı körükleyen ifadeler,</li>
            <li>Spam ve Reklam: Ticari amaçlı mesajlar, tekrarlanan linkler, sahte ürün/hizmet tanıtımları,</li>
            <li>Telif ve Fikri Mülkiyet İhlali: Hak sahibi olunmayan, internetten izinsiz alınan veya üçüncü şahıslara ait telifli fotoğraf, video ve yazılı materyallerin paylaşılması,</li>
            <li>Kişisel Veri İhlali: Rızası bulunmayan kişilerin fotoğraflarının, isimlerinin veya iletişim bilgilerinin izinsiz yayınlanması.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>4. Uygunsuz İçerik Bildirimi ve Müdahale Mekanizması</h2>
          <p>Platformda yukarıdaki kurallara uymayan, sizi rahatsız eden veya can dostlarımızın anısına yakışmadığını düşündüğünüz bir içerik ya da yorum gördüğünüzde, bunu ilgili sayfada yer alan ""İçerik Bildir"" butonu üzerinden veya doğrudan info@styever.com e-posta adresimize iletebilirsiniz.</p>
          <p>Gelen bildirimler teknik ekibimiz tarafından derhal incelemeye alınır. Kuralları ihlal ettiği tespit edilen içerikler:</p>
          <ul>
            <li>Herhangi bir ön bildirim yapılmaksızın düzenlenebilir veya tamamen platformdan kaldırılabilir,</li>
            <li>Kural ihlalini alışkanlık haline getiren veya ağır ihlalde bulunan kullanıcıların anı sayfaları dondurulabilir, üyelik hesapları askıya alınabilir veya kalıcı olarak kapatılabilir.</li>
          </ul>
        </section>
        <section>
          <h2>5. Yasal Sorumluluk</h2>
          <p>5651 sayılı Kanun uyarınca ""Yer Sağlayıcı"" konumunda olan Styever, kullanıcılar tarafından yüklenen içerikleri önceden editoryal olarak inceleme yükümlülüğüne sahip değildir. Anı sayfalarına yüklenen görsellerin, yazılan metinlerin ve taziye yorumlarının tüm hukuki, cezai, idari ve mali sorumluluğu doğrudan o içeriği oluşturan kişiye aittir. Styever, platformun yasalara ve topluluk huzuruna uygunluğunu sağlamak adına gerekli her türlü teknik ve idari tedbiri alma hakkını saklı tutar.</p>
        </section>", ContentEn = @"<section class=""mb-8"">
          <p>Styever was established to preserve the memories of our beloved companions we have lost in a respectful, peaceful and safe environment. Everyone who visits our Platform, creates a memorial page or interacts with it agrees in advance to comply with the following basic rules in order to preserve the peace and legality of this shared space.</p>
        </section>
        <section class=""mb-8"">
          <h2>1. Respect and Memorial Policy</h2>
          <p>Our primary rule is to always act respectfully toward the grief experienced after the loss of our companions and the memories left behind. Condolence messages and comments posted on memorial pages must be constructive, kind and respectful, and users should remember that this is a quiet and peaceful memorial space. Content containing insults, threats, harassment, personal attacks or hate speech is not permitted on the Platform.</p>
        </section>
        <section class=""mb-8"">
          <h2>2. Content Boundaries and Moderation</h2>
          <p>Styever memorial pages may only be used to preserve the memories of our pets. The following are strictly prohibited on pages, profile areas or in comments:</p>
          <ul>
            <li>Political propaganda, ideological symbols or debates,</li>
            <li>Commercial advertising, product sales, listings or unauthorized redirect links,</li>
            <li>Personal data belonging to third parties shared in violation of Law No. 6698 (KVKK), such as telephone numbers, full addresses or images violating privacy,</li>
            <li>Visual or written content contrary to public morals, public order or applicable legislation.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>3. Prohibited Conduct on the Platform</h2>
          <p>The following actions constitute direct violations of the Community Guidelines and are subject to sanctions:</p>
          <ul>
            <li>Aggressive Language and Insults: Comments or posts containing profanity, insults, humiliation, threats or harassment,</li>
            <li>Discrimination and Hate Speech: Statements involving discrimination based on race, religion, language, gender or social status, or statements that encourage social polarization,</li>
            <li>Spam and Advertising: Commercial messages, repeated links and promotions of fake products/services,</li>
            <li>Copyright and Intellectual Property Infringement: Sharing copyrighted photographs, videos or written materials that the user does not own, that were taken from the internet without permission or that belong to third parties,</li>
            <li>Personal Data Violations: Publishing photographs, names or contact information of persons without their consent.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>4. Reporting Inappropriate Content and Intervention Mechanism</h2>
          <p>If you encounter content or a comment on the Platform that violates the above rules, disturbs you or that you believe is inappropriate for the memory of our beloved companions, you may report it through the ""Report Content"" button on the relevant page or directly via our e-mail address info@styever.com.</p>
          <p>Incoming reports are immediately reviewed by our technical team. Content found to violate the rules may be subject to the following actions:</p>
          <ul>
            <li>It may be edited or completely removed from the Platform without prior notice,</li>
            <li>Memorial pages of users who repeatedly violate the rules or commit serious violations may be frozen, and their membership accounts may be suspended or permanently terminated.</li>
          </ul>
        </section>
        <section>
          <h2>5. Legal Responsibility</h2>
          <p>As a ""Hosting Provider"" under Law No. 5651, Styever has no obligation to editorially review user-uploaded content in advance. All legal, criminal, administrative and financial responsibility for images uploaded to memorial pages, written texts and condolence comments belongs directly to the person who created the content. Styever reserves the right to take all necessary technical and administrative measures to ensure that the Platform complies with the law and maintains community peace.</p>
        </section>", SortOrder = 8, IsDeleted = false },
                new LegalContent { Id = 9, Slug = "moderation-policy", Category = "Community", Title = @"Moderasyon Politikası ve İçerik Yönetimi", TitleEn = @"Moderation Policy and Content Review", Content = @"<section class=""mb-8"">
          <h2>1. Güvenli ve Saygılı Bir Anma Alanı</h2>
          <p>Styever olarak en büyük önceliğimiz, kaybettiğimiz can dostlarımızın hatıralarının hak ettiği saygıyı, huzuru ve güveni platformumuzda bulabilmesidir. Bu amaçla, platform genelinde oluşturulan anı sayfaları, yüklenen medya dosyaları ve yazılan taziye yorumları belirli moderasyon kurallarına ve yasal standartlara tabidir.</p>
        </section>
        <section class=""mb-8"">
          <h2>2. Yer Sağlayıcı Statüsü ve İçerik Sorumluluğu</h2>
          <p>2.1. Styever, 5651 sayılı İnternet Ortamında Yapılan Yayınların Düzenlenmesi ve Bu Yayınlar Yoluyla İşlenen Suçlarla Mücadele Edilmesi Hakkında Kanun kapsamında ""Yer Sağlayıcı"" sıfatına sahiptir.</p>
          <p>2.2. Platformumuzda oluşturulan anı sayfalarının, bu sayfalara yüklenen görsellerin, videoların ve yazılan her türlü metnin hukuki, cezai, mali ve idari sorumluluğu tamamen o içeriği yükleyen veya yorumu yapan kullanıcıya aittir. Styever, yasal bir zorunluluk veya haklı bir şikayet olmadıkça kullanıcı içeriklerini önceden editoryal olarak incelemek veya doğruluğunu kontrol etmekle yükümlü değildir.</p>
          <p>2.3. Bununla birlikte Styever; platformun huzurunu bozacak, genel ahlaka, kamu düzenine, telif haklarına veya topluluk kurallarımıza aykırı bir durum tespit edildiğinde, söz konusu içeriği herhangi bir ön bildirim yapmaksızın kaldırma, yayından çekme, düzenleme veya erişimini engelleme hakkını saklı tutar.</p>
        </section>
        <section class=""mb-8"">
          <h2>3. Denetim Kapsamı ve Müdahale Kriterleri</h2>
          <p>Aşağıda belirtilen kriterlere uymayan anı sayfaları, fotoğraflar, videolar, kullanıcı yorumları ve profil bilgileri moderasyon sistemimize takılır ve derhal müdahale edilir:</p>
          <ul>
            <li>Diğer kullanıcılara, vefat eden canlıların hatırasına veya üçüncü şahıslara yönelik hakaret, küfür, aşağılama ve saldırgan ifadeler,</li>
<li>Toplumu kutuplaştıran veya inciten nefret söylemleri, tehdit, taciz ve şantaj nitelikli paylaşımlar,</li>
<li>Ticari reklamlar, ürün/hizmet tanıtımları, ilanlar, spam içerikler ve yönlendirici izinsiz bağlantılar (linkler),</li>
<li>5846 sayılı Fikir ve Sanat Eserleri Kanunu’na aykırı olarak hak sahibi olunmayan telifli görseller, videolar veya yazılı materyaller,</li>
<li>6698 sayılı KVKK’ya aykırı şekilde üçüncü şahıslara ait kişisel verilerin (açık adres, telefon numarası, izinsiz kişi görselleri vb.) paylaşılması,</li>
<li>Evcil hayvan anma amacı dışındaki yasa dışı, genel ahlaka aykırı, şiddet övücü veya rahatsız edici paylaşımlar.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>4. Moderasyon Kararları ve Yaptırım Derecelendirmesi</h2>
          <p>Kuralların ihlal edilmesi durumunda platform yönetimimiz; ihlalin niteliğine ve ağırlığına göre aşağıdaki adımları anlık olarak uygulama yetkisine sahiptir:</p>
          <ul>
            <li>Uyarı ve İçerik Kaldırma: Hukuka veya kurallara aykırı yorum, metin veya medyanın derhal platformdan silinmesi.</li>
            <li>Erişim Kısıtlaması: İlgili anı sayfasının geçici veya kalıcı olarak yorumlara/ziyarete kapatılması.</li>
            <li>Hesap Askıya Alma / Kapatma: Kural ihlalini alışkanlık haline getiren veya ağır ihlalde bulunan kullanıcıların üyelik hesaplarının dondurulması veya kalıcı olarak sonlandırılması.</li>
          </ul>
          <p>Moderasyon kararları platformun huzurunu ve yasal güvenliğini korumak adına anlık olarak uygulanır. Gerekli görülen durumlarda işlem detayları hesap sahibine e-posta yoluyla bildirilebilir.</p>
        </section>
        <section>
          <h2>5. İçerik Bildirim Mekanizması ve Uyar-Kaldır Süreci</h2>
          <p>Platformumuzda kurallara aykırı, telif ihlali içeren veya can dostlarımızın anısına yakışmayan bir içerik ya da yorumla karşılaştığınızda:</p>
          <ul>
            <li>İlgili sayfada yer alan ""İçerik Bildir"" butonunu kullanabilir veya</li>
            <li>Doğrudan info@styever.com e-posta adresi üzerinden gerekçeli bildirimde bulunabilirsiniz.</li>
          </ul>
          <p>Gelen bildirimler ekibimiz tarafından 5651 sayılı Kanun ve ""Uyar-Kaldır"" prensibi uyarınca en kısa sürede titizlikle incelenir; ihlal tespit edilen içerikler derhal yayından kaldırılır. Hak ihlaline konu resmi adli veya idari taleplerde, yasal mevzuatın gerektirdiği log ve veriler yetkili makamlarla paylaşılabilir.</p>
        </section>", ContentEn = @"<section class=""mb-8"">
          <h2>1. A Safe and Respectful Memorial Space</h2>
          <p>At Styever, our highest priority is to ensure that the memories of our beloved companions receive the respect, peace and security they deserve on our Platform. For this purpose, memorial pages created across the Platform, uploaded media files and condolence comments are subject to specific moderation rules and legal standards.</p>
        </section>
        <section class=""mb-8"">
          <h2>2. Hosting Provider Status and Content Responsibility</h2>
          <p>2.1. Styever acts as a ""Hosting Provider"" under Law No. 5651 on the Regulation of Publications on the Internet and Combating Crimes Committed by Means of Such Publications.</p>
          <p>2.2. All legal, criminal, financial and administrative responsibility for memorial pages created on our Platform, images and videos uploaded to those pages, and all written content belongs entirely to the user who uploads the content or posts the comment. Unless required by law or upon a justified complaint, Styever is not obliged to editorially review user content in advance or verify its accuracy.</p>
          <p>2.3. Nevertheless, where Styever identifies content that disrupts the peace of the Platform or violates public morals, public order, copyrights or our Community Guidelines, Styever reserves the right to remove, withdraw, edit or block access to such content without prior notice.</p>
        </section>
        <section class=""mb-8"">
          <h2>3. Scope of Review and Intervention Criteria</h2>
          <p>Memorial pages, photographs, videos, user comments and profile information that fail to comply with the criteria below are subject to our moderation system and may be immediately acted upon:</p>
          <ul>
            <li>Insults, profanity, humiliation or aggressive statements directed at other users, the memory of deceased animals or third parties,</li>
<li>Hate speech that polarizes or harms society, as well as threats, harassment or blackmail,</li>
<li>Commercial advertisements, product/service promotions, listings, spam content and unauthorized redirect links,</li>
<li>Copyrighted images, videos or written materials shared without ownership or authorization in violation of Law No. 5846 on Intellectual and Artistic Works,</li>
<li>Sharing third-party personal data such as full addresses, telephone numbers or unauthorized images in violation of Law No. 6698 (KVKK),</li>
<li>Illegal, immoral, violence-promoting or disturbing content unrelated to the purpose of pet memorialization.</li>
          </ul>
        </section>
        <section class=""mb-8"">
          <h2>4. Moderation Decisions and Levels of Sanctions</h2>
          <p>In the event of a violation, Platform management is authorized to immediately apply the following measures depending on the nature and severity of the violation:</p>
          <ul>
            <li>Warning and Content Removal: Immediate removal from the Platform of comments, text or media that violate the law or the rules.</li>
            <li>Access Restriction: Temporarily or permanently closing the relevant memorial page to comments and/or visits.</li>
            <li>Account Suspension / Termination: Freezing or permanently terminating the membership accounts of users who repeatedly violate the rules or commit serious violations.</li>
          </ul>
          <p>Moderation decisions are applied immediately in order to protect the peace and legal security of the Platform. Where deemed necessary, details of the action may be communicated to the account holder by e-mail.</p>
        </section>
        <section>
          <h2>5. Content Reporting Mechanism and Notice-and-Takedown Process</h2>
          <p>If you encounter content or a comment on our Platform that violates the rules, infringes copyright or is inappropriate for the memory of our beloved companions:</p>
          <ul>
            <li>You may use the ""Report Content"" button on the relevant page, or</li>
            <li>You may submit a reasoned report directly via info@styever.com.</li>
          </ul>
          <p>Reports received are carefully reviewed by our team as soon as possible pursuant to Law No. 5651 and the ""Notice-and-Takedown"" principle; content found to be in violation is immediately removed from publication. In the case of official judicial or administrative requests concerning rights violations, logs and data required by applicable legislation may be shared with the competent authorities.</p>
        </section>", SortOrder = 9, IsDeleted = false }
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "Yetki Ekranı Sayfalama Yetkisi", Code = "PermissionScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 2, Name = "Yetki Ekranı Kayıt Yetkisi", Code = "PermissionScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 3, Name = "Yetki Ekranı Güncelleme Yetkisi", Code = "PermissionScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 4, Name = "Yetki Ekranı Silme Yetkisi", Code = "PermissionScene.Delete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 5, Name = "Yetki Ekranı Listeleme Yetkisi", Code = "PermissionScene.List.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 6, Name = "Yetki Ekranı Yetki Alma Yetkisi", Code = "PermissionScene.GetById.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 7, Name = "Rol Ekranı Sayfalama Yetkisi", Code = "RoleScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 8, Name = "Rol Ekranı Kayıt Yetkisi", Code = "RoleScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 9, Name = "Rol Ekranı Güncelleme Yetkisi", Code = "RoleScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 10, Name = "Rol Ekranı Silme Yetkisi", Code = "RoleScene.Delete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 11, Name = "Rol Ekranı Listeleme Yetkisi", Code = "RoleScene.List.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 12, Name = "Rol Ekranı Rol Alma Yetkisi", Code = "RoleScene.GetById.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 13, Name = "Kullanıcı Ekranı Sayfalama Yetkisi", Code = "UserScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 14, Name = "Kullanıcı Ekranı Kayıt Yetkisi", Code = "UserScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 15, Name = "Kullanıcı Ekranı Güncelleme Yetkisi", Code = "UserScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 16, Name = "Kullanıcı Ekranı Silme Yetkisi", Code = "UserScene.Delete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 17, Name = "Kullanıcı Ekranı Listeleme Yetkisi", Code = "UserScene.List.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 18, Name = "Profil Ekranı Şifre Değiştirme Yetkisi", Code = "ProfileScene.ChangePw.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 19, Name = "Profil Ekranı Güncelleme Yetkisi", Code = "ProfileScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 20, Name = "Profil Ekranı Avatar Güncelleme Yetkisi", Code = "ProfileScene.AvatarEdit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 21, Name = "Profil Ekranı Adres Listeleme Yetkisi", Code = "ProfileScene.ListAddress.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 22, Name = "Profil Ekranı Adres Kayıt Yetkisi", Code = "ProfileScene.SaveAddress.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 23, Name = "Profil Ekranı Adres Güncelleme Yetkisi", Code = "ProfileScene.EditAddress.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 24, Name = "Profil Ekranı Adres Silme Yetkisi", Code = "ProfileScene.DeletAddress.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 25, Name = "Profil Ekranı Adres Alma Yetkisi", Code = "ProfileScene.GetAddressById.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 26, Name = "Ödeme Ekranı Üyelik Ödeme Yetkisi", Code = "PaymentScene.MembershipPayment.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 27, Name = "Ödeme Ekranı Üyelik Satın Alma Yetkisi", Code = "PaymentScene.BuyMembership.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 28, Name = "Dosya Yükleme Yetkisi", Code = "File.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 29, Name = "Dosya Silme Yetkisi", Code = "File.Delete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 30, Name = "Hatıra Ekranı Kayıt Yetkisi", Code = "MemoryScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 31, Name = "Hatıra Ekranı Güncelleme Yetkisi", Code = "MemoryScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 32, Name = "Hatıra Ekranı Sayaç Yetkisi", Code = "MemoryScene.Count.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 33, Name = "Hatıra Ekranı Dosya Güncelleme Yetkisi", Code = "MemoryScene.FileUpdate.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 34, Name = "Hatıra Ekranı Dosya Ekleme Yetkisi", Code = "MemoryScene.FileAdd.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 35, Name = "Hatıra Ekranı Dosya Silme Yetkisi", Code = "MemoryScene.FileDelete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 36, Name = "Hatıra Ekranı Mum Yakma Yetkisi", Code = "MemoryScene.LightCandle.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 37, Name = "Hatıra Ekranı Mum Yakma Güncelleme Yetkisi", Code = "MemoryScene.UpdateCandle.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 38, Name = "Hatıra Ekranı Yorum Yapma Yetkisi", Code = "MemoryScene.AddComment.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 39, Name = "Hatıra Ekranı Yorum Silme Yetkisi", Code = "MemoryScene.DeleteComment.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 40, Name = "Hatıra Ekranı Beğeni Yetkisi", Code = "MemoryScene.Like.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 41, Name = "Hatıra Ekranı Beğeni Silme Yetkisi", Code = "MemoryScene.Dislike.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 42, Name = "Hatıra Ekranı Yorum Onaylama Yetkisi", Code = "MemoryScene.ApproveComment.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 43, Name = "Dashboard Görüntüleme Yetkisi", Code = "DashboardScene.View.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 44, Name = "Destek Ekranı Sayfalama Yetkisi", Code = "SupportScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 45, Name = "Destek Ekranı Kayıt Yetkisi", Code = "SupportScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 46, Name = "Destek Ekranı Güncelleme Yetkisi", Code = "SupportScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 47, Name = "Destek Ekranı Silme Yetkisi", Code = "SupportScene.Delete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 48, Name = "Destek Ekranı Kayıt Alma Yetkisi", Code = "SupportScene.GetById.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 49, Name = "SSS Ekranı Sayfalama Yetkisi", Code = "FAQScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 50, Name = "SSS Ekranı Kayıt Yetkisi", Code = "FAQScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 51, Name = "SSS Ekranı Güncelleme Yetkisi", Code = "FAQScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 52, Name = "SSS Ekranı Silme Yetkisi", Code = "FAQScene.Delete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 53, Name = "SSS Ekranı Kayıt Alma Yetkisi", Code = "FAQScene.GetById.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 54, Code = "PlanScene.Paging.Permission", IsDeleted = false, IsSystemData = true, Name = "Plan Ekranı Sayfalama Yetkisi" },
                new Permission { Id = 55, Code = "PlanScene.Save.Permission", IsDeleted = false, IsSystemData = true, Name = "Plan Ekranı Kayıt Yetkisi" },
                new Permission { Id = 56, Code = "PlanScene.Edit.Permission", IsDeleted = false, IsSystemData = true, Name = "Plan Ekranı Güncelleme Yetkisi" },
                new Permission { Id = 57, Code = "PlanScene.Delete.Permission", IsDeleted = false, IsSystemData = true, Name = "Plan Ekranı Silme Yetkisi" },
                new Permission { Id = 58, Code = "PlanScene.GetById.Permission", IsDeleted = false, IsSystemData = true, Name = "Plan Ekranı Kayıt Alma Yetkisi" },
                new Permission { Id = 59, Code = "LegalScene.Paging.Permission", IsDeleted = false, IsSystemData = true, Name = "Yasal İçerik Ekranı Sayfalama Yetkisi" },
                new Permission { Id = 60, Code = "LegalScene.Save.Permission", IsDeleted = false, IsSystemData = true, Name = "Yasal İçerik Ekranı Kayıt Yetkisi" },
                new Permission { Id = 61, Code = "LegalScene.Edit.Permission", IsDeleted = false, IsSystemData = true, Name = "Yasal İçerik Ekranı Güncelleme Yetkisi" },
                new Permission { Id = 62, Code = "LegalScene.Delete.Permission", IsDeleted = false, IsSystemData = true, Name = "Yasal İçerik Ekranı Silme Yetkisi" }
             );


            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SuperAdmin", IsDeleted = false, IsSystemData = true },
                new Role { Id = 2, Name = "Origin", IsDeleted = false, IsSystemData = true },
                new Role { Id = 3, Name = "Heart", IsDeleted = false, IsSystemData = true },
                new Role { Id = 4, Name = "Family", IsDeleted = false, IsSystemData = true }
            );

            modelBuilder.Entity<RolePermission>().HasData(
                //SuperAdmin Role Perms
                new RolePermission { Id = 1, RoleId = 1, PermissionId = 1, IsDeleted = false },
                new RolePermission { Id = 2, RoleId = 1, PermissionId = 2, IsDeleted = false },
                new RolePermission { Id = 3, RoleId = 1, PermissionId = 3, IsDeleted = false },
                new RolePermission { Id = 4, RoleId = 1, PermissionId = 4, IsDeleted = false },
                new RolePermission { Id = 5, RoleId = 1, PermissionId = 5, IsDeleted = false },
                new RolePermission { Id = 6, RoleId = 1, PermissionId = 6, IsDeleted = false },
                new RolePermission { Id = 7, RoleId = 1, PermissionId = 7, IsDeleted = false },
                new RolePermission { Id = 8, RoleId = 1, PermissionId = 8, IsDeleted = false },
                new RolePermission { Id = 9, RoleId = 1, PermissionId = 9, IsDeleted = false },
                new RolePermission { Id = 10, RoleId = 1, PermissionId = 10, IsDeleted = false },
                new RolePermission { Id = 11, RoleId = 1, PermissionId = 11, IsDeleted = false },
                new RolePermission { Id = 12, RoleId = 1, PermissionId = 12, IsDeleted = false },
                new RolePermission { Id = 13, RoleId = 1, PermissionId = 13, IsDeleted = false },
                new RolePermission { Id = 14, RoleId = 1, PermissionId = 14, IsDeleted = false },
                new RolePermission { Id = 15, RoleId = 1, PermissionId = 15, IsDeleted = false },
                new RolePermission { Id = 16, RoleId = 1, PermissionId = 16, IsDeleted = false },
                new RolePermission { Id = 17, RoleId = 1, PermissionId = 17, IsDeleted = false },
                new RolePermission { Id = 18, RoleId = 1, PermissionId = 18, IsDeleted = false },
                new RolePermission { Id = 19, RoleId = 1, PermissionId = 19, IsDeleted = false },
                new RolePermission { Id = 20, RoleId = 1, PermissionId = 20, IsDeleted = false },
                new RolePermission { Id = 21, RoleId = 1, PermissionId = 21, IsDeleted = false },
                new RolePermission { Id = 22, RoleId = 1, PermissionId = 22, IsDeleted = false },
                new RolePermission { Id = 23, RoleId = 1, PermissionId = 23, IsDeleted = false },
                new RolePermission { Id = 24, RoleId = 1, PermissionId = 24, IsDeleted = false },
                new RolePermission { Id = 25, RoleId = 1, PermissionId = 25, IsDeleted = false },
                new RolePermission { Id = 26, RoleId = 1, PermissionId = 26, IsDeleted = false },
                new RolePermission { Id = 27, RoleId = 1, PermissionId = 27, IsDeleted = false },
                new RolePermission { Id = 28, RoleId = 1, PermissionId = 28, IsDeleted = false },
                new RolePermission { Id = 29, RoleId = 1, PermissionId = 29, IsDeleted = false },
                new RolePermission { Id = 30, RoleId = 1, PermissionId = 30, IsDeleted = false },
                new RolePermission { Id = 31, RoleId = 1, PermissionId = 31, IsDeleted = false },
                new RolePermission { Id = 32, RoleId = 1, PermissionId = 32, IsDeleted = false },
                new RolePermission { Id = 33, RoleId = 1, PermissionId = 33, IsDeleted = false },
                new RolePermission { Id = 34, RoleId = 1, PermissionId = 34, IsDeleted = false },
                new RolePermission { Id = 35, RoleId = 1, PermissionId = 35, IsDeleted = false },
                new RolePermission { Id = 36, RoleId = 1, PermissionId = 36, IsDeleted = false },
                new RolePermission { Id = 37, RoleId = 1, PermissionId = 37, IsDeleted = false },
                new RolePermission { Id = 38, RoleId = 1, PermissionId = 38, IsDeleted = false },
                new RolePermission { Id = 39, RoleId = 1, PermissionId = 39, IsDeleted = false },
                new RolePermission { Id = 40, RoleId = 1, PermissionId = 40, IsDeleted = false },
                new RolePermission { Id = 41, RoleId = 1, PermissionId = 41, IsDeleted = false },
                new RolePermission { Id = 42, RoleId = 1, PermissionId = 42, IsDeleted = false },
                new RolePermission { Id = 43, RoleId = 1, PermissionId = 43, IsDeleted = false },
                new RolePermission { Id = 44, RoleId = 1, PermissionId = 44, IsDeleted = false },
                new RolePermission { Id = 45, RoleId = 1, PermissionId = 45, IsDeleted = false },
                new RolePermission { Id = 46, RoleId = 1, PermissionId = 46, IsDeleted = false },
                new RolePermission { Id = 47, RoleId = 1, PermissionId = 47, IsDeleted = false },
                new RolePermission { Id = 48, RoleId = 1, PermissionId = 48, IsDeleted = false },
                new RolePermission { Id = 49, RoleId = 1, PermissionId = 49, IsDeleted = false },
                new RolePermission { Id = 50, RoleId = 1, PermissionId = 50, IsDeleted = false },
                new RolePermission { Id = 51, RoleId = 1, PermissionId = 51, IsDeleted = false },
                new RolePermission { Id = 52, RoleId = 1, PermissionId = 52, IsDeleted = false },
                new RolePermission { Id = 53, RoleId = 1, PermissionId = 53, IsDeleted = false },
                //Origin Role Perms
                new RolePermission { Id = 54, RoleId = 2, PermissionId = 18, IsDeleted = false },
                new RolePermission { Id = 55, RoleId = 2, PermissionId = 19, IsDeleted = false },
                new RolePermission { Id = 56, RoleId = 2, PermissionId = 20, IsDeleted = false },
                new RolePermission { Id = 57, RoleId = 2, PermissionId = 21, IsDeleted = false },
                new RolePermission { Id = 58, RoleId = 2, PermissionId = 22, IsDeleted = false },
                new RolePermission { Id = 59, RoleId = 2, PermissionId = 23, IsDeleted = false },
                new RolePermission { Id = 60, RoleId = 2, PermissionId = 24, IsDeleted = false },
                new RolePermission { Id = 61, RoleId = 2, PermissionId = 25, IsDeleted = false },
                new RolePermission { Id = 62, RoleId = 2, PermissionId = 26, IsDeleted = false },
                new RolePermission { Id = 63, RoleId = 2, PermissionId = 27, IsDeleted = false },
                new RolePermission { Id = 64, RoleId = 2, PermissionId = 28, IsDeleted = false },
                new RolePermission { Id = 65, RoleId = 2, PermissionId = 29, IsDeleted = false },
                new RolePermission { Id = 66, RoleId = 2, PermissionId = 30, IsDeleted = false },
                new RolePermission { Id = 67, RoleId = 2, PermissionId = 31, IsDeleted = false },
                new RolePermission { Id = 68, RoleId = 2, PermissionId = 32, IsDeleted = false },
                new RolePermission { Id = 69, RoleId = 2, PermissionId = 33, IsDeleted = false },
                new RolePermission { Id = 70, RoleId = 2, PermissionId = 34, IsDeleted = false },
                new RolePermission { Id = 71, RoleId = 2, PermissionId = 35, IsDeleted = false },
                new RolePermission { Id = 72, RoleId = 2, PermissionId = 36, IsDeleted = false },
                new RolePermission { Id = 73, RoleId = 2, PermissionId = 37, IsDeleted = false },
                new RolePermission { Id = 74, RoleId = 2, PermissionId = 38, IsDeleted = false },
                new RolePermission { Id = 75, RoleId = 2, PermissionId = 39, IsDeleted = false },
                new RolePermission { Id = 76, RoleId = 2, PermissionId = 40, IsDeleted = false },
                new RolePermission { Id = 77, RoleId = 2, PermissionId = 41, IsDeleted = false },
                //Heart Role Perms
                new RolePermission { Id = 78, RoleId = 3, PermissionId = 18, IsDeleted = false },
                new RolePermission { Id = 79, RoleId = 3, PermissionId = 19, IsDeleted = false },
                new RolePermission { Id = 80, RoleId = 3, PermissionId = 20, IsDeleted = false },
                new RolePermission { Id = 81, RoleId = 3, PermissionId = 21, IsDeleted = false },
                new RolePermission { Id = 82, RoleId = 3, PermissionId = 22, IsDeleted = false },
                new RolePermission { Id = 83, RoleId = 3, PermissionId = 23, IsDeleted = false },
                new RolePermission { Id = 84, RoleId = 3, PermissionId = 24, IsDeleted = false },
                new RolePermission { Id = 85, RoleId = 3, PermissionId = 25, IsDeleted = false },
                new RolePermission { Id = 86, RoleId = 3, PermissionId = 26, IsDeleted = false },
                new RolePermission { Id = 87, RoleId = 3, PermissionId = 27, IsDeleted = false },
                new RolePermission { Id = 88, RoleId = 3, PermissionId = 28, IsDeleted = false },
                new RolePermission { Id = 89, RoleId = 3, PermissionId = 29, IsDeleted = false },
                new RolePermission { Id = 90, RoleId = 3, PermissionId = 30, IsDeleted = false },
                new RolePermission { Id = 91, RoleId = 3, PermissionId = 31, IsDeleted = false },
                new RolePermission { Id = 92, RoleId = 3, PermissionId = 32, IsDeleted = false },
                new RolePermission { Id = 93, RoleId = 3, PermissionId = 33, IsDeleted = false },
                new RolePermission { Id = 94, RoleId = 3, PermissionId = 34, IsDeleted = false },
                new RolePermission { Id = 95, RoleId = 3, PermissionId = 35, IsDeleted = false },
                new RolePermission { Id = 96, RoleId = 3, PermissionId = 36, IsDeleted = false },
                new RolePermission { Id = 97, RoleId = 3, PermissionId = 37, IsDeleted = false },
                new RolePermission { Id = 98, RoleId = 3, PermissionId = 38, IsDeleted = false },
                new RolePermission { Id = 99, RoleId = 3, PermissionId = 39, IsDeleted = false },
                new RolePermission { Id = 100, RoleId = 3, PermissionId = 40, IsDeleted = false },
                new RolePermission { Id = 101, RoleId = 3, PermissionId = 41, IsDeleted = false },
                //Family Role Perms
                new RolePermission { Id = 102, RoleId = 4, PermissionId = 18, IsDeleted = false },
                new RolePermission { Id = 103, RoleId = 4, PermissionId = 19, IsDeleted = false },
                new RolePermission { Id = 104, RoleId = 4, PermissionId = 20, IsDeleted = false },
                new RolePermission { Id = 105, RoleId = 4, PermissionId = 21, IsDeleted = false },
                new RolePermission { Id = 106, RoleId = 4, PermissionId = 22, IsDeleted = false },
                new RolePermission { Id = 107, RoleId = 4, PermissionId = 23, IsDeleted = false },
                new RolePermission { Id = 108, RoleId = 4, PermissionId = 24, IsDeleted = false },
                new RolePermission { Id = 109, RoleId = 4, PermissionId = 25, IsDeleted = false },
                new RolePermission { Id = 110, RoleId = 4, PermissionId = 26, IsDeleted = false },
                new RolePermission { Id = 111, RoleId = 4, PermissionId = 27, IsDeleted = false },
                new RolePermission { Id = 112, RoleId = 4, PermissionId = 28, IsDeleted = false },
                new RolePermission { Id = 113, RoleId = 4, PermissionId = 29, IsDeleted = false },
                new RolePermission { Id = 114, RoleId = 4, PermissionId = 30, IsDeleted = false },
                new RolePermission { Id = 115, RoleId = 4, PermissionId = 31, IsDeleted = false },
                new RolePermission { Id = 116, RoleId = 4, PermissionId = 32, IsDeleted = false },
                new RolePermission { Id = 117, RoleId = 4, PermissionId = 33, IsDeleted = false },
                new RolePermission { Id = 118, RoleId = 4, PermissionId = 34, IsDeleted = false },
                new RolePermission { Id = 119, RoleId = 4, PermissionId = 35, IsDeleted = false },
                new RolePermission { Id = 120, RoleId = 4, PermissionId = 36, IsDeleted = false },
                new RolePermission { Id = 121, RoleId = 4, PermissionId = 37, IsDeleted = false },
                new RolePermission { Id = 122, RoleId = 4, PermissionId = 38, IsDeleted = false },
                new RolePermission { Id = 123, RoleId = 4, PermissionId = 39, IsDeleted = false },
                new RolePermission { Id = 124, RoleId = 4, PermissionId = 40, IsDeleted = false },
                new RolePermission { Id = 125, RoleId = 4, PermissionId = 41, IsDeleted = false },
                new RolePermission { Id = 126, RoleId = 1, PermissionId = 42, IsDeleted = false },
                new RolePermission { Id = 127, RoleId = 2, PermissionId = 42, IsDeleted = false },
                new RolePermission { Id = 128, RoleId = 3, PermissionId = 42, IsDeleted = false },
                new RolePermission { Id = 129, RoleId = 4, PermissionId = 42, IsDeleted = false },
                new RolePermission { Id = 130, RoleId = 1, PermissionId = 54, IsDeleted = false },
                new RolePermission { Id = 131, RoleId = 1, PermissionId = 55, IsDeleted = false },
                new RolePermission { Id = 132, RoleId = 1, PermissionId = 56, IsDeleted = false },
                new RolePermission { Id = 133, RoleId = 1, PermissionId = 57, IsDeleted = false },
                new RolePermission { Id = 134, RoleId = 1, PermissionId = 58, IsDeleted = false },
                new RolePermission { Id = 135, RoleId = 1, PermissionId = 59, IsDeleted = false },
                new RolePermission { Id = 136, RoleId = 1, PermissionId = 60, IsDeleted = false },
                new RolePermission { Id = 137, RoleId = 1, PermissionId = 61, IsDeleted = false },
                new RolePermission { Id = 138, RoleId = 1, PermissionId = 62, IsDeleted = false }
            );

            modelBuilder.Entity<User>().HasData(
                new User { 
                    Id = 1, 
                    Name = "Admin",
                    Surname = "Admin",
                    Email = "admin@test.com",
                    Password = "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C", 
                    Phone = "+905077352772",
                    Username = "admin",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    TrialExpirationDate = DateTime.UtcNow.AddDays(7),
                    ExpirationDate = DateTime.UtcNow.AddYears(1),
                    Salt = Convert.FromBase64String("A/u2bAGlBV91ByotxKC+wkGpMDFjFnixpfY5ul7YO1Aw5dIfBa3bhlNJWsTc2KMO22o0tw36D4+a0FUtHTQNaQ=="),
                    IsDeleted = false,
                    IsSystemData = true,
                    FileId = null,
                    IsTrial = false,
                },
                new User
                {
                    Id = 2,
                    Name = "Origin",
                    Surname = "User",
                    Email = "origin@test.com",
                    Password = "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C",
                    Phone = "+905077352772",
                    Username = "originuser",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    TrialExpirationDate = DateTime.UtcNow.AddDays(7),
                    ExpirationDate = DateTime.UtcNow.AddYears(1),
                    Salt = Convert.FromBase64String("A/u2bAGlBV91ByotxKC+wkGpMDFjFnixpfY5ul7YO1Aw5dIfBa3bhlNJWsTc2KMO22o0tw36D4+a0FUtHTQNaQ=="),
                    IsDeleted = false,
                    IsSystemData = true,
                    FileId = 1,
                    IsTrial = false,
                },
                new User
                {
                    Id = 3,
                    Name = "Heart",
                    Surname = "User",
                    Email = "heart@test.com",
                    Password = "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C",
                    Phone = "+905077352772",
                    Username = "heartuser",
                    CreatedDate = DateTime.UtcNow,
                    TrialExpirationDate = DateTime.UtcNow.AddDays(7),
                    ExpirationDate = DateTime.UtcNow.AddYears(1),
                    Salt = Convert.FromBase64String("A/u2bAGlBV91ByotxKC+wkGpMDFjFnixpfY5ul7YO1Aw5dIfBa3bhlNJWsTc2KMO22o0tw36D4+a0FUtHTQNaQ=="),
                    IsDeleted = false,
                    IsSystemData = true,
                    FileId = 2,
                    IsActive = true,
                    IsTrial = false
                },
                new User
                {
                    Id = 4,
                    Name = "Family",
                    Surname = "User",
                    Email = "family@test.com",
                    Password = "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C",
                    Phone = "+905077352772",
                    Username = "familyuser",
                    CreatedDate = DateTime.UtcNow,
                    TrialExpirationDate = DateTime.UtcNow.AddDays(7),
                    ExpirationDate = DateTime.UtcNow.AddYears(1),
                    Salt = Convert.FromBase64String("A/u2bAGlBV91ByotxKC+wkGpMDFjFnixpfY5ul7YO1Aw5dIfBa3bhlNJWsTc2KMO22o0tw36D4+a0FUtHTQNaQ=="),
                    IsDeleted = false,
                    IsSystemData = true,
                    FileId = 3,
                    IsActive = true,
                    IsTrial = false
                }
            );

            modelBuilder.Entity<UserRole>().HasData(
                //SuperAdmin User Role
                new UserRole
                {
                    Id = 1,
                    RoleId = 1,
                    UserId = 1,
                    IsDeleted = false
                },
                //Memory User Role
                new UserRole
                {
                    Id = 2,
                    RoleId = 2,
                    UserId = 2,
                    IsDeleted = false
                },
                //Tribute User Role
                new UserRole
                {
                    Id = 3,
                    RoleId = 3,
                    UserId = 3,
                    IsDeleted = false
                },
                //Eternal User Role
                new UserRole
                {
                    Id = 4,
                    RoleId = 4,
                    UserId = 4,
                    IsDeleted = false
                }
            );

            modelBuilder.Entity<UserPermission>().HasData(
                //SuperAdmin User Permissions
                new UserPermission { Id = 1, UserId = 1, PermissionId = 1, IsDeleted = false },
                new UserPermission { Id = 2, UserId = 1, PermissionId = 2, IsDeleted = false },
                new UserPermission { Id = 3, UserId = 1, PermissionId = 3, IsDeleted = false },
                new UserPermission { Id = 4, UserId = 1, PermissionId = 4, IsDeleted = false },
                new UserPermission { Id = 5, UserId = 1, PermissionId = 5, IsDeleted = false },
                new UserPermission { Id = 6, UserId = 1, PermissionId = 6, IsDeleted = false },
                new UserPermission { Id = 7, UserId = 1, PermissionId = 7, IsDeleted = false },
                new UserPermission { Id = 8, UserId = 1, PermissionId = 8, IsDeleted = false },
                new UserPermission { Id = 9, UserId = 1, PermissionId = 9, IsDeleted = false },
                new UserPermission { Id = 10, UserId = 1, PermissionId = 10, IsDeleted = false },
                new UserPermission { Id = 11, UserId = 1, PermissionId = 11, IsDeleted = false },
                new UserPermission { Id = 12, UserId = 1, PermissionId = 12, IsDeleted = false },
                new UserPermission { Id = 13, UserId = 1, PermissionId = 13, IsDeleted = false },
                new UserPermission { Id = 14, UserId = 1, PermissionId = 14, IsDeleted = false },
                new UserPermission { Id = 15, UserId = 1, PermissionId = 15, IsDeleted = false },
                new UserPermission { Id = 16, UserId = 1, PermissionId = 16, IsDeleted = false },
                new UserPermission { Id = 17, UserId = 1, PermissionId = 17, IsDeleted = false },
                new UserPermission { Id = 18, UserId = 1, PermissionId = 18, IsDeleted = false },
                new UserPermission { Id = 19, UserId = 1, PermissionId = 19, IsDeleted = false },
                new UserPermission { Id = 20, UserId = 1, PermissionId = 20, IsDeleted = false },
                new UserPermission { Id = 21, UserId = 1, PermissionId = 21, IsDeleted = false },
                new UserPermission { Id = 22, UserId = 1, PermissionId = 22, IsDeleted = false },
                new UserPermission { Id = 23, UserId = 1, PermissionId = 23, IsDeleted = false },
                new UserPermission { Id = 24, UserId = 1, PermissionId = 24, IsDeleted = false },
                new UserPermission { Id = 25, UserId = 1, PermissionId = 25, IsDeleted = false },
                new UserPermission { Id = 26, UserId = 1, PermissionId = 26, IsDeleted = false },
                new UserPermission { Id = 27, UserId = 1, PermissionId = 27, IsDeleted = false },
                new UserPermission { Id = 28, UserId = 1, PermissionId = 28, IsDeleted = false },
                new UserPermission { Id = 29, UserId = 1, PermissionId = 29, IsDeleted = false },
                new UserPermission { Id = 30, UserId = 1, PermissionId = 30, IsDeleted = false },
                new UserPermission { Id = 31, UserId = 1, PermissionId = 31, IsDeleted = false },
                new UserPermission { Id = 32, UserId = 1, PermissionId = 32, IsDeleted = false },
                new UserPermission { Id = 33, UserId = 1, PermissionId = 33, IsDeleted = false },
                new UserPermission { Id = 34, UserId = 1, PermissionId = 34, IsDeleted = false },
                new UserPermission { Id = 35, UserId = 1, PermissionId = 35, IsDeleted = false },
                new UserPermission { Id = 36, UserId = 1, PermissionId = 36, IsDeleted = false },
                new UserPermission { Id = 37, UserId = 1, PermissionId = 37, IsDeleted = false },
                new UserPermission { Id = 38, UserId = 1, PermissionId = 38, IsDeleted = false },
                new UserPermission { Id = 39, UserId = 1, PermissionId = 39, IsDeleted = false },
                new UserPermission { Id = 40, UserId = 1, PermissionId = 40, IsDeleted = false },
                new UserPermission { Id = 41, UserId = 1, PermissionId = 41, IsDeleted = false },
                new UserPermission { Id = 42, UserId = 1, PermissionId = 42, IsDeleted = false },
                new UserPermission { Id = 43, UserId = 1, PermissionId = 43, IsDeleted = false },
                new UserPermission { Id = 44, UserId = 1, PermissionId = 44, IsDeleted = false },
                new UserPermission { Id = 45, UserId = 1, PermissionId = 45, IsDeleted = false },
                new UserPermission { Id = 46, UserId = 1, PermissionId = 46, IsDeleted = false },
                new UserPermission { Id = 47, UserId = 1, PermissionId = 47, IsDeleted = false },
                new UserPermission { Id = 48, UserId = 1, PermissionId = 48, IsDeleted = false },
                new UserPermission { Id = 49, UserId = 1, PermissionId = 49, IsDeleted = false },
                new UserPermission { Id = 50, UserId = 1, PermissionId = 50, IsDeleted = false },
                new UserPermission { Id = 51, UserId = 1, PermissionId = 51, IsDeleted = false },
                new UserPermission { Id = 52, UserId = 1, PermissionId = 52, IsDeleted = false },
                new UserPermission { Id = 53, UserId = 1, PermissionId = 53, IsDeleted = false },
                //Origin User Perms
                new UserPermission { Id = 54, UserId = 2, PermissionId = 18, IsDeleted = false },
                new UserPermission { Id = 55, UserId = 2, PermissionId = 19, IsDeleted = false },
                new UserPermission { Id = 56, UserId = 2, PermissionId = 20, IsDeleted = false },
                new UserPermission { Id = 57, UserId = 2, PermissionId = 21, IsDeleted = false },
                new UserPermission { Id = 58, UserId = 2, PermissionId = 22, IsDeleted = false },
                new UserPermission { Id = 59, UserId = 2, PermissionId = 23, IsDeleted = false },
                new UserPermission { Id = 60, UserId = 2, PermissionId = 24, IsDeleted = false },
                new UserPermission { Id = 61, UserId = 2, PermissionId = 25, IsDeleted = false },
                new UserPermission { Id = 62, UserId = 2, PermissionId = 26, IsDeleted = false },
                new UserPermission { Id = 63, UserId = 2, PermissionId = 27, IsDeleted = false },
                new UserPermission { Id = 64, UserId = 2, PermissionId = 28, IsDeleted = false },
                new UserPermission { Id = 65, UserId = 2, PermissionId = 29, IsDeleted = false },
                new UserPermission { Id = 66, UserId = 2, PermissionId = 30, IsDeleted = false },
                new UserPermission { Id = 67, UserId = 2, PermissionId = 31, IsDeleted = false },
                new UserPermission { Id = 68, UserId = 2, PermissionId = 32, IsDeleted = false },
                new UserPermission { Id = 69, UserId = 2, PermissionId = 33, IsDeleted = false },
                new UserPermission { Id = 70, UserId = 2, PermissionId = 34, IsDeleted = false },
                new UserPermission { Id = 71, UserId = 2, PermissionId = 35, IsDeleted = false },
                new UserPermission { Id = 72, UserId = 2, PermissionId = 36, IsDeleted = false },
                new UserPermission { Id = 73, UserId = 2, PermissionId = 37, IsDeleted = false },
                new UserPermission { Id = 74, UserId = 2, PermissionId = 38, IsDeleted = false },
                new UserPermission { Id = 75, UserId = 2, PermissionId = 39, IsDeleted = false },
                new UserPermission { Id = 76, UserId = 2, PermissionId = 40, IsDeleted = false },
                new UserPermission { Id = 77, UserId = 2, PermissionId = 41, IsDeleted = false },
                //Heart Role Perms
                new UserPermission { Id = 78, UserId = 3, PermissionId = 18, IsDeleted = false },
                new UserPermission { Id = 79, UserId = 3, PermissionId = 19, IsDeleted = false },
                new UserPermission { Id = 80, UserId = 3, PermissionId = 20, IsDeleted = false },
                new UserPermission { Id = 81, UserId = 3, PermissionId = 21, IsDeleted = false },
                new UserPermission { Id = 82, UserId = 3, PermissionId = 22, IsDeleted = false },
                new UserPermission { Id = 83, UserId = 3, PermissionId = 23, IsDeleted = false },
                new UserPermission { Id = 84, UserId = 3, PermissionId = 24, IsDeleted = false },
                new UserPermission { Id = 85, UserId = 3, PermissionId = 25, IsDeleted = false },
                new UserPermission { Id = 86, UserId = 3, PermissionId = 26, IsDeleted = false },
                new UserPermission { Id = 87, UserId = 3, PermissionId = 27, IsDeleted = false },
                new UserPermission { Id = 88, UserId = 3, PermissionId = 28, IsDeleted = false },
                new UserPermission { Id = 89, UserId = 3, PermissionId = 29, IsDeleted = false },
                new UserPermission { Id = 90, UserId = 3, PermissionId = 30, IsDeleted = false },
                new UserPermission { Id = 91, UserId = 3, PermissionId = 31, IsDeleted = false },
                new UserPermission { Id = 92, UserId = 3, PermissionId = 32, IsDeleted = false },
                new UserPermission { Id = 93, UserId = 3, PermissionId = 33, IsDeleted = false },
                new UserPermission { Id = 94, UserId = 3, PermissionId = 34, IsDeleted = false },
                new UserPermission { Id = 95, UserId = 3, PermissionId = 35, IsDeleted = false },
                new UserPermission { Id = 96, UserId = 3, PermissionId = 36, IsDeleted = false },
                new UserPermission { Id = 97, UserId = 3, PermissionId = 37, IsDeleted = false },
                new UserPermission { Id = 98, UserId = 3, PermissionId = 38, IsDeleted = false },
                new UserPermission { Id = 99, UserId = 3, PermissionId = 39, IsDeleted = false },
                new UserPermission { Id = 100, UserId = 3, PermissionId = 40, IsDeleted = false },
                new UserPermission { Id = 101, UserId = 3, PermissionId = 41, IsDeleted = false },
                //Family User Perms
                new UserPermission { Id = 102, UserId = 4, PermissionId = 18, IsDeleted = false },
                new UserPermission { Id = 103, UserId = 4, PermissionId = 19, IsDeleted = false },
                new UserPermission { Id = 104, UserId = 4, PermissionId = 20, IsDeleted = false },
                new UserPermission { Id = 105, UserId = 4, PermissionId = 21, IsDeleted = false },
                new UserPermission { Id = 106, UserId = 4, PermissionId = 22, IsDeleted = false },
                new UserPermission { Id = 107, UserId = 4, PermissionId = 23, IsDeleted = false },
                new UserPermission { Id = 108, UserId = 4, PermissionId = 24, IsDeleted = false },
                new UserPermission { Id = 109, UserId = 4, PermissionId = 25, IsDeleted = false },
                new UserPermission { Id = 110, UserId = 4, PermissionId = 26, IsDeleted = false },
                new UserPermission { Id = 111, UserId = 4, PermissionId = 27, IsDeleted = false },
                new UserPermission { Id = 112, UserId = 4, PermissionId = 28, IsDeleted = false },
                new UserPermission { Id = 113, UserId = 4, PermissionId = 29, IsDeleted = false },
                new UserPermission { Id = 114, UserId = 4, PermissionId = 30, IsDeleted = false },
                new UserPermission { Id = 115, UserId = 4, PermissionId = 31, IsDeleted = false },
                new UserPermission { Id = 116, UserId = 4, PermissionId = 32, IsDeleted = false },
                new UserPermission { Id = 117, UserId = 4, PermissionId = 33, IsDeleted = false },
                new UserPermission { Id = 118, UserId = 4, PermissionId = 34, IsDeleted = false },
                new UserPermission { Id = 119, UserId = 4, PermissionId = 35, IsDeleted = false },
                new UserPermission { Id = 120, UserId = 4, PermissionId = 36, IsDeleted = false },
                new UserPermission { Id = 121, UserId = 4, PermissionId = 37, IsDeleted = false },
                new UserPermission { Id = 122, UserId = 4, PermissionId = 38, IsDeleted = false },
                new UserPermission { Id = 123, UserId = 4, PermissionId = 39, IsDeleted = false },
                new UserPermission { Id = 124, UserId = 4, PermissionId = 40, IsDeleted = false },
                new UserPermission { Id = 125, UserId = 4, PermissionId = 41, IsDeleted = false },
                new UserPermission { Id = 126, UserId = 1, PermissionId = 42, IsDeleted = false },
                new UserPermission { Id = 127, UserId = 2, PermissionId = 42, IsDeleted = false },
                new UserPermission { Id = 128, UserId = 3, PermissionId = 42, IsDeleted = false },
                new UserPermission { Id = 129, UserId = 4, PermissionId = 42, IsDeleted = false },
                new UserPermission { Id = 130, UserId = 1, PermissionId = 54, IsDeleted = false },
                new UserPermission { Id = 131, UserId = 1, PermissionId = 55, IsDeleted = false },
                new UserPermission { Id = 132, UserId = 1, PermissionId = 56, IsDeleted = false },
                new UserPermission { Id = 133, UserId = 1, PermissionId = 57, IsDeleted = false },
                new UserPermission { Id = 134, UserId = 1, PermissionId = 58, IsDeleted = false },
                new UserPermission { Id = 135, UserId = 1, PermissionId = 59, IsDeleted = false },
                new UserPermission { Id = 136, UserId = 1, PermissionId = 60, IsDeleted = false },
                new UserPermission { Id = 137, UserId = 1, PermissionId = 61, IsDeleted = false },
                new UserPermission { Id = 138, UserId = 1, PermissionId = 62, IsDeleted = false }
            );
        }

    }
}
