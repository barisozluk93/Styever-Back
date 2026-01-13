using FAQManagement.Entity;
using Microsoft.EntityFrameworkCore;


namespace FAQManagement.DbContexts

{
    public class FAQManagementContext : DbContext
    {
        public FAQManagementContext(DbContextOptions<FAQManagementContext> options) : base(options)
        {
           
        }

        public DbSet<FAQ> FAQs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FAQ>().HasData(
                new FAQ
                {
                    Id = 1,
                    Header = "Bir \"anma sayfası\" tam olarak ne demek?",
                    HeaderEn = "What exactly does a \"memorial page\" mean?",
                    Content = "Bir anma sayfası, kaybettiğin can dostunu sevgiyle hatırlamanın içten bir yoludur. Fotoğraflar, anılar ve kalpten gelen cümlelerle o güzel ruhun hikâyesini yaşatmana izin verir. Ayrica mum yakarak hem dostunu anabilirsin, hem de tercih ettigin vakıflara destekte bulunabilirsin. Styever, bu hatırayı kalıcı bir sevgi defterine dönüştürür.",
                    ContentEn = "A memorial page is a heartfelt way to lovingly remember a dear friend you've lost. It allows you to keep the story of that beautiful soul alive through photos, memories, and heartfelt words. You can also light a candle to both commemorate your friend and support charities of your choice. Styever transforms this memory into a lasting book of love.",
                    IsDeleted = false,
                },
                new FAQ
                {
                    Id = 2,
                    Header = "Hizmet ücretsiz mi?",
                    HeaderEn = "Is the service free?",
                    Content = "Hayır. Çünkü Styever’ın sunduğu güvenli depolama, fotoğraf/video barındırma, teknik altyapı ve bakım maliyetleri gibi genel maliyetler mevcut. Bu yüzden sayfaların uzun yıllar ayakta kalabilmesi için küçük bir ücretlendirme gerekiyor.",
                    ContentEn = "No. Because Stever has general costs such as secure storage, photo/video hosting, technical infrastructure, and maintenance. Therefore, a small fee is necessary to ensure the pages remain operational for many years.",
                    IsDeleted = false,
                },
                new FAQ
                {
                    Id = 3,
                    Header = "Anma sayfamı kimler görebilir?",
                    HeaderEn = "Who can see my memorial page?",
                    Content = "Bu sayfa herkese açık. Yani dilersen ailen, arkadaşların, hatta dostunu tanıyan herkes sayfayı ziyaret edebilir. Linki kime gönderirsen, dostunun sevgisini paylaşabilir.",
                    ContentEn = "This page is open to everyone. So, if you wish, your family, friends, and even anyone who knows your friend can visit the page. Whoever you send the link to can share in your friend's love.",
                    IsDeleted = false,
                },
                new FAQ
                {
                    Id = 4,
                    Header = "Aboneliği istediğim zaman iptal edebilir miyim?",
                    HeaderEn = "Can I cancel my subscription at any time?",
                    Content = "Evet. Styever seni bağlayan bir zorunluluk koymaz. Hazır hissetmediğin anda bile hesabından aboneliğini iptal edebilirsin.",
                    ContentEn = "Yes. Stever doesn't impose any obligation on you. You can cancel your subscription from your account even if you don't feel ready.",
                    IsDeleted = false,
                },
                new FAQ
                {
                    Id = 5,
                    Header = "İptal edince anma sayfam silinir mi?",
                    HeaderEn = "Will my memorial page be deleted if I cancel?",
                    Content = "Hayır. Sayfan yayında kalır. Sadece yeni fotoğraf ekleme veya metin düzenleme gibi hakların kapanır. Eğer tamamen silinmesini istersen, Styever ekibine (hesap e-postan ve pet ismiyle) yazdığında sayfa en geç 48 saat içinde kaldırılır.",
                    ContentEn = "No. Your page will remain online. Only your rights to add new photos or edit text will be revoked. If you want it completely deleted, contact the Styever team (with your account email and pet name) and the page will be removed within 48 hours at the latest.",
                    IsDeleted = false,
                },
                new FAQ
                {
                    Id = 6,
                    Header = "Planlar arasındaki fark ne?",
                    HeaderEn = "What's the difference between the plans?",
                    Content = "Anı paketi – Tek fotoğraf ve kısa bir biyografiyle sade ama anlamlı bir anı bırakmak isteyenlere.\r\nHatıra paketi – Birden fazla fotoğraf, video, uzun biyografi ve istersen YouTube içerikleriyle daha kapsamlı bir hikâye oluşturma seçeneği.\r\nSonsuz paket – Birden fazla dost için aynı anda dört anma sayfasına kadar alan sunar; her biri ayrı galeri ve hikâye bölümüyle yer alır. Ayrica 4 ayrı anı alanına kadar anı ürünü.\r\n",
                    ContentEn = "Memory Pack – For those who want to leave a simple but meaningful memory with a single photo and a short biography.\r\nTribute Pack – The option to create a more comprehensive story with multiple photos, videos, a long biography, and YouTube content if you wish.\r\nEternal Pack – Offers space for up to four memorial pages simultaneously for multiple friends; each with its own gallery and story section. Also includes up to 4 separate memory areas for souvenir items.",
                    IsDeleted = false,
                },
                new FAQ
                {
                    Id = 7,
                    Header = "Yıllar önce kaybettiğim pet için de anma oluşturabilir miyim?",
                    HeaderEn = "Can I also create a memorial for my pet that I lost years ago?",
                    Content = "Elbette. Aradan ne kadar zaman geçmiş olursa olsun, sevgi eskimez. Hazır hissettiğin gün Styever’da sayfayı açabilir, onun izini kalıcılaştırabilirsin.",
                    ContentEn = "Of course. No matter how much time passes, love never fades. Whenever you feel ready, you can open a new page on Stever and immortalize its memory.",
                    IsDeleted = false,
                },
                new FAQ
                {
                    Id = 8,
                    Header = "Ödeme modeli nasıl işliyor?",
                    HeaderEn = "How does the payment model work?",
                    Content = "Styever’da ödemeler tek seferliktir. Anma sayfasını oluşturduğunda sana ait olur. Ayrıca 12 ay boyunca sayfanı özgürce düzenleyebilir, yeni fotoğraflar ekleyebilir, dilediğinde yeniden şekillendirebilirsin. Bu sürenin sonunda düzenleme hakkı sona erer ama anı sayfan yayında kalmaya devam eder. Sayfani diledigin gibi duzenlemeye devam etmek  istersen tabii ki aboneligini yenileyebilirsin.",
                    ContentEn = "At Styever, payments are one-time only. Once you create your memorial page, it becomes yours. You can also freely edit your page, add new photos, and reshape it as you wish for 12 months. At the end of this period, your editing rights expire, but your memorial page remains live. If you want to continue editing your page as you wish, you can of course renew your subscription.",
                    IsDeleted = false,
                },
                new FAQ
                {
                    Id = 9,
                    Header = "Daha sonra düzenleme yapabilir miyim?",
                    HeaderEn = "Can I make changes later?",
                    Content = "Evet. 12 aylık düzenleme süresi boyunca sayfayı ne kadar istersen yenileyebilir, anılar ekleyebilir, anlatımını derinleştirebilirsin.",
                    ContentEn = "Yes. During the 12-month editing period, you can refresh the page as much as you want, add memories, and deepen your narrative.",
                    IsDeleted = false,
                },
                new FAQ
                {
                    Id = 10,
                    Header = "Planımı sonradan yükseltebilir miyim?",
                    HeaderEn = "Can I upgrade my plan later?",
                    Content = "Evet. Anı ya da Hatıra Paketlerinden biri ile başladığında, daha sonra planını yükseltip daha kapsamlı bir anı alanına geçebilirsin.",
                    ContentEn = "Yes. When you start with either a Memory or Tribute Package, you can later upgrade your plan to a more comprehensive memory package.",
                    IsDeleted = false,
                },
                new FAQ
                {
                    Id = 11,
                    Header = "Birden fazla can dostum için anma sayfası açabilir miyim?",
                    HeaderEn = "Can I create memorial pages for more than one best friend?",
                    Content = "Evet — özellikle Sonsuz Paket bunun için var. Aynı hesapla dört farklı dostun için dört ayrı anma sayfası oluşturabilir, her birine özel bir hikâye alanı ayırabilirsin.",
                    ContentEn = "Yes — that's what the Eternal Pack is specifically for. You can create four separate memorial pages for four different friends with the same account, dedicating a unique story area to each.",
                    IsDeleted = false,
                },
                new FAQ
                {
                    Id = 12,
                    Header = "Bağış sistemi nedir ve süreç nasıl?",
                    HeaderEn = "What is the donation system and how does the process work?",
                    Content = "Styever’a üye olduğunda, belirlenen dernekler arasından dilediğini seçersin. Seçtiğin bu derneğe, abonelik ücretinin %20’si senin adına otomatik olarak bağışlanır. Dostunun anı sayfasında her mum yakıldığında, istersen belirlediğin tutarda ek bağış yapılabilir. Bu bağışın %80’i, anı sayfasında seçtiğin derneğe yine otomatik olarak aktarılır. Bağış yapılan dernek, senin adınla düzenlenmiş bir bağış sertifikası oluşturur ve bunu e-posta yoluyla sana gönderir. Styever, bağış sürecinde küçük bir payı kendi bünyesinde tutar. Bu pay, platformun devamlılığını sağlamak, anı sayfalarının güvenli şekilde barındırılması ve teknik altyapının sürdürülebilmesi için gereken sabit giderleri karşılamak amacıyla alınır. Böylece hem bağışlar sağlıklı biçimde derneklere ulaşır hem de dostlarımızın hatıralarını yaşatan bu alan uzun yıllar kesintisiz varlığını sürdürebilir.",
                    ContentEn = "When you become a member of Styever, you choose any of the designated charities. 20% of your subscription fee is automatically donated to your chosen charity. Each time a candle is lit on your friend's memorial page, you can optionally make an additional donation of a specified amount. 80% of this donation is also automatically transferred to the charity you selected on the memorial page. The charity receiving the donation creates a donation certificate in your name and sends it to you via email. Styever retains a small share of the donation process. This share is taken to cover the fixed costs necessary to ensure the platform's continuity, securely host the memorial pages, and maintain the technical infrastructure. This ensures that donations reach the associations effectively and that this space, which preserves the memories of our friends, can continue its existence uninterrupted for many years to come.",
                    IsDeleted = false,
                },
                new FAQ
                {
                    Id = 13,
                    Header = "Mum yakmak nedir?",
                    HeaderEn = "What is lighting a candle?",
                    Content = "Her Mum Bir Hatıra, Her Hatıra Bir Umut. Can dostlarımızın anılarını yaşatmak için oluşturduğumuz platformda her mum bir teşekkür, her bağış bir umuttur. Ziyaretçiler, dilediği anda bir mum yakarak sevgiyle hatırlar ve ihtiyaç sahibi hayvanlara destek olabilir. Amacımız, dijital anıları gerçek yardıma dönüştürerek sürdürülebilir bir hayvanseverlik ekosistemi kurmaktır. Platformumuzda her yakılan mum, yalnızca duygusal bir ritüel değil, aynı zamanda somut bir yardım fırsatıdır. Kullanıcılar, mum yakma esnasında ihtiyaç sahibi hayvanlara bağış yapmayı tercih edebilir. Tüm bağışlar şeffaf şekilde kayıt altına alınır ve güvenilir kurumlara yönlendirilir; böylece bir anı, başka bir canın hayatında fark yaratan gerçek bir desteğe dönüşür. Biz, hatırlamanın iyileştirdiğine inanıyoruz. Biz, her yaşamın değerli olduğunu biliyoruz. Biz, bir mumun aydınlığıyla binlerce dostun hayatına ışık olmayı seçiyoruz. Kaybettiklerimizi sevgiyle anarken, bugün yaşayanları korumayı görev kabul ediyoruz. Her mum bir söz, her bağış bir niyet, her anı bir birliktir. Biz bir topluluğuz. Ve birlikte daha fazla hayatı değiştirebiliriz. Bu platform; hayvan anı sayfası, online mum yakma, hayvansever bağış sistemi, sokak hayvanları için destek ve dijital anma deneyimi sunar. Kaybettiğimiz dostlarımızı saygıyla anarken, ziyaretçilere ihtiyaç sahibi hayvanlara bağış yapma imkânı sağlayan sürdürülebilir bir sistem kuruyoruz. Misyonumuz; anıları canlı tutmak, bağış süreçlerini şeffaf şekilde yönetmek ve hayvanlara dair toplumsal farkındalığı artırmaktır. Her mum bir hatırlama olduğu kadar, bugün yaşayan canlar için gerçek bir yardım anlamına gelir. Bir mum yak. Bir anı hatırla. Bir cana umut ol. Her ışık bir sevgi, her bağış bir nefes. Biz burada; anıları yaşatmak ve iyiliği çoğaltmak için varız.",
                    ContentEn = "Every Candle a Memory, Every Memory a Hope. On this platform, created to keep the memories of our beloved animal companions alive, every candle is a thank you, and every donation is a hope. Visitors can light a candle at any time to remember with love and support animals in need. Our goal is to create a sustainable animal welfare ecosystem by transforming digital memories into real help. On our platform, every candle lit is not just an emotional ritual, but also a tangible opportunity to help. Users can choose to donate to animals in need while lighting their candles. All donations are transparently recorded and channeled to reputable organizations, so that a memory becomes real support that makes a difference in another life. We believe that remembering heals. We know that every life is precious. We choose to illuminate the lives of thousands of friends with the light of a single candle. While we remember those we have lost with love, we consider it our duty to protect those who are alive today. Every candle is a promise, every donation is an intention, every moment is a togetherness. We are a community. And together we can change more lives. This platform offers an animal memorial page, online candle lighting, an animal lover donation system, support for street animals, and a digital memorial experience. While respectfully remembering our lost friends, we are establishing a sustainable system that allows visitors to donate to animals in need. Our mission is to keep memories alive, manage donation processes transparently, and increase social awareness about animals. Each candle is not only a remembrance but also a real help for the living beings today. Light a candle. Remember a memory. Be a source of hope for a life. Every light is love, every donation is a breath of fresh air. We are here to keep memories alive and multiply kindness.",
                    IsDeleted = false,
                }
            );
        }
    }
}
