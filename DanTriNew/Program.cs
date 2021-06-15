using DanTriNew.Entity;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DanTriNew
{
    class Program
    {

     

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Debug.WriteLine(e.Exception.Message);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine((e.ExceptionObject as Exception).Message);
        }
        static void Main(string[] args)
        {

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            try
            {
                var task1 = Task.Run(() =>
                {
                    startCrawlerasync();
                    EventAsync();
                });
                Task.WaitAll(task1);
            }
            catch (Exception ae)
            {
                Console.WriteLine(ae.Message);
            }

            Console.ReadLine();
        }

        private static async Task EventAsync()
        {
            var url = "https://dantri.com.vn";
            var httpClient = new HttpClient();
            var  html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var ulsk = htmlDocument.DocumentNode.SelectSingleNode("/html/body/main/div[2]/div[4]/div[1]/ul[1]");
            var lis = ulsk.SelectNodes("li");
            var ListPost = new List<Post>();
            foreach (var li in lis)
            {

                var htmlPost = await httpClient.GetStringAsync(url + li.Descendants("a").FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value);
                var htmlDocumentPost = new HtmlDocument();
                htmlDocumentPost.LoadHtml(htmlPost);

                var content = htmlDocumentPost.DocumentNode.SelectSingleNode("/html/body/main/div[1]/div[1]/article/div[3]/div/div[2]").InnerText;
                var title = htmlDocumentPost.DocumentNode.SelectSingleNode("/html/body/main/div[1]/div[1]/article/h1").InnerText;

                Regex expression = new Regex(@"(?<slug>.+?)\/(.+?)(?<postid>[0-9]{8,20})(.+?)$");

                Match match = expression.Match(li.Descendants("a").FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value);
                var postID = "";
                var cateName = "";
                if (match.Success)
                {
                    // ... Get group by name.
                    postID = match.Groups["postid"].Value;
                    cateName = match.Groups["slug"].Value;
                    Console.WriteLine("postid: ", postID);
                }

                try
                {
                    var newPost = new Post
                    {
                        Title = title,
                        Content = content,
                        Id = long.Parse(postID),
                        CategoryName = cateName
                        
                    };
                    ListPost.Add(newPost);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    continue;
                    //throw;
                }

            }

            string MyConnection = "Data Source=localhost;Initial Catalog=crawler;User ID=sa;Password=abc123";

            try
            {

                foreach (var item in ListPost)
                {

                    using (SqlConnection openCon = new SqlConnection(MyConnection))
                    {
                        using (var command = new SqlCommand("SP_ListPost", openCon)
                        {
                            CommandType = CommandType.StoredProcedure
                        })
                        {
                            command.Parameters.Add(new SqlParameter("@ID", item.Id));
                            command.Parameters.Add(new SqlParameter("@Title", item.Title));
                            command.Parameters.Add(new SqlParameter("@Content", item.Content));
                            command.Parameters.Add(new SqlParameter("@CategorName", item.CategoryName));
                            openCon.Open();
                            command.ExecuteNonQuery();
                        }

                    }
                }

            }
            catch (Exception e)
            {

                Console.WriteLine("Lỗi" + e);
            }
        }

        private static async Task startCrawlerasync()
        {
            var url = "https://dantri.com.vn";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);


            var olNode = htmlDocument.DocumentNode.SelectSingleNode("/html/body/header/nav/div/ol");
            var listCategory = olNode.SelectNodes("li[@class='dropdown dropdown--hover']");
            var categories = new List<Category>();


            foreach (var category in listCategory)
            {
                
                try
                {
                    var links = category.Descendants("a").FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value;
              
                    Regex expression = new Regex(@"https:\/\/dantri.com.vn\/(?<slug>(.+?))\.(.+?)$");

                    Match match = expression.Match(url + links);
                    var slug = "";
                    if (match.Success)
                    {
                        // ... Get group by name.
                        slug = match.Groups["slug"].Value;
                        Console.WriteLine("slug: ", slug);
                    }

                    var newCategory = new Category
                    {
                        id = int.Parse(category.Descendants("a").FirstOrDefault().ChildAttributes("data-cat-id").FirstOrDefault().Value),
                        link = links,
                        slug = slug,
                        name = category.Descendants("a").FirstOrDefault().InnerText,

                    };
                    categories.Add(newCategory);
                }
                catch (Exception)
                {
                    continue;
                    //throw;
                }
               
            }

            string MyConnection = "Data Source=localhost;Initial Catalog=crawler;User ID=sa;Password=abc123";

            try
            {
                

                foreach (var item in categories)
                {
                    //for (int i = 0; i < count; i++)
                    //{
                        using (SqlConnection openCon = new SqlConnection(MyConnection))
                        {
                            using (var command = new SqlCommand("SP_Insert_Category", openCon)
                            {
                                CommandType = CommandType.StoredProcedure
                            })
                            {
                                command.Parameters.Add(new SqlParameter("@ID", item.id));
                                command.Parameters.Add(new SqlParameter("@Name", item.name));
                                command.Parameters.Add(new SqlParameter("@Link", item.link));
                                command.Parameters.Add(new SqlParameter("@Slug", item.slug));

                                openCon.Open();
                                command.ExecuteNonQuery();
                            }
                        //}
                    }
                    
                }



           
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

        }
    }

}
