using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_three.Messages
{
    public static class Messages
    {

        //ProductManager ile ilgili bilgilendirme mesajları
        public static string ProductAdded = "Ürün eklendi";
        public static string ProductNameInvalid = " Ürün ismi geçersiz";
        public static string Bakimda = "Sistem şuan da bakımda, Anlayışınız için teşekkür ederiz";
        public static string ProductsListed = "Ürünler listelendi";
        public static string ProductListed = "Ürün listelendi";
        public static string ProductDetail = "Ürün detayları listelendi";


        //CategoryManager ile ilgili bilgilendirme mesajları
        public static string CategorysListed = "Kategoriler listelendi.";
        public static string CategoryListed = "İlgili kategori listelendi";

        public static string ProductCountOfCategoryError = "Kategoride 10 dan fazla ürün var";
        public static string SameProductNameTrue = "Aynı ürün isminde zaten bir ürün mevcut.";
        public static string CategoryLimitedExceded = "Kategori limiti aşıldı.";

        public static string? AuthorizationDenied = "Yetkiniz yok.";
        public static string UserRegistered = "Kullanıcı kayıt oldu.";

        public static string SuccessfulLogin = "Başarılı giriş.";
        public static string UserAlreadyExists = "UserAlreadyExists, böyle bir kullanıcı zaten mevcut";
        public static string AccessTokenCreated = "Token oluşturuldu";
        public static string UserNotFound = " Kullanıcı bulunamadı";

        public static string PasswordError = "Parola hatası";
    }
}
