﻿using System;
using System.Net;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics.Assets
{
    public class EmbeddedFile
    {
        private readonly Lazy<byte[]> _contents;
        private readonly string _cacheHeader = "private, max-age={0}".ToFormat(AssetSettings.MaxAgeInSeconds);
        

        public EmbeddedFile(Assembly assembly, string resource)
        {
            Name = resource;
            ContentType = MimeType.MimeTypeByFileName(Name);


            _contents = new Lazy<byte[]>(() =>
            {
                var stream = assembly.GetManifestResourceStream(resource);
                return stream.ReadAllBytes();
            });

            Version = assembly.GetName().Version.ToString();
        }

        public bool Matches(string file)
        {
            return Name.EndsWith(file, StringComparison.OrdinalIgnoreCase);
        }

        // Only hitting this with integration tests
        public void Write(IHttpResponse response)
        {
            response.WriteContentType(ContentType.Value);
            response.WriteResponseCode(HttpStatusCode.OK);
            /* TODO -- add later, but not NOW
            response.AppendHeader(HttpResponseHeaders.CacheControl, _cacheHeader);
            var expiresKey = DateTime.UtcNow.AddSeconds(AssetSettings.MaxAgeInSeconds).ToString("R");
            response.AppendHeader(HttpResponseHeaders.Expires, expiresKey);
             */

            response.Write(stream => stream.Write(Contents(), 0, Contents().Length));
        }


        public MimeType ContentType { get; private set; }

        public string Name { get; private set; }
        public string Version { get; private set; }

        public byte[] Contents()
        {
            return _contents.Value;
        }

        public string Url
        {
            get { return "_fubu/asset/{0}/{1}".ToFormat(Version, Name); }
        }

        public HtmlTag ToStyleTag()
        {
            return new StylesheetLinkTag(Url);
        }

        public HtmlTag ToScriptTag()
        {
            return new HtmlTag("script").Attr("language", "javascript").Attr("src", Url);
        }
    }
}