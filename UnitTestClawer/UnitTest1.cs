using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;

namespace UnitTestClawer
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // ... Input string.
            string input = "https://dantri.com.vn/kinh-doanh/du-an-dien-gio-mat-thiet-bi-chu-tich-bac-lieu-yeu-cau-lap-chuyen-an-xu-ly-20210610111341960.htm#dt_source=Cate_KinhDoanh&dt_campaign=MainList&dt_medium=21";

            // ... Use named group in regular expression.
            Regex expression = new Regex(@"https:\/\/dantri.com.vn\/(?<slug>.+?)\/(.+?)(?<postid>[0-9]{8,20})(.+?)$");

            // ... See if we matched.
            Match match = expression.Match(input);
            if (match.Success)
            {
                // ... Get group by name.
                string slug = match.Groups["slug"].Value;
                string result = match.Groups["postid"].Value;
                Console.WriteLine("postid: {0}", result);
            }
            // Done.
            Console.ReadLine();
        }
    }
}
