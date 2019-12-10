using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoFeedbackFschool.Core
{
    public class AutoFeedback
    {
        public ChromeDriver Driver { get; private set; }
        public string Domain { get; set; } = "http://fschool.fpt.edu.vn/";
        public bool IsLogin { get; private set; }

        private bool isBrowserClosed = false;

        public AutoFeedback()
        {
            ChromeOptions options = new ChromeOptions();
            Driver = new ChromeDriver(options);
        }

        public void NavigateToLogin()
        {
            Driver.Navigate().GoToUrl(Domain);
            WaitForJsExec(TimeSpan.FromSeconds(10));
            Driver.FindElementById("imgBtnLogin").Click();

            //Wait for user to login Google OAuth
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(60));
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("nnumber")));
            IsLogin = true;
        }

        public List<TeacherProfileModel> GetTeachers()
        {
            HtmlDocument document = new HtmlDocument();
            List<TeacherProfileModel> feedback = new List<TeacherProfileModel>();

            while (true)
            {
                document.LoadHtml(Driver.PageSource);

                var teachersTable = document.GetElementbyId("stFeedbackList").ChildNodes["tbody"].Descendants("tr")
                    .Where(x => x.Descendants("a").Any())
                    .Where(x => x.Descendants("a").FirstOrDefault().InnerText.Contains("Open"))
                    .Select(x => x.ChildNodes)
                    .Select(item => new TeacherProfileModel()
                    {
                        ClassName = item.ToList()[3].InnerHtml.Replace('\"', ' ').Trim(),
                        AcademicYear = item.ToList()[5].InnerHtml.Replace('\"', ' ').Trim(),
                        Term = item.ToList()[7].InnerHtml.Replace('\"', ' ').Trim(),
                        Teacher = item.ToList()[9].InnerHtml.Replace('\"', ' ').Trim(),
                        FeedbackFor = item.ToList()[11].InnerHtml.Replace('\"', ' ').Trim(),
                        FeedbackLink = item.ToList()[17].ChildNodes[1].Attributes["href"].Value
                    });
            
                if (!teachersTable.Any())
                    break;

                feedback.AddRange(teachersTable);
                Driver.FindElementById("stFeedbackList_next").Click();
            }

            return feedback;
        }

        public void FeedbackTest(IList<TeacherProfileModel> teachers, FeedbackForms forms)
        {

            foreach (var item in teachers)
            {
                if(!item.IsExclude)
                {
                    Driver.Navigate().GoToUrl(Domain + item.FeedbackLink);
                    WaitForJsExec(TimeSpan.FromSeconds(10));

                    Driver.FindElementById($"ContentPlaceHolder1_reload_chkList_0_{(int)forms.SuDungGio}_0").Click();
                    Driver.FindElementById($"ContentPlaceHolder1_reload_chkList_1_{(int)forms.KhaNangTruyenDat}_1").Click();
                    Driver.FindElementById($"ContentPlaceHolder1_reload_chkList_2_{(int)forms.DamBaoKhoiLuong}_2").Click();
                    Driver.FindElementById($"ContentPlaceHolder1_reload_chkList_3_{(int)forms.KhaNangTaoKK}_3").Click();
                    Driver.FindElementById($"ContentPlaceHolder1_reload_chkList_4_{(int)forms.HoTro}_4").Click();
                    Driver.FindElementById($"ContentPlaceHolder1_reload_chkList_5_{(int)forms.Ktra}_5").Click();
                    Driver.FindElementById("ContentPlaceHolder1_txtComment").SendKeys(forms.Comment);

                    // Uncomment in final
                    Driver.FindElementById("ContentPlaceHolder1_btSendFeedback").Click();
                    WaitForJsExec(TimeSpan.FromSeconds(10));
                }
            }
        }

        public void Close()
        {
            if(isBrowserClosed)
                Driver.Close();
        }

        private void WaitForJsExec(TimeSpan timeout)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
            wait.Until(x => ((IJavaScriptExecutor)x).ExecuteScript("return document.readyState").Equals("complete"));
        }
    }
}
