﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Web.Http;
using Newtonsoft.Json;

namespace BitStamp.Model
{
    public class SmmsUploadImage : UploadImageTask
    {
        public SmmsUploadImage(StorageFile file)
            : base(file)
        {
        }

        public SmmsUploadImage(UploadImageTask uploadImageTask)
            : base(uploadImageTask)
        {
        }

        public string ResponseString
        {
            set;
            get;
        }

        public override async void UploadImage()
        {
            string url = "https://sm.ms/api/upload";
            HttpClient webHttpClient =
                new HttpClient();
            HttpMultipartFormDataContent httpMultipartFormDataContent =
                new HttpMultipartFormDataContent();
            var fileContent = new HttpStreamContent(await File.OpenAsync(FileAccessMode.Read));
            fileContent.Headers.Add("Content-Type", "application/octet-stream");

            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
            webHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

            httpMultipartFormDataContent.Add(fileContent, "smfile", File.Name);
            try
            {
                var str = await webHttpClient.PostAsync(new Uri(url), httpMultipartFormDataContent);
                ResponseString = str.Content.ToString();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                OnUploaded?.Invoke(this, false);
                return;
            }

            Smms smms = JsonConvert.DeserializeObject<Smms>(ResponseString);
            if (smms != null)
            {
                if (smms.code == "success")
                {
                    Url = smms.data.url;
                    OnUploaded?.Invoke(this, true);
                }
                else
                {
                    OnUploaded?.Invoke(this, false);
                }
            }
            else
            {
                OnUploaded?.Invoke(this, false);
            }
        }

        private class Smms
        {
            public string code { get; set; }
            public Data data { get; set; }
        }

        private class Data
        {
            public int width { get; set; }
            public int height { get; set; }
            public string filename { get; set; }
            public string storename { get; set; }
            public int size { get; set; }
            public string path { get; set; }
            public string hash { get; set; }
            public int timestamp { get; set; }
            public string ip { get; set; }
            public string url { get; set; }
            public string delete { get; set; }
        }
    }
}
