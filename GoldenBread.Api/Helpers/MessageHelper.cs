using GoldenBread.Shared.Entities;
using Humanizer;

namespace GoldenBread.Api.Helpers
{
    public static class MessageHelper
    {
        // == Base Massages ==
        public const string ErrorFromApi = "Произошла ошибка на стороне сервера";
        public const string SuccesFromApi = "Запрос выполнен успешно";


        // == For Login ==
        public static string CorrectData(User user)
        {
            return $"Вход выполнен успешно. Добро пожаловать в систему" +
                   $"\n\nПользователь: {user.Fullname}" +
                   $"\nДолжность: {user.RoleValue}";
        }

        public const string IncorrectData = "Введены неверные учётные данные";
        public const string PendingStatus = "Ваша учетная запись ожидает подтверждения.\n" +
            "Пожалуйста, дождитесь решения менеджера или обратитесь к администратору";
        public const string RejectedStatus = "Ваша учетная запись не прошла проверку. Приносим извинения";
        public const string SuspendedStatus = "Ваша учетная запись заморожена. Обратитесь к администратору";
        public const string UnknownStatus = "Роль пользователя не определена";


        // == For Users ==
        public const string UserNotFound = "Пользователь не найден";
        public const string UserCreated = "Пользователь успешно создан";
        public const string UserUpdated = "Пользователь успешно обновлён";
        public const string UserDeleted = "Пользователь успешно удалён";
    }
}
