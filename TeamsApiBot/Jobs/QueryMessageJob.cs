using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamsApiBot.CacheManages;
using TeamsApiBot.Models;
using TeamsApiBot.Services;
using Telegram.Bot;

namespace TeamsApiBot.Jobs
{
    [DisallowConcurrentExecution, PersistJobDataAfterExecution]
    public class QueryMessageJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string ChatId;
        public QueryMessageJob(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            ChatId = configuration.GetValue<string>("TelegramBotNotifyGroup");
        }
        public async Task Execute(IJobExecutionContext context)
        {
            using var tempscope = _serviceProvider.CreateScope();
            var _graphService = tempscope.ServiceProvider.GetRequiredService<IGraphService>();
            var _memoryCache = tempscope.ServiceProvider.GetRequiredService<IMemoryCache>();
            var _bot = tempscope.ServiceProvider.GetRequiredService<TelegramBotClient>();
            var _CacheManage = tempscope.ServiceProvider.GetRequiredService<CacheManage>();
            var _accessService = tempscope.ServiceProvider.GetRequiredService<IAccessService>();
            var ChatLastTimes = (Dictionary<string, DateTime>)_memoryCache.Get("QueryTimes");
            var ChatTimes = (Dictionary<string, string>)_memoryCache.Get("ChatTimes");
            if (ChatTimes == null) ChatTimes = new Dictionary<string, string>();
            if (ChatLastTimes == null) ChatLastTimes = new Dictionary<string, DateTime>();
            try
            {
                if (_CacheManage.GetInfo() != null)
                {
                    //查询聊天信息
                    var Chats = await _graphService.MeChats();
                    var UserChats = Chats;
                    var user = _CacheManage.GetInfo();
                    Chat HR = new Chat();
                    foreach (var chat in UserChats)
                    {
                        var messages = await _graphService.GetMessagesByUser(chat.id, 50);
                        if (messages != null && messages.Any())
                        {
                            if (ChatLastTimes.ContainsKey(chat.id))
                            {
                                var UnreadMessage = messages.Where(m =>
                                {
                                    var time = DateTime.Parse(m.createdDateTime);
                                    var temptime = ChatLastTimes[chat.id];
                                    if (DateTime.Parse(m.createdDateTime) > ChatLastTimes[chat.id] && m.from.user.id != user.id)
                                    {
                                        return true;
                                    }
                                    return false;
                                }).OrderBy(s => s.createdDateTime)?.ToList();
                                if (UnreadMessage != null && UnreadMessage.Any())
                                {
                                    if (chat.chatType == ChatType.oneOnOne.ToString())
                                    {
                                        var HRCheck = UnreadMessage.FirstOrDefault(s => s.body.content.Contains("例行查岗"));
                                        if (HRCheck != null)
                                        {
                                            string HRCheckText = "查岗通知：\r\n" + HRCheck.from.user.displayName + ": " + HRCheck.body.content + "\r\n" + HRCheck.createdDateTime;
                                            await _bot.SendTextMessageAsync(ChatId, HRCheckText);
                                        }

                                      
                                        ChatLastTimes[chat.id] = UnreadMessage.Max(s => DateTime.Parse(s.createdDateTime));
                                        foreach (var um in UnreadMessage)
                                        {
                                            string text = UnreadMessage?.FirstOrDefault().from.user.displayName + "\r\n" + $"{um.body.content}";
                                            if (text.Length > 3072)
                                            {
                                                text = "消息内容过长,请打开APP查看";
                                            }
                                            await _bot.SendTextMessageAsync(ChatId, text);
                                        }
                                        if (ChatTimes.ContainsKey(messages.FirstOrDefault(s => s.from.user.id != user.id).from.user.displayName))
                                        {
                                            ChatTimes[messages.FirstOrDefault(s => s.from.user.id != user.id).from.user.displayName] = UnreadMessage.Max(s => DateTime.Parse(s.createdDateTime)).ToString("yyyy-MM-dd HH:mm:ss");
                                        }
                                        else
                                        {
                                            ChatTimes.Add(messages.FirstOrDefault(s => s.from.user.id != user.id).from.user.displayName, UnreadMessage.Max(s => DateTime.Parse(s.createdDateTime)).ToString("yyyy-MM-dd HH:mm:ss"));
                                        }
                                    }
                                    else
                                    {
                                        ChatLastTimes[chat.id] = UnreadMessage.Max(s => DateTime.Parse(s.createdDateTime));
                                        foreach (var um in UnreadMessage)
                                        {
                                            string text = chat.topic + ":\r\n" + $"{um.from.user.displayName}:{um.body.content}";
                                            if (text.Length > 3072)
                                            {
                                                text = chat.topic + ":\r\n" + "消息内容过长,请打开APP查看";
                                            }
                                            await _bot.SendTextMessageAsync(ChatId, text);
                                        }
                                        if (ChatTimes.ContainsKey(chat.topic))
                                        {
                                            ChatTimes[chat.topic] = UnreadMessage.Max(s => DateTime.Parse(s.createdDateTime)).ToString("yyyy-MM-dd HH:mm:ss");
                                        }
                                        else
                                        {
                                            ChatTimes.Add(chat.topic, UnreadMessage.Max(s => DateTime.Parse(s.createdDateTime)).ToString("yyyy-MM-dd HH:mm:ss"));
                                        }
                                    }

                                }
                            }
                            else
                            {
                                ChatLastTimes.Add(chat.id, messages.Max(s => DateTime.Parse(s.createdDateTime)));
                                if (chat.chatType == ChatType.oneOnOne.ToString())
                                {
                                    ChatTimes.Add(messages.FirstOrDefault(s => s.from.user.id != user.id).from.user.displayName, messages.Max(s => DateTime.Parse(s.createdDateTime)).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                else
                                {
                                    ChatTimes.Add(chat.topic, messages.Max(s => DateTime.Parse(s.createdDateTime)).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("401"))
                {
                    await _accessService.RefreshToken();
                }
                else if (e.Message.Contains("retry after"))
                {
                    //Retry-After
                    int.TryParse(e.Message.Split("retry after").Last(), out int time);
                    await _bot.SendTextMessageAsync(ChatId, $"休眠{time}秒,原因:{e.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(time));
                }
                else if (e.Message.Contains("The operation was canceled") || e.Message.Contains("Bad Gateway"))
                {
                    ////Retry-After
                    //int.TryParse(e.Message.Split("Retry-After").Last(), out int time);
                    //await Task.Delay(TimeSpan.FromSeconds(time));
                    //await _bot.SendTextMessageAsync(ChatId, $"休眠{time}秒,原因:{e.Message}");
                }
                else
                {
                    await _bot.SendTextMessageAsync(ChatId, e.Message);
                    await _bot.SendTextMessageAsync(ChatId, e.StackTrace);
                }
            }
            finally
            {
                if (ChatLastTimes != null && ChatLastTimes.Any())
                {
                    _memoryCache.Set("QueryTimes", ChatLastTimes);
                    _memoryCache.Set("ChatTimes", ChatTimes);
                    _CacheManage.SetChatTimeConfigJson(ChatLastTimes);
                }
            }
        }
    }
}
