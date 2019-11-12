using System;

namespace ArticoliWebService.Models
{
    public class InfoMsg
    {
        public DateTime Data { get; set; }
        public string Message { get; set; }
        public InfoMsg(DateTime data, string message)
        {
            this.Data = Data;
            this.Message = Message;
        }


    }
}