﻿using LiteDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using URLDTO.Controllers;
using URLDTO.Models;
using URLPayRoc.Models;

namespace URLPayRoc
{
	public class Shortener
	{
		public string Token { get; set; }
		public UrlDTO biturl;
		public string url2 { get; set; }
        // The method with which we generate the token
        public Shortener GenerateToken()
		{
			string urlsafe = string.Empty;
			Enumerable.Range(48, 75)
			  .Where(i => i < 58 || i > 64 && i < 91 || i > 96)
			  .OrderBy(o => new Random().Next())
			  .ToList()
			  .ForEach(i => urlsafe += Convert.ToChar(i)); // Store each char into urlsafe
			Token = urlsafe.Substring(new Random().Next(0, urlsafe.Length), new Random().Next(2, 6));
			return this;
		}
		public string SaveUrl(string url)
		{
			var connectionString = @$"Filename={"Data/Urls.db"}; Connection=Shared;";
			var db = new LiteDatabase(connectionString);
			var urls = db.GetCollection<UrlDTO>();
			// While the token exists in our LiteDB we generate a new one
			// It basically means that if a token already exists we simply generate a new one
			while (urls.Exists(u => u.Token == GenerateToken().Token)) ;
			// Store the values in the NixURL model
			var uri = new Uri(url);
			var host = uri.Host;
			biturl = new UrlDTO()
			{
				Token = Token,
				URL = url,
				ShortenedURL = host + "/" + Token 

			};
			url2 = host + "/" + Token;
			if (urls.Exists(u => u.URL == url))
				throw new Exception("URL already exists");
			// Save the NixURL model to  the DB
			urls.Insert(biturl);
			return url2;
		}


  //      public IEnumerable<UrlDTO> UrlData(string token)
		//{
		//	var MainDB = new LiteDB.LiteDatabase(@"Data/Urls.db");
		//	var collection = MainDB.GetCollection<UrlDTO>();
		//	return collection.Find(n => n.Token == token);
		//}

	}


	internal class NixConf
    {
        public string Host()
        {
			UrlDTO bitUrl = new UrlDTO();
			string baseUrl = bitUrl.URL;
			var uri = new Uri(baseUrl);
			var host = uri.Host;

			return host;

		}

	//static Task Redirect(HttpContext context)
	//{
	
	//	var collection = db.GetCollection<UrlDTO>();

	//	var path = context.Request.Path.ToUriComponent().Trim('/');
	//	var id = ShortLink.GetId(path);
	//	var entry = collection.Find(p => p.Id == id).FirstOrDefault();

	//	if (entry != null)
	//		context.Response.Redirect(entry.Url);
	//	else
	//		context.Response.Redirect("/");

	//	return Task.CompletedTask;
	//}
	}


}
 
    