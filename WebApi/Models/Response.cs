using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class Response<T> where T : class, new()
    {
        public bool OK { get; set; }
        public T Data { get; set; }
        public IEnumerable<string> Errors { get; set; }

        public Response()
        {
            OK = true;

            try
            {
                Data = new T();
            }
            catch (Exception )
            {
                
                throw;
            }
            
            Errors = new List<string>();
        }

        public static Response<T> Success(T data)
        {
            Response<T> response = new Response<T>();
            response.OK = true;
            response.Data = data;

            return response;
        }
    }
}