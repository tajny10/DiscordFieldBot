﻿using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordFieldBot
{
    public class DiscordConnection
    {
        public static DiscordSocketClient socketClient;
        private readonly string _token = "TOKEN";

        public async Task MainAsync()
        {
            socketClient = new DiscordSocketClient();
            socketClient.MessageReceived += MessageHandler;
            socketClient.Log += Logging;
            socketClient.Ready += ClientReady;

            await socketClient.LoginAsync(TokenType.Bot, _token);
            await socketClient.StartAsync();

            Console.WriteLine("\nAll prefixes: ");
            for (int i = 0; i < Bots.botsPrefixes.Count; i++)
            {
                Console.WriteLine($"{(Bots.BotType)i}: \"{Bots.botsPrefixes[i]}\"");
            }
            Console.WriteLine('\n');

            await Task.Delay(-1);
        }

        private async Task MessageHandler(SocketMessage message)
        {
            char messagePrefix = message.Content[0];

            if (Bots.botsPrefixes.Exists(s => s == messagePrefix))
            {
                Console.WriteLine($"Command: {message.Content} from user {message.Author.Username}#{message.Author.Id}");

                Bots.BotType botType = Bots.GetBotType(messagePrefix);

                string command = message.Content.Substring(1).ToLower().Trim();

                try
                {
                    switch (botType)
                    {
                        case Bots.BotType.CamBot:
                            await CamBot.CamBotCommandsAsync(command, message);
                            break;
                        case Bots.BotType.CleanerBot:
                            await CleanerBot.DeleteLastMessages(command, message);
                            break;
                        case Bots.BotType.MusicBot:
                            break;
                        case Bots.BotType.ChanceCalculateBot:
                            await CalculateChanceBot.HandleComandAsync(command, message);
                            break;
                        case Bots.BotType.Unknown:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Helper.ConsoleWriteExceptionInfo(ex);
                }
            }
        }

        private Task Logging(LogMessage arg)
        {
            Console.WriteLine("--- " + arg.Message);
            return Task.CompletedTask;
        }

        private Task ClientReady()
        {
            CalculateChanceBot.DeserializeDictionary();
            return Task.CompletedTask;
        }
    }
}