using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Constant
{
    public static class Messages
    {
        public static class Wallet
        {
            public static string WalletNotFound => "Gönderene ait cüzdan bulunamadı";
            public static string ReceiveWalletNotFound => "Alıcıya ait cüzdan bulunamadı";
            public static string TransactionNotFoundWallet => "Kullanıcının cüzdanı oluşmamış";
            public static string UsernameRegistered(string username) => $"{username} kayıt oldu";
            public static string UserLoggin(string username) => $"{username} giriş yaptı";
            public static string WalletNotFound_TWO => "Cüzdan Bulunamadı";
        }
    }
}
