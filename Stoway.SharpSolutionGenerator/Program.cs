using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoway.SharpSolutionGenerator
{
    class Program
    {
        private static string Template_SolutionName = ConfigurationManager.AppSettings["TemplateSolutionName"];

        private static string[] Content_Replace_Files = null;
        static void Main(string[] args)
        {
            try
            {
                var exts = ConfigurationManager.AppSettings["Content_Replace_Files"].Split('|');
                Content_Replace_Files = exts.Select(p => "." + p).ToArray();

                Console.Write("请输入解决方案名称：");
                var solutionName = Console.ReadLine();
                Console.Write("解决方案名称：{0}，是否生成解决方案？[Y] or [N]：", solutionName);
                var result = Console.ReadLine();
                if (result.ToLower() != "y") return;

                BuildSolution(solutionName);

                Console.WriteLine("生成完毕！");

            }
            catch (Exception ex)
            {

                Console.WriteLine("程序异常：{0}.", ex.Message);
            }

            Console.Read();
        }
        static void BuildSolution(string solutionName)
        {
            string templateFolder = "Template";
            string solutionFolder = string.Format("{0}.Mvc", solutionName);

            if (Directory.Exists(solutionFolder))
            {
                Console.WriteLine("正在删除已存在的目录...");
                Directory.Delete(solutionFolder, true);
            }

            var files = Directory.GetFiles(templateFolder, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var folderPath = Path.GetDirectoryName(file);

                if (folderPath.IndexOf("\\") == -1)
                {
                    folderPath = solutionFolder;
                }
                else
                {
                    folderPath = folderPath.Substring(folderPath.IndexOf("\\") + 1);
                    folderPath = Path.Combine(solutionFolder, folderPath.Replace(Template_SolutionName, solutionName));
                }

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var new_file = Path.Combine(folderPath, Path.GetFileName(file).Replace(Template_SolutionName, solutionName));
                File.Copy(file, new_file, true);
                if (Content_Replace_Files.Contains(Path.GetExtension(new_file).ToLower()))
                {
                    ContentReplace(new_file, solutionName);
                }
                Console.WriteLine("生成文件：{0}", new_file);
            }
        }

        static void ContentReplace(string filePath, string solutionName)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            var content = sr.ReadToEnd();
            content = content.Replace(Template_SolutionName, solutionName);
            fs.Flush();
            sr.Close();
            fs.Close();

            FileStream fs2 = new FileStream(filePath, FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs2, Encoding.UTF8);
            sw.WriteLine(content);
            fs2.Flush();
            sw.Close();
            fs2.Close();
        }
    }

}
