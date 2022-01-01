using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using ICSharpCode.SharpZipLib.Zip;

namespace System_Init_Toolbox___Setup
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public static void Delay(int mm)
        {
            DateTime current = DateTime.Now;
            while (current.AddMilliseconds(mm) > DateTime.Now)
            {
                System.Windows.Forms.Application.DoEvents();
            }
            return;
        }
        public static bool UnZip(string fileToUnZip, string zipedFolder, string password)//解压函数
        {
            bool result = true;
            FileStream fs = null;
            ZipInputStream zipStream = null;
            ZipEntry ent = null;
            string fileName;

            if (!File.Exists(fileToUnZip))
                return false;

            if (!Directory.Exists(zipedFolder))
                Directory.CreateDirectory(zipedFolder);

            try
            {
                zipStream = new ZipInputStream(File.OpenRead(fileToUnZip));
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    if (!string.IsNullOrEmpty(ent.Name))
                    {
                        fileName = Path.Combine(zipedFolder, ent.Name);
                        fileName = fileName.Replace('/', '\\');//change by Mr.HopeGi

                        int index = ent.Name.LastIndexOf('/');
                        if (index != -1 || fileName.EndsWith("\\"))
                        {
                            string tmpDir = (index != -1 ? fileName.Substring(0, fileName.LastIndexOf('\\')) : fileName) + "\\";
                            if (!Directory.Exists(tmpDir))
                            {
                                Directory.CreateDirectory(tmpDir);
                            }
                            if (tmpDir == fileName)
                            {
                                continue;
                            }
                        }

                        fs = File.Create(fileName);
                        int size = 2048;
                        byte[] data = new byte[size];
                        while (true)
                        {
                            size = zipStream.Read(data, 0, data.Length);
                            if (size > 0)
                                fs.Write(data, 0, data.Length);
                            else
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", "安装失败！\n解压程序文件包时出错，错误信息：\n" + ex,MessageBoxButton.OK, MessageBoxImage.Error);
                result = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return result;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = "C:/Program Files/Stargazing Studio/System Init Toolbox";
            //如果已经存在默认路径了
            if (Directory.Exists(path))
            {
                //那就什么都不干
            }
            else
            {
                //不然就创建
                Process nf_Process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo("./SetupBATFiles/mkdir_mr.bat");
                startInfo.CreateNoWindow = true;//无窗口
                nf_Process.StartInfo = startInfo;
                nf_Process.Start();
                nf_Process.WaitForExit();
            }
            //复制文件
            status_label.Content = "复制文件包中...";
            pb.Value = 25;
            Delay(1000);//给予label和pb.value切换的反应时间
            string sourcePath = "win10-x64.zip";//自解压后程序压缩包名
            string targetPath = path;//目标位置 直接默认目录
            bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之
            System.IO.File.Copy(sourcePath, targetPath, isrewrite);//复制~
            if(UnZip(path + "/win10-x64.zip", path, null))//如果解压成功
            {

            }
            else
            {
                MessageBox.Show("Error", "安装失败",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
    }
}
