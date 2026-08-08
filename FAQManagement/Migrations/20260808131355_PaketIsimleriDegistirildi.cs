using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FAQManagement.Migrations
{
    /// <inheritdoc />
    public partial class PaketIsimleriDegistirildi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FAQs",
                keyColumn: "Id",
                keyValue: 6L,
                column: "Content",
                value: "<p>Styever, her ihtiyaca ve her anıya uygun üç farklı abonelik seçeneği sunar:</p><ul><li><strong>Origin Paketi:</strong> Sade ve anlamlı bir hatıra bırakmak isteyenler için tasarlandı. Tek bir özel fotoğraf ve biyografi alanı ile dostunuzun temel bilgilerini ve hatırasını dijital dünyada ölümsüzleştirebilirsiniz.</li><li><strong>Heart Paketi:</strong> Daha derin bir hikâye anlatmak isteyenler için idealdir. Birden fazla fotoğraf, video galerisi, kapsamlı bir biyografi bölümü ve YouTube içerikleriyle dostunuzun yaşam yolculuğunu tüm detaylarıyla paylaşmanıza imkan tanır.</li><li><strong>Family Paketi:</strong> Birden fazla dostunu aynı anda anmak isteyen büyük aileler için en kapsamlı seçeneğimizdir. Bu paketle dört ayrı anı sayfası oluşturabilir; her dostunuz için bağımsız galeriler, hikâyeler ve dijital alanlar tanımlayabilirsiniz.</li></ul>");

            migrationBuilder.UpdateData(
                table: "FAQs",
                keyColumn: "Id",
                keyValue: 10L,
                column: "Content",
                value: "<p>Evet.</p><p>Origin ya da Heart Paketlerinden biri ile başladığınızda, daha sonra planınızı yükseltip daha kapsamlı bir anı alanına geçebilirsiniz.</p>");

            migrationBuilder.UpdateData(
                table: "FAQs",
                keyColumn: "Id",
                keyValue: 11L,
                column: "Content",
                value: "<p>Evet — özellikle <strong>Family Paketi</strong> bunun için tasarlanmıştır.</p><p>Aynı hesapla dört farklı dostunuz için dört ayrı anma sayfası oluşturabilir, her birine özel bir hikâye alanı ayırabilirsiniz.</p>");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FAQs",
                keyColumn: "Id",
                keyValue: 6L,
                column: "Content",
                value: "<p>Styever, her ihtiyaca ve her anıya uygun üç farklı abonelik seçeneği sunar:</p><ul><li><strong>Başlangıç Paketi:</strong> Sade ve anlamlı bir hatıra bırakmak isteyenler için tasarlandı. Tek bir özel fotoğraf ve biyografi alanı ile dostunuzun temel bilgilerini ve hatırasını dijital dünyada ölümsüzleştirebilirsiniz.</li><li><strong>Gönül Paketi:</strong> Daha derin bir hikâye anlatmak isteyenler için idealdir. Birden fazla fotoğraf, video galerisi, kapsamlı bir biyografi bölümü ve YouTube içerikleriyle dostunuzun yaşam yolculuğunu tüm detaylarıyla paylaşmanıza imkan tanır.</li><li><strong>Aile Paketi:</strong> Birden fazla dostunu aynı anda anmak isteyen büyük aileler için en kapsamlı seçeneğimizdir. Bu paketle dört ayrı anı sayfası oluşturabilir; her dostunuz için bağımsız galeriler, hikâyeler ve dijital alanlar tanımlayabilirsiniz.</li></ul>");

            migrationBuilder.UpdateData(
                table: "FAQs",
                keyColumn: "Id",
                keyValue: 10L,
                column: "Content",
                value: "<p>Evet.</p><p>Başlangıç ya da Gönül Paketlerinden biri ile başladığınızda, daha sonra planınızı yükseltip daha kapsamlı bir anı alanına geçebilirsiniz.</p>");

            migrationBuilder.UpdateData(
                table: "FAQs",
                keyColumn: "Id",
                keyValue: 11L,
                column: "Content",
                value: "<p>Evet — özellikle <strong>Aile Paketi</strong> bunun için tasarlanmıştır.</p><p>Aynı hesapla dört farklı dostunuz için dört ayrı anma sayfası oluşturabilir, her birine özel bir hikâye alanı ayırabilirsiniz.</p>");
        }
    }
}
