using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Helpers
{
    public static class MessageBoxHelper
    {
        // Ok ButtonEnum.Ok
        public async static Task ShowOkMessageBox(string message) =>
            await MessageBoxManager.GetMessageBoxStandard(
                "Успех",
                message,
                ButtonEnum.Ok,
                Icon.Success).ShowAsync();

        // Error ButtonEnum.Ok
        public async static Task ShowErrorMessageBox(string message) =>
            await MessageBoxManager.GetMessageBoxStandard(
                "Ошибка",
                message,
                ButtonEnum.Ok,
                Icon.Error).ShowAsync();

        // Question ButtonEnum.YesNo
        public async static Task<bool> ShowQuestionMessageBox(string message)
        {
            var box = MessageBoxManager.GetMessageBoxCustom(
                new MessageBoxCustomParams
                {
                    ContentTitle = "Подтверждение",
                    ContentMessage = message,
                    Icon = Icon.Question,
                    ButtonDefinitions = new List<ButtonDefinition>
                    {
                        new ButtonDefinition { Name = "Да", IsDefault = true },
                        new ButtonDefinition { Name = "Нет", IsCancel = true }
                    },
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                });

            var result = await box.ShowWindowAsync();
            return result == "Да";
        }
    }
}
