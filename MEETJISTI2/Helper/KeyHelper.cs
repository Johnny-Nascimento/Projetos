using System;
using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace MEETJITSI.Helper
{
    public class KeyHelper
    {
        // Carrega private.pem (PKCS#1 ou PKCS#8) e retorna RSACryptoServiceProvider
        public static RSACryptoServiceProvider LoadPrivateKey(string privatePemPath)
        {
            using (var reader = File.OpenText(privatePemPath))
            {
                var pemReader = new PemReader(reader);
                var keyObj = pemReader.ReadObject();

                AsymmetricKeyParameter privateKeyParam = null;

                if (keyObj is AsymmetricCipherKeyPair)
                {
                    privateKeyParam = ((AsymmetricCipherKeyPair)keyObj).Private;
                }
                else if (keyObj is AsymmetricKeyParameter)
                {
                    privateKeyParam = (AsymmetricKeyParameter)keyObj;
                }
                else
                {
                    throw new InvalidOperationException("Formato de chave privada não reconhecido.");
                }

                var rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)privateKeyParam);
                var rsa = new RSACryptoServiceProvider();
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(rsaParams);
                return rsa;
            }
        }

        // Carrega public.pem e retorna RSACryptoServiceProvider
        public static RSACryptoServiceProvider LoadPublicKey(string publicPemPath)
        {
            using (var reader = File.OpenText(publicPemPath))
            {
                var pemReader = new PemReader(reader);
                var keyObj = pemReader.ReadObject();

                AsymmetricKeyParameter publicKeyParam = null;

                if (keyObj is AsymmetricKeyParameter)
                {
                    publicKeyParam = (AsymmetricKeyParameter)keyObj;
                }
                else if (keyObj is AsymmetricCipherKeyPair)
                {
                    publicKeyParam = ((AsymmetricCipherKeyPair)keyObj).Public;
                }
                else
                {
                    throw new InvalidOperationException("Formato de chave pública não reconhecido.");
                }

                var rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKeyParam);
                var rsa = new RSACryptoServiceProvider();
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(rsaParams);
                return rsa;
            }
        }

        // Extrai componentes n (modulus) e e (exponent) em Base64Url (para JWKS)
        public static (string n, string e) GetPublicKeyComponentsBase64Url(string publicPemPath)
        {
            using (var reader = File.OpenText(publicPemPath))
            {
                var pemReader = new PemReader(reader);
                var keyObj = pemReader.ReadObject();

                AsymmetricKeyParameter publicKeyParam;

                if (keyObj is AsymmetricCipherKeyPair)
                    publicKeyParam = ((AsymmetricCipherKeyPair)keyObj).Public;
                else if (keyObj is AsymmetricKeyParameter)
                    publicKeyParam = (AsymmetricKeyParameter)keyObj;
                else
                    throw new InvalidOperationException("Formato de chave pública não reconhecido.");

                var rsaKey = (RsaKeyParameters)publicKeyParam;
                var n = rsaKey.Modulus.ToByteArrayUnsigned();
                var e = rsaKey.Exponent.ToByteArrayUnsigned();

                return (Base64UrlEncode(n), Base64UrlEncode(e));
            }
        }

        private static string Base64UrlEncode(byte[] input)
        {
            var s = Convert.ToBase64String(input) // Regular base64
                .TrimEnd('=') // remove padding
                .Replace('+', '-') // 62nd char of encoding
                .Replace('/', '_'); // 63rd char of encoding
            return s;
        }
    }
}