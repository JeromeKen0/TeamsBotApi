using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamsApiBot.Models
{
    //如果好用，请收藏地址，帮忙分享。
    public class User
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string displayName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string userIdentityType { get; set; }
    }

    public class from
    {
        /// <summary>
        /// 
        /// </summary>
        public string application { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string device { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string conversation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public User user { get; set; }
    }

    public class Body
    {
        /// <summary>
        /// 
        /// </summary>
        public string contentType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string content { get; set; }
    }

    public class Message
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string replyToId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string etag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string messageType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string createdDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lastModifiedDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lastEditedDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string deletedDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string subject { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string summary { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string chatId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string importance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string locale { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string webUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string channelIdentity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string policyViolation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public from from { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Body body { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public List<object> attachments { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public List<object> mentions { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public List<object> reactions { get; set; }
    }

}
