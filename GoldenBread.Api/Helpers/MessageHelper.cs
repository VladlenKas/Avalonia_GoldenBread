using GoldenBread.Domain.Models;
using Humanizer;

namespace GoldenBread.Api.Helpers
{
    public static class MessageHelper
    {
        // Базовые сообщения
        public const string ErrorFromApi = "Произошла ошибка на стороне сервера";
        public const string SuccessFromApi = "Запрос выполнен успешно";


        // Для авторизации
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


        // Для пользователей
        public const string UserNotFound = "Пользователь не найден";
        public const string UserCreated = "Пользователь успешно создан";
        public const string UserUpdated = "Пользователь успешно обновлён";
        public const string UserDeleted = "Пользователь успешно удалён";

        // Для товаров
        public const string ProductNotFound = "Товар не найден";
        public const string ProductDeleted = "Товар успешно удален";
        public const string ProductCreated = "Товар успешно создан";
        public const string ProductUpdated = "Товар успешно обновлен";
    }
}
