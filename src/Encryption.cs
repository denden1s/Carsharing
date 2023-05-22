using System.Text;
using System.Security.Cryptography;

namespace testing_system.Classes
{
  class Encryption
  {
    public Encryption() { }

    /// <summary>
    /// Метод шифрует пароль пользователя
    /// </summary>
    /// <param name="password">непосредственно пароль</param>
    /// <param name="sole">Так называемая "Соль хеша", для более сложного шифрования</param>
    /// <returns>Хеш пароля в текстовом виде</returns>
    public string EncryptPassword(string password, string sole)
    {
      string encryptPassword = "";
      if(password.Length < 20)
      {
        encryptPassword = ComputeSha256Hash(password + sole);
      }
      else
      {
        encryptPassword = password;
      }
      return encryptPassword;
    }

    /// <summary>
    /// Метод необходим для вычисления хеша строки
    /// </summary>
    /// <param name="_rawData">Текст, который нужно преобразовать</param>
    /// <returns>Хеш строки в текстовом виде</returns>
    private string ComputeSha256Hash(string _rawData)
    {
      //добавление SHA256
      using(SHA256 sha256Hash = SHA256.Create())
      {
        //ComputeHash - возвращает массив byte
        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(_rawData));
        //конвертирует массив байтов в строку 
        StringBuilder builder = new StringBuilder();
        for(int i = 0; i < bytes.Length; i++)
        {
          builder.Append(bytes[i].ToString("x2"));
        }
        return builder.ToString();

      }
    }
  }
}