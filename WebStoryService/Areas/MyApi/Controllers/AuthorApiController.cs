﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebStoryService.Models.ModelData;
using WebStoryService.Models.Repositories;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("api/authors")]
    public class AuthorApiController : ApiController
    {
        [Route("get")]
        [HttpGet]
        public IEnumerable<Author> Get()
        {
            try
            {
                AuthorRes res = new AuthorRes();
                return res.GetAuthors();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAuthors: {ex.Message}");
            }
            return new List<Author>();
        }
        [Route("get/{id}")]
        [HttpGet]
        public Author GetById(int id)
        {
            try
            {
                AuthorRes res = new AuthorRes();
                return res.getAuthorById(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAuthors: {ex.Message}");
            }
            return new Author();
        }
        [Route("post")]
        [HttpPost]
        public int Post(Author author)
        {
            try
            {
                if (author != null)
                {
                    AuthorRes res = new AuthorRes();
                    res.AddAuthor(author);
                }
                return 0;
            }
            catch (Exception ex)
            {
            }
            return 0;
        }
    }
}